using AutoMapper;
using Buildify.APIs.Errors;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Buildify.Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buildify.APIs.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public OrdersController(IUnitOfWork unitOfWork, ICartRepository cartRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get current user's orders
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var spec = new OrdersByUserSpecification(userId);
            var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);
            var ordersDto = _mapper.Map<IReadOnlyList<OrderDto>>(orders);
            return Ok(ordersDto);
        }

        /// <summary>
        /// Get single order by ID (user can only see their own orders)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var spec = new OrderWithItemsSpecification(id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found"));

            // Verify the order belongs to the user (unless admin)
            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Unauthorized(new ApiResponse(401, "You are not authorized to view this order"));

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        /// <summary>
        /// Create new order from cart
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            // Get cart with items
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                return BadRequest(new ApiResponse(400, "Cart is empty"));

            // Validate stock availability for all items
            foreach (var cartItem in cart.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.ProductId);
                if (product == null)
                    return BadRequest(new ApiResponse(400, $"Product {cartItem.ProductId} not found"));

                if (product.Stock < cartItem.Quantity)
                    return BadRequest(new ApiResponse(400, $"Insufficient stock for {product.Name}. Available: {product.Stock}"));
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingFirstName = createOrderDto.ShippingAddress.FirstName,
                ShippingLastName = createOrderDto.ShippingAddress.LastName,
                ShippingStreet = createOrderDto.ShippingAddress.Street,
                ShippingCity = createOrderDto.ShippingAddress.City,
                ShippingState = createOrderDto.ShippingAddress.State,
                ShippingZipCode = createOrderDto.ShippingAddress.ZipCode,
                ShippingCountry = createOrderDto.ShippingAddress.Country
            };

            // Create order items and calculate total
            decimal totalPrice = 0;
            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ProductImageUrl = cartItem.Product.ImageUrl,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity
                };
                order.OrderItems.Add(orderItem);
                totalPrice += cartItem.Price * cartItem.Quantity;

                // Reduce product stock
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.Stock -= cartItem.Quantity;
                    _unitOfWork.Repository<Product>().Update(product);
                }
            }

            order.TotalPrice = totalPrice;

            // Save order
            _unitOfWork.Repository<Order>().Add(order);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create order"));

            // Clear the cart after successful order
            await _cartRepository.ClearCartAsync(userId);

            // Return created order
            var spec = new OrderWithItemsSpecification(order.Id);
            var createdOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            var orderDto = _mapper.Map<OrderDto>(createdOrder);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllOrders()
        {
            var spec = new OrderWithItemsSpecification();
            var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);
            var ordersDto = _mapper.Map<IReadOnlyList<OrderDto>>(orders);
            return Ok(ordersDto);
        }

        /// <summary>
        /// Update order status (Admin only)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found"));

            // Update status
            order.Status = updateStatusDto.Status;
            order.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Repository<Order>().Update(order);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update order status"));

            // Return updated order
            var spec = new OrderWithItemsSpecification(id);
            var updatedOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            var orderDto = _mapper.Map<OrderDto>(updatedOrder);

            return Ok(orderDto);
        }
    }
}

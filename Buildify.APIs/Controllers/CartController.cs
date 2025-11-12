using AutoMapper;
using Buildify.APIs.Errors;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buildify.APIs.Controllers
{
    [Authorize]
    public class CartController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartController(IUnitOfWork unitOfWork, ICartRepository cartRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get current user's cart
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            
            if (cart == null)
            {
                // Return empty cart if none exists
                return Ok(new CartDto
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Items = new List<CartItemDto>()
                });
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto addToCartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            // Check if product exists
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(addToCartDto.ProductId);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            // Check stock availability
            if (product.Stock < addToCartDto.Quantity)
                return BadRequest(new ApiResponse(400, $"Only {product.Stock} items available in stock"));

            // Get or create cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                _unitOfWork.Repository<Cart>().Add(cart);
                await _unitOfWork.Complete();
            }

            // Check if product already in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);
            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += addToCartDto.Quantity;
                
                // Check stock again after adding
                if (product.Stock < existingItem.Quantity)
                    return BadRequest(new ApiResponse(400, $"Only {product.Stock} items available in stock"));

                _unitOfWork.Repository<CartItem>().Update(existingItem);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    Price = product.Price
                };
                _unitOfWork.Repository<CartItem>().Add(cartItem);
            }

            cart.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.Repository<Cart>().Update(cart);
            await _unitOfWork.Complete();

            // Reload cart with items
            cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("{itemId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int itemId, UpdateCartItemDto updateCartItemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var cartItem = await _cartRepository.GetCartItemAsync(itemId);
            if (cartItem == null)
                return NotFound(new ApiResponse(404, "Cart item not found"));

            // Verify the cart item belongs to the user
            if (cartItem.Cart.UserId != userId)
                return Unauthorized(new ApiResponse(401, "You are not authorized to update this cart item"));

            // Check stock availability
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.ProductId);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            if (product.Stock < updateCartItemDto.Quantity)
                return BadRequest(new ApiResponse(400, $"Only {product.Stock} items available in stock"));

            // Update quantity
            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.Cart.UpdatedDate = DateTime.UtcNow;
            
            _unitOfWork.Repository<CartItem>().Update(cartItem);
            _unitOfWork.Repository<Cart>().Update(cartItem.Cart);
            await _unitOfWork.Complete();

            // Reload cart with items
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        /// <summary>
        /// Delete item from cart
        /// </summary>
        [HttpDelete("{itemId}")]
        public async Task<ActionResult<CartDto>> DeleteCartItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var cartItem = await _cartRepository.GetCartItemAsync(itemId);
            if (cartItem == null)
                return NotFound(new ApiResponse(404, "Cart item not found"));

            // Verify the cart item belongs to the user
            if (cartItem.Cart.UserId != userId)
                return Unauthorized(new ApiResponse(401, "You are not authorized to delete this cart item"));

            // Update cart's updated date
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                cart.UpdatedDate = DateTime.UtcNow;
                _unitOfWork.Repository<Cart>().Update(cart);
            }

            // Delete the cart item
            var result = await _cartRepository.DeleteCartItemAsync(itemId);
            await _unitOfWork.Complete();

            if (!result)
                return BadRequest(new ApiResponse(400, "Failed to delete cart item"));

            // Reload cart with items
            cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var result = await _cartRepository.ClearCartAsync(userId);
            
            if (!result)
                return NotFound(new ApiResponse(404, "Cart not found or already empty"));

            return Ok(new ApiResponse(200, "Cart cleared successfully"));
        }
    }
}

using AutoMapper;
using Buildify.APIs.Errors;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Buildify.Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildify.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts()
        {
            var spec = new ProductWithCategorySpecification();
            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
            var productsDto = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Ok(productsDto);
        }

        /// <summary>
        /// Get a single product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductWithCategorySpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        /// <summary>
        /// Get products by category ID
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            // Check if category exists
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found"));

            var spec = new ProductsByCategorySpecification(categoryId);
            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
            var productsDto = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Ok(productsDto);
        }

        /// <summary>
        /// Create a new product (Admin and Seller only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            // Check if category exists
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(createProductDto.CategoryId);
            if (category == null)
                return BadRequest(new ApiResponse(400, "Invalid category ID"));

            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedDate = DateTime.UtcNow;

            _unitOfWork.Repository<Product>().Add(product);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create product"));

            // Reload product with category
            var spec = new ProductWithCategorySpecification(product.Id);
            var createdProduct = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        /// <summary>
        /// Update an existing product (Admin and Seller only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            // Check if category exists
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(updateProductDto.CategoryId);
            if (category == null)
                return BadRequest(new ApiResponse(400, "Invalid category ID"));

            // Update product properties
            _mapper.Map(updateProductDto, product);

            _unitOfWork.Repository<Product>().Update(product);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update product"));

            // Reload product with category
            var spec = new ProductWithCategorySpecification(id);
            var updatedProduct = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            var productDto = _mapper.Map<ProductDto>(updatedProduct);

            return Ok(productDto);
        }

        /// <summary>
        /// Delete a product (Admin and Seller only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            _unitOfWork.Repository<Product>().Delete(product);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to delete product"));

            return Ok(new ApiResponse(200, "Product deleted successfully"));
        }
    }
}

using AutoMapper;
using Buildify.APIs.Errors;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildify.APIs.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var categoriesDto = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        /// <summary>
        /// Get a single category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);

            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found"));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        /// <summary>
        /// Create a new category (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            // Check if category name already exists
            var existingCategories = await _unitOfWork.Repository<Category>().GetAllAsync();
            if (existingCategories.Any(c => c.Name.ToLower() == createCategoryDto.Name.ToLower()))
                return BadRequest(new ApiResponse(400, "Category with this name already exists"));

            var category = _mapper.Map<Category>(createCategoryDto);

            _unitOfWork.Repository<Category>().Add(category);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create category"));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        /// <summary>
        /// Update an existing category (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, CreateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found"));

            // Check if category name already exists (excluding current category)
            var existingCategories = await _unitOfWork.Repository<Category>().GetAllAsync();
            if (existingCategories.Any(c => c.Id != id && c.Name.ToLower() == updateCategoryDto.Name.ToLower()))
                return BadRequest(new ApiResponse(400, "Category with this name already exists"));

            _mapper.Map(updateCategoryDto, category);

            _unitOfWork.Repository<Category>().Update(category);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update category"));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found"));

            // Check if category has products
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            if (products.Any(p => p.CategoryId == id))
                return BadRequest(new ApiResponse(400, "Cannot delete category with existing products"));

            _unitOfWork.Repository<Category>().Delete(category);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to delete category"));

            return Ok(new ApiResponse(200, "Category deleted successfully"));
        }
    }
}

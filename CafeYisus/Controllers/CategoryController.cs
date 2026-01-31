using CafeYisus.DTOs;
using CafeYisus.Models;
using CafeYisus.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CafeYisusDbContext _context;
        public CategoryController(CafeYisusDbContext context) {
            _context = context;
        }

        [Authorize(Policy = "StaffOrAdminRole")]
        [HttpGet("Categories")]
        public async Task<IActionResult> GetAll([FromQuery] GetCategoriesQuery query)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 5;

            var obj = _context.Categories.AsQueryable();

            var totalItems = await obj.CountAsync();
            if (totalItems == 0)
                return NoContent(); 

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                obj = obj.Where(c =>
                    c.Name.ToLower().Contains(query.Name.ToLower()));
            }
            var categories = await obj
              .OrderBy(c => c.Id)
              .Skip((query.Page - 1) * query.PageSize)
              .Take(query.PageSize)
              .ToListAsync();

            var result = new
            {
                page = query.Page,
                pageSize = query.PageSize,
                TotalItems = totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / query.PageSize),
                Data = categories
            };

            return Ok(result);
        }

        [Authorize(Policy = "StaffOrAdminRole")]
        [HttpGet("Category/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [Authorize(Policy = "StaffRole")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            if (await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower()))
                return Conflict("Category already exists");

            try
            {
                var category = new Category
                {
                    Name = dto.Name,
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, category);
            }
            catch (Exception ex)
            {
                return BadRequest("Error creating category: " + ex.Message);
            }
        }

        [Authorize(Policy = "StaffRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Category not found");

            bool isDuplicate = await _context.Categories.AnyAsync(c =>
                c.Id != id && c.Name.ToLower() == dto.Name.ToLower());

            if (isDuplicate)
                return Conflict("Category name already exists");

            try
            {
                category.Name = dto.Name;
                category.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest("Error updating category: " + ex.Message);
            }
        }

        [Authorize(Policy = "StaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound("Category not found");
            var drink = await _context.Drinks.AnyAsync(d => d.Category.Id == id);
            if (drink)
            {
                return BadRequest("There is drink with this category, cannot delete!");
            }
            _context.Categories.Remove(category);
            return NoContent();
        }


    }
}


using CafeYisus.DTOs;
using CafeYisus.Models;
using CafeYisus.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeYisus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        private readonly CafeYisusDbContext _context;
        public DrinkController(CafeYisusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetDrinksQuery query)
        {
            var obj =  _context.Drinks
                .Include(d => d.Category)
                .AsQueryable();
            var totalItems = await obj.CountAsync();
            if (totalItems == 0)
                return NoContent();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                obj = obj.Where(d => d.Name.ToLower().Contains(query.Name.ToLower()));
            }

            if (query.Price.HasValue)
            {
                obj = obj.Where(d => d.Price <= query.Price.Value);
            }

            if (query.CategoryId.HasValue)
            {
                obj = obj.Where(d => d.CategoryId == query.CategoryId.Value);
            }

            switch (query.OrderBy?.ToLower()) {
                case "name":
                    obj = obj.OrderBy(d => d.Name); break;
                case "category":
                    obj = obj.OrderBy(d => d.Category.Name); break;
                default:
                    obj = obj.OrderBy(d => d.Id); break;
            }

            var drinks = obj
                         .Skip((query.Page - 1) * query.PageSize)
                         .Take(query.PageSize)
                         .Select(d => new DrinkDto
                         {
                             Name = d.Name,
                             Price = d.Price,
                             ImageUrl = d.ImageUrl,
                             CategoryName = d.Category.Name,
                             IsAvailable = d.IsAvailable,
                         })
                         .ToListAsync();
                         ;

            var result = new
            {
                page = query.Page,
                pageSize = query.PageSize,
                TotalItems = totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / query.PageSize),
                Data = drinks
            };

            return Ok(result);

        }
    }
}

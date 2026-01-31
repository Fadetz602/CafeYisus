using CafeYisus.Models;
using CafeYisus.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeYisus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly CafeYisusDbContext _context;
        public UserController(CafeYisusDbContext context)
        {
            _context = context;
        }

        //[Authorize(Policy = "AdminRole")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 5;

            var obj = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Username))
                obj = obj.Where(u => u.Username.Contains(query.Username));

            if (!string.IsNullOrWhiteSpace(query.Email))
                obj = obj.Where(u => u.Email.Contains(query.Email));

            if (query.RoleId.HasValue)
                obj = obj.Where(u => u.RoleId == query.RoleId.Value);

            var totalItems = await obj.CountAsync();

            obj = query.OrderBy?.ToLower() switch
            {
                "username" => obj.OrderBy(u => u.Username),
                _ => obj.OrderBy(u => u.Id)
            };

            var users = await obj
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                Role = new
                {
                    u.Role.Id,
                    u.Role.Name
                }
            })
    .ToListAsync();



            var result = new
            {
                page = query.Page,
                pageSize = query.PageSize,
                TotalItems = totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / query.PageSize),
                Data = users
            };

            return Ok(result);
        }
    }
}

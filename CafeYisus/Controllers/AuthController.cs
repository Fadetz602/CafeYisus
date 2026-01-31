using CafeYisus.DTOs;
using CafeYisus.Helpers;
using CafeYisus.Models;
using CafeYisus.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeYisus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CafeYisusDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(CafeYisusDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest("Please fill in all field");

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower()))
                return BadRequest("Username already exists");

            if (!RegexHelper.IsMatch(dto.Email, RegexPatterns.Email))
                return BadRequest("Wrong email format");

            if (!RegexHelper.IsMatch(dto.Password, RegexPatterns.Password))
                return BadRequest("Password must has at least 8 characters");

            if (await _context.Users.AnyAsync(u => u.Email.Equals(dto.Email)))
                return BadRequest("Email has already existed");
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = dto.Password,
                FullName = dto.FullName,
                Email = dto.Email,
                RoleId = 3
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Register success");

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required");

            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            if (dto.Password != user.PasswordHash)
                return Unauthorized("Invalid email or password");


            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Username,
                    Role = user.Role.Name
                }
            });
        }

    }
}


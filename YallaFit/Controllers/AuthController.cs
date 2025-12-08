using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.DTOs;
using YallaFit.Models;
using YallaFit.Services;
using BCrypt.Net;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly YallaFitDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(YallaFitDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.MotDePasse))
            {
                return BadRequest(new { message = "Email et mot de passe requis" });
            }

            var user = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            // Verify password with BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.MotDePasse, user.MotDePasse))
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    NomComplet = user.NomComplet,
                    Email = user.Email,
                    Role = user.Role
                }
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.MotDePasse))
            {
                return BadRequest(new { message = "Email et mot de passe requis" });
            }

            if (string.IsNullOrEmpty(request.NomComplet))
            {
                return BadRequest(new { message = "Nom complet requis" });
            }

            // Check if user already exists
            var existingUser = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Un utilisateur avec cet email existe déjà" });
            }

            // Hash password with BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.MotDePasse);

            // Create new user
            var newUser = new Utilisateur
            {
                NomComplet = request.NomComplet,
                Email = request.Email,
                MotDePasse = hashedPassword,
                Role = request.Role
            };

            _context.Utilisateurs.Add(newUser);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtService.GenerateToken(newUser);

            var response = new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = newUser.Id,
                    NomComplet = newUser.NomComplet,
                    Email = newUser.Email,
                    Role = newUser.Role
                }
            };

            return Ok(response);
        }
    }
}

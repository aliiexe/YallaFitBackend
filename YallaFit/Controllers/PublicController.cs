using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.Models;

namespace YallaFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public PublicController(YallaFitDbContext context)
        {
            _context = context;
        }

        [HttpGet("landing-stats")]
        public async Task<IActionResult> GetLandingStats()
        {
            try
            {
                var activeUsers = await _context.Utilisateurs.CountAsync();
                var workoutsLogged = await _context.TrainingSessions.CountAsync();
                // Coaches are users with Role = "Coach"
                var activeCoaches = await _context.Utilisateurs.CountAsync(u => u.Role == "Coach");
                
                // Assuming app store rating is static for now as we don't have a reviews table
                var appRating = 4.9;

                return Ok(new
                {
                    activeUsers,
                    workoutsLogged,
                    activeCoaches,
                    appRating
                });
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, new { message = "Error fetching stats", error = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.DTOs;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public DashboardController(YallaFitDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                // Get total counts
                var totalUsers = await _context.Utilisateurs.CountAsync();
                var totalPrograms = await _context.Programmes.CountAsync();
                var totalSessions = await _context.Seances.CountAsync();

                // Calculate growth rates (mock for now - would need historical data)
                var stats = new DashboardStatsDto
                {
                    TotalUsers = totalUsers,
                    ActivePrograms = totalPrograms,
                    CompletedSessions = totalSessions,
                    UserGrowth = 12.3m,
                    ProgramGrowth = 8.5m,
                    SessionGrowth = 6.7m
                };

                // Get top programs with stats
                var programs = await _context.Programmes
                    .Include(p => p.Seances)
                    .OrderByDescending(p => p.Seances.Count)
                    .Take(3)
                    .ToListAsync();

                var topPrograms = programs.Select((p, index) => new ProgramStatsDto
                {
                    Id = p.Id,
                    Nom = p.Titre,
                    Description = $"Programme de {p.DureeSemaines} semaines",
                    ActiveUsers = new Random().Next(20, 50), // Mock - would need user-program relationship
                    CompletionRate = new Random().Next(60, 90),
                    GrowthRate = index == 0 ? 13.62m : index == 1 ? 12.72m : 6.29m,
                    ChartData = GenerateChartData()
                }).ToList();

                // Get recent activity (mock for now)
                var recentActivity = new List<RecentActivityDto>
                {
                    new RecentActivityDto
                    {
                        UserName = "Marie Dubois",
                        Action = "A complété",
                        Item = "Programme Force",
                        Timestamp = DateTime.UtcNow.AddMinutes(-5)
                    },
                    new RecentActivityDto
                    {
                        UserName = "Jean Martin",
                        Action = "A commencé",
                        Item = "Plan Nutrition",
                        Timestamp = DateTime.UtcNow.AddMinutes(-12)
                    },
                    new RecentActivityDto
                    {
                        UserName = "Sophie Laurent",
                        Action = "A rejoint",
                        Item = "YallaFit",
                        Timestamp = DateTime.UtcNow.AddMinutes(-23)
                    }
                };

                var dashboardData = new DashboardDataDto
                {
                    Stats = stats,
                    TopPrograms = topPrograms,
                    RecentActivity = recentActivity
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données", error = ex.Message });
            }
        }

        private List<int> GenerateChartData()
        {
            var random = new Random();
            return Enumerable.Range(0, 8).Select(_ => random.Next(15, 40)).ToList();
        }
    }
}

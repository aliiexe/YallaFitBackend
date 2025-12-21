using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public AdminController(YallaFitDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            try
            {
                var now = DateTime.Now;
                var last30Days = now.AddDays(-30);

                // User counts by role
                var users = await _context.Utilisateurs.ToListAsync();
                var totalUsers = users.Count;
                var adminCount = users.Count(u => u.Role == "Admin");
                var coachCount = users.Count(u => u.Role == "Coach");
                var sportifCount = users.Count(u => u.Role == "Sportif");

                // Program stats
                var allPrograms = await _context.Programmes.ToListAsync();
                var totalPrograms = allPrograms.Count;
                var publicPrograms = allPrograms.Count(p => p.IsPublic);
                var privatePrograms = totalPrograms - publicPrograms;

                // Enrollment stats
                var enrollments = await _context.ProgrammeEnrollments.ToListAsync();
                var activeEnrollments = enrollments.Count(e => e.IsActive);

                // Session stats (use DateCompleted)
                var allSessions = await _context.TrainingSessions.ToListAsync();
                var totalTrainingSessions = allSessions.Count;
                var sessionsLast30 = allSessions.Count(ts => ts.DateCompleted >= last30Days);

                // Nutrition stats  
                var allPlans = await _context.PlansNutrition.ToListAsync();
                var totalNutritionPlans = allPlans.Count;
                var activePlans = allPlans.Count(p => p.EstActif);
                var nutritionLast30 = allPlans.Count(p => p.DateGeneration >= last30Days);

                // Content stats
                var totalExercises = await _context.Exercices.CountAsync();
                var totalAliments = await _context.Aliments.CountAsync();
                var totalPhotoAnalyses = await _context.AnalysesRepasPhoto.CountAsync();
                var photoAnalysesLast30 = await _context.AnalysesRepasPhoto.CountAsync(a => a.DateAnalyse >= last30Days);

                // Calculate engagement rate (users with training sessions in last 30 days)
                var activeUserIds = allSessions
                    .Where(ts => ts.DateCompleted >= last30Days)
                    .Select(ts => ts.SportifId)
                    .Distinct()
                    .Count();
                
                var engagementRate = sportifCount > 0 ? (activeUserIds / (decimal)sportifCount) * 100 : 0;

                // Monthly trend data (last 6 months) - simplified
                var monthlyData = new List<object>();
                for (int i = 5; i >= 0; i--)
                {
                    var monthStart = now.AddMonths(-i).Date;
                    var monthEnd = monthStart.AddMonths(1);
                    
                    var monthSessions = allSessions.Count(ts =>ts.DateCompleted >= monthStart && ts.DateCompleted < monthEnd);
                    var monthPlans = allPlans.Count(p => p.DateGeneration >= monthStart && p.DateGeneration < monthEnd);
                    
                    monthlyData.Add(new
                    {
                        month = monthStart.ToString("MMM"),
                        users = totalUsers, // Simplified - showing cumulative
                        sessions = monthSessions,
                        nutritionPlans = monthPlans
                    });
                }

                var stats = new
                {
                    // User metrics
                    totalUsers,
                    adminCount,
                    coachCount,
                    sportifCount,
                    userGrowth = 0m, // Simplified - no date tracking on users
                    usersLast30Days = 0,

                    // Program metrics
                    totalPrograms,
                    publicPrograms,
                    privatePrograms,
                    activeEnrollments,
                    programGrowth = 0m, // Simplified
                    programsLast30Days = 0,

                    // Session metrics
                    totalTrainingSessions,
                    completedSessions = totalTrainingSessions, // All sessions are completed
                    completionRate = 100m,
                    sessionGrowth = 0m, // Simplified
                    sessionsLast30Days = sessionsLast30,

                    // Nutrition metrics
                    totalNutritionPlans,
                    activeNutritionPlans = activePlans,
                    nutritionGrowth = 0m, // Simplified
                    nutritionPlansLast30Days = nutritionLast30,

                    // Content metrics
                    totalExercises,
                    totalAliments,
                    totalPhotoAnalyses,
                    photoAnalysesLast30Days = photoAnalysesLast30,

                    // Engagement
                    engagementRate = Math.Round(engagementRate, 1),
                    activeUsersLast30Days = activeUserIds,

                    // Trends
                    monthlyTrends = monthlyData
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du chargement des statistiques", error = ex.Message });
            }
        }

        [HttpGet("recent-users")]
        public async Task<IActionResult> GetRecentUsers([FromQuery] int count = 10)
        {
            try
            {
                var users = await _context.Utilisateurs
                    .OrderByDescending(u => u.Id) // Use ID since no DateCreation
                    .Take(count)
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.NomComplet,
                        u.Role,
                        DateCreation = DateTime.Now // Placeholder
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du chargement des utilisateurs", error = ex.Message });
            }
        }

        [HttpGet("top-programs")]
        public async Task<IActionResult> GetTopPrograms([FromQuery] int count = 5)
        {
            try
            {
                var programs = await _context.Programmes
                    .Include(p => p.Coach)
                    .Select(p => new
                    {
                        p.Id,
                        p.Titre,
                        p.DureeSemaines,
                        p.IsPublic,
                        CoachName = p.Coach.NomComplet,
                        EnrollmentCount = _context.ProgrammeEnrollments.Count(e => e.ProgrammeId == p.Id),
                        SessionCount = _context.Seances.Count(s => s.ProgrammeId == p.Id)
                    })
                    .OrderByDescending(p => p.EnrollmentCount)
                    .Take(count)
                    .ToListAsync();

                return Ok(programs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du chargement des programmes", error = ex.Message });
            }
        }
    }
}

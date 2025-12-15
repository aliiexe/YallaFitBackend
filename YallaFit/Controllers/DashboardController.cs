using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YallaFit.Data;
using YallaFit.DTOs;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public DashboardController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
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

        // GET: api/dashboard/sportif
        [HttpGet("sportif")]
        public async Task<IActionResult> GetSportifDashboard()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userId = int.Parse(userIdClaim ?? "0");

                Console.WriteLine($"[DEBUG] Dashboard request for userId: {userId}");

                // Get user profile with assigned program
                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                        .ThenInclude(p => p.Programme)
                            .ThenInclude(prog => prog.Coach)
                    .Include(u => u.ProfilSportif)
                        .ThenInclude(p => p.Programme)
                            .ThenInclude(prog => prog.Seances)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    Console.WriteLine($"[DEBUG] User {userId} not found");
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                Console.WriteLine($"[DEBUG] User found: {user.NomComplet}, Profile exists: {user.ProfilSportif != null}");
                if (user.ProfilSportif != null)
                {
                    Console.WriteLine($"[DEBUG] Profile Goal: {user.ProfilSportif.ObjectifPrincipal}, Activity: {user.ProfilSportif.NiveauActivite}");
                    if (user.ProfilSportif.Programme != null)
                    {
                        Console.WriteLine($"[DEBUG] Assigned Program: {user.ProfilSportif.Programme.Titre}");
                    }
                }

                // Get latest biometric data
                var latestBiometric = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == userId)
                    .OrderByDescending(b => b.DateMesure)
                    .FirstOrDefaultAsync();

                // Get biometric history for trend (last 6 entries)
                var biometricHistory = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == userId)
                    .OrderByDescending(b => b.DateMesure)
                    .Take(6)
                    .OrderBy(b => b.DateMesure)
                    .Select(b => new
                    {
                        Date = b.DateMesure,
                        Weight = b.PoidsKg
                    })
                    .ToListAsync();

                // Calculate BMI if we have height and weight
                float? bmi = null;
                string? bmiCategory = null;
                if (user.ProfilSportif?.Taille != null && latestBiometric != null)
                {
                    var heightM = (float)user.ProfilSportif.Taille;
                    var weight = latestBiometric.PoidsKg;
                    bmi = weight / (heightM * heightM);

                    // Categorize BMI
                    if (bmi < 18.5) bmiCategory = "Insuffisance pondérale";
                    else if (bmi < 25) bmiCategory = "Normal";
                    else if (bmi < 30) bmiCategory = "Surpoids";
                    else bmiCategory = "Obésité";
                }

                // Calculate weight change (compare latest with first measurement)
                float? weightChange = null;
                if (biometricHistory.Count >= 2)
                {
                    weightChange = latestBiometric.PoidsKg - biometricHistory.First().Weight;
                }

                var dashboard = new
                {
                    Profile = new
                    {
                        Name = user.NomComplet,
                        Age = user.ProfilSportif?.Age,
                        Gender = user.ProfilSportif?.Sexe,
                        Height = user.ProfilSportif?.Taille,
                        Goal = user.ProfilSportif?.ObjectifPrincipal,
                        ActivityLevel = user.ProfilSportif?.NiveauActivite,
                        DietaryPreferences = user.ProfilSportif?.PreferencesAlim,
                        Allergies = user.ProfilSportif?.Allergies
                    },
                    Biometrics = new
                    {
                        CurrentWeight = latestBiometric?.PoidsKg,
                        WeightUnit = "kg",
                        BodyFatPercent = latestBiometric?.TauxMasseGrassePercent,
                        WaistCircumference = latestBiometric?.TourDeTailleCm,
                        LastMeasurement = latestBiometric?.DateMesure,
                        BMI = bmi != null ? (decimal?)Math.Round((decimal)bmi, 1) : null,
                        BMICategory = bmiCategory,
                        WeightChange = weightChange != null ? (decimal?)Math.Round((decimal)weightChange, 1) : null
                    },
                    WeightHistory = biometricHistory.Select((b, index) => new
                    {
                        Week = $"S{index + 1}",
                        Weight = b.Weight,
                        Date = b.Date
                    }).ToList(),
                    Stats = new
                    {
                        WeeklySessions = 0, // TODO: Implement when training tracking is added
                        WeeklyGoal = 5,
                        CaloriesBurned = 0, // TODO: Implement when training tracking is added
                        CurrentStreak = 0, // TODO: Implement when training tracking is added
                        RecordStreak = 0
                    },
                    AssignedProgram = user.ProfilSportif?.Programme != null ? new
                    {
                        Id = user.ProfilSportif.Programme.Id,
                        Titre = user.ProfilSportif.Programme.Titre,
                        CoachName = user.ProfilSportif.Programme.Coach?.NomComplet ?? "Coach Inconnu",
                        DureeSemaines = user.ProfilSportif.Programme.DureeSemaines,
                        SessionCount = user.ProfilSportif.Programme.Seances?.Count ?? 0
                    } : null
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du tableau de bord", error = ex.Message });
            }
        }

        // GET: api/dashboard/coach
        [HttpGet("coach")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> GetCoachDashboard()
        {
            try
            {
                var coachId = GetCurrentUserId();
                
                // Get coach stats
                var totalProgrammes = await _context.Programmes
                    .Where(p => p.CoachId == coachId)
                    .CountAsync();

                var totalAthletes = await _context.Utilisateurs
                    .Where(u => u.Role == "Sportif")
                    .CountAsync();

                var totalSessions = await _context.Seances
                    .Where(s => s.Programme.CoachId == coachId)
                    .CountAsync();

                // Get my athletes with their latest activity
                var allSportifs = await _context.Utilisateurs
                    .Where(u => u.Role == "Sportif")
                    .Include(u => u.ProfilSportif)
                    .Take(10)
                    .ToListAsync();

                var athletes = new List<dynamic>();
                foreach (var sportif in allSportifs)
                {
                    var latestBiometric = await _context.BiometriesActuelles
                        .Where(b => b.SportifId == sportif.Id)
                        .OrderByDescending(b => b.DateMesure)
                        .FirstOrDefaultAsync();

                    athletes.Add(new
                    {
                        Id = sportif.Id,
                        NomComplet = sportif.NomComplet,
                        Email = sportif.Email,
                        LastWeight = latestBiometric?.PoidsKg,
                        LastActive = latestBiometric?.DateMesure,
                        ObjectifPrincipal = sportif.ProfilSportif?.ObjectifPrincipal
                    });
                }

                // Calculate compliance (mock for now - would need actual session completion data)
                var athletesWithCompliance = athletes.Select(a => new
                {
                    a.Id,
                    a.NomComplet,
                    a.Email,
                    Program = a.ObjectifPrincipal ?? "Aucun programme",
                    Compliance = new Random().Next(70, 95), // Mock data
                    LastActive = a.LastActive != null 
                        ? GetTimeAgo(a.LastActive) 
                        : "Jamais"
                }).ToList();

                var dashboard = new
                {
                    Stats = new
                    {
                        TotalAthletes = totalAthletes,
                        ActiveProgrammes = totalProgrammes,
                        TotalSessions = totalSessions,
                        ComplianceRate = 85.0m // Mock
                    },
                    Athletes = athletesWithCompliance
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du tableau de bord coach", error = ex.Message });
            }
        }

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;
            
            if (timeSpan.TotalMinutes < 60)
                return $"Il y a {(int)timeSpan.TotalMinutes}min";
            else if (timeSpan.TotalHours < 24)
                return $"Il y a {(int)timeSpan.TotalHours}h";
            else if (timeSpan.TotalDays < 7)
                return $"Il y a {(int)timeSpan.TotalDays}j";
            else
                return $"Il y a {(int)(timeSpan.TotalDays / 7)} sem";
        }
    }
}

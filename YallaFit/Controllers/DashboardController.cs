using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YallaFit.Data;
using YallaFit.DTOs;
using YallaFit.Services;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly YallaFitDbContext _context;
        private readonly MacroCalculationService _macroService;

        public DashboardController(YallaFitDbContext context, MacroCalculationService macroService)
        {
            _context = context;
            _macroService = macroService;
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

                Console.WriteLine($"[DASHBOARD] Latest biometric for user {userId}: Weight={latestBiometric?.PoidsKg}kg, Date={latestBiometric?.DateMesure}");

                // Get biometric history for trend (last 6 entries)
                var biometricHistory = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == userId)
                    .OrderByDescending(b => b.DateMesure)
                    .Take(6)
                    .OrderBy(b => b.DateMesure)
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
                    weightChange = latestBiometric.PoidsKg - biometricHistory[biometricHistory.Count - 2].PoidsKg;
                }

                // Get week start (Monday)
                var today = DateTime.UtcNow.Date;
                var daysSinceMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
                var weekStart = today.AddDays(-daysSinceMonday);
                
                // Count sessions this week
                var weeklySessions = await _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId && ts.DateCompleted >= weekStart)
                    .CountAsync();

                // Calculate current streak
                var allSessions = await _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId)
                    .OrderByDescending(ts => ts.DateCompleted)
                    .Select(ts => ts.DateCompleted.Date)
                    .Distinct()
                    .ToListAsync();

                int currentStreak = 0;
                int recordStreak = 0;
                int tempStreak = 0;
                DateTime? lastDate = null;

                foreach (var sessionDate in allSessions)
                {
                    if (lastDate == null || (lastDate.Value - sessionDate).Days == 1)
                    {
                        tempStreak++;
                        if (lastDate == null && sessionDate == today)
                        {
                            currentStreak = tempStreak;
                        }
                    }
                    else if ((lastDate.Value - sessionDate).Days > 1)
                    {
                        if (currentStreak == 0) currentStreak = tempStreak;
                        tempStreak = 1;
                    }
                    
                    if (tempStreak > recordStreak) recordStreak = tempStreak;
                    lastDate = sessionDate;
                }

                // Calculate daily macro goals
                DailyMacroGoals? macroGoals = null;
                if (user.ProfilSportif != null && latestBiometric != null)
                {
                    macroGoals = _macroService.CalculateForProfile(user.ProfilSportif, latestBiometric.PoidsKg);
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
                        Allergies = user.ProfilSportif?.Allergies,
                        UserId = userId
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
                        Weight = b.PoidsKg,
                        Date = b.DateMesure,
                        BodyFatPercent = b.TauxMasseGrassePercent,
                        WaistCircumference = b.TourDeTailleCm
                    }).ToList(),
                    DailyMacroGoals = macroGoals != null ? new
                    {
                        Calories = macroGoals.Calories,
                        Protein = macroGoals.Protein,
                        Carbs = macroGoals.Carbs,
                        Fats = macroGoals.Fats
                    } : null,
                    Stats = new
                    {
                        WeeklySessions = weeklySessions,
                        WeeklyGoal = 5,
                        CaloriesBurned = 0, // TODO: Implement when calorie tracking per session is added
                        CurrentStreak = currentStreak,
                        RecordStreak = recordStreak
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
                var now = DateTime.Now;
                var last7Days = now.AddDays(-7);
                var last30Days = now.AddDays(-30);
                
                // Get coach's programs WITH seances loaded
                var coachPrograms = await _context.Programmes
                    .Where(p => p.CoachId == coachId)
                    .Include(p => p.Seances)
                    .ToListAsync();

                var totalProgrammes = coachPrograms.Count;
                var totalSeancesPlanned = coachPrograms.Sum(p => p.Seances.Count);

                // Get athletes enrolled in coach's programs
                var enrolledAthletes = await _context.ProgrammeEnrollments
                    .Where(e => coachPrograms.Select(p => p.Id).Contains(e.ProgrammeId) && e.IsActive)
                    .Select(e => e.SportifId)
                    .Distinct()
                    .ToListAsync();

                var totalAthletes = enrolledAthletes.Count;

                // Get all training sessions for coach's programs
                var allSessions = await _context.TrainingSessions
                    .Where(ts => coachPrograms.Select(p => p.Id).Contains(ts.ProgrammeId))
                    .ToListAsync();

                var totalSessionsCompleted = allSessions.Count;
                var sessionsLast7Days = allSessions.Count(ts => ts.DateCompleted >= last7Days);
                var averageDuration = allSessions.Any() ? (int)allSessions.Average(s => s.DurationMinutes) : 0;

                // Calculate overall compliance rate
                decimal complianceRate = 0;
                if (totalAthletes > 0 && totalSeancesPlanned > 0)
                {
                    var expectedSessions = totalAthletes * totalSeancesPlanned;
                    complianceRate = expectedSessions > 0 ? (totalSessionsCompleted / (decimal)expectedSessions) * 100 : 0;
                }

                // Weekly activity data (last 7 days)
                var weeklyActivity = new List<object>();
                for (int i = 6; i >= 0; i--)
                {
                    var day = now.AddDays(-i).Date;
                    var dayEnd = day.AddDays(1);
                    var sessionsOnDay = allSessions.Count(s => s.DateCompleted >= day && s.DateCompleted < dayEnd);
                    
                    weeklyActivity.Add(new
                    {
                        day = day.ToString("ddd"),
                        sessions = sessionsOnDay
                    });
                }

                // Program performance breakdown
                var programPerformance = new List<object>();
                foreach (var program in coachPrograms.Take(5))
                {
                    var programEnrollments = await _context.ProgrammeEnrollments
                        .CountAsync(e => e.ProgrammeId == program.Id && e.IsActive);
                    
                    var programSessions = allSessions.Count(s => s.ProgrammeId == program.Id);
                    var expectedSessionsForProgram = programEnrollments * program.Seances.Count;
                    var programCompletion = expectedSessionsForProgram > 0 
                        ? (programSessions / (decimal)expectedSessionsForProgram) * 100 
                        : 0;

                    programPerformance.Add(new
                    {
                        name = program.Titre,
                        enrollments = programEnrollments,
                        completionRate = Math.Round(programCompletion, 1),
                        totalSessions = programSessions
                    });
                }


                // Get detailed athlete info with real compliance rates
                var athletesWithStats = new List<object>();
                var athletesForSorting = new List<(int id, string nom, string email, string program, decimal compliance, int sessions, string lastActive)>();
                
                foreach (var athleteId in enrolledAthletes.Take(10))
                {
                    var sportif = await _context.Utilisateurs
                        .Include(u => u.ProfilSportif)
                        .FirstOrDefaultAsync(u => u.Id == athleteId);

                    if (sportif == null) continue;

                    // Get athlete's enrollment to find their program (with Seances loaded)
                    var enrollment = await _context.ProgrammeEnrollments
                        .Where(e => e.SportifId == athleteId && e.IsActive)
                        .Include(e => e.Programme)
                            .ThenInclude(p => p.Seances)
                        .FirstOrDefaultAsync();

                    // Get athlete's training sessions
                    var athleteSessions = allSessions
                        .Where(ts => ts.SportifId == athleteId)
                        .ToList();

                    // Get athlete's last activity
                    var lastSession = athleteSessions
                        .OrderByDescending(ts => ts.DateCompleted)
                        .FirstOrDefault();

                    // Calculate individual compliance rate
                    var programSeances = enrollment?.Programme?.Seances?.Count ?? 0;
                    var athleteCompliance = programSeances > 0 
                        ? (athleteSessions.Count / (decimal)programSeances) * 100 
                        : 0;

                    // Cap compliance at 100%
                    athleteCompliance = Math.Min(athleteCompliance, 100);
                    var roundedCompliance = Math.Round(athleteCompliance, 0);
                    var lastActiveStr = lastSession != null ? GetTimeAgo(lastSession.DateCompleted) : "Jamais";
                    var programName = enrollment?.Programme?.Titre ?? "Aucun programme actif";

                    athletesWithStats.Add(new
                    {
                        id = sportif.Id,
                        nomComplet = sportif.NomComplet,
                        email = sportif.Email,
                        program = programName,
                        compliance = roundedCompliance,
                        sessionsCompleted = athleteSessions.Count,
                        lastActive = lastActiveStr
                    });
                    
                    athletesForSorting.Add((sportif.Id, sportif.NomComplet, sportif.Email, programName, roundedCompliance, athleteSessions.Count, lastActiveStr));
                }

                // Sort and get top athletes
                var topAthletesData = athletesForSorting
                    .OrderByDescending(a =>a.compliance)
                    .Take(5)
                    .Select(a => new {
                        id = a.id,
                        nomComplet = a.nom,
                        email = a.email,
                        program = a.program,
                        compliance = a.compliance,
                        sessionsCompleted = a.sessions,
                        lastActive = a.lastActive
                    })
                    .ToList();

                var dashboard = new
                {
                    stats = new
                    {
                        totalAthletes,
                        activeProgrammes = totalProgrammes,
                        totalSessions = totalSessionsCompleted,
                        complianceRate = Math.Round(complianceRate, 1),
                        sessionsLast7Days,
                        totalSeancesPlanned,
                        averageSessionDuration = averageDuration
                    },
                    athletes = athletesWithStats,
                    topAthletes = topAthletesData,
                    weeklyActivity,
                    programPerformance
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

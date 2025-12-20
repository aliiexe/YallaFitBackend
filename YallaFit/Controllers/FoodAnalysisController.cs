using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using YallaFit.Data;
using YallaFit.DTOs;
using YallaFit.Models;
using YallaFit.Services;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodAnalysisController : ControllerBase
    {
        private readonly YallaFitDbContext _context;
        private readonly MistralVisionService _visionService;
        private readonly IWebHostEnvironment _environment;

        public FoodAnalysisController(
            YallaFitDbContext context,
            MistralVisionService visionService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _visionService = visionService;
            _environment = environment;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // POST: api/foodanalysis/analyze-photo
        [HttpPost("analyze-photo")]
        public async Task<IActionResult> AnalyzePhoto([FromForm] IFormFile photo)
        {
            try
            {
                if (photo == null || photo.Length == 0)
                {
                    return BadRequest(new { message = "Aucune photo fournie" });
                }

                // Validate image type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Format d'image non supporté. Utilisez JPG ou PNG." });
                }

                // Validate file size (max 10MB)
                if (photo.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image trop grande. Maximum 10MB." });
                }

                var userId = GetCurrentUserId();

                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "food-photos");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_{Path.GetFileName(photo.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save photo to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Convert to base64 for AI
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    photo.OpenReadStream().CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                var base64Image = Convert.ToBase64String(imageBytes);

                // Analyze with AI
                var aiResponse = await _visionService.AnalyzeFoodPhoto(base64Image);

                // Clean JSON response
                var cleanedResponse = aiResponse.Trim();
                if (cleanedResponse.StartsWith("```"))
                {
                    var lines = cleanedResponse.Split('\n');
                    cleanedResponse = string.Join('\n', lines.Skip(1).TakeWhile(l => !l.StartsWith("```")));
                }

                var jsonStart = cleanedResponse.IndexOf('{');
                var jsonEnd = cleanedResponse.LastIndexOf('}') + 1;
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    cleanedResponse = cleanedResponse.Substring(jsonStart, jsonEnd - jsonStart);
                }

                // Parse AI response
                var aiAnalysis = JsonSerializer.Deserialize<AIFoodAnalysisResponse>(cleanedResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (aiAnalysis == null || aiAnalysis.Foods == null || aiAnalysis.Foods.Count == 0)
                {
                    return BadRequest(new { message = "L'IA n'a pas pu analyser cette photo" });
                }

                // Save to database
                var analysis = new AnalyseRepasPhoto
                {
                    SportifId = userId,
                    DateAnalyse = DateTime.UtcNow,
                    CheminPhoto = $"/uploads/food-photos/{fileName}",
                    AlimentsDetectes = JsonSerializer.Serialize(aiAnalysis.Foods),
                    CaloriesEstimees = aiAnalysis.TotalNutrition.Calories,
                    ProteinesEstimees = aiAnalysis.TotalNutrition.Protein,
                    GlucidesEstimees = aiAnalysis.TotalNutrition.Carbs,
                    LipidesEstimees = aiAnalysis.TotalNutrition.Fats,
                    AnalyseIA = aiAnalysis.Analysis,
                    Recommandations = aiAnalysis.Recommendations
                };

                _context.AnalysesRepasPhoto.Add(analysis);
                await _context.SaveChangesAsync();

                // Return response
                return Ok(new FoodAnalysisResponse
                {
                    Id = analysis.Id,
                    Foods = aiAnalysis.Foods,
                    TotalNutrition = aiAnalysis.TotalNutrition,
                    Analysis = aiAnalysis.Analysis,
                    Recommendations = aiAnalysis.Recommendations,
                    DateAnalyse = analysis.DateAnalyse,
                    PhotoPath = analysis.CheminPhoto
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] AnalyzePhoto failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[Error] Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { message = "Erreur lors de l'analyse", error = ex.Message });
            }
        }

        // GET: api/foodanalysis/my-analyses
        [HttpGet("my-analyses")]
        [Authorize(Roles = "Sportif")]
        public async Task<IActionResult> GetMyAnalyses()
        {
            try
            {
                var userId = GetCurrentUserId();

                var analyses = await _context.AnalysesRepasPhoto
                    .Where(a => a.SportifId == userId)
                    .OrderByDescending(a => a.DateAnalyse)
                    .ToListAsync();

                var results = analyses.Select(a => new FoodAnalysisSummaryDto
                {
                    Id = a.Id,
                    DateAnalyse = a.DateAnalyse,
                    PhotoPath = a.CheminPhoto,
                    CaloriesEstimees = a.CaloriesEstimees,
                    ProteinesEstimees = a.ProteinesEstimees,
                    FoodCount = !string.IsNullOrEmpty(a.AlimentsDetectes) ? 
                        JsonSerializer.Deserialize<List<DetectedFoodDto>>(a.AlimentsDetectes)!.Count : 0
                }).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération", error = ex.Message });
            }
        }

        // GET: api/foodanalysis/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnalysis(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var analysis = await _context.AnalysesRepasPhoto
                    .Where(a => a.Id == id && a.SportifId == userId)
                    .FirstOrDefaultAsync();

                if (analysis == null)
                {
                    return NotFound(new { message = "Analyse non trouvée" });
                }

                var foods = JsonSerializer.Deserialize<List<DetectedFoodDto>>(analysis.AlimentsDetectes ?? "[]");

                return Ok(new FoodAnalysisResponse
                {
                    Id = analysis.Id,
                    Foods = foods ?? new(),
                    TotalNutrition = new TotalNutritionDto
                    {
                        Calories = analysis.CaloriesEstimees,
                        Protein = analysis.ProteinesEstimees,
                        Carbs = analysis.GlucidesEstimees,
                        Fats = analysis.LipidesEstimees
                    },
                    Analysis = analysis.AnalyseIA ?? "",
                    Recommendations = analysis.Recommandations ?? "",
                    DateAnalyse = analysis.DateAnalyse,
                    PhotoPath = analysis.CheminPhoto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération", error = ex.Message });
            }
        }

        // DELETE: api/foodanalysis/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnalysis(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var analysis = await _context.AnalysesRepasPhoto
                    .Where(a => a.Id == id && a.SportifId == userId)
                    .FirstOrDefaultAsync();

                if (analysis == null)
                {
                    return NotFound(new { message = "Analyse non trouvée" });
                }

                // Delete photo file
                var photoPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", analysis.CheminPhoto.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }

                _context.AnalysesRepasPhoto.Remove(analysis);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Analyse supprimée" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression", error = ex.Message });
            }
        }

        // GET: api/foodanalysis/today
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayNutrition()
        {
            try
            {
                var userId = GetCurrentUserId();
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);

                var todayAnalyses = await _context.AnalysesRepasPhoto
                    .Where(a => a.SportifId == userId && a.DateAnalyse >= today && a.DateAnalyse < tomorrow)
                    .ToListAsync();

                var summary = new DailyNutritionSummaryDto
                {
                    Date = today,
                    TotalCalories = todayAnalyses.Sum(a => a.CaloriesEstimees),
                    TotalProtein = todayAnalyses.Sum(a => a.ProteinesEstimees),
                    TotalCarbs = todayAnalyses.Sum(a => a.GlucidesEstimees),
                    TotalFats = todayAnalyses.Sum(a => a.LipidesEstimees),
                    MealCount = todayAnalyses.Count
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération", error = ex.Message });
            }
        }

        // GET: api/foodanalysis/history?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("history")]
        public async Task<IActionResult> GetNutritionHistory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var start = startDate ?? DateTime.UtcNow.AddDays(-30).Date;
                var end = endDate ?? DateTime.UtcNow.Date.AddDays(1);

                var analyses = await _context.AnalysesRepasPhoto
                    .Where(a => a.SportifId == userId && a.DateAnalyse >= start && a.DateAnalyse < end)
                    .ToListAsync();

                var dailySummaries = analyses
                    .GroupBy(a => a.DateAnalyse.Date)
                    .Select(g => new DailyNutritionSummaryDto
                    {
                        Date = g.Key,
                        TotalCalories = g.Sum(a => a.CaloriesEstimees),
                        TotalProtein = g.Sum(a => a.ProteinesEstimees),
                        TotalCarbs = g.Sum(a => a.GlucidesEstimees),
                        TotalFats = g.Sum(a => a.LipidesEstimees),
                        MealCount = g.Count()
                    })
                    .OrderByDescending(s => s.Date)
                    .ToList();

                return Ok(new NutritionHistoryDto { DailySummaries = dailySummaries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération", error = ex.Message });
            }
        }

        // GET: api/foodanalysis/daily/2024-12-20
        [HttpGet("daily/{date}")]
        public async Task<IActionResult> GetDailyAnalyses(DateTime date)
        {
            try
            {
                var userId = GetCurrentUserId();
                var dayStart = date.Date;
                var dayEnd = dayStart.AddDays(1);

                var analyses = await _context.AnalysesRepasPhoto
                    .Where(a => a.SportifId == userId && a.DateAnalyse >= dayStart && a.DateAnalyse < dayEnd)
                    .OrderBy(a => a.DateAnalyse)
                    .ToListAsync();

                var summaries = analyses.Select(a => new FoodAnalysisSummaryDto
                {
                    Id = a.Id,
                    DateAnalyse = a.DateAnalyse,
                    PhotoPath = a.CheminPhoto,
                    CaloriesEstimees = a.CaloriesEstimees,
                    ProteinesEstimees = a.ProteinesEstimees,
                    FoodCount = !string.IsNullOrEmpty(a.AlimentsDetectes) ?
                        JsonSerializer.Deserialize<List<DetectedFoodDto>>(a.AlimentsDetectes)!.Count : 0
                }).ToList();

                var summary = new DailyNutritionSummaryDto
                {
                    Date = dayStart,
                    TotalCalories = analyses.Sum(a => a.CaloriesEstimees),
                    TotalProtein = analyses.Sum(a => a.ProteinesEstimees),
                    TotalCarbs = analyses.Sum(a => a.GlucidesEstimees),
                    TotalFats = analyses.Sum(a => a.LipidesEstimees),
                    MealCount = analyses.Count
                };

                return Ok(new DailyAnalysesDto
                {
                    Date = dayStart,
                    Analyses = summaries,
                    Summary = summary
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération", error = ex.Message });
            }
        }
    }
}

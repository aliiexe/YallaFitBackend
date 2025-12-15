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
    }
}

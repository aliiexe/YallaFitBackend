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
    public class NutritionController : ControllerBase
    {
        private readonly YallaFitDbContext _context;
        private readonly MistralAIService _mistralService;

        public NutritionController(YallaFitDbContext context, MistralAIService mistralService)
        {
            _context = context;
            _mistralService = mistralService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // POST: api/nutrition/generate-plan
        [HttpPost("generate-plan")]
        public async Task<IActionResult> GenerateMealPlan([FromBody] GenerateMealPlanRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get user profile for context
                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user?.ProfilSportif == null)
                {
                    return BadRequest(new { message = "Profil sportif non trouvé" });
                }

                // Prepare AI request with ALL biometric data
                var aiRequest = new YallaFit.Services.MealPlanRequestInternal
                {
                    Age = user.ProfilSportif.Age ?? 30,
                    Gender = user.ProfilSportif.Genre ?? user.ProfilSportif.Sexe,
                    Weight = (float)(user.ProfilSportif.Poids ?? 70m),
                    Height = (float)(user.ProfilSportif.Taille ?? 1.75m),
                    Goal = user.ProfilSportif.ObjectifPrincipal ?? "maintenance",
                    ActivityLevel = user.ProfilSportif.NiveauActivite ?? "moderate",
                    DietaryRestrictions = request.DietaryRestrictions ?? user.ProfilSportif.PreferencesAlim,
                    Allergies = user.ProfilSportif.Allergies,
                    FoodPreferences = user.ProfilSportif.PreferencesAlim,
                    HealthIssues = user.ProfilSportif.ProblemesSante,
                    NumberOfMeals = request.NumberOfMeals,
                    TargetCalories = request.TargetCalories ?? 2000,
                    TargetProtein = request.TargetProtein ?? 150,
                    TargetCarbs = request.TargetCarbs ?? 250,
                    TargetFats = request.TargetFats ?? 65
                };

                // Generate meal plan with AI
                var aiResponse = await _mistralService.GenerateMealPlan(aiRequest);

                // Clean up response (remove markdown if present)
                var cleanedResponse = aiResponse.Trim();
                if (cleanedResponse.StartsWith("```"))
                {
                    // Remove markdown code blocks
                    var lines = cleanedResponse.Split('\n');
                    cleanedResponse = string.Join('\n', lines.Skip(1).TakeWhile(l => !l.StartsWith("```")));
                }

                var jsonStart = cleanedResponse.IndexOf('{');
                var jsonEnd = cleanedResponse.LastIndexOf('}') + 1;
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    cleanedResponse = cleanedResponse.Substring(jsonStart, jsonEnd - jsonStart);
                }

                // Parse AI response with new structure
                var aiPlanResponse = JsonSerializer.Deserialize<AIMealPlanResponse>(cleanedResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (aiPlanResponse?.Meals == null || aiPlanResponse.Meals.Count == 0)
                {
                    return BadRequest(new { message = "Erreur lors de la génération du plan" });
                }

                // Create nutrition plan in database WITH AI analysis
                var nutritionPlan = new PlanNutrition
                {
                    SportifId = userId,
                    DateGeneration = DateTime.UtcNow,
                    ObjectifCaloriqueJournalier = aiRequest.TargetCalories,
                    ObjectifProteinesG = aiRequest.TargetProtein,
                    ObjectifGlucidesG = aiRequest.TargetCarbs,
                    ObjectifLipidesG = aiRequest.TargetFats,
                    EstActif = true,
                    AnalyseGlobale = aiPlanResponse.OverallAnalysis,
                    ConseilsPersonnalises = aiPlanResponse.PersonalizedAdvice
                };

                _context.PlansNutrition.Add(nutritionPlan);
                await _context.SaveChangesAsync();

                // Add meals to the plan WITH explanations AND FOOD ITEMS
                foreach (var meal in aiPlanResponse.Meals)
                {
                    var repas = new Repas
                    {
                        PlanId = nutritionPlan.Id,
                        Nom = meal.Meal,
                        HeurePrevue = TimeSpan.Parse(meal.Time),
                        Explication = meal.Explanation,
                        BeneficesNutritionnels = meal.NutritionalBenefits
                    };

                    _context.Repas.Add(repas);
                    await _context.SaveChangesAsync(); // Save to get repas.Id

                    // Add food items for this meal
                    foreach (var food in meal.Foods)
                    {
                        try
                        {
                            // Try to find existing aliment or create new one
                            var aliment = await _context.Aliments
                                .FirstOrDefaultAsync(a => a.Nom.ToLower() == food.Name.ToLower());

                            if (aliment == null)
                            {
                                // Parse quantity safely - extract numbers only
                                var quantityStr = new string(food.Quantity.Where(char.IsDigit).ToArray());
                                if (string.IsNullOrEmpty(quantityStr))
                                {
                                    // Default to 100g if we can't parse
                                    quantityStr = "100";
                                }
                                var quantity = int.Parse(quantityStr);

                                // Create new aliment entry with calculated per-100g values
                                aliment = new Aliment
                                {
                                    Nom = food.Name,
                                    Calories100g = quantity > 0 ? (int)(food.Calories * 100.0 / quantity) : (int)food.Calories,
                                    Proteines100g = quantity > 0 ? food.Protein * 100.0f / quantity : food.Protein,
                                    Glucides100g = quantity > 0 ? food.Carbs * 100.0f / quantity : food.Carbs,
                                    Lipides100g = quantity > 0 ? food.Fats * 100.0f / quantity : food.Fats
                                };
                                _context.Aliments.Add(aliment);
                                await _context.SaveChangesAsync();
                            }

                            // Parse quantity for composition
                            var compositionQuantityStr = new string(food.Quantity.Where(char.IsDigit).ToArray());
                            var compositionQuantity = string.IsNullOrEmpty(compositionQuantityStr) ? 100 : int.Parse(compositionQuantityStr);

                            // Add composition entry
                            var composition = new CompositionRepas
                            {
                                RepasId = repas.Id,
                                AlimentId = aliment.Id,
                                QuantiteGrammes = compositionQuantity
                            };
                            _context.CompositionRepas.Add(composition);
                        }
                        catch (Exception ex)
                        {
                            // Log error but continue with other foods
                            Console.WriteLine($"Error saving food '{food.Name}': {ex.Message}");
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(new GenerateMealPlanResponse
                {
                    PlanId = nutritionPlan.Id,
                    Meals = aiPlanResponse.Meals,
                    Message = "Plan nutritionnel généré avec succès!",
                    OverallAnalysis = aiPlanResponse.OverallAnalysis,
                    PersonalizedAdvice = aiPlanResponse.PersonalizedAdvice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la génération du plan", error = ex.Message });
            }
        }

        // POST: api/nutrition/calculate-macros
        [HttpPost("calculate-macros")]
        public async Task<IActionResult> CalculateMacros([FromBody] CalculateMacrosRequest request)
        {
            try
            {
                // Call AI to calculate macros
                var aiResponse = await _mistralService.CalculateMacros(request);

                // Clean JSON if needed
                var jsonStart = aiResponse.IndexOf('{');
                var jsonEnd = aiResponse.LastIndexOf('}') + 1;
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    aiResponse = aiResponse.Substring(jsonStart, jsonEnd - jsonStart);
                }

                var macros = JsonSerializer.Deserialize<MacrosResponse>(aiResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(macros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du calcul des macros", error = ex.Message });
            }
        }

        // GET: api/nutrition/my-plans
        [HttpGet("my-plans")]
        [Authorize(Roles = "Sportif")]
        public async Task<IActionResult> GetMyPlans()
        {
            try
            {
                var userId = GetCurrentUserId();

                var plans = await _context.PlansNutrition
                    .Where(p => p.SportifId == userId)
                    .OrderByDescending(p => p.DateGeneration)
                    .Select(p => new
                    {
                        p.Id,
                        p.DateGeneration,
                        p.ObjectifCaloriqueJournalier,
                        p.ObjectifProteinesG,
                        p.ObjectifGlucidesG,
                        p.ObjectifLipidesG,
                        p.AnalyseGlobale,
                        p.ConseilsPersonnalises,
                        MealCount = p.Repas.Count
                    })
                    .ToListAsync();

                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des plans", error = ex.Message });
            }
        }

        // GET: api/nutrition/plan/{id}
        [HttpGet("plan/{id}")]
        [Authorize(Roles = "Sportif")]
        public async Task<IActionResult> GetPlanDetails(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var plan = await _context.PlansNutrition
                    .Include(p => p.Repas)
                        .ThenInclude(r => r.CompositionRepas)
                        .ThenInclude(c => c.Aliment)
                    .Where(p => p.Id == id && p.SportifId == userId)
                    .FirstOrDefaultAsync();

                if (plan == null)
                {
                    return NotFound(new { message = "Plan non trouvé" });
                }

                var meals = plan.Repas.Select(r => new MealDto
                {
                    Meal = r.Nom,
                    Time = r.HeurePrevue.ToString(@"hh\:mm"),
                    Explanation = r.Explication,
                    NutritionalBenefits = r.BeneficesNutritionnels,
                    Foods = r.CompositionRepas.Select(c => new FoodItemDto
                    {
                        Name = c.Aliment.Nom,
                        Quantity = $"{c.QuantiteGrammes}g",
                        Calories = (int)(c.Aliment.Calories100g * c.QuantiteGrammes / 100.0),
                        Protein = c.Aliment.Proteines100g * c.QuantiteGrammes / 100.0f,
                        Carbs = c.Aliment.Glucides100g * c.QuantiteGrammes / 100.0f,
                        Fats = c.Aliment.Lipides100g * c.QuantiteGrammes / 100.0f
                    }).ToList(),
                    TotalMacros = new MacroTotalsDto
                    {
                        Calories = (int)r.CompositionRepas.Sum(c => c.Aliment.Calories100g * c.QuantiteGrammes / 100.0),
                        Protein = r.CompositionRepas.Sum(c => c.Aliment.Proteines100g * c.QuantiteGrammes / 100.0f),
                        Carbs = r.CompositionRepas.Sum(c => c.Aliment.Glucides100g * c.QuantiteGrammes / 100.0f),
                        Fats = r.CompositionRepas.Sum(c => c.Aliment.Lipides100g * c.QuantiteGrammes / 100.0f)
                    }
                }).ToList();

                return Ok(new GenerateMealPlanResponse
                {
                    PlanId = plan.Id,
                    Meals = meals,
                    Message = "Plan récupéré avec succès",
                    OverallAnalysis = plan.AnalyseGlobale,
                    PersonalizedAdvice = plan.ConseilsPersonnalises
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du plan", error = ex.Message });
            }
        }


        // GET: api/nutrition/plans/{id}
        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetPlan(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var plan = await _context.PlansNutrition
                    .Where(p => p.Id == id && p.SportifId == userId)
                    .Include(p => p.Repas)
                    .ThenInclude(r => r.CompositionRepas)
                    .ThenInclude(c => c.Aliment)
                    .FirstOrDefaultAsync();

                if (plan == null)
                {
                    return NotFound(new { message = "Plan non trouvé" });
                }

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du plan", error = ex.Message });
            }
        }
    }

    // Helper DTO for parsing AI response
    public class AIMealPlanResponse
    {
        public List<MealDto> Meals { get; set; } = new();
        public string? OverallAnalysis { get; set; }
        public string? PersonalizedAdvice { get; set; }
    }
}

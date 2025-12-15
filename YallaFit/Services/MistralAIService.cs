using System.Net.Http.Json;
using System.Text.Json;
using YallaFit.DTOs;

namespace YallaFit.Services
{
    public class MistralAIService
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "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";
        private const string BASE_URL = "https://openrouter.ai/api/v1";
        private const string MODEL = "mistralai/devstral-2512:free";

        public MistralAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://yallafit.com");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "YallaFit Nutrition AI");
        }

        public async Task<string> GenerateMealPlan(MealPlanRequestInternal request)
        {
            var prompt = BuildMealPlanPrompt(request);

            var payload = new
            {
                model = MODEL,
                messages = new[]
                {
                    new { role = "system", content = "You are a certified sports nutritionist specializing in personalized meal planning. Always return valid JSON." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 3000
            };

            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/chat/completions", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
            return result?.Choices?[0]?.Message?.Content ?? "";
        }

        public async Task<string> CalculateMacros(CalculateMacrosRequest request)
        {
            var prompt = $@"
Calculate optimal daily macros for an athlete using proven formulas:

Profile:
- Age: {request.Age}, Gender: {request.Gender}
- Weight: {request.Weight}kg, Height: {request.Height}cm
- Goal: {request.Goal}
- Activity Level: {request.ActivityLevel}
- Training Days/Week: {request.TrainingDaysPerWeek}

Use Mifflin-St Jeor equation and activity multipliers.
Return ONLY this JSON (no markdown, no explanation):
{{
  ""calories"": 2500,
  ""protein"": 150,
  ""carbs"": 280,
  ""fats"": 70,
  ""explanation"": ""Based on your stats, this supports {request.Goal}""
}}
";

            var payload = new
            {
                model = MODEL,
                messages = new[]
                {
                    new { role = "system", content = "You are a nutrition calculator. Return only JSON." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3,
                max_tokens = 500
            };

            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/chat/completions", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
            return result?.Choices?[0]?.Message?.Content ?? "{}";
        }

        private string BuildMealPlanPrompt(MealPlanRequestInternal request)
        {
            return $@"
Vous êtes un nutritionniste sportif expert créant un plan de repas personnalisé avec des explications détaillées EN FRANÇAIS.

PROFIL COMPLET DE L'ATHLÈTE:
- Âge: {request.Age} ans
- Genre: {request.Gender ?? "Non spécifié"}
- Poids: {request.Weight}kg
- Taille: {request.Height}m ({request.Height * 100}cm)
- Niveau d'activité: {request.ActivityLevel}
- Objectif principal: {request.Goal}
- Problèmes de santé: {request.HealthIssues ?? "Aucun"}
- Allergies: {request.Allergies ?? "Aucune"}
- Préférences alimentaires: {request.FoodPreferences ?? "Aucune"}
- Restrictions alimentaires: {request.DietaryRestrictions ?? "Aucune"}

OBJECTIFS MACROS JOURNALIERS:
- Calories: {request.TargetCalories} kcal
- Protéines: {request.TargetProtein}g ({(request.TargetProtein * 4.0 / request.TargetCalories * 100):F1}% des calories)
- Glucides: {request.TargetCarbs}g ({(request.TargetCarbs * 4.0 / request.TargetCalories * 100):F1}% des calories)
- Lipides: {request.TargetFats}g ({(request.TargetFats * 9.0 / request.TargetCalories * 100):F1}% des calories)

EXIGENCES:
1. Créer exactement {request.NumberOfMeals} repas
2. Atteindre les objectifs macros à ±5%
3. Utiliser des aliments communs disponibles sur le marché
4. Inclure des portions pratiques
5. Respecter TOUTES les restrictions et allergies
6. Considérer l'objectif de l'athlète ({request.Goal})
7. Fournir des EXPLICATIONS DÉTAILLÉES EN FRANÇAIS pour chaque repas
8. Inclure les bénéfices nutritionnels

IMPORTANT: Répondre UNIQUEMENT en FRANÇAIS. Retourner UNIQUEMENT du JSON valide (PAS de markdown, PAS de blocs de code):
{{
  ""meals"": [
    {{
      ""meal"": ""Petit-déjeuner"",
      ""time"": ""07:00"",
      ""explanation"": ""Ce petit-déjeuner fournit une énergie soutenue pour votre entraînement matinal et soutient {request.Goal}. La combinaison de glucides complexes et de protéines aide à la récupération musculaire."",
      ""nutritionalBenefits"": ""L'avoine fournit de l'énergie à libération lente, la banane offre des glucides rapides et du potassium pour la fonction musculaire, le beurre d'arachide ajoute des graisses saines et des protéines pour la satiété."",
      ""foods"": [
        {{""name"": ""Flocons d'avoine"", ""quantity"": ""80g"", ""calories"": 296, ""protein"": 10.7, ""carbs"": 51.8, ""fats"": 5.4}},
        {{""name"": ""Banane"", ""quantity"": ""1 moyenne (120g)"", ""calories"": 105, ""protein"": 1.3, ""carbs"": 27, ""fats"": 0.4}},
        {{""name"": ""Beurre d'arachide"", ""quantity"": ""20g"", ""calories"": 120, ""protein"": 5, ""carbs"": 4, ""fats"": 10}}
      ],
      ""totalMacros"": {{""calories"": 521, ""protein"": 17, ""carbs"": 82.8, ""fats"": 15.8}}
    }}
  ],
  ""overallAnalysis"": ""Ce plan de {request.NumberOfMeals} repas est spécifiquement conçu pour un(e) athlète {request.Gender} de {request.Age} ans pesant {request.Weight}kg avec comme objectif {request.Goal}. La répartition des macros soutient votre niveau d'activité {request.ActivityLevel} tout en respectant vos besoins alimentaires."",
  ""personalizedAdvice"": ""Selon votre profil: 1) Restez hydraté (visez 2,5-3L d'eau par jour), 2) Prenez votre repas pré-entraînement 1-2 heures avant, 3) Consommez des protéines dans les 30 minutes post-entraînement, 4) Surveillez vos niveaux d'énergie et ajustez les portions si nécessaire.""
}}

CRITIQUE: Retourner UNIQUEMENT l'objet JSON EN FRANÇAIS. Aucun texte explicatif avant ou après.
";
        }
    }

    // Internal request DTO for AI service
    public class MealPlanRequestInternal
    {
        public int Age { get; set; }
        public string? Gender { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;
        public string? DietaryRestrictions { get; set; }
        public string? Allergies { get; set; }
        public string? FoodPreferences { get; set; }
        public string? HealthIssues { get; set; }
        public int NumberOfMeals { get; set; } = 3;
        public int TargetCalories { get; set; }
        public int TargetProtein { get; set; }
        public int TargetCarbs { get; set; }
        public int TargetFats { get; set; }
    }

    // Response DTOs for AI
    public class ChatCompletionResponse
    {
        public List<Choice> Choices { get; set; } = new();
    }

    public class Choice
    {
        public Message Message { get; set; } = new();
    }

    public class Message
    {
        public string Content { get; set; } = string.Empty;
    }
}

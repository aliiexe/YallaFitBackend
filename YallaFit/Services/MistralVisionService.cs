using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using YallaFit.DTOs;

namespace YallaFit.Services
{
    public class MistralVisionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BASE_URL = "https://openrouter.ai/api/v1";
        private const string VISION_MODEL = "google/gemini-2.0-flash-exp:free";

        public MistralVisionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouter:ApiKey"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://yallafit.com");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "YallaFit Food Analysis");
        }

        public async Task<string> AnalyzeFoodPhoto(string base64Image)
        {
            var prompt = BuildFoodAnalysisPrompt();

            var payload = new
            {
                model = VISION_MODEL,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = prompt },
                            new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                        }
                    }
                },
                temperature = 0.3,
                max_tokens = 2000
            };

            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/chat/completions", payload);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenRouter API Error ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
            return result?.Choices?[0]?.Message?.Content ?? "{}";
        }

        private string BuildFoodAnalysisPrompt()
        {
            return @"
Analysez cette photo de repas en français de manière détaillée.

Identifiez:
1. Tous les aliments visibles dans l'assiette
2. Les quantités estimées de chaque aliment (en grammes)
3. Les valeurs nutritionnelles de chaque aliment
4. Le total nutritionnel du repas
5. Une analyse de la qualité nutritionnelle
6. Des recommandations pour améliorer le repas

IMPORTANT: Répondez UNIQUEMENT en JSON, PAS de texte avant ou après.

Format JSON attendu:
{
  ""foods"": [
    {
      ""name"": ""Poulet grillé"",
      ""quantity"": ""150g"",
      ""calories"": 248,
      ""protein"": 46,
      ""carbs"": 0,
      ""fats"": 5
    },
    {
      ""name"": ""Riz blanc"",
      ""quantity"": ""200g"",
      ""calories"": 260,
      ""protein"": 5,
      ""carbs"": 58,
      ""fats"": 0.5
    }
  ],
  ""totalNutrition"": {
    ""calories"": 508,
    ""protein"": 51,
    ""carbs"": 58,
    ""fats"": 5.5
  },
  ""analysis"": ""Repas équilibré avec un excellent ratio protéines/glucides. Bon choix pour la récupération après l'entraînement. La quantité de protéines est idéale pour la construction musculaire."",
  ""recommendations"": ""Pour améliorer ce repas: 1) Ajouter des légumes verts (brocoli ou épinards) pour augmenter l'apport en fibres et micronutriments. 2) Remplacer le riz blanc par du riz complet pour un meilleur contrôle glycémique. 3) Ajouter un peu d'huile d'olive pour des graisses saines.""
}

CRITIQUE: Retourner UNIQUEMENT l'objet JSON en français. Soyez précis dans les estimations.
";
        }
    }

    // Response DTO is shared from MistralAIService
}

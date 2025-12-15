namespace YallaFit.DTOs
{
    // Food Analysis Request/Response

    public class FoodAnalysisRequest
    {
        // Photo will come as IFormFile in controller
    }

    public class DetectedFoodDto
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public int Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
    }

    public class TotalNutritionDto
    {
        public int Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
    }

    public class FoodAnalysisResponse
    {
        public int Id { get; set; }
        public List<DetectedFoodDto> Foods { get; set; } = new();
        public TotalNutritionDto TotalNutrition { get; set; } = new();
        public string Analysis { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public DateTime DateAnalyse { get; set; }
        public string PhotoPath { get; set; } = string.Empty;
    }

    public class FoodAnalysisSummaryDto
    {
        public int Id { get; set; }
        public DateTime DateAnalyse { get; set; }
        public string PhotoPath { get; set; } = string.Empty;
        public int CaloriesEstimees { get; set; }
        public float ProteinesEstimees { get; set; }
        public int FoodCount { get; set; }
    }

    // Internal DTO for AI response parsing
    public class AIFoodAnalysisResponse
    {
        public List<DetectedFoodDto> Foods { get; set; } = new();
        public TotalNutritionDto TotalNutrition { get; set; } = new();
        public string Analysis { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
    }
}

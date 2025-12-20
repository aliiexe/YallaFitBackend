using System;

namespace YallaFit.DTOs
{
    // Summary of nutrition totals for a single day
    public class DailyNutritionSummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalCalories { get; set; }
        public float TotalProtein { get; set; }
        public float TotalCarbs { get; set; }
        public float TotalFats { get; set; }
        public int MealCount { get; set; }
    }

    // Response wrapper for nutrition history
    public class NutritionHistoryDto
    {
        public List<DailyNutritionSummaryDto> DailySummaries { get; set; } = new();
    }

    // Detailed view of a specific day with individual analyses
    public class DailyAnalysesDto
    {
        public DateTime Date { get; set; }
        public List<FoodAnalysisSummaryDto> Analyses { get; set; } = new();
        public DailyNutritionSummaryDto Summary { get; set; } = new();
    }
}

namespace YallaFit.DTOs
{
    // Meal Plan Generation
    public class GenerateMealPlanRequest
    {
        public int SportifId { get; set; }
        public int NumberOfMeals { get; set; } = 3;
        public string? DietaryRestrictions { get; set; }
        public int? TargetCalories { get; set; }
        public int? TargetProtein { get; set; }
        public int? TargetCarbs { get; set; }
        public int? TargetFats { get; set; }
    }

    public class MealDto
    {
        public string Meal { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public List<FoodItemDto> Foods { get; set; } = new();
        public MacroTotalsDto TotalMacros { get; set; } = new();
        public string? Explanation { get; set; } // AI explanation for this meal
        public string? NutritionalBenefits { get; set; } // Why this meal is good
    }

    public class GenerateMealPlanResponse
    {
        public int PlanId { get; set; }
        public List<MealDto> Meals { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public string? OverallAnalysis { get; set; } // AI analysis of the entire plan
        public string? PersonalizedAdvice { get; set; } // Tips specific to user
    }

    public class FoodItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
    }

    public class MacroTotalsDto
    {
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
    }

    // Macro Calculation
    public class CalculateMacrosRequest
    {
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public float Weight { get; set; }
        public float Height { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;
        public int TrainingDaysPerWeek { get; set; }
    }

    public class MacrosResponse
    {
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fats { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }

    // Nutrition Plan Summary
    public class NutritionPlanSummaryDto
    {
        public int Id { get; set; }
        public DateTime DateGeneration { get; set; }
        public int ObjectifCaloriqueJournalier { get; set; }
        public int ObjectifProteinesG { get; set; }
        public int ObjectifGlucidesG { get; set; }
        public int ObjectifLipidesG { get; set; }
        public bool EstActif { get; set; }
        public int MealCount { get; set; }
    }
}

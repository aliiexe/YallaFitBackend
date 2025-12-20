using YallaFit.Models;

namespace YallaFit.Services
{
    public class MacroCalculationService
    {
        /// <summary>
        /// Calculates Basal Metabolic Rate using Mifflin-St Jeor equation
        /// </summary>
        public decimal CalculateBMR(float weightKg, float heightM, int age, string gender)
        {
            // Convert height to cm for formula
            float heightCm = heightM * 100;
            
            // Mifflin-St Jeor Equation
            // Men: BMR = 10 × weight(kg) + 6.25 × height(cm) - 5 × age(y) + 5
            // Women: BMR = 10 × weight(kg) + 6.25 × height(cm) - 5 × age(y) - 161
            
            decimal bmr = (decimal)(10 * weightKg + 6.25 * heightCm - 5 * age);
            
            if (gender?.ToLower() == "homme" || gender?.ToLower() == "male" || gender?.ToLower() == "m")
            {
                bmr += 5;
            }
            else
            {
                bmr -= 161;
            }
            
            return bmr;
        }

        /// <summary>
        /// Calculates Total Daily Energy Expenditure based on activity level
        /// </summary>
        public decimal CalculateTDEE(decimal bmr, string activityLevel)
        {
            // Activity multipliers
            var multiplier = activityLevel?.ToLower() switch
            {
                "sédentaire" => 1.2m,
                "sedentaire" => 1.2m,
                "léger" => 1.375m,
                "leger" => 1.375m,
                "modéré" => 1.55m,
                "modere" => 1.55m,
                "actif" => 1.725m,
                "très actif" => 1.9m,
                "tres actif" => 1.9m,
                _ => 1.55m // Default to moderate
            };

            return bmr * multiplier;
        }

        /// <summary>
        /// Calculates daily macro goals based on TDEE and fitness goal
        /// </summary>
        public DailyMacroGoals CalculateDailyGoals(decimal tdee, string goal, float weightKg)
        {
            decimal dailyCalories;
            decimal proteinGrams;
            decimal fatGrams;
            decimal carbsGrams;

            // Adjust calories based on goal
            switch (goal?.ToLower())
            {
                case "perte de poids":
                case "perte":
                    dailyCalories = tdee - 500; // 500 cal deficit for ~1 lb/week loss
                    proteinGrams = (decimal)weightKg * 2.0m; // 2g per kg for muscle preservation
                    break;

                case "prise de masse":
                case "gain musculaire":
                case "muscle":
                    dailyCalories = tdee + 300; // 300 cal surplus for lean gains
                    proteinGrams = (decimal)weightKg * 2.2m; // 2.2g per kg for muscle building
                    break;

                case "maintien":
                case "maintenance":
                default:
                    dailyCalories = tdee;
                    proteinGrams = (decimal)weightKg * 1.8m; // 1.8g per kg for maintenance
                    break;
            }

            // Fat: 25-30% of total calories (using 27.5%)
            decimal fatCalories = dailyCalories * 0.275m;
            fatGrams = fatCalories / 9m; // 9 cal per gram of fat

            // Protein calories
            decimal proteinCalories = proteinGrams * 4m; // 4 cal per gram of protein

            // Carbs: Remaining calories
            decimal carbCalories = dailyCalories - proteinCalories - fatCalories;
            carbsGrams = carbCalories / 4m; // 4 cal per gram of carbs

            return new DailyMacroGoals
            {
                Calories = Math.Round(dailyCalories, 0),
                Protein = Math.Round(proteinGrams, 1),
                Carbs = Math.Round(carbsGrams, 1),
                Fats = Math.Round(fatGrams, 1)
            };
        }

        /// <summary>
        /// Calculates complete macro goals for a user profile
        /// </summary>
        public DailyMacroGoals? CalculateForProfile(ProfilSportif profile, float currentWeightKg)
        {
            if (profile.Taille == null || profile.Age == null || string.IsNullOrEmpty(profile.Sexe))
            {
                return null; // Not enough data
            }

            var bmr = CalculateBMR(currentWeightKg, (float)profile.Taille, (int)profile.Age, profile.Sexe);
            var tdee = CalculateTDEE(bmr, profile.NiveauActivite ?? "Modéré");
            var macros = CalculateDailyGoals(tdee, profile.ObjectifPrincipal ?? "Maintien", currentWeightKg);

            return macros;
        }
    }

    public class DailyMacroGoals
    {
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fats { get; set; }
    }
}

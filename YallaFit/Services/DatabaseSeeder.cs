using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.Models;

namespace YallaFit.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDatabase(YallaFitDbContext context)
        {
            // Seed Aliments (Food Items)
            if (!await context.Aliments.AnyAsync())
            {
                await SeedAliments(context);
            }

            // Seed Exercices
            if (await context.Exercices.CountAsync() < 30)
            {
                await SeedExercices(context);
            }
        }

        private static async Task SeedAliments(YallaFitDbContext context)
        {
            var aliments = new List<Aliment>
            {
                // Proteins
                new Aliment { Nom = "Poulet (blanc, cuit)", Calories100g = 165,Proteines100g = 31, Glucides100g = 0, Lipides100g = 3.6f },
                new Aliment { Nom = "Boeuf (steak maigre)", Calories100g = 250, Proteines100g = 26, Glucides100g = 0, Lipides100g = 15 },
                new Aliment { Nom = "Saumon (cuit)", Calories100g = 206, Proteines100g = 22, Glucides100g = 0, Lipides100g = 13 },
                new Aliment { Nom = "Thon (en conserve, eau)", Calories100g = 116, Proteines100g = 26, Glucides100g = 0, Lipides100g = 0.8f },
                new Aliment { Nom = "Oeufs (entier, cuit)", Calories100g = 155, Proteines100g = 13, Glucides100g = 1.1f, Lipides100g = 11 },
                new Aliment { Nom = "Blanc d'oeuf", Calories100g = 52, Proteines100g = 11, Glucides100g = 0.7f, Lipides100g = 0.2f },
                new Aliment { Nom = "Dinde (blanc, cuit)", Calories100g = 135, Proteines100g = 30, Glucides100g = 0, Lipides100g = 0.7f },
                new Aliment { Nom = "Fromage blanc 0%", Calories100g = 46, Proteines100g = 8, Glucides100g = 4, Lipides100g = 0.2f },
                new Aliment { Nom = "Yaourt grec 0%", Calories100g = 59, Proteines100g = 10, Glucides100g = 3.6f, Lipides100g = 0.4f },
                new Aliment { Nom = "Cottage cheese", Calories100g = 98, Proteines100g = 11, Glucides100g = 3.4f, Lipides100g = 4.3f },
                new Aliment { Nom = "Tofu ferme", Calories100g = 144, Proteines100g = 17, Glucides100g = 3, Lipides100g = 9 },
                new Aliment { Nom = "Tempeh", Calories100g = 193, Proteines100g = 19, Glucides100g = 9, Lipides100g = 11 },
                new Aliment { Nom = "Seitan", Calories100g = 370, Proteines100g = 75, Glucides100g = 14, Lipides100g = 1.9f },
                new Aliment { Nom = "Whey protein", Calories100g = 412, Proteines100g = 82, Glucides100g = 7, Lipides100g = 7 },

                // Carbs - Grains
                new Aliment { Nom = "Riz basmati (cuit)", Calories100g = 121, Proteines100g = 2.7f, Glucides100g = 25, Lipides100g = 0.4f },
                new Aliment { Nom = "Riz complet (cuit)", Calories100g = 123, Proteines100g = 2.7f, Glucides100g = 26, Lipides100g = 1 },
                new Aliment { Nom = "Quinoa (cuit)", Calories100g = 120, Proteines100g = 4.4f, Glucides100g = 21, Lipides100g = 1.9f },
                new Aliment { Nom = "Pâtes complètes (cuites)", Calories100g = 131, Proteines100g = 5, Glucides100g = 26, Lipides100g = 1.1f },
                new Aliment { Nom = "Pain complet", Calories100g = 247, Proteines100g = 13, Glucides100g = 41, Lipides100g = 3.4f },
                new Aliment { Nom = "Flocons d'avoine", Calories100g = 389, Proteines100g = 17, Glucides100g = 66, Lipides100g = 7 },
                new Aliment { Nom = "Pain de mie complet", Calories100g = 265, Proteines100g = 9, Glucides100g = 47, Lipides100g = 3.5f },

                // Carbs - Legumes
                new Aliment { Nom = "Patate douce (cuite)", Calories100g = 90, Proteines100g = 2, Glucides100g = 21, Lipides100g = 0.2f },
                new Aliment { Nom = "Pomme de terre (cuite)", Calories100g = 93, Proteines100g = 2, Glucides100g = 21, Lipides100g = 0.1f },
                new Aliment { Nom = "Lentilles (cuites)", Calories100g = 116, Proteines100g = 9, Glucides100g = 20, Lipides100g = 0.4f },
                new Aliment { Nom = "Pois chiches (cuits)", Calories100g = 164, Proteines100g = 8.9f, Glucides100g = 27, Lipides100g = 2.6f },
                new Aliment { Nom = "Haricots rouges (cuits)", Calories100g = 127, Proteines100g = 8.7f, Glucides100g = 23, Lipides100g = 0.5f },

                // Vegetables
                new Aliment { Nom = "Brocoli (cuit)", Calories100g = 35, Proteines100g = 2.4f, Glucides100g = 7, Lipides100g = 0.4f },
                new Aliment { Nom = "Épinards (cuits)", Calories100g = 23, Proteines100g = 2.9f, Glucides100g = 3.6f, Lipides100g = 0.3f },
                new Aliment { Nom = "Haricots verts", Calories100g = 31, Proteines100g = 1.8f, Glucides100g = 7, Lipides100g = 0.1f },
                new Aliment { Nom = "Courgette", Calories100g = 17, Proteines100g = 1.2f, Glucides100g = 3.1f, Lipides100g = 0.3f },
                new Aliment { Nom = "Tomate", Calories100g = 18, Proteines100g = 0.9f, Glucides100g = 3.9f, Lipides100g = 0.2f },
                new Aliment { Nom = "Concombre", Calories100g = 16, Proteines100g = 0.7f, Glucides100g = 3.6f, Lipides100g = 0.1f },
                new Aliment { Nom = "Poivron rouge", Calories100g = 31, Proteines100g = 1, Glucides100g = 6, Lipides100g = 0.3f },
                new Aliment { Nom = "Carottes (cuites)", Calories100g = 35, Proteines100g = 0.8f, Glucides100g = 8, Lipides100g = 0.2f },
                new Aliment { Nom = "Chou-fleur", Calories100g = 25, Proteines100g = 1.9f, Glucides100g = 5, Lipides100g = 0.3f },
                new Aliment { Nom = "Asperges", Calories100g = 20, Proteines100g = 2.2f, Glucides100g = 3.9f, Lipides100g = 0.1f },
                new Aliment { Nom = "Champignons", Calories100g = 22, Proteines100g = 3.1f, Glucides100g = 3.3f, Lipides100g = 0.3f },

                // Fruits
                new Aliment { Nom = "Banane", Calories100g = 89, Proteines100g = 1.1f, Glucides100g = 23, Lipides100g = 0.3f },
                new Aliment { Nom = "Pomme", Calories100g = 52, Proteines100g = 0.3f, Glucides100g = 14, Lipides100g = 0.2f },
                new Aliment { Nom = "Orange", Calories100g = 47, Proteines100g = 0.9f, Glucides100g = 12, Lipides100g = 0.1f },
                new Aliment { Nom = "Fraises", Calories100g = 32, Proteines100g = 0.7f, Glucides100g = 7.7f, Lipides100g = 0.3f },
                new Aliment { Nom = "Myrtilles", Calories100g = 57, Proteines100g = 0.7f, Glucides100g = 14, Lipides100g = 0.3f },
                new Aliment { Nom = "Avocat", Calories100g = 160, Proteines100g = 2, Glucides100g = 8.5f, Lipides100g = 15 },
                new Aliment { Nom = "Kiwi", Calories100g = 61, Proteines100g = 1.1f, Glucides100g = 15, Lipides100g = 0.5f },
                new Aliment { Nom = "Ananas", Calories100g = 50, Proteines100g = 0.5f, Glucides100g = 13, Lipides100g = 0.1f },

                // Fats & Nuts
                new Aliment { Nom = "Amandes", Calories100g = 579, Proteines100g = 21, Glucides100g = 22, Lipides100g = 50 },
                new Aliment { Nom = "Noix de cajou", Calories100g = 553, Proteines100g = 18, Glucides100g = 30, Lipides100g = 44 },
                new Aliment { Nom = "Beurre de cacahuète", Calories100g = 588, Proteines100g = 25, Glucides100g = 20, Lipides100g = 50 },
                new Aliment { Nom = "Huile d'olive", Calories100g = 884, Proteines100g = 0, Glucides100g = 0, Lipides100g = 100 },
                new Aliment { Nom = "Graines de chia", Calories100g = 486, Proteines100g = 17, Glucides100g = 42, Lipides100g = 31 },
                new Aliment { Nom = "Graines de lin", Calories100g = 534, Proteines100g = 18, Glucides100g = 29, Lipides100g = 42 },
                new Aliment { Nom = "Noix", Calories100g = 654, Proteines100g = 15, Glucides100g = 14, Lipides100g = 65 },

                // Dairy & Alternatives
                new Aliment { Nom = "Lait écrémé", Calories100g = 34, Proteines100g = 3.4f, Glucides100g = 5, Lipides100g = 0.1f },
                new Aliment { Nom = "Lait d'amande (non sucré)", Calories100g = 17, Proteines100g = 0.6f, Glucides100g = 0.3f, Lipides100g = 1.4f },
                new Aliment { Nom = "Lait de soja", Calories100g = 33, Proteines100g = 2.9f, Glucides100g = 1.6f, Lipides100g = 1.8f },
                new Aliment { Nom = "Mozzarella light", Calories100g = 254, Proteines100g = 24, Glucides100g = 2.2f, Lipides100g = 16 },

                // Other
                new Aliment { Nom = "Miel", Calories100g = 304, Proteines100g = 0.3f, Glucides100g = 82, Lipides100g = 0 },
                new Aliment { Nom = "Chocolat noir 70%", Calories100g = 598, Proteines100g = 7.8f, Glucides100g = 45, Lipides100g = 43 },
            };

            await context.Aliments.AddRangeAsync(aliments);
            await context.SaveChangesAsync();
        }

        private static async Task SeedExercices(YallaFitDbContext context)
        {
            var exercices = new List<Exercice>
            {
                // Chest
                new Exercice { Nom = "Bench Press (Développé couché)", MuscleCible = "Pectoraux", VideoUrl = "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
                new Exercice { Nom = "Incline Dumbbell Press", MuscleCible = "Pectoraux", VideoUrl = "https://www.youtube.com/watch?v=8iPEnn-ltC8" },
                new Exercice { Nom = "Push-Ups (Pompes)", MuscleCible = "Pectoraux", VideoUrl = "https://www.youtube.com/watch?v=IODxDxX7oi4" },
                new Exercice { Nom = "Dumbbell Flyes", MuscleCible = "Pectoraux", VideoUrl = "https://www.youtube.com/watch?v=eozdVDA78K0" },

                // Back
                new Exercice { Nom = "Pull-Ups (Tractions)", MuscleCible = "Dos", VideoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
               new Exercice { Nom = "Barbell Row (Rowing barre)", MuscleCible = "Dos", VideoUrl = "https://www.youtube.com/watch?v=9efgcAjQe7E" },
                new Exercice { Nom = "Lat Pulldown", MuscleCible = "Dos", VideoUrl = "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
                new Exercice { Nom = "Deadlift (Soulevé de terre)", MuscleCible = "Dos", VideoUrl = "https://www.youtube.com/watch?v=op9kVnSso6Q" },
                new Exercice { Nom = "Seated Cable Row", MuscleCible = "Dos", VideoUrl = "https://www.youtube.com/watch?v=UCXxvVItLoM" },

                // Shoulders
                new Exercice { Nom = "Overhead Press (Développé militaire)", MuscleCible = "Épaules", VideoUrl = "https://www.youtube.com/watch?v=QAQ64hK4Xxs" },
                new Exercice { Nom = "Lateral Raises (Élévations latérales)", MuscleCible = "Épaules", VideoUrl = "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
                new Exercice { Nom = "Front Raises", MuscleCible = "Épaules", VideoUrl = "https://www.youtube.com/watch?v=S9rxCJmRMpY" },
                new Exercice { Nom = "Face Pulls", MuscleCible = "Épaules", VideoUrl = "https://www.youtube.com/watch?v=rep-qVOkqgk" },

                // Arms - Biceps
                new Exercice { Nom = "Barbell Curl (Curl barre)", MuscleCible = "Biceps", VideoUrl = "https://www.youtube.com/watch?v=LY1V6UbRHFM" },
                new Exercice { Nom = "Hammer Curls", MuscleCible = "Biceps", VideoUrl = "https://www.youtube.com/watch?v=zC3nLlEvin4" },
                new Exercice { Nom = "Concentration Curl", MuscleCible = "Biceps", VideoUrl = "https://www.youtube.com/watch?v=Jvj2wV0pPy0" },

                // Arms - Triceps
                new Exercice { Nom = "Tricep Dips", MuscleCible = "Triceps", VideoUrl = "https://www.youtube.com/watch?v=6kALZikXxLc" },
                new Exercice { Nom = "Close-Grip Bench Press", MuscleCible = "Triceps", VideoUrl = "https://www.youtube.com/watch?v=nEF0bv2FW94" },
                new Exercice { Nom = "Overhead Tricep Extension", MuscleCible = "Triceps", VideoUrl = "https://www.youtube.com/watch?v=nRiJVZDpdL0" },
                new Exercice { Nom = "Tricep Pushdown", MuscleCible = "Triceps", VideoUrl = "https://www.youtube.com/watch?v=2-LAMcpzODU" },

                // Legs - Quadriceps
                new Exercice { Nom = "Barbell Squat (Squat barre)", MuscleCible = "Quadriceps", VideoUrl = "https://www.youtube.com/watch?v=ultWZbUMPL8" },
                new Exercice { Nom = "Leg Press", MuscleCible = "Quadriceps", VideoUrl = "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
                new Exercice { Nom = "Bulgarian Split Squat", MuscleCible = "Quadriceps", VideoUrl = "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
                new Exercice { Nom = "Leg Extension", MuscleCible = "Quadriceps", VideoUrl = "https://www.youtube.com/watch?v=YyvSfEjZaHY" },

                // Legs - Hamstrings
                new Exercice { Nom = "Romanian Deadlift", MuscleCible = "Ischio-jambiers", VideoUrl = "https://www.youtube.com/watch?v=JCXUYuzwNrM" },
                new Exercice { Nom = "Leg Curl", MuscleCible = "Ischio-jambiers", VideoUrl = "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
                new Exercice { Nom = "Walking Lunges (Fentes marchées)", MuscleCible = "Quadriceps", VideoUrl = "https://www.youtube.com/watch?v=L8fvypPrzzs" },

                // Glutes
                new Exercice { Nom = "Hip Thrust", MuscleCible = "Fessiers", VideoUrl = "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
                new Exercice { Nom = "Glute Kickbacks", MuscleCible = "Fessiers", VideoUrl = "https://www.youtube.com/watch?v=SuyPnTVlSWo" },

                // Core/Abs
                new Exercice { Nom = "Plank (Gainage)", MuscleCible = "Abdominaux", VideoUrl = "https://www.youtube.com/watch?v=ASdvN_XEl_c" },
                new Exercice { Nom = "Russian Twists", MuscleCible = "Abdominaux", VideoUrl = "https://www.youtube.com/watch?v=wkD8rjkodUI" },
                new Exercice { Nom = "Hanging Leg Raises", MuscleCible = "Abdominaux", VideoUrl = "https://www.youtube.com/watch?v=hdng3Nm1x_E" },
                new Exercice { Nom = "Cable Crunches", MuscleCible = "Abdominaux", VideoUrl = "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },

                // Calves
                new Exercice { Nom = "Standing Calf Raises", MuscleCible = "Mollets", VideoUrl = "https://www.youtube.com/watch?v=gwLzBJYoWlI" },
            };

            await context.Exercices.AddRangeAsync(exercices);
            await context.SaveChangesAsync();
        }
    }
}

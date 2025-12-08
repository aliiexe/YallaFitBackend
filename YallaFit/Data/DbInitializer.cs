using YallaFit.Models;

namespace YallaFit.Data
{
    public static class DbInitializer
    {
        public static void Initialize(YallaFitDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Utilisateurs.Any())
            {
                return;
            }

            var adminUser = new Utilisateur
            {
                NomComplet = "Admin YallaFit",
                Email = "admin@yallafit.com",
                MotDePasse = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin"
            };
            context.Utilisateurs.Add(adminUser);
            context.SaveChanges();

            var exercises = new[]
            {
                new Exercice { Nom = "Squat", MuscleCible = "Jambes", VideoUrl = "https://example.com/squat" },
                new Exercice { Nom = "Développé couché", MuscleCible = "Pectoraux", VideoUrl = "https://example.com/bench-press" },
                new Exercice { Nom = "Soulevé de terre", MuscleCible = "Dos", VideoUrl = "https://example.com/deadlift" },
                new Exercice { Nom = "Développé militaire", MuscleCible = "Épaules", VideoUrl = "https://example.com/military-press" },
                new Exercice { Nom = "Tractions", MuscleCible = "Dos", VideoUrl = "https://example.com/pull-ups" },
                new Exercice { Nom = "Dips", MuscleCible = "Triceps", VideoUrl = "https://example.com/dips" },
                new Exercice { Nom = "Curl biceps", MuscleCible = "Biceps", VideoUrl = "https://example.com/bicep-curl" },
                new Exercice { Nom = "Fentes", MuscleCible = "Jambes", VideoUrl = "https://example.com/lunges" },
                new Exercice { Nom = "Rowing barre", MuscleCible = "Dos", VideoUrl = "https://example.com/barbell-row" },
                new Exercice { Nom = "Crunch", MuscleCible = "Abdominaux", VideoUrl = "https://example.com/crunch" },
                new Exercice { Nom = "Planche", MuscleCible = "Abdominaux", VideoUrl = "https://example.com/plank" },
                new Exercice { Nom = "Extension triceps", MuscleCible = "Triceps", VideoUrl = "https://example.com/tricep-extension" },
                new Exercice { Nom = "Élévations latérales", MuscleCible = "Épaules", VideoUrl = "https://example.com/lateral-raise" },
                new Exercice { Nom = "Leg press", MuscleCible = "Jambes", VideoUrl = "https://example.com/leg-press" },
                new Exercice { Nom = "Pompes", MuscleCible = "Pectoraux", VideoUrl = "https://example.com/push-ups" }
            };
            context.Exercices.AddRange(exercises);
            context.SaveChanges();

            var aliments = new[]
            {
                new Aliment { Nom = "Poulet (blanc)", Calories100g = 165, Proteines100g = 31f, Glucides100g = 0f, Lipides100g = 3.6f },
                new Aliment { Nom = "Bœuf (maigre)", Calories100g = 250, Proteines100g = 26f, Glucides100g = 0f, Lipides100g = 15f },
                new Aliment { Nom = "Saumon", Calories100g = 208, Proteines100g = 20f, Glucides100g = 0f, Lipides100g = 13f },
                new Aliment { Nom = "Œufs", Calories100g = 155, Proteines100g = 13f, Glucides100g = 1.1f, Lipides100g = 11f },
                new Aliment { Nom = "Thon (conserve)", Calories100g = 116, Proteines100g = 26f, Glucides100g = 0f, Lipides100g = 0.8f },
                
                new Aliment { Nom = "Riz blanc", Calories100g = 130, Proteines100g = 2.7f, Glucides100g = 28f, Lipides100g = 0.3f },
                new Aliment { Nom = "Riz complet", Calories100g = 111, Proteines100g = 2.6f, Glucides100g = 23f, Lipides100g = 0.9f },
                new Aliment { Nom = "Pâtes", Calories100g = 131, Proteines100g = 5f, Glucides100g = 25f, Lipides100g = 1.1f },
                new Aliment { Nom = "Pain complet", Calories100g = 247, Proteines100g = 13f, Glucides100g = 41f, Lipides100g = 3.4f },
                new Aliment { Nom = "Patate douce", Calories100g = 86, Proteines100g = 1.6f, Glucides100g = 20f, Lipides100g = 0.1f },
                new Aliment { Nom = "Flocons d'avoine", Calories100g = 389, Proteines100g = 17f, Glucides100g = 66f, Lipides100g = 7f },
                
                new Aliment { Nom = "Brocoli", Calories100g = 34, Proteines100g = 2.8f, Glucides100g = 7f, Lipides100g = 0.4f },
                new Aliment { Nom = "Épinards", Calories100g = 23, Proteines100g = 2.9f, Glucides100g = 3.6f, Lipides100g = 0.4f },
                new Aliment { Nom = "Tomate", Calories100g = 18, Proteines100g = 0.9f, Glucides100g = 3.9f, Lipides100g = 0.2f },
                new Aliment { Nom = "Carotte", Calories100g = 41, Proteines100g = 0.9f, Glucides100g = 10f, Lipides100g = 0.2f },
                
                new Aliment { Nom = "Banane", Calories100g = 89, Proteines100g = 1.1f, Glucides100g = 23f, Lipides100g = 0.3f },
                new Aliment { Nom = "Pomme", Calories100g = 52, Proteines100g = 0.3f, Glucides100g = 14f, Lipides100g = 0.2f },
                new Aliment { Nom = "Orange", Calories100g = 47, Proteines100g = 0.9f, Glucides100g = 12f, Lipides100g = 0.1f },
                
                new Aliment { Nom = "Avocat", Calories100g = 160, Proteines100g = 2f, Glucides100g = 9f, Lipides100g = 15f },
                new Aliment { Nom = "Amandes", Calories100g = 579, Proteines100g = 21f, Glucides100g = 22f, Lipides100g = 50f },
                new Aliment { Nom = "Huile d'olive", Calories100g = 884, Proteines100g = 0f, Glucides100g = 0f, Lipides100g = 100f },
                
                new Aliment { Nom = "Yaourt grec (0%)", Calories100g = 59, Proteines100g = 10f, Glucides100g = 3.6f, Lipides100g = 0.4f },
                new Aliment { Nom = "Fromage blanc", Calories100g = 72, Proteines100g = 13f, Glucides100g = 2.7f, Lipides100g = 0.9f },
                new Aliment { Nom = "Lait écrémé", Calories100g = 34, Proteines100g = 3.4f, Glucides100g = 5f, Lipides100g = 0.1f },
                
                new Aliment { Nom = "Lentilles", Calories100g = 116, Proteines100g = 9f, Glucides100g = 20f, Lipides100g = 0.4f },
                new Aliment { Nom = "Pois chiches", Calories100g = 164, Proteines100g = 8.9f, Glucides100g = 27f, Lipides100g = 2.6f },
                new Aliment { Nom = "Haricots noirs", Calories100g = 132, Proteines100g = 8.9f, Glucides100g = 24f, Lipides100g = 0.5f }
            };
            context.Aliments.AddRange(aliments);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
        }
    }
}

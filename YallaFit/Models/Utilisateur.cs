using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Utilisateur")]
    public class Utilisateur
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("nom_complet")]
        public string NomComplet { get; set; } = string.Empty;

        [Required]
        [MaxLength(191)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("mot_de_passe")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("role")]
        public string Role { get; set; } = "Sportif";

        public ProfilSportif? ProfilSportif { get; set; }
        public ICollection<Programme> ProgrammesCreated { get; set; } = new List<Programme>();
        public ICollection<PlanNutrition> PlansNutrition { get; set; } = new List<PlanNutrition>();
        public ICollection<BiometrieActuelle> BiometriesMeasurements { get; set; } = new List<BiometrieActuelle>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Repas")]
    public class Repas
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("plan_id")]
        public int PlanId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [Column("heure_prevue")]
        public TimeSpan HeurePrevue { get; set; }

        [ForeignKey("PlanId")]
        public PlanNutrition Plan { get; set; } = null!;

        public ICollection<CompositionRepas> CompositionRepas { get; set; } = new List<CompositionRepas>();
    }
}

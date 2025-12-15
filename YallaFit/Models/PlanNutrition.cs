using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Plan_Nutrition")]
    public class PlanNutrition
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sportif_id")]
        public int SportifId { get; set; }

        [Required]
        [Column("date_generation")]
        public DateTime DateGeneration { get; set; }

        [Required]
        [Column("objectif_calorique_journalier")]
        public int ObjectifCaloriqueJournalier { get; set; }

        [Required]
        [Column("objectif_proteines_g")]
        public int ObjectifProteinesG { get; set; }

        [Required]
        [Column("objectif_glucides_g")]
        public int ObjectifGlucidesG { get; set; }

        [Required]
        [Column("objectif_lipides_g")]
        public int ObjectifLipidesG { get; set; }

        [Required]
        [Column("est_actif")]
        public bool EstActif { get; set; } = true;

        [Column("analyse_globale")]
        public string? AnalyseGlobale { get; set; }

        [Column("conseils_personnalises")]
        public string? ConseilsPersonnalises { get; set; }

        [ForeignKey("SportifId")]
        public Utilisateur Sportif { get; set; } = null!;

        public ICollection<Repas> Repas { get; set; } = new List<Repas>();
    }
}

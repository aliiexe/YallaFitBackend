using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Analyse_Repas_Photo")]
    public class AnalyseRepasPhoto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sportif_id")]
        public int SportifId { get; set; }

        [Required]
        [Column("date_analyse")]
        public DateTime DateAnalyse { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("chemin_photo")]
        public string CheminPhoto { get; set; } = string.Empty;

        [Column("aliments_detectes")]
        public string? AlimentsDetectes { get; set; } // JSON array of detected foods

        [Column("calories_estimees")]
        public int CaloriesEstimees { get; set; }

        [Column("proteines_estimees")]
        public float ProteinesEstimees { get; set; }

        [Column("glucides_estimees")]
        public float GlucidesEstimees { get; set; }

        [Column("lipides_estimees")]
        public float LipidesEstimees { get; set; }

        [Column("analyse_ia")]
        public string? AnalyseIA { get; set; }

        [Column("recommandations")]
        public string? Recommandations { get; set; }

        [ForeignKey("SportifId")]
        public Utilisateur Sportif { get; set; } = null!;
    }
}

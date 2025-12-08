using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Biometrie_Actuelle")]
    public class BiometrieActuelle
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sportif_id")]
        public int SportifId { get; set; }

        [Required]
        [Column("date_mesure")]
        public DateTime DateMesure { get; set; }

        [Required]
        [Column("poids_kg")]
        public float PoidsKg { get; set; }

        [Column("taux_masse_grasse_percent")]
        public float? TauxMasseGrassePercent { get; set; }

        [Column("tour_de_taille_cm")]
        public float? TourDeTailleCm { get; set; }

        [ForeignKey("SportifId")]
        public Utilisateur Sportif { get; set; } = null!;
    }
}

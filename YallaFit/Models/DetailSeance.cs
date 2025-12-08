using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Detail_Seance")]
    public class DetailSeance
    {
        [Required]
        [Column("seance_id")]
        public int SeanceId { get; set; }

        [Required]
        [Column("exercice_id")]
        public int ExerciceId { get; set; }

        [Required]
        [Column("series")]
        public int Series { get; set; }

        [Required]
        [Column("repetitions")]
        public int Repetitions { get; set; }

        [Column("poids_conseille")]
        public float? PoidsConseille { get; set; }

        // Navigation properties
        [ForeignKey("SeanceId")]
        public Seance Seance { get; set; } = null!;

        [ForeignKey("ExerciceId")]
        public Exercice Exercice { get; set; } = null!;
    }
}

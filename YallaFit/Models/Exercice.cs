using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Exercice")]
    public class Exercice
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("video_url")]
        public string? VideoUrl { get; set; }

        [MaxLength(100)]
        [Column("muscle_cible")]
        public string? MuscleCible { get; set; }

        [MaxLength(100)]
        [Column("categorie")]
        public string? Categorie { get; set; }

        [MaxLength(255)]
        [Column("muscles_cibles")]
        public string? MusclesCibles { get; set; }

        public ICollection<DetailSeance> DetailSeances { get; set; } = new List<DetailSeance>();
    }
}

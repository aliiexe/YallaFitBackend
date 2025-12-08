using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Seance")]
    public class Seance
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("programme_id")]
        public int ProgrammeId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [Column("jour_semaine")]
        public int JourSemaine { get; set; } // 1-7 for Monday-Sunday

        [ForeignKey("ProgrammeId")]
        public Programme Programme { get; set; } = null!;

        public ICollection<DetailSeance> DetailSeances { get; set; } = new List<DetailSeance>();
    }
}

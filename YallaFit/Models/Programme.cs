using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Programme")]
    public class Programme
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("coach_id")]
        public int CoachId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("titre")]
        public string Titre { get; set; } = string.Empty;

        [Required]
        [Column("duree_semaines")]
        public int DureeSemaines { get; set; }

        [Column("is_public")]
        public bool IsPublic { get; set; } = false;

        [ForeignKey("CoachId")]
        public Utilisateur Coach { get; set; } = null!;

        public ICollection<Seance> Seances { get; set; } = new List<Seance>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Programme_Enrollment")]
    public class ProgrammeEnrollment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sportif_id")]
        public int SportifId { get; set; }

        [Column("programme_id")]
        public int ProgrammeId { get; set; }

        [Column("enrolled_at")]
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("SportifId")]
        public ProfilSportif ProfilSportif { get; set; } = null!;

        [ForeignKey("ProgrammeId")]
        public Programme Programme { get; set; } = null!;
    }
}

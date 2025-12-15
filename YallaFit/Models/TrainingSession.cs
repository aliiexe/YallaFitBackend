using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Training_Session")]
    public class TrainingSession
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sportif_id")]
        public int SportifId { get; set; }

        [Column("programme_id")]
        public int ProgrammeId { get; set; }

        [Column("seance_id")]
        public int SeanceId { get; set; }

        [Column("date_completed")]
        public DateTime DateCompleted { get; set; }

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SportifId")]
        public ProfilSportif ProfilSportif { get; set; } = null!;

        [ForeignKey("ProgrammeId")]
        public Programme Programme { get; set; } = null!;

        [ForeignKey("SeanceId")]
        public Seance Seance { get; set; } = null!;

        public ICollection<TrainingExercise> TrainingExercises { get; set; } = new List<TrainingExercise>();
    }
}

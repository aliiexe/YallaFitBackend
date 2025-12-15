using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Training_Exercise")]
    public class TrainingExercise
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("training_session_id")]
        public int TrainingSessionId { get; set; }

        [Column("exercice_id")]
        public int ExerciceId { get; set; }

        [Column("order_index")]
        public int OrderIndex { get; set; }

        // Navigation properties
        [ForeignKey("TrainingSessionId")]
        public TrainingSession TrainingSession { get; set; } = null!;

        [ForeignKey("ExerciceId")]
        public Exercice Exercice { get; set; } = null!;

        public ICollection<TrainingSet> TrainingSets { get; set; } = new List<TrainingSet>();
    }
}

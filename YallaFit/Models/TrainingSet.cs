using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Training_Set")]
    public class TrainingSet
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("training_exercise_id")]
        public int TrainingExerciseId { get; set; }

        [Column("set_number")]
        public int SetNumber { get; set; }

        [Column("reps")]
        public int Reps { get; set; }

        [Column("weight_kg")]
        public decimal? WeightKg { get; set; }

        [Column("completed")]
        public bool Completed { get; set; } = true;

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("TrainingExerciseId")]
        public TrainingExercise TrainingExercise { get; set; } = null!;
    }
}

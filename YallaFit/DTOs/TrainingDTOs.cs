namespace YallaFit.DTOs
{
    // Request DTO for saving a completed workout
    public class SaveTrainingSessionDto
    {
        public int ProgrammeId { get; set; }
        public int SeanceId { get; set; }
        public DateTime DateCompleted { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public List<TrainingExerciseDto> Exercises { get; set; } = new();
    }

    public class TrainingExerciseDto
    {
        public int ExerciceId { get; set; }
        public int OrderIndex { get; set; }
        public List<TrainingSetDto> Sets { get; set; } = new();
    }

    public class TrainingSetDto
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKg { get; set; }
        public bool Completed { get; set; } = true;
        public string? Notes { get; set; }
    }

    // Response DTO for workout history
    public class TrainingSessionHistoryDto
    {
        public int Id { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeTitre { get; set; } = string.Empty;
        public int SeanceId { get; set; }
        public string SeanceNom { get; set; } = string.Empty;
        public DateTime DateCompleted { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public int ExerciseCount { get; set; }
        public int TotalSets { get; set; }
    }

    // Detailed session response
    public class TrainingSessionDetailDto
    {
        public int Id { get; set; }

        public int ProgrammeId { get; set; }
        public string ProgrammeTitre { get; set; } = string.Empty;
        public int SeanceId { get; set; }
        public string SeanceNom { get; set; } = string.Empty;
        public DateTime DateCompleted { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public List<TrainingExerciseDetailDto> Exercises { get; set; } = new();
    }

    public class TrainingExerciseDetailDto
    {
        public int ExerciceId { get; set; }
        public string ExerciceNom { get; set; } = string.Empty;
        public string? MuscleCible { get; set; }
        public int OrderIndex { get; set; }
        public List<TrainingSetDetailDto> Sets { get; set; } = new();
    }

    public class TrainingSetDetailDto
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKg { get; set; }
        public bool Completed { get; set; }
        public string? Notes { get; set; }
    }
}

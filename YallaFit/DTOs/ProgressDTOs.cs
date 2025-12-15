namespace YallaFit.DTOs
{
    // Progress tracking DTOs
    public class ExerciseProgressDto
    {
        public int ExerciceId { get; set; }
        public string ExerciceNom { get; set; } = string.Empty;
        public string? MuscleCible { get; set; }
        public List<ProgressDataPoint> DataPoints { get; set; } = new();
        public ProgressStats Stats { get; set; } = new();
    }

    public class ProgressDataPoint
    {
        public DateTime Date { get; set; }
        public decimal? MaxWeight { get; set; }
        public int TotalReps { get; set; }
        public decimal TotalVolume { get; set; }
        public int SessionId { get; set; }
        public int SetsCompleted { get; set; }
    }

    public class ProgressStats
    {
        public decimal? PersonalRecord { get; set; }
        public int TotalSessions { get; set; }
        public decimal AverageVolume { get; set; }
        public DateTime? LastPerformed { get; set; }
        public decimal? AverageWeight { get; set; }
    }

    // Exercise with history info
    public class ExerciseWithHistoryDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? MuscleCible { get; set; }
        public int SessionCount { get; set; }
        public DateTime? LastPerformed { get; set; }
    }
}

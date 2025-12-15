namespace YallaFit.DTOs
{
    // Exercise DTOs
    public class ExerciceDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? MuscleCible { get; set; }
        public string? Categorie { get; set; }
    }

    public class CreateExerciceDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? MuscleCible { get; set; }
        public string? Categorie { get; set; }
    }

    public class UpdateExerciceDto
    {
        public string? Nom { get; set; }
        public string? VideoUrl { get; set; }
        public string? MuscleCible { get; set; }
        public string? Categorie { get; set; }
    }
}

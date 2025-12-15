namespace YallaFit.DTOs
{
    // Programme list view
    public class ProgrammeListDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public int DureeSemaines { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public int SessionCount { get; set; }
        public bool IsPublic { get; set; }
    }

    // Programme detailed view with sessions
    public class ProgrammeDetailDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public int DureeSemaines { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public List<SeanceDto> Seances { get; set; } = new List<SeanceDto>();
    }

    // Create/update programme
    public class CreateProgrammeDto
    {
        public string Titre { get; set; } = string.Empty;
        public int DureeSemaines { get; set; }
        public bool IsPublic { get; set; }
    }

    public class UpdateProgrammeDto
    {
        public string? Titre { get; set; }
        public int? DureeSemaines { get; set; }
        public bool? IsPublic { get; set; }
    }

    // Session (Seance) DTOs
    public class SeanceDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public int JourSemaine { get; set; }
        public List<DetailSeanceDto> Exercices { get; set; } = new List<DetailSeanceDto>();
    }

    public class CreateSeanceDto
    {
        public string Nom { get; set; } = string.Empty;
        public int JourSemaine { get; set; }
        public List<CreateDetailSeanceDto> Exercices { get; set; } = new List<CreateDetailSeanceDto>();
    }

    public class UpdateSeanceDto
    {
        public string? Nom { get; set; }
        public int? JourSemaine { get; set; }
        public List<CreateDetailSeanceDto>? Exercices { get; set; }
    }

    // Exercise details within session
    public class DetailSeanceDto
    {
        public int ExerciceId { get; set; }
        public string ExerciceNom { get; set; } = string.Empty;
        public string? MuscleCible { get; set; }
        public int Series { get; set; }
        public int Repetitions { get; set; }
        public float? PoidsConseille { get; set; }
    }

    public class CreateDetailSeanceDto
    {
        public int ExerciceId { get; set; }
        public int Series { get; set; }
        public int Repetitions { get; set; }
        public float? PoidsConseille { get; set; }
    }
}

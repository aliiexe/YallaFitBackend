namespace YallaFit.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NomComplet { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public SportifProfileDto? ProfilSportif { get; set; }
    }

    public class UpdateProfileDto
    {
        public string NomComplet { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    public class SportifProfileDto
    {
        public int? Age { get; set; }
        public decimal? Poids { get; set; }
        public decimal? Taille { get; set; }
        public string? Sexe { get; set; }
        public string? NiveauActivite { get; set; }
        public string? ObjectifPrincipal { get; set; }
        public string? PreferencesAlimentaires { get; set; }
        public string? Allergies { get; set; }
    }

    public class UpdateSportifProfileDto
    {
        public int? Age { get; set; }
        public decimal? Poids { get; set; }
        public decimal? Taille { get; set; }
        public string? Sexe { get; set; }
        public string? NiveauActivite { get; set; }
        public string? ObjectifPrincipal { get; set; }
        public string? PreferencesAlimentaires { get; set; }
        public string? Allergies { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

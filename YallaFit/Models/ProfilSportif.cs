using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Profil_Sportif")]
    public class ProfilSportif
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        // Alias for UserId (used by some controllers)
        [NotMapped]
        public int SportifId
        {
            get => UserId;
            set => UserId = value;
        }

        [Column("age")]
        public int? Age { get; set; }

        [Column("poids")]
        public decimal? Poids { get; set; }

        [Column("taille")]
        public decimal? Taille { get; set; }

        [MaxLength(50)]
        [Column("sexe")]
        public string? Sexe { get; set; }

        [Column("date_naissance")]
        public DateTime? DateNaissance { get; set; }

        [MaxLength(50)]
        [Column("genre")]
        public string? Genre { get; set; }

        [Column("taille_cm")]
        public int? TailleCm { get; set; }

        [MaxLength(100)]
        [Column("niveau_activite")]
        public string? NiveauActivite { get; set; }

        [MaxLength(255)]
        [Column("objectif_principal")]
        public string? ObjectifPrincipal { get; set; }

        [Column("allergies")]
        public string? Allergies { get; set; }

        [Column("preferences_alim")]
        public string? PreferencesAlim { get; set; }

        // Alias for PreferencesAlim (used by some controllers)
        [NotMapped]
        public string? PreferencesAlimentaires
        {
            get => PreferencesAlim;
            set => PreferencesAlim = value;
        }

        [Column("problemes_sante")]
        public string? ProblemesSante { get; set; }

        [ForeignKey("UserId")]
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}

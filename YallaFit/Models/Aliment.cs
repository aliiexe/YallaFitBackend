using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Aliment")]
    public class Aliment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [Column("calories_100g")]
        public int Calories100g { get; set; }

        [Required]
        [Column("proteines_100g")]
        public float Proteines100g { get; set; }

        [Required]
        [Column("glucides_100g")]
        public float Glucides100g { get; set; }

        [Required]
        [Column("lipides_100g")]
        public float Lipides100g { get; set; }

        public ICollection<CompositionRepas> CompositionRepas { get; set; } = new List<CompositionRepas>();
    }
}

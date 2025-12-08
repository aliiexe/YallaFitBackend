using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaFit.Models
{
    [Table("Composition_Repas")]
    public class CompositionRepas
    {
        [Required]
        [Column("repas_id")]
        public int RepasId { get; set; }

        [Required]
        [Column("aliment_id")]
        public int AlimentId { get; set; }

        [Required]
        [Column("quantite_grammes")]
        public int QuantiteGrammes { get; set; }

        [ForeignKey("RepasId")]
        public Repas Repas { get; set; } = null!;

        [ForeignKey("AlimentId")]
        public Aliment Aliment { get; set; } = null!;
    }
}

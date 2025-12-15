using System.ComponentModel.DataAnnotations;

namespace YallaFit.DTOs
{
    public class AssignProgramDto
    {
        [Required]
        public int AthleteId { get; set; }

        [Required]
        public int ProgramId { get; set; }
    }
}

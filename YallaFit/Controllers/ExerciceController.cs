using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.DTOs;
using YallaFit.Models;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExerciceController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public ExerciceController(YallaFitDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExercices()
        {
            try
            {
                var exercices = await _context.Exercices
                    .Select(e => new ExerciceDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        VideoUrl = e.VideoUrl,
                        MuscleCible = e.MuscleCible,
                        Categorie = e.Categorie
                    })
                    .ToListAsync();

                return Ok(exercices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des exercices", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExercice(int id)
        {
            try
            {
                var exercice = await _context.Exercices
                    .Where(e => e.Id == id)
                    .Select(e => new ExerciceDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        VideoUrl = e.VideoUrl,
                        MuscleCible = e.MuscleCible,
                        Categorie = e.Categorie
                    })
                    .FirstOrDefaultAsync();

                if (exercice == null)
                {
                    return NotFound(new { message = "Exercice non trouvé" });
                }

                return Ok(exercice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération de l'exercice", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> CreateExercice([FromBody] CreateExerciceDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nom))
                {
                    return BadRequest(new { message = "Le nom de l'exercice est requis" });
                }

                var exercice = new Exercice
                {
                    Nom = dto.Nom,
                    VideoUrl = dto.VideoUrl,
                    MuscleCible = dto.MuscleCible,
                    Categorie = dto.Categorie,
                    MusclesCibles = dto.MuscleCible // For backward compatibility
                };

                _context.Exercices.Add(exercice);
                await _context.SaveChangesAsync();

                var result = new ExerciceDto
                {
                    Id = exercice.Id,
                    Nom = exercice.Nom,
                    VideoUrl = exercice.VideoUrl,
                    MuscleCible = exercice.MuscleCible,
                    Categorie = exercice.Categorie
                };

                return CreatedAtAction(nameof(GetExercice), new { id = exercice.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création de l'exercice", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateExercice(int id, [FromBody] UpdateExerciceDto dto)
        {
            try
            {
                var exercice = await _context.Exercices.FindAsync(id);
                if (exercice == null)
                {
                    return NotFound(new { message = "Exercice non trouvé" });
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(dto.Nom))
                    exercice.Nom = dto.Nom;
                
                if (dto.VideoUrl != null)
                    exercice.VideoUrl = dto.VideoUrl;
                
                if (dto.MuscleCible != null)
                {
                    exercice.MuscleCible = dto.MuscleCible;
                    exercice.MusclesCibles = dto.MuscleCible;
                }
                
                if (dto.Categorie != null)
                    exercice.Categorie = dto.Categorie;

                await _context.SaveChangesAsync();

                var result = new ExerciceDto
                {
                    Id = exercice.Id,
                    Nom = exercice.Nom,
                    VideoUrl = exercice.VideoUrl,
                    MuscleCible = exercice.MuscleCible,
                    Categorie = exercice.Categorie
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour de l'exercice", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> DeleteExercice(int id)
        {
            try
            {
                var exercice = await _context.Exercices.FindAsync(id);
                if (exercice == null)
                {
                    return NotFound(new { message = "Exercice non trouvé" });
                }

                _context.Exercices.Remove(exercice);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Exercice supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression de l'exercice", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetExercicesByCategory(string category)
        {
            try
            {
                var exercices = await _context.Exercices
                    .Where(e => e.Categorie == category)
                    .Select(e => new ExerciceDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        VideoUrl = e.VideoUrl,
                        MuscleCible = e.MuscleCible,
                        Categorie = e.Categorie
                    })
                    .ToListAsync();

                return Ok(exercices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des exercices", error = ex.Message });
            }
        }

        [HttpGet("muscle/{muscle}")]
        public async Task<IActionResult> GetExercicesByMuscle(string muscle)
        {
            try
            {
                var exercices = await _context.Exercices
                    .Where(e => e.MusclesCibles != null && e.MusclesCibles.Contains(muscle))
                    .Select(e => new ExerciceDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        VideoUrl = e.VideoUrl,
                        MuscleCible = e.MuscleCible,
                        Categorie = e.Categorie
                    })
                    .ToListAsync();

                return Ok(exercices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des exercices", error = ex.Message });
            }
        }
    }
}

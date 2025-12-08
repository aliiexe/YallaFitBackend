using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
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
                var exercices = await _context.Exercices.ToListAsync();
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
                var exercice = await _context.Exercices.FindAsync(id);

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
        public async Task<IActionResult> CreateExercice([FromBody] Exercice exercice)
        {
            try
            {
                _context.Exercices.Add(exercice);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetExercice), new { id = exercice.Id }, exercice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création de l'exercice", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateExercice(int id, [FromBody] Exercice exercice)
        {
            if (id != exercice.Id)
            {
                return BadRequest(new { message = "ID de l'exercice invalide" });
            }

            try
            {
                _context.Entry(exercice).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(exercice);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Exercices.AnyAsync(e => e.Id == id))
                {
                    return NotFound(new { message = "Exercice non trouvé" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour de l'exercice", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

                return NoContent();
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
                    .Where(e => e.MusclesCibles.Contains(muscle))
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

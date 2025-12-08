using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YallaFit.Data;
using YallaFit.Models;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgrammeController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public ProgrammeController(YallaFitDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProgrammes()
        {
            try
            {
                var programmes = await _context.Programmes
                    .Include(p => p.Seances)
                    .ToListAsync();

                return Ok(programmes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des programmes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramme(int id)
        {
            try
            {
                var programme = await _context.Programmes
                    .Include(p => p.Seances)
                    .ThenInclude(s => s.DetailSeances)
                    .ThenInclude(d => d.Exercice)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                return Ok(programme);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du programme", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProgramme([FromBody] Programme programme)
        {
            try
            {
                _context.Programmes.Add(programme);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProgramme), new { id = programme.Id }, programme);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création du programme", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgramme(int id, [FromBody] Programme programme)
        {
            if (id != programme.Id)
            {
                return BadRequest(new { message = "ID du programme invalide" });
            }

            try
            {
                _context.Entry(programme).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(programme);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Programmes.AnyAsync(p => p.Id == id))
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du programme", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgramme(int id)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(id);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                _context.Programmes.Remove(programme);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression du programme", error = ex.Message });
            }
        }
    }
}

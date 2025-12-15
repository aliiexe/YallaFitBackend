using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YallaFit.Data;
using YallaFit.DTOs;
using YallaFit.Models;

namespace YallaFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BiometricsController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public BiometricsController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/biometrics
        [HttpGet]
        public async Task<IActionResult> GetAllBiometrics()
        {
            try
            {
                var userId = GetCurrentUserId();
                var biometrics = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == userId)
                    .OrderByDescending(b => b.DateMesure)
                    .Select(b => new
                    {
                        b.Id,
                        b.SportifId,
                        b.DateMesure,
                        b.PoidsKg,
                        b.TauxMasseGrassePercent,
                        b.TourDeTailleCm
                    })
                    .ToListAsync();

                return Ok(biometrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données biométriques", error = ex.Message });
            }
        }

        // GET: api/biometrics/latest
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestBiometrics()
        {
            try
            {
                var userId = GetCurrentUserId();
                var latest = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == userId)
                    .OrderByDescending(b => b.DateMesure)
                    .Select(b => new
                    {
                        b.Id,
                        b.SportifId,
                        b.DateMesure,
                        b.PoidsKg,
                        b.TauxMasseGrassePercent,
                        b.TourDeTailleCm
                    })
                    .FirstOrDefaultAsync();

                if (latest == null)
                {
                    return NotFound(new { message = "Aucune donnée biométrique trouvée" });
                }

                return Ok(latest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données", error = ex.Message });
            }
        }

        // POST: api/biometrics
        [HttpPost]
        public async Task<IActionResult> CreateBiometrics([FromBody] CreateBiometricsDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var biometric = new BiometrieActuelle
                {
                    SportifId = userId,
                    DateMesure = dto.DateMesure ?? DateTime.Now,
                    PoidsKg = dto.PoidsKg,
                    TauxMasseGrassePercent = dto.TauxMasseGrassePercent,
                    TourDeTailleCm = dto.TourDeTailleCm
                };

                _context.BiometriesActuelles.Add(biometric);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Données biométriques enregistrées avec succès",
                    id = biometric.Id,
                    dateMesure = biometric.DateMesure
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'enregistrement des données", error = ex.Message });
            }
        }

        // DELETE: api/biometrics/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBiometrics(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var biometric = await _context.BiometriesActuelles
                    .FirstOrDefaultAsync(b => b.Id == id && b.SportifId == userId);

                if (biometric == null)
                {
                    return NotFound(new { message = "Donnée biométrique non trouvée" });
                }

                _context.BiometriesActuelles.Remove(biometric);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Donnée supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression", error = ex.Message });
            }
        }
    }

    public class CreateBiometricsDto
    {
        public DateTime? DateMesure { get; set; }
        public float PoidsKg { get; set; }
        public float? TauxMasseGrassePercent { get; set; }
        public float? TourDeTailleCm { get; set; }
    }
}

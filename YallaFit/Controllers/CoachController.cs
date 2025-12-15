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
    [Authorize(Roles = "Coach,Admin")]
    public class CoachController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public CoachController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/coach/athletes?filter=my or filter=all
        [HttpGet("athletes")]
        public async Task<IActionResult> GetMyAthletes([FromQuery] string? filter = "all")
        {
            try
            {
                var coachId = GetCurrentUserId();

                List<Utilisateur> sportifs;

                if (filter == "my")
                {
                    // Get only athletes who have active enrollments in this coach's programs
                    var coachProgramIds = await _context.Programmes
                        .Where(p => p.CoachId == coachId)
                        .Select(p => p.Id)
                        .ToListAsync();

                    var athleteIds = await _context.ProgrammeEnrollments
                        .Where(e => e.IsActive && coachProgramIds.Contains(e.ProgrammeId))
                        .Select(e => e.SportifId)
                        .Distinct()
                        .ToListAsync();

                    sportifs = await _context.Utilisateurs
                        .Where(u => athleteIds.Contains(u.Id))
                        .Include(u => u.ProfilSportif)
                        .ToListAsync();
                }
                else
                {
                    // Get all sportifs (default behavior)
                    sportifs = await _context.Utilisateurs
                        .Where(u => u.Role == "Sportif")
                        .Include(u => u.ProfilSportif)
                        .ToListAsync();
                }

                var athletes = new List<object>();
                foreach (var sportif in sportifs)
                {
                    var latestBiometric = await _context.BiometriesActuelles
                        .Where(b => b.SportifId == sportif.Id)
                        .OrderByDescending(b => b.DateMesure)
                        .FirstOrDefaultAsync();

                    athletes.Add(new
                    {
                        Id = sportif.Id,
                        NomComplet = sportif.NomComplet,
                        Email = sportif.Email,
                        Age = sportif.ProfilSportif?.Age,
                        Poids = sportif.ProfilSportif?.Poids,
                        Taille = sportif.ProfilSportif?.Taille,
                        ObjectifPrincipal = sportif.ProfilSportif?.ObjectifPrincipal,
                        LastWeight = latestBiometric?.PoidsKg,
                        LastActive = latestBiometric?.DateMesure
                    });
                }

                return Ok(athletes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des athlètes", error = ex.Message });
            }
        }

        // GET: api/coach/athletes/{id}
        [HttpGet("athletes/{id}")]
        public async Task<IActionResult> GetAthleteDetails(int id)
        {
            try
            {
                var coachId = GetCurrentUserId();
                var athlete = await _context.Utilisateurs
                    .Where(u => u.Id == id && u.Role == "Sportif")
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync();

                if (athlete == null)
                {
                    return NotFound(new { message = "Athlète non trouvé" });
                }

                var biometrics = await _context.BiometriesActuelles
                    .Where(b => b.SportifId == id)
                    .OrderByDescending(b => b.DateMesure)
                    .Take(10)
                    .ToListAsync();

                var details = new
                {
                    Id = athlete.Id,
                    NomComplet = athlete.NomComplet,
                    Email = athlete.Email,
                    Profile = athlete.ProfilSportif != null ? new
                    {
                        Age = athlete.ProfilSportif.Age,
                        Poids = athlete.ProfilSportif.Poids,
                        Taille = athlete.ProfilSportif.Taille,
                        Sexe = athlete.ProfilSportif.Sexe,
                        NiveauActivite = athlete.ProfilSportif.NiveauActivite,
                        ObjectifPrincipal = athlete.ProfilSportif.ObjectifPrincipal,
                        PreferencesAlimentaires = athlete.ProfilSportif.PreferencesAlim,
                        Allergies = athlete.ProfilSportif.Allergies
                    } : null,
                    Biometrics = biometrics.Select(b => new
                    {
                        Date = b.DateMesure,
                        Poids = b.PoidsKg,
                        TauxMasseGrasse = b.TauxMasseGrassePercent,
                        TourDeTaille = b.TourDeTailleCm
                    }).ToList()
                };

                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des détails de l'athlète", error = ex.Message });
            }
        }

        // GET: api/coach/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetCoachStats()
        {
            try
            {
                var coachId = GetCurrentUserId();
                
                var totalProgrammes = await _context.Programmes
                    .Where(p => p.CoachId == coachId)
                    .CountAsync();

                var totalAthletes = await _context.Utilisateurs
                    .Where(u => u.Role == "Sportif")
                    .CountAsync();

                var totalSessions = await _context.Seances
                    .Where(s => s.Programme.CoachId == coachId)
                    .CountAsync();

                var stats = new
                {
                    TotalProgrammes = totalProgrammes,
                    TotalAthletes = totalAthletes,
                    TotalSessions = totalSessions,
                    ActiveProgrammes = totalProgrammes // Simplified for now
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des statistiques", error = ex.Message });
            }
        }
        // POST: api/coach/assign-program
        [HttpPost("assign-program")]
        public async Task<IActionResult> AssignProgram([FromBody] AssignProgramDto dto)
        {
            try
            {
                var coachId = GetCurrentUserId();

                // Verify the program belongs to the coach (or is public/admin if we had that concept)
                var program = await _context.Programmes.FindAsync(dto.ProgramId);
                if (program == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                if (program.CoachId != coachId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                // Verify athlete exists and is a Sportif
                var athlete = await _context.Utilisateurs.FindAsync(dto.AthleteId);
                if (athlete == null || athlete.Role != "Sportif")
                {
                    return NotFound(new { message = "Athlète non trouvé" });
                }

                // Check if already enrolled
                var existingEnrollment = await _context.ProgrammeEnrollments
                    .FirstOrDefaultAsync(e => e.SportifId == dto.AthleteId && e.ProgrammeId == dto.ProgramId && e.IsActive);

                if (existingEnrollment != null)
                {
                    return BadRequest(new { message = "L'athlète est déjà inscrit à ce programme" });
                }

                // Create new enrollment
                var enrollment = new ProgrammeEnrollment
                {
                    SportifId = dto.AthleteId,
                    ProgrammeId = dto.ProgramId,
                    EnrolledAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ProgrammeEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Programme assigné avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'assignation du programme", error = ex.Message });
            }
        }
    }
}


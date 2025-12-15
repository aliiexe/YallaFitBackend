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
    public class ProgrammeController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public ProgrammeController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/programme
        [HttpGet]
        public async Task<IActionResult> GetAllProgrammes()
        {
            try
            {
                var programmes = await _context.Programmes
                    .Include(p => p.Coach)
                    .Include(p => p.Seances)
                    .Select(p => new ProgrammeListDto
                    {
                        Id = p.Id,
                        Titre = p.Titre,
                        DureeSemaines = p.DureeSemaines,
                        CoachId = p.CoachId,
                        CoachName = p.Coach != null ? p.Coach.NomComplet : "Coach Inconnu",
                        SessionCount = p.Seances.Count,
                        IsPublic = p.IsPublic
                    })
                    .ToListAsync();

                return Ok(programmes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des programmes", error = ex.Message });
            }
        }

        // GET: api/programme/assigned
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedProgramme()
        {
            try
            {
                var userId = GetCurrentUserId();
                var userProfile = await _context.ProfilsSportifs
                    .Include(p => p.Programme)
                    .ThenInclude(prog => prog.Coach)
                    .Include(p => p.Programme)
                    .ThenInclude(prog => prog.Seances) // Include basic session info
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (userProfile?.Programme == null)
                {
                    return Ok(null); // No program assigned
                }

                var programme = userProfile.Programme;
                var result = new ProgrammeListDto
                {
                    Id = programme.Id,
                    Titre = programme.Titre,
                    DureeSemaines = programme.DureeSemaines,
                    CoachId = programme.CoachId,
                    CoachName = programme.Coach?.NomComplet ?? "Coach Inconnu",
                    SessionCount = programme.Seances.Count,
                    IsPublic = programme.IsPublic
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du programme assigné", error = ex.Message });
            }
        }

        // GET: api/programme/my-programs
        [HttpGet("my-programs")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> GetMyProgrammes()
        {
            try
            {
                var coachId = GetCurrentUserId();
                var programmes = await _context.Programmes
                    .Where(p => p.CoachId == coachId)
                    .Include(p => p.Coach)
                    .Include(p => p.Seances)
                    .Select(p => new ProgrammeListDto
                    {
                        Id = p.Id,
                        Titre = p.Titre,
                        DureeSemaines = p.DureeSemaines,
                        CoachId = p.CoachId,
                        CoachName = p.Coach.NomComplet,
                        SessionCount = p.Seances.Count,
                        IsPublic = p.IsPublic
                    })
                    .ToListAsync();

                return Ok(programmes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération de vos programmes", error = ex.Message });
            }
        }

        // GET: api/programme/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramme(int id)
        {
            try
            {
                var programme = await _context.Programmes
                    .Include(p => p.Coach)
                    .Include(p => p.Seances)
                    .ThenInclude(s => s.DetailSeances)
                    .ThenInclude(d => d.Exercice)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                var result = new ProgrammeDetailDto
                {
                    Id = programme.Id,
                    Titre = programme.Titre,
                    DureeSemaines = programme.DureeSemaines,
                    CoachId = programme.CoachId,
                    CoachName = programme.Coach?.NomComplet ?? "Coach Inconnu",
                    IsPublic = programme.IsPublic,
                    Seances = programme.Seances.Select(s => new SeanceDto
                    {
                        Id = s.Id,
                        Nom = s.Nom,
                        JourSemaine = s.JourSemaine,
                        Exercices = s.DetailSeances.Select(d => new DetailSeanceDto
                        {
                            ExerciceId = d.ExerciceId,
                            ExerciceNom = d.Exercice?.Nom ?? "Exercice Supprimé",
                            MuscleCible = d.Exercice?.MuscleCible,
                            Series = d.Series,
                            Repetitions = d.Repetitions,
                            PoidsConseille = d.PoidsConseille
                        }).ToList()
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du programme", error = ex.Message });
            }
        }

        // POST: api/programme
        [HttpPost]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> CreateProgramme([FromBody] CreateProgrammeDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Titre))
                {
                    return BadRequest(new { message = "Le titre du programme est requis" });
                }

                if (dto.DureeSemaines <= 0)
                {
                    return BadRequest(new { message = "La durée doit être supérieure à 0" });
                }

                var coachId = GetCurrentUserId();
                var programme = new Programme
                {
                    Titre = dto.Titre,
                    DureeSemaines = dto.DureeSemaines,
                    CoachId = coachId,
                    IsPublic = dto.IsPublic
                };

                _context.Programmes.Add(programme);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProgramme), new { id = programme.Id }, new
                {
                    id = programme.Id,
                    titre = programme.Titre,
                    dureeSemaines = programme.DureeSemaines
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création du programme", error = ex.Message });
            }
        }

        // PUT: api/programme/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> UpdateProgramme(int id, [FromBody] UpdateProgrammeDto dto)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(id);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                // Verify ownership (coach can only edit their own programs)
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");
                if (programme.CoachId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(dto.Titre))
                    programme.Titre = dto.Titre;

                if (dto.DureeSemaines.HasValue && dto.DureeSemaines.Value > 0)
                    programme.DureeSemaines = dto.DureeSemaines.Value;

                if (dto.IsPublic.HasValue)
                    programme.IsPublic = dto.IsPublic.Value;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = programme.Id,
                    titre = programme.Titre,
                    dureeSemaines = programme.DureeSemaines
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du programme", error = ex.Message });
            }
        }

        // DELETE: api/programme/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> DeleteProgramme(int id)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(id);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                // Verify ownership
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");
                if (programme.CoachId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                _context.Programmes.Remove(programme);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Programme supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression du programme", error = ex.Message });
            }
        }

        // POST: api/programme/{programmeId}/seance
        [HttpPost("{programmeId}/seance")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> AddSeance(int programmeId, [FromBody] CreateSeanceDto dto)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(programmeId);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                // Verify ownership
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");
                if (programme.CoachId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                if (string.IsNullOrWhiteSpace(dto.Nom))
                {
                    return BadRequest(new { message = "Le nom de la séance est requis" });
                }

                if (dto.JourSemaine < 1 || dto.JourSemaine > 7)
                {
                    return BadRequest(new { message = "Le jour de la semaine doit être entre 1 et 7" });
                }

                var seance = new Seance
                {
                    ProgrammeId = programmeId,
                    Nom = dto.Nom,
                    JourSemaine = dto.JourSemaine
                };

                _context.Seances.Add(seance);
                await _context.SaveChangesAsync();

                // Add exercises to the session
                if (dto.Exercices != null && dto.Exercices.Count > 0)
                {
                    foreach (var exerciceDto in dto.Exercices)
                    {
                        var detailSeance = new DetailSeance
                        {
                            SeanceId = seance.Id,
                            ExerciceId = exerciceDto.ExerciceId,
                            Series = exerciceDto.Series,
                            Repetitions = exerciceDto.Repetitions,
                            PoidsConseille = exerciceDto.PoidsConseille
                        };
                        _context.DetailSeances.Add(detailSeance);
                    }
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetProgramme), new { id = programmeId }, new
                {
                    id = seance.Id,
                    nom = seance.Nom,
                    jourSemaine = seance.JourSemaine
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'ajout de la séance", error = ex.Message });
            }
        }

        // PUT: api/programme/{programmeId}/seance/{seanceId}
        [HttpPut("{programmeId}/seance/{seanceId}")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> UpdateSeance(int programmeId, int seanceId, [FromBody] UpdateSeanceDto dto)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(programmeId);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                // Verify ownership
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");
                if (programme.CoachId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                var seance = await _context.Seances
                    .Include(s => s.DetailSeances)
                    .FirstOrDefaultAsync(s => s.Id == seanceId && s.ProgrammeId == programmeId);

                if (seance == null)
                {
                    return NotFound(new { message = "Séance non trouvée" });
                }

                // Update session details
                if (!string.IsNullOrWhiteSpace(dto.Nom))
                    seance.Nom = dto.Nom;

                if (dto.JourSemaine.HasValue && dto.JourSemaine.Value >= 1 && dto.JourSemaine.Value <= 7)
                    seance.JourSemaine = dto.JourSemaine.Value;

                // Update exercises if provided
                if (dto.Exercices != null)
                {
                    // Remove existing exercises
                    _context.DetailSeances.RemoveRange(seance.DetailSeances);

                    // Add new exercises
                    foreach (var exerciceDto in dto.Exercices)
                    {
                        var detailSeance = new DetailSeance
                        {
                            SeanceId = seance.Id,
                            ExerciceId = exerciceDto.ExerciceId,
                            Series = exerciceDto.Series,
                            Repetitions = exerciceDto.Repetitions,
                            PoidsConseille = exerciceDto.PoidsConseille
                        };
                        _context.DetailSeances.Add(detailSeance);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Séance mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour de la séance", error = ex.Message });
            }
        }

        // DELETE: api/programme/{programmeId}/seance/{seanceId}
        [HttpDelete("{programmeId}/seance/{seanceId}")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> DeleteSeance(int programmeId, int seanceId)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(programmeId);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                // Verify ownership
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");
                if (programme.CoachId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                var seance = await _context.Seances.FirstOrDefaultAsync(s => s.Id == seanceId && s.ProgrammeId == programmeId);
                if (seance == null)
                {
                    return NotFound(new { message = "Séance non trouvée" });
                }

                _context.Seances.Remove(seance);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Séance supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression de la séance", error = ex.Message });
            }
        }
        // GET: api/programme/public
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicProgrammes()
        {
            try
            {
                var programmes = await _context.Programmes
                    .Where(p => p.IsPublic)
                    .Include(p => p.Coach)
                    .Include(p => p.Seances)
                    .Select(p => new ProgrammeListDto
                    {
                        Id = p.Id,
                        Titre = p.Titre,
                        DureeSemaines = p.DureeSemaines,
                        CoachId = p.CoachId,
                        CoachName = p.Coach != null ? p.Coach.NomComplet : "Coach Inconnu",
                        SessionCount = p.Seances.Count,
                        IsPublic = p.IsPublic
                    })
                    .ToListAsync();

                return Ok(programmes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des programmes publics", error = ex.Message });
            }
        }

        // POST: api/programme/{id}/enroll
        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> EnrollInProgramme(int id)
        {
            try
            {
                var programme = await _context.Programmes.FindAsync(id);
                if (programme == null)
                {
                    return NotFound(new { message = "Programme non trouvé" });
                }

                if (!programme.IsPublic)
                {
                    return BadRequest(new { message = "Ce programme n'est pas public" });
                }

                var userId = GetCurrentUserId();
                
                // Check if already enrolled
                var existingEnrollment = await _context.ProgrammeEnrollments
                    .FirstOrDefaultAsync(e => e.SportifId == userId && e.ProgrammeId == id && e.IsActive);
                
                if (existingEnrollment != null)
                {
                    return BadRequest(new { message = "Vous êtes déjà inscrit à ce programme" });
                }

                // Create new enrollment
                var enrollment = new ProgrammeEnrollment
                {
                    SportifId = userId,
                    ProgrammeId = id,
                    EnrolledAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ProgrammeEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Vous avez rejoint le programme avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'inscription au programme", error = ex.Message });
            }
        }

        // GET: api/programme/{id}/enrollment-status
        [HttpGet("{id}/enrollment-status")]
        public async Task<IActionResult> GetEnrollmentStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var enrollment = await _context.ProgrammeEnrollments
                    .FirstOrDefaultAsync(e => e.SportifId == userId && e.ProgrammeId == id && e.IsActive);

                return Ok(new { isEnrolled = enrollment != null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la vérification de l'inscription", error = ex.Message });
            }
        }
    }
}

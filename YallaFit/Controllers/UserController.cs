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
    public class UserController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public UserController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                var profile = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    NomComplet = user.NomComplet,
                    Role = user.Role,
                    ProfilSportif = user.ProfilSportif != null ? new SportifProfileDto
                    {
                        Age = user.ProfilSportif.Age,
                        Poids = user.ProfilSportif.Poids,
                        Taille = user.ProfilSportif.Taille,
                        Sexe = user.ProfilSportif.Sexe,
                        NiveauActivite = user.ProfilSportif.NiveauActivite,
                        ObjectifPrincipal = user.ProfilSportif.ObjectifPrincipal,
                        PreferencesAlimentaires = user.ProfilSportif.PreferencesAlimentaires,
                        Allergies = user.ProfilSportif.Allergies
                    } : null
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du profil", error = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Utilisateurs.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                user.NomComplet = dto.NomComplet;
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    user.Email = dto.Email;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Profil mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du profil", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                var profile = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    NomComplet = user.NomComplet,
                    Role = user.Role,
                    ProfilSportif = user.ProfilSportif != null ? new SportifProfileDto
                    {
                        Age = user.ProfilSportif.Age,
                        Poids = user.ProfilSportif.Poids,
                        Taille = user.ProfilSportif.Taille,
                        Sexe = user.ProfilSportif.Sexe,
                        NiveauActivite = user.ProfilSportif.NiveauActivite,
                        ObjectifPrincipal = user.ProfilSportif.ObjectifPrincipal,
                        PreferencesAlimentaires = user.ProfilSportif.PreferencesAlimentaires,
                        Allergies = user.ProfilSportif.Allergies
                    } : null
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération de l'utilisateur", error = ex.Message });
            }
        }

        [HttpGet("sportif-profile")]
        public async Task<IActionResult> GetSportifProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _context.ProfilsSportifs
                    .FirstOrDefaultAsync(p => p.SportifId == userId);

                if (profile == null)
                {
                    return NotFound(new { message = "Profil sportif non trouvé" });
                }

                var dto = new SportifProfileDto
                {
                    Age = profile.Age,
                    Poids = profile.Poids,
                    Taille = profile.Taille,
                    Sexe = profile.Sexe,
                    NiveauActivite = profile.NiveauActivite,
                    ObjectifPrincipal = profile.ObjectifPrincipal,
                    PreferencesAlimentaires = profile.PreferencesAlimentaires,
                    Allergies = profile.Allergies
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du profil sportif", error = ex.Message });
            }
        }

        [HttpPut("sportif-profile")]
        public async Task<IActionResult> UpdateSportifProfile([FromBody] UpdateSportifProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _context.ProfilsSportifs
                    .FirstOrDefaultAsync(p => p.SportifId == userId);

                if (profile == null)
                {
                    // Create new profile
                    profile = new ProfilSportif
                    {
                        SportifId = userId,
                        Age = dto.Age,
                        Poids = dto.Poids,
                        Taille = dto.Taille,
                        Sexe = dto.Sexe,
                        NiveauActivite = dto.NiveauActivite,
                        ObjectifPrincipal = dto.ObjectifPrincipal,
                        PreferencesAlimentaires = dto.PreferencesAlimentaires,
                        Allergies = dto.Allergies
                    };
                    _context.ProfilsSportifs.Add(profile);
                }
                else
                {
                    // Update existing profile
                    profile.Age = dto.Age;
                    profile.Poids = dto.Poids;
                    profile.Taille = dto.Taille;
                    profile.Sexe = dto.Sexe;
                    profile.NiveauActivite = dto.NiveauActivite;
                    profile.ObjectifPrincipal = dto.ObjectifPrincipal;
                    profile.PreferencesAlimentaires = dto.PreferencesAlimentaires;
                    profile.Allergies = dto.Allergies;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Profil sportif mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du profil sportif", error = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Utilisateurs.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.MotDePasse))
                {
                    return BadRequest(new { message = "Mot de passe actuel incorrect" });
                }

                // Hash and update new password
                user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mot de passe modifié avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du changement de mot de passe", error = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Utilisateurs
                    .Select(u => new UserProfileDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        NomComplet = u.NomComplet,
                        Role = u.Role
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des utilisateurs", error = ex.Message });
            }
        }
    }
}

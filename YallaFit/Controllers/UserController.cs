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
                    .FirstOrDefaultAsync(p => p.UserId == userId);

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
                Console.WriteLine($"[DEBUG] UpdateSportifProfile - UserId: {userId}");
                Console.WriteLine($"[DEBUG] Received DTO - Age: {dto.Age}, Sexe: {dto.Sexe}, Taille: {dto.Taille}");
                Console.WriteLine($"[DEBUG] Received DTO - NiveauActivite: {dto.NiveauActivite}, ObjectifPrincipal: {dto.ObjectifPrincipal}");
                Console.WriteLine($"[DEBUG] Received DTO - PreferencesAlim: {dto.PreferencesAlim}, Allergies: {dto.Allergies}");
                
                var profile = await _context.ProfilsSportifs
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    Console.WriteLine($"[DEBUG] Profile not found for user {userId}, creating new profile");
                    // Create new profile
                    profile = new ProfilSportif
                    {
                        UserId = userId,
                        Age = dto.Age,
                        Poids = dto.Poids,
                        Taille = dto.Taille,
                        Sexe = dto.Sexe,
                        DateNaissance = dto.DateNaissance,
                        NiveauActivite = dto.NiveauActivite,
                        ObjectifPrincipal = dto.ObjectifPrincipal,
                        PreferencesAlim = dto.PreferencesAlim,  // Use actual property name
                        Allergies = dto.Allergies,
                        ProblemesSante = dto.ProblemesSante
                    };
                    _context.ProfilsSportifs.Add(profile);
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Profile found for user {userId}, updating existing profile");
                    Console.WriteLine($"[DEBUG] Before update - ObjectifPrincipal: {profile.ObjectifPrincipal}, NiveauActivite: {profile.NiveauActivite}");
                    
                    // Update existing profile
                    profile.Age = dto.Age;
                    profile.Poids = dto.Poids;
                    profile.Taille = dto.Taille;
                    profile.Sexe = dto.Sexe;
                    profile.DateNaissance = dto.DateNaissance;
                    profile.NiveauActivite = dto.NiveauActivite;
                    profile.ObjectifPrincipal = dto.ObjectifPrincipal;
                    profile.PreferencesAlim = dto.PreferencesAlim;  // Use actual property name
                    profile.Allergies = dto.Allergies;
                    profile.ProblemesSante = dto.ProblemesSante;
                    
                    Console.WriteLine($"[DEBUG] After update - ObjectifPrincipal: {profile.ObjectifPrincipal}, NiveauActivite: {profile.NiveauActivite}");
                }

                var changes = await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] SaveChangesAsync completed - {changes} entities updated");

                return Ok(new { message = "Profil sportif mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateSportifProfile failed: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NomComplet) ||
                    string.IsNullOrWhiteSpace(dto.MotDePasse) || string.IsNullOrWhiteSpace(dto.Role))
                {
                    return BadRequest(new { message = "Tous les champs sont requis" });
                }

                // Check if email already exists
                var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Cet email est déjà utilisé" });
                }

                // Validate role
                var validRoles = new[] { "Admin", "Coach", "Sportif" };
                if (!validRoles.Contains(dto.Role))
                {
                    return BadRequest(new { message = "Rôle invalide" });
                }

                // Create new user
                var newUser = new Utilisateur
                {
                    Email = dto.Email,
                    NomComplet = dto.NomComplet,
                    MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse),
                    Role = dto.Role
                };

                _context.Utilisateurs.Add(newUser);
                await _context.SaveChangesAsync();

                var userProfile = new UserProfileDto
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    NomComplet = newUser.NomComplet,
                    Role = newUser.Role
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création de l'utilisateur", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await _context.Utilisateurs.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                // Update email if provided
                if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
                {
                    // Check if new email already exists
                    var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);
                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Cet email est déjà utilisé" });
                    }
                    user.Email = dto.Email;
                }

                // Update name if provided
                if (!string.IsNullOrWhiteSpace(dto.NomComplet))
                {
                    user.NomComplet = dto.NomComplet;
                }

                // Update role if provided
                if (!string.IsNullOrWhiteSpace(dto.Role))
                {
                    var validRoles = new[] { "Admin", "Coach", "Sportif" };
                    if (!validRoles.Contains(dto.Role))
                    {
                        return BadRequest(new { message = "Rôle invalide" });
                    }
                    user.Role = dto.Role;
                }

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(dto.MotDePasse))
                {
                    user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse);
                }

                await _context.SaveChangesAsync();

                var userProfile = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    NomComplet = user.NomComplet,
                    Role = user.Role
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour de l'utilisateur", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Prevent self-deletion
                if (id == currentUserId)
                {
                    return BadRequest(new { message = "Vous ne pouvez pas supprimer votre propre compte" });
                }

                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                // Delete related profile if exists
                if (user.ProfilSportif != null)
                {
                    _context.ProfilsSportifs.Remove(user.ProfilSportif);
                }

                // Delete user
                _context.Utilisateurs.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Utilisateur supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression de l'utilisateur", error = ex.Message });
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteOwnAccount()
        {
            try
            {   
                var userId = GetCurrentUserId();
                Console.WriteLine($"[DEBUG] Delete account request from userId: {userId}");
                
                var user = await _context.Utilisateurs
                    .Include(u => u.ProfilSportif)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                // Delete related profile if exists
                if (user.ProfilSportif != null)
                {
                    Console.WriteLine($"[DEBUG] Deleting profile for user {userId}");
                    _context.ProfilsSportifs.Remove(user.ProfilSportif);
                }

                // Delete user
                Console.WriteLine($"[DEBUG] Deleting user {userId}");
                _context.Utilisateurs.Remove(user);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] User {userId} deleted successfully");

                return Ok(new { message = "Compte supprimé avec succès" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Delete account failed: {ex.Message}");
                return StatusCode(500, new { message = "Erreur lors de la suppression du compte", error = ex.Message });
            }
        }
    }
}

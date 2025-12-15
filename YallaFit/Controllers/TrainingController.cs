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
    public class TrainingController : ControllerBase
    {
        private readonly YallaFitDbContext _context;

        public TrainingController(YallaFitDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // POST: api/training/sessions
        [HttpPost("sessions")]
        public async Task<IActionResult> SaveTrainingSession([FromBody] SaveTrainingSessionDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Create training session
                var trainingSession = new TrainingSession
                {
                    SportifId = userId,
                    ProgrammeId = dto.ProgrammeId,
                    SeanceId = dto.SeanceId,
                    DateCompleted = dto.DateCompleted,
                    DurationMinutes = dto.DurationMinutes,
                    Notes = dto.Notes
                };

                _context.TrainingSessions.Add(trainingSession);
                await _context.SaveChangesAsync();

                // Create training exercises and sets
                foreach (var exerciseDto in dto.Exercises)
                {
                    var trainingExercise = new TrainingExercise
                    {
                        TrainingSessionId = trainingSession.Id,
                        ExerciceId = exerciseDto.ExerciceId,
                        OrderIndex = exerciseDto.OrderIndex
                    };

                    _context.TrainingExercises.Add(trainingExercise);
                    await _context.SaveChangesAsync();

                    foreach (var setDto in exerciseDto.Sets)
                    {
                        var trainingSet =new TrainingSet
                        {
                            TrainingExerciseId = trainingExercise.Id,
                            SetNumber = setDto.SetNumber,
                            Reps = setDto.Reps,
                            WeightKg = setDto.WeightKg,
                            Completed = setDto.Completed,
                            Notes = setDto.Notes
                        };

                        _context.TrainingSets.Add(trainingSet);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Séance d'entraînement enregistrée avec succès", sessionId = trainingSession.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'enregistrement de la séance", error = ex.Message });
            }
        }

        // GET: api/training/sessions
        [HttpGet("sessions")]
        public async Task<IActionResult> GetTrainingSessions([FromQuery] int? programmeId = null, [FromQuery] int limit = 20)
        {
            try
            {
                var userId = GetCurrentUserId();

                var query = _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId)
                    .Include(ts => ts.Programme)
                    .Include(ts => ts.Seance)
                    .Include(ts => ts.TrainingExercises)
                        .ThenInclude(te => te.TrainingSets)
                    .AsQueryable();

                if (programmeId.HasValue)
                {
                    query = query.Where(ts => ts.ProgrammeId == programmeId.Value);
                }

                var sessions = await query
                    .OrderByDescending(ts => ts.DateCompleted)
                    .Take(limit)
                    .Select(ts => new TrainingSessionHistoryDto
                    {
                        Id = ts.Id,
                        ProgrammeId = ts.ProgrammeId,
                        ProgrammeTitre = ts.Programme.Titre,
                        SeanceId = ts.SeanceId,
                        SeanceNom = ts.Seance.Nom,
                        DateCompleted = ts.DateCompleted,
                        DurationMinutes = ts.DurationMinutes,
                        Notes = ts.Notes,
                        ExerciseCount = ts.TrainingExercises.Count,
                        TotalSets = ts.TrainingExercises.Sum(te => te.TrainingSets.Count)
                    })
                    .ToListAsync();

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération de l'historique", error = ex.Message });
            }
        }

        // GET: api/training/sessions/{id}
        [HttpGet("sessions/{id}")]
        public async Task<IActionResult> GetTrainingSessionDetail(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var session = await _context.TrainingSessions
                    .Where(ts => ts.Id == id && ts.SportifId == userId)
                    .Include(ts => ts.Programme)
                    .Include(ts => ts.Seance)
                    .Include(ts => ts.TrainingExercises)
                        .ThenInclude(te => te.Exercice)
                    .Include(ts => ts.TrainingExercises)
                        .ThenInclude(te => te.TrainingSets)
                    .FirstOrDefaultAsync();

                if (session == null)
                {
                    return NotFound(new { message = "Séance non trouvée" });
                }

                var detailDto = new TrainingSessionDetailDto
                {
                    Id = session.Id,
                    ProgrammeId = session.ProgrammeId,
                    ProgrammeTitre = session.Programme.Titre,
                    SeanceId = session.SeanceId,
                    SeanceNom = session.Seance.Nom,
                    DateCompleted = session.DateCompleted,
                    DurationMinutes = session.DurationMinutes,
                    Notes = session.Notes,
                    Exercises = session.TrainingExercises
                        .OrderBy(te => te.OrderIndex)
                        .Select(te => new TrainingExerciseDetailDto
                        {
                            ExerciceId = te.ExerciceId,
                            ExerciceNom = te.Exercice.Nom,
                            MuscleCible = te.Exercice.MuscleCible,
                            OrderIndex = te.OrderIndex,
                            Sets = te.TrainingSets
                                .OrderBy(ts => ts.SetNumber)
                                .Select(ts => new TrainingSetDetailDto
                                {
                                    SetNumber = ts.SetNumber,
                                    Reps = ts.Reps,
                                    WeightKg = ts.WeightKg,
                                    Completed = ts.Completed,
                                    Notes = ts.Notes
                                })
                                .ToList()
                        })
                        .ToList()
                };

                return Ok(detailDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération de la séance", error = ex.Message });
            }
        }

        // GET: api/training/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetTrainingStats([FromQuery] int? programmeId = null)
        {
            try
            {
                var userId = GetCurrentUserId();

                var query = _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId)
                    .AsQueryable();

                if (programmeId.HasValue)
                {
                    query = query.Where(ts => ts.ProgrammeId == programmeId.Value);
                }

                var totalSessions = await query.CountAsync();
                var totalMinutes = await query.SumAsync(ts => ts.DurationMinutes);
                var lastSessionDate = await query.MaxAsync(ts => (DateTime?)ts.DateCompleted);

                return Ok(new
                {
                    totalSessions,
                    totalMinutes,
                    totalHours = Math.Round(totalMinutes / 60.0, 1),
                    lastSessionDate,
                    averageDuration = totalSessions > 0 ? Math.Round(totalMinutes / (double)totalSessions, 0) : 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des statistiques", error = ex.Message });
            }
        }

        // GET: api/training/progress/{exerciceId}
        [HttpGet("progress/{exerciceId}")]
        public async Task<IActionResult> GetExerciseProgress(int exerciceId, [FromQuery] int? programmeId = null, [FromQuery] int limit = 20)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get exercise info
                var exercice = await _context.Exercices.FindAsync(exerciceId);
                if (exercice == null)
                {
                    return NotFound(new { message = "Exercice non trouvé" });
                }

                // Get all sessions containing this exercise
                var query = _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId)
                    .Include(ts => ts.TrainingExercises)
                        .ThenInclude(te => te.TrainingSets)
                    .Where(ts => ts.TrainingExercises.Any(te => te.ExerciceId == exerciceId))
                    .AsQueryable();

                if (programmeId.HasValue)
                {
                    query = query.Where(ts => ts.ProgrammeId == programmeId.Value);
                }

                var sessions = await query
                    .OrderByDescending(ts => ts.DateCompleted)
                    .Take(limit)
                    .ToListAsync();

                // Calculate data points
                var dataPoints = sessions
                    .OrderBy(ts => ts.DateCompleted)
                    .Select(ts =>
                    {
                        var exerciseSets = ts.TrainingExercises
                            .Where(te => te.ExerciceId == exerciceId)
                            .SelectMany(te => te.TrainingSets)
                            .ToList();

                        var maxWeight = exerciseSets.Any(s => s.WeightKg.HasValue)
                            ? exerciseSets.Where(s => s.WeightKg.HasValue).Max(s => s.WeightKg!.Value)
                            : (decimal?)null;

                        var totalReps = exerciseSets.Sum(s => s.Reps);
                        var totalVolume = exerciseSets
                             .Where(s => s.WeightKg.HasValue)
                            .Sum(s => s.Reps * s.WeightKg!.Value);

                        return new ProgressDataPoint
                        {
                            Date = ts.DateCompleted,
                            MaxWeight = maxWeight,
                            TotalReps = totalReps,
                            TotalVolume = totalVolume,
                            SessionId = ts.Id,
                            SetsCompleted = exerciseSets.Count
                        };
                    })
                    .ToList();

                // Calculate stats
                var allWeights = dataPoints.Where(dp => dp.MaxWeight.HasValue).Select(dp => dp.MaxWeight!.Value).ToList();
                var allVolumes = dataPoints.Select(dp => dp.TotalVolume).ToList();

                var stats = new ProgressStats
                {
                    PersonalRecord = allWeights.Any() ? allWeights.Max() : null,
                    TotalSessions = dataPoints.Count,
                    AverageVolume = allVolumes.Any() ? Math.Round(allVolumes.Average(), 2) : 0,
                    LastPerformed = dataPoints.Any() ? dataPoints.Max(dp => dp.Date) : null,
                    AverageWeight = allWeights.Any() ? Math.Round(allWeights.Average(), 2) : null
                };

                var result = new ExerciseProgressDto
                {
                    ExerciceId = exercice.Id,
                    ExerciceNom = exercice.Nom,
                    MuscleCible = exercice.MuscleCible,
                    DataPoints = dataPoints,
                    Stats = stats
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des progrès", error = ex.Message });
            }
        }

        // GET: api/training/exercises-with-history
        [HttpGet("exercises-with-history")]
        public async Task<IActionResult> GetExercisesWithHistory()
        {
            try
            {
                var userId = GetCurrentUserId();

                var exercises = await _context.TrainingSessions
                    .Where(ts => ts.SportifId == userId)
                    .Include(ts => ts.TrainingExercises)
                        .ThenInclude(te => te.Exercice)
                    .SelectMany(ts => ts.TrainingExercises)
                    .GroupBy(te => te.ExerciceId)
                    .Select(g => new ExerciseWithHistoryDto
                    {
                        Id = g.Key,
                        Nom = g.First().Exercice.Nom,
                        MuscleCible = g.First().Exercice.MuscleCible,
                        SessionCount = g.Select(te => te.TrainingSessionId).Distinct().Count(),
                        LastPerformed = g.Select(te => te.TrainingSession.DateCompleted).Max()
                    })
                    .OrderByDescending(e => e.LastPerformed)
                    .ToListAsync();

                return Ok(exercises);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des exercices", error = ex.Message });
            }
        }
    }
}

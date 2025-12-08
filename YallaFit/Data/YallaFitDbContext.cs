using Microsoft.EntityFrameworkCore;
using YallaFit.Models;

namespace YallaFit.Data
{
    public class YallaFitDbContext : DbContext
    {
        public YallaFitDbContext(DbContextOptions<YallaFitDbContext> options) : base(options)
        {
        }

        // DbSet properties for all entities
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<ProfilSportif> ProfilsSportifs { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Exercice> Exercices { get; set; }
        public DbSet<Seance> Seances { get; set; }
        public DbSet<DetailSeance> DetailSeances { get; set; }
        public DbSet<PlanNutrition> PlansNutrition { get; set; }
        public DbSet<Repas> Repas { get; set; }
        public DbSet<Aliment> Aliments { get; set; }
        public DbSet<CompositionRepas> CompositionRepas { get; set; }
        public DbSet<BiometrieActuelle> BiometriesActuelles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Utilisateur
            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasOne(u => u.ProfilSportif)
                    .WithOne(p => p.Utilisateur)
                    .HasForeignKey<ProfilSportif>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Programme
            modelBuilder.Entity<Programme>(entity =>
            {
                entity.HasOne(p => p.Coach)
                    .WithMany(u => u.ProgrammesCreated)
                    .HasForeignKey(p => p.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Seances)
                    .WithOne(s => s.Programme)
                    .HasForeignKey(s => s.ProgrammeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DetailSeance (composite key)
            modelBuilder.Entity<DetailSeance>(entity =>
            {
                entity.HasKey(ds => new { ds.SeanceId, ds.ExerciceId });

                entity.HasOne(ds => ds.Seance)
                    .WithMany(s => s.DetailSeances)
                    .HasForeignKey(ds => ds.SeanceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ds => ds.Exercice)
                    .WithMany(e => e.DetailSeances)
                    .HasForeignKey(ds => ds.ExerciceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PlanNutrition
            modelBuilder.Entity<PlanNutrition>(entity =>
            {
                entity.HasOne(pn => pn.Sportif)
                    .WithMany(u => u.PlansNutrition)
                    .HasForeignKey(pn => pn.SportifId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(pn => pn.Repas)
                    .WithOne(r => r.Plan)
                    .HasForeignKey(r => r.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CompositionRepas (composite key)
            modelBuilder.Entity<CompositionRepas>(entity =>
            {
                entity.HasKey(cr => new { cr.RepasId, cr.AlimentId });

                entity.HasOne(cr => cr.Repas)
                    .WithMany(r => r.CompositionRepas)
                    .HasForeignKey(cr => cr.RepasId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cr => cr.Aliment)
                    .WithMany(a => a.CompositionRepas)
                    .HasForeignKey(cr => cr.AlimentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure BiometrieActuelle
            modelBuilder.Entity<BiometrieActuelle>(entity =>
            {
                entity.HasOne(b => b.Sportif)
                    .WithMany(u => u.BiometriesMeasurements)
                    .HasForeignKey(b => b.SportifId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(b => new { b.SportifId, b.DateMesure });
            });

            // Configure decimal precision for float fields
            modelBuilder.Entity<Aliment>(entity =>
            {
                entity.Property(a => a.Proteines100g).HasPrecision(5, 2);
                entity.Property(a => a.Glucides100g).HasPrecision(5, 2);
                entity.Property(a => a.Lipides100g).HasPrecision(5, 2);
            });

            modelBuilder.Entity<DetailSeance>(entity =>
            {
                entity.Property(ds => ds.PoidsConseille).HasPrecision(6, 2);
            });

            modelBuilder.Entity<BiometrieActuelle>(entity =>
            {
                entity.Property(b => b.PoidsKg).HasPrecision(5, 2);
                entity.Property(b => b.TauxMasseGrassePercent).HasPrecision(5, 2);
                entity.Property(b => b.TourDeTailleCm).HasPrecision(5, 2);
            });
        }
    }
}

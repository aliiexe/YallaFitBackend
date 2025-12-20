# Entity Framework Migration Guide

## Problem: New Migrations Not Applied After Pulling from Git

If you've cloned the project or pulled new changes and your database doesn't have the latest tables, this guide will help you fix it.

## Root Cause

The issue occurs when:
1. Your database has a `__EFMigrationsHistory` table that tracks which migrations have been applied
2. This table might have incorrect entries (migrations marked as applied when tables don't exist)
3. Entity Framework checks this table and skips migrations it thinks are already applied

## Solution Options

### Option 1: Drop and Recreate Database (Recommended for Development)

**⚠️ WARNING: This will DELETE ALL DATA in your database!**

If you're in development and don't have important data:

```bash
# Navigate to the project folder
cd YallaFit

# Drop the existing database
dotnet ef database drop --force

# Recreate the database with all migrations
dotnet ef database update
```

### Option 2: Fix Migration History (If you have important data)

If you need to keep your data, you need to manually fix the `__EFMigrationsHistory` table:

1. **Check which migrations are recorded as applied:**
   ```sql
   SELECT * FROM __EFMigrationsHistory;
   ```

2. **Check which tables actually exist:**
   ```sql
   SHOW TABLES;
   ```

3. **Remove incorrect migration entries:**
   - Connect to your MySQL database
   - Delete entries from `__EFMigrationsHistory` for migrations that weren't actually applied
   - Only keep entries for migrations where the corresponding tables/columns actually exist

4. **Then run:**
   ```bash
   dotnet ef database update
   ```

### Option 3: Manual Migration Check (Advanced)

1. **List all migrations in the project:**
   ```bash
   dotnet ef migrations list
   ```

2. **Check your database migration history:**
   ```sql
   SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId;
   ```

3. **Compare the two lists** - any migrations in the project that aren't in your database need to be applied

4. **Apply missing migrations:**
   ```bash
   dotnet ef database update
   ```

## For First-Time Cloners

If you're cloning the project for the first time:

1. **Install .NET EF Core Tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef --version 9.0.0
   ```

2. **Configure your database connection** in `YallaFit/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=YallaFit;User=root;Password=YOUR_PASSWORD;CharSet=utf8mb4;"
     }
   }
   ```

3. **Restore NuGet packages:**
   ```bash
   cd YallaFit
   dotnet restore
   ```

4. **Create the database and apply all migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

The database will be created automatically with all tables and initial seed data.

## Files That Should NOT Be in Git

The following files/folders should be ignored (now handled by `.gitignore`):

- ✅ **`bin/`** - Compiled binaries (should be ignored)
- ✅ **`obj/`** - Build artifacts (should be ignored)
- ✅ **`.vs/`** - Visual Studio cache (should be ignored)
- ✅ **`*.user`** - User-specific settings (should be ignored)
- ✅ **`appsettings.Development.json`** - Local development settings (should be ignored)
- ✅ **`*.log`** - Log files (should be ignored)

## Files That SHOULD Be in Git

These files MUST be committed to Git:

- ✅ **`Migrations/`** folder - All migration files (`.cs` and `.Designer.cs`)
- ✅ **`Migrations/YallaFitDbContextModelSnapshot.cs`** - EF Core model snapshot
- ✅ **`Models/`** - All entity model files
- ✅ **`Data/YallaFitDbContext.cs`** - Database context
- ✅ **`appsettings.json`** - Base configuration (without secrets)
- ✅ **`*.csproj`** - Project file with dependencies

## Verifying Your Setup

After applying migrations, verify your database has all tables:

```sql
SHOW TABLES;
```

You should see these tables (and more):
- Utilisateur
- Profil_Sportif
- Programme
- Exercice
- Seance
- Detail_Seance
- Plan_Nutrition
- Repas
- Aliment
- Composition_Repas
- Biometrie_Actuelle
- ProgrammeEnrollment
- TrainingSession
- TrainingExercise
- TrainingSet
- Analyse_Repas_Photo

## Troubleshooting

### Error: "Migration already applied"
- Your `__EFMigrationsHistory` table has incorrect entries
- Use Option 2 above to fix it, or Option 1 to start fresh

### Error: "Table already exists"
- Some tables exist but migrations aren't recorded
- Drop the database and recreate (Option 1)

### Error: "Cannot find migration"
- Migration files might not be in Git
- Check that all files in `Migrations/` folder are committed
- Run `git status` to see uncommitted files

## Next Steps After Fixing

1. **Commit the `.gitignore` file** to prevent future issues:
   ```bash
   git add .gitignore
   git commit -m "Add .gitignore to exclude build artifacts"
   ```

2. **Remove build artifacts from Git** (if they were previously committed):
   ```bash
   git rm -r --cached YallaFit/bin/ YallaFit/obj/
   git commit -m "Remove build artifacts from Git"
   ```

3. **Push to GitHub** so your friend can pull the fix:
   ```bash
   git push
   ```

---

**Last Updated**: December 2025



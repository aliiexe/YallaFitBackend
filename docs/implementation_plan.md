# Implementation Plan - Sprint 1: Database Models & Configuration

## Goal
Set up the complete database foundation for YallaFit backend by creating all entity models, configuring MySQL with Entity Framework Core, and running migrations to establish the database schema.

---

## User Review Required

> [!IMPORTANT]
> **Database Configuration**
> - **Database**: MySQL on localhost:3306
> - **User**: root (no password)
> - **Database Name**: YallaFit
> - **Provider**: Pomelo.EntityFrameworkCore.MySql

> [!IMPORTANT]
> **AI Nutrition Service**
> - **Model**: DeepSeek (fine-tuned as nutritionist)
> - This will be integrated in Sprint 4 with custom prompts and fine-tuning for French nutrition recommendations

> [!IMPORTANT]
> **French Language Support**
> The database will be configured with `utf8mb4` character set to properly support French language content (accents, special characters).

> [!WARNING]
> **Password Storage**
> User passwords will be stored using secure hashing (BCrypt). The initial implementation will include a plain `mot_de_passe` field, but password hashing will be implemented in Sprint 2 (Authentication).

---

## Proposed Changes

### Backend Core

#### [NEW] [Utilisateur.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Utilisateur.cs)
Create the main user entity model with:
- Primary key: `Id`
- Fields: `NomComplet`, `Email`, `MotDePasse`, `Role`
- Navigation properties: `ProfilSportif`, `ProgrammesCreated` (for coaches)
- Data annotations for validation

#### [NEW] [ProfilSportif.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/ProfilSportif.cs)
Create athletic profile entity with:
- Foreign key: `UserId` → `Utilisateur`
- Biometric fields: `DateNaissance`, `Genre`, `TailleCm`, `NiveauActivite`
- Goal fields: `ObjectifPrincipal`, `Allergies`, `PreferencesAlim`, `ProblemesSante`
- Navigation property: `Utilisateur`

#### [NEW] [Programme.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Programme.cs)
Create training program entity with:
- Primary key: `Id`
- Foreign key: `CoachId` → `Utilisateur`
- Fields: `Titre`, `DureeSemaines`
- Navigation properties: `Coach`, `Seances` collection

#### [NEW] [Exercice.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Exercice.cs)
Create exercise library entity with:
- Primary key: `Id`
- Fields: `Nom`, `VideoUrl`, `MuscleCible`
- Navigation property: `DetailSeances` collection

#### [NEW] [Seance.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Seance.cs)
Create training session entity with:
- Primary key: `Id`
- Foreign key: `ProgrammeId` → `Programme`
- Fields: `Nom`, `JourSemaine` (int: 1-7)
- Navigation properties: `Programme`, `DetailSeances` collection

#### [NEW] [DetailSeance.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/DetailSeance.cs)
Create session detail entity (composite key) with:
- Foreign keys: `SeanceId` → `Seance`, `ExerciceId` → `Exercice`
- Fields: `Series`, `Repetitions`, `PoidsConseille` (float)
- Navigation properties: `Seance`, `Exercice`

#### [NEW] [PlanNutrition.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/PlanNutrition.cs)
Create nutrition plan entity with:
- Primary key: `Id`
- Foreign key: `SportifId` → `Utilisateur`
- Fields: `DateGeneration`, `ObjectifCaloriqueJournalier`, `ObjectifProteinesG`, `ObjectifGlucidesG`, `ObjectifLipidesG`, `EstActif` (boolean)
- Navigation properties: `Sportif`, `Repas` collection

#### [NEW] [Repas.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Repas.cs)
Create meal entity with:
- Primary key: `Id`
- Foreign key: `PlanId` → `PlanNutrition`
- Fields: `Nom`, `HeurePrevue` (TimeSpan)
- Navigation properties: `Plan`, `CompositionRepas` collection

#### [NEW] [Aliment.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Aliment.cs)
Create food item entity with:
- Primary key: `Id`
- Fields: `Nom`, `Calories100g`, `Proteines100g`, `Glucides100g`, `Lipides100g` (all int/float)
- Navigation property: `CompositionRepas` collection

#### [NEW] [CompositionRepas.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/CompositionRepas.cs)
Create meal composition entity (composite key) with:
- Foreign keys: `RepasId` → `Repas`, `AlimentId` → `Aliment`
- Field: `QuantiteGrammes` (int)
- Navigation properties: `Repas`, `Aliment`

#### [NEW] [BiometrieActuelle.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/BiometrieActuelle.cs)
Create biometric measurement entity with:
- Primary key: `Id`
- Foreign key: `SportifId` → `Utilisateur`
- Fields: `DateMesure`, `PoidsKg`, `TauxMasseGrassePercent`, `TourDeTailleCm` (all float)
- Navigation property: `Sportif`

---

### Database Configuration

#### [NEW] [YallaFitDbContext.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Data/YallaFitDbContext.cs)
Create Entity Framework DbContext with:
- DbSet properties for all 11 entities
- `OnModelCreating` method with fluent API configurations:
  - Configure composite keys for `DetailSeance` and `CompositionRepas`
  - Configure relationships (one-to-many, many-to-many)
  - Configure foreign key constraints with cascade delete where appropriate
  - Configure indexes on foreign keys and frequently queried fields
  - Configure decimal precision for float fields
  - Set table names to match French naming convention

#### [MODIFY] [appsettings.json](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/appsettings.json)
Add MySQL connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=YallaFit;User=root;CharSet=utf8mb4;"
  }
}
```

#### [MODIFY] [Program.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Program.cs)
Configure Entity Framework Core with MySQL:
- Add `builder.Services.AddDbContext<YallaFitDbContext>()` with Pomelo MySQL provider
- Configure connection string from appsettings
- Add connection pooling settings
- Configure server version detection

---

### Migrations

#### [NEW] Migration: `InitialCreate`
Create initial migration containing:
- All 11 tables with proper column types
- Primary key constraints
- Foreign key constraints with appropriate cascade rules
- Indexes on foreign keys
- Unique constraints (e.g., email uniqueness)

---

### Data Seeding

#### [NEW] [DbInitializer.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Data/DbInitializer.cs)
Create database seeding class with:
- Seed common exercises (10-15 basic exercises covering major muscle groups)
- Seed common food items (20-30 basic foods with nutritional data)
- Seed admin user account (email: admin@yallafit.com)
- Method to check if seeding is needed (avoid duplicates)

---

## Verification Plan

### Automated Tests

#### Database Connection Test
```bash
# Navigate to project directory
cd c:\Users\abour\.gemini\antigravity\scratch\YallaFit\YallaFitBackend\YallaFit

# Build the project to verify no compilation errors
dotnet build

# This will verify that the DbContext can be instantiated and EF Core is configured correctly
```

#### Migration Verification
```bash
# Create the initial migration
dotnet ef migrations add InitialCreate

# Review the generated migration file in Migrations folder
# Verify all tables and relationships are correct

# Apply migration to database
dotnet ef database update

# This will create the database and all tables
```

#### Database Schema Validation
```bash
# Run the application briefly to trigger seeding
dotnet run

# Press Ctrl+C after application starts successfully
```

### Manual Verification

#### MySQL Database Inspection
1. Connect to MySQL using MySQL Workbench or command line:
   ```bash
   mysql -u root -p
   ```

2. Verify database and tables exist:
   ```sql
   USE yallafit;
   SHOW TABLES;
   ```
   Expected output: 11 tables (Utilisateur, ProfilSportif, Programme, Exercice, Seance, DetailSeance, PlanNutrition, Repas, Aliment, CompositionRepas, BiometrieActuelle)

3. Verify table structure for key entities:
   ```sql
   DESCRIBE Utilisateur;
   DESCRIBE ProfilSportif;
   DESCRIBE Programme;
   ```

4. Verify foreign key constraints:
   ```sql
   SELECT 
     TABLE_NAME,
     COLUMN_NAME,
     CONSTRAINT_NAME,
     REFERENCED_TABLE_NAME,
     REFERENCED_COLUMN_NAME
   FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
   WHERE TABLE_SCHEMA = 'yallafit' 
     AND REFERENCED_TABLE_NAME IS NOT NULL;
   ```

5. Verify seed data:
   ```sql
   SELECT COUNT(*) FROM Exercice;  -- Should have 10-15 records
   SELECT COUNT(*) FROM Aliment;   -- Should have 20-30 records
   SELECT COUNT(*) FROM Utilisateur WHERE Role = 'Admin';  -- Should have 1 record
   ```

#### Connection String Test
1. Verify the application can connect to MySQL
2. Check for any connection errors in console output
3. Confirm character set is utf8mb4 for French language support

---

## Dependencies to Install

The following NuGet packages need to be installed:
- `Pomelo.EntityFrameworkCore.MySql` (version 7.0.0 or 8.0.0 depending on .NET version)
- `Microsoft.EntityFrameworkCore.Tools` (for migrations)
- `Microsoft.EntityFrameworkCore.Design` (for design-time support)

---

## Success Criteria

✅ All 11 entity models created with proper relationships  
✅ DbContext configured with all entities and relationships  
✅ MySQL connection string configured  
✅ Initial migration created successfully  
✅ Migration applied to database without errors  
✅ All 11 tables exist in MySQL database  
✅ Foreign key constraints properly configured  
✅ Seed data populated (exercises, foods, admin user)  
✅ Application runs without database connection errors  
✅ Character set configured for French language support

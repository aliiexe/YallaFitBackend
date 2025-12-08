# Sprint 1 Walkthrough - Database Models & Configuration

## ✅ Sprint 1 Completed Successfully

Sprint 1 focused on establishing the complete database foundation for the YallaFit backend application.

---

## What Was Accomplished

### 1. Entity Models Created (11 Total)

All entity models were created with proper data annotations, validation attributes, and navigation properties:

#### Core User Models
- [Utilisateur.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Utilisateur.cs) - Main user entity with authentication fields
- [ProfilSportif.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/ProfilSportif.cs) - Athletic profile with biometrics and preferences

#### Training Program Models
- [Programme.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Programme.cs) - Training programs
- [Exercice.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Exercice.cs) - Exercise library
- [Seance.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Seance.cs) - Training sessions
- [DetailSeance.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/DetailSeance.cs) - Session details (composite key)

#### Nutrition Models
- [PlanNutrition.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/PlanNutrition.cs) - Nutrition plans with macro targets
- [Repas.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Repas.cs) - Meals within plans
- [Aliment.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/Aliment.cs) - Food items database
- [CompositionRepas.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/CompositionRepas.cs) - Meal composition (composite key)

#### Progress Tracking
- [BiometrieActuelle.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Models/BiometrieActuelle.cs) - Biometric measurements

### 2. Database Context Configuration

Created [YallaFitDbContext.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Data/YallaFitDbContext.cs) with:

- **DbSet properties** for all 11 entities
- **Composite keys** configured for `DetailSeance` and `CompositionRepas`
- **Relationships** properly configured:
  - One-to-One: `Utilisateur` ↔ `ProfilSportif`
  - One-to-Many: `Programme` → `Seance`, `PlanNutrition` → `Repas`, etc.
  - Many-to-Many (via junction tables): Session exercises, meal compositions
- **Cascade behaviors** set appropriately (Cascade vs Restrict)
- **Unique index** on email field
- **Decimal precision** configured for float fields
- **Indexes** on foreign keys and frequently queried fields

### 3. MySQL Database Configuration

#### Connection String
Configured in [appsettings.json](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=YallaFit;User=root;CharSet=utf8mb4;"
  }
}
```

#### Entity Framework Core Setup
Updated [Program.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Program.cs) with:
- Pomelo MySQL provider configuration
- Auto-detect server version
- Database seeding on startup

#### Packages Installed
- `Pomelo.EntityFrameworkCore.MySql` (v9.0.0)
- `Microsoft.EntityFrameworkCore.Tools` (v9.0.0)
- `Microsoft.EntityFrameworkCore.Design` (v9.0.0)

### 4. Database Migration

Created and applied `InitialCreate` migration:
- ✅ All 11 tables created in MySQL
- ✅ Primary keys configured
- ✅ Foreign keys with proper constraints
- ✅ Unique indexes created
- ✅ Character set: utf8mb4 (French language support)

#### Key Fix Applied
Reduced email field length from 255 to 191 characters to accommodate MySQL utf8mb4 unique index limitation (4 bytes per character × 191 = 764 bytes < 1000 byte limit).

### 5. Database Seeding

Created [DbInitializer.cs](file:///c:/Users/abour/.gemini/antigravity/scratch/YallaFit/YallaFitBackend/YallaFit/Data/DbInitializer.cs) with initial data:

#### Admin User
- Email: `admin@yallafit.com`
- Password: `Admin123!` (will be hashed in Sprint 2)
- Role: Admin

#### Exercise Library (15 exercises)
Covering all major muscle groups:
- **Jambes**: Squat, Fentes, Leg press
- **Pectoraux**: Développé couché, Pompes
- **Dos**: Soulevé de terre, Tractions, Rowing barre
- **Épaules**: Développé militaire, Élévations latérales
- **Biceps**: Curl biceps
- **Triceps**: Dips, Extension triceps
- **Abdominaux**: Crunch, Planche

#### Food Database (27 items)
Organized by category with accurate nutritional values:
- **Proteins** (5): Poulet, Bœuf, Saumon, Œufs, Thon
- **Carbs** (6): Riz blanc, Riz complet, Pâtes, Pain complet, Patate douce, Flocons d'avoine
- **Vegetables** (4): Brocoli, Épinards, Tomate, Carotte
- **Fruits** (3): Banane, Pomme, Orange
- **Fats** (3): Avocat, Amandes, Huile d'olive
- **Dairy** (3): Yaourt grec, Fromage blanc, Lait écrémé
- **Legumes** (3): Lentilles, Pois chiches, Haricots noirs

---

## Verification Results

### ✅ Build Status
```
Générer a réussi dans 2,7s
YallaFit a réussi (1,4s) → bin\Debug\net9.0\YallaFit.dll
```

### ✅ Database Creation
- Database `YallaFit` created successfully
- All 11 tables created with proper schema
- Foreign key constraints applied
- Indexes created

### ✅ Data Seeding
- Admin user inserted
- 15 exercises inserted
- 27 food items inserted
- Application ran without errors

### ✅ Application Startup
Application started successfully with database seeding:
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
Database seeded successfully!
```

---

## Database Schema Summary

| Table | Primary Key | Foreign Keys | Purpose |
|-------|-------------|--------------|---------|
| `Utilisateur` | id | - | User accounts |
| `Profil_Sportif` | user_id | user_id → Utilisateur | Athletic profiles |
| `Programme` | id | coach_id → Utilisateur | Training programs |
| `Exercice` | id | - | Exercise library |
| `Seance` | id | programme_id → Programme | Training sessions |
| `Detail_Seance` | seance_id, exercice_id | Both are FKs | Session exercises |
| `Plan_Nutrition` | id | sportif_id → Utilisateur | Nutrition plans |
| `Repas` | id | plan_id → Plan_Nutrition | Meals |
| `Aliment` | id | - | Food database |
| `Composition_Repas` | repas_id, aliment_id | Both are FKs | Meal composition |
| `Biometrie_Actuelle` | id | sportif_id → Utilisateur | Biometric data |

---

## Technical Decisions Made

### 1. Email Field Length
**Decision**: Reduced from 255 to 191 characters  
**Reason**: MySQL utf8mb4 uses 4 bytes per character. Unique index limit is 1000 bytes (191 × 4 = 764 bytes)

### 2. Cascade Delete Behaviors
- **Cascade**: User → ProfilSportif, Programme → Seance, PlanNutrition → Repas
- **Restrict**: Coach → Programme (prevent deleting coaches with programs), Exercice references

### 3. Decimal Precision
Configured precision for all float fields:
- Nutritional values: `Precision(5, 2)` (e.g., 999.99)
- Weight/measurements: `Precision(5, 2)` or `Precision(6, 2)`

### 4. Character Set
**utf8mb4** chosen for full Unicode support, essential for French language content (accents, special characters)

---

## Files Modified/Created

### Models (11 files)
- All entity models in `Models/` folder

### Data Layer (2 files)
- `Data/YallaFitDbContext.cs` - Database context
- `Data/DbInitializer.cs` - Seed data

### Configuration (3 files)
- `Program.cs` - EF Core and seeding setup
- `appsettings.json` - Connection string
- `YallaFit.csproj` - NuGet packages

### Migrations (1 folder)
- `Migrations/` - InitialCreate migration files

---

## Next Steps - Sprint 2

With the database foundation complete, Sprint 2 will focus on:

1. **User Authentication**
   - JWT token generation
   - Password hashing (BCrypt)
   - Login/Register endpoints

2. **User Management**
   - Profile CRUD operations
   - Role-based authorization
   - Athletic profile management

3. **API Controllers**
   - AuthController
   - UserController

---

## Sprint 1 Success Criteria - All Met ✅

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

**Sprint 1 Status**: ✅ **COMPLETE**

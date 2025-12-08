# YallaFit Backend

Plateforme de suivi sportif et nutritionnel avec plan personnalisé.

## 📋 Prerequisites

Before cloning and running this project, ensure you have the following installed:

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **MySQL 8.0+** - [Download here](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Download here](https://git-scm.com/downloads)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd YallaFit/YallaFitBackend
```

### 2. Install .NET EF Core Tools

The Entity Framework Core tools are required for database migrations:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
```

If already installed, update to the latest version:

```bash
dotnet tool update --global dotnet-ef
```

### 3. Configure MySQL Database

#### Option A: Using Default Configuration (Recommended for Development)

The project is pre-configured to connect to:
- **Host**: localhost
- **Port**: 3306
- **Database**: YallaFit
- **User**: root
- **Password**: (no password)

Make sure your MySQL server is running with these settings.

#### Option B: Custom Configuration

If you need different database settings, update the connection string in `YallaFit/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_HOST;Port=YOUR_PORT;Database=YallaFit;User=YOUR_USER;Password=YOUR_PASSWORD;CharSet=utf8mb4;"
  }
}
```

### 4. Restore NuGet Packages

Navigate to the project directory and restore dependencies:

```bash
cd YallaFit
dotnet restore
```

### 5. Create the Database

The database will be created automatically when you run the application for the first time. However, you can create it manually:

```bash
dotnet ef database update
```

This command will:
- Create the `YallaFit` database
- Create all 11 tables with proper relationships
- Apply all migrations

### 6. Run the Application

```bash
dotnet run
```

The application will:
- Start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP)
- Automatically seed the database with initial data (admin user, exercises, food items)
- Display "Database seeded successfully!" if this is the first run

### 7. Verify Installation

Open your browser and navigate to:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

You should see the default ASP.NET Core MVC home page.

## 📦 Database Schema

The application includes 11 tables:

### Core Tables
- **Utilisateur** - User accounts (Sportif, Coach, Admin)
- **Profil_Sportif** - Athletic profiles with biometrics
- **Programme** - Training programs
- **Exercice** - Exercise library
- **Seance** - Training sessions
- **Detail_Seance** - Session exercise details
- **Plan_Nutrition** - Nutrition plans
- **Repas** - Meals
- **Aliment** - Food database
- **Composition_Repas** - Meal composition
- **Biometrie_Actuelle** - Biometric measurements

## 🌱 Seed Data

On first run, the database is seeded with:

### Admin Account
- **Email**: admin@yallafit.com
- **Password**: Admin123!
- **Role**: Admin

### Exercise Library
15 exercises covering all major muscle groups (Jambes, Pectoraux, Dos, Épaules, Biceps, Triceps, Abdominaux)

### Food Database
27 common food items with accurate nutritional values (proteins, carbs, vegetables, fruits, fats, dairy, legumes)

## 🛠️ Development Commands

### Build the Project
```bash
dotnet build
```

### Run in Development Mode
```bash
dotnet run
```

### Watch Mode (Auto-reload on changes)
```bash
dotnet watch run
```

### Create a New Migration
```bash
dotnet ef migrations add MigrationName
```

### Apply Migrations
```bash
dotnet ef database update
```

### Rollback Last Migration
```bash
dotnet ef migrations remove
```

### Drop Database (⚠️ Caution: Deletes all data)
```bash
dotnet ef database drop --force
```

### Recreate Database from Scratch
```bash
dotnet ef database drop --force
dotnet ef database update
```

## 🏗️ Project Structure

```
YallaFit/
├── Controllers/          # API Controllers (to be added in Sprint 2)
├── Data/
│   ├── YallaFitDbContext.cs    # Entity Framework DbContext
│   └── DbInitializer.cs        # Database seeding logic
├── Models/              # Entity models (11 files)
│   ├── Utilisateur.cs
│   ├── ProfilSportif.cs
│   ├── Programme.cs
│   ├── Exercice.cs
│   ├── Seance.cs
│   ├── DetailSeance.cs
│   ├── PlanNutrition.cs
│   ├── Repas.cs
│   ├── Aliment.cs
│   ├── CompositionRepas.cs
│   └── BiometrieActuelle.cs
├── Migrations/          # EF Core migrations
├── Views/               # MVC Views
├── wwwroot/            # Static files
├── Program.cs          # Application entry point
├── appsettings.json    # Configuration
└── YallaFit.csproj     # Project file
```

## 🔧 Troubleshooting

### MySQL Connection Issues

**Error**: "Unable to connect to any of the specified MySQL hosts"

**Solution**:
1. Verify MySQL is running: `mysql --version`
2. Check connection string in `appsettings.json`
3. Ensure MySQL is listening on port 3306
4. Verify user credentials

### Migration Issues

**Error**: "Specified key was too long; max key length is 1000 bytes"

**Solution**: This has been fixed in the current version (email field limited to 191 characters for utf8mb4 compatibility)

**Error**: "Table already exists"

**Solution**: Drop and recreate the database:
```bash
dotnet ef database drop --force
dotnet ef database update
```

### Port Already in Use

**Error**: "Failed to bind to address https://localhost:5001"

**Solution**: Change the port in `Properties/launchSettings.json` or kill the process using the port

## 📚 Technology Stack

- **Framework**: .NET 9.0 (ASP.NET Core MVC)
- **Database**: MySQL 8.0+
- **ORM**: Entity Framework Core 9.0
- **MySQL Provider**: Pomelo.EntityFrameworkCore.MySql 9.0
- **AI Integration**: DeepSeek (planned for Sprint 4)

## 🗓️ Development Sprints

- ✅ **Sprint 1**: Database Models & Configuration (COMPLETE)
- 🔄 **Sprint 2**: Authentication & User Management (NEXT)
- ⏳ **Sprint 3**: Sports Program Management
- ⏳ **Sprint 4**: Nutrition AI Integration
- ⏳ **Sprint 5**: Biometric & Progress Tracking
- ⏳ **Sprint 6**: Coach Features
- ⏳ **Sprint 7**: Admin Features & Final Integration

## 🤝 Contributing

1. Create a new branch for your feature
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## 📝 Notes

- The database uses **utf8mb4** character set for full French language support
- Passwords are currently stored in plain text (will be hashed in Sprint 2)
- The application automatically seeds data on first run
- All entity models use French naming conventions to match the database schema

## 📞 Support

For issues or questions, please contact the development team.

---

**Last Updated**: Sprint 1 Completion - December 2025

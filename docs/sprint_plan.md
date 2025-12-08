# YallaFit Backend - Sprint Plan

## Project Overview
**Platform**: Sports and Nutrition Tracking with Personalized Plans  
**Technology Stack**: .NET Core, MySQL, Entity Framework Core  
**AI Integration**: Nutrition API/Model for meal planning and analysis

---

## Database Schema Overview

![Database Schema](C:/Users/abour/.gemini/antigravity/brain/60e0627e-25c3-496d-baae-4a7bb034c65b/uploaded_image_0_1765152434908.png)

### Core Entities
- **Utilisateur** (User): Main user entity with profile and authentication
- **Profil_Sportif**: Athletic profile with biometrics and goals
- **Programme**: Training programs created by coaches
- **Exercice**: Exercise library
- **Seance**: Training sessions
- **Detail_Seance**: Session details (sets, reps, weight)
- **Plan_Nutrition**: Personalized nutrition plans
- **Repas**: Meals within nutrition plans
- **Aliment**: Food items database
- **Composition_Repas**: Meal composition with quantities
- **Biometrie_Actuelle**: Current biometric measurements

---

## Use Case Overview

![Use Case Diagram](C:/Users/abour/.gemini/antigravity/brain/60e0627e-25c3-496d-baae-4a7bb034c65b/uploaded_image_1_1765152434908.png)

### User Roles
1. **Sportif** (Athlete): Main user consuming training and nutrition plans
2. **Coach**: Creates programs, assigns to athletes, monitors progress
3. **Administrateur**: Manages users, exercises, and AI configuration

---

## Sprint 1: Database Models & Configuration ✅ COMPLETE
**Duration**: 1 week  
**Goal**: Set up database foundation with all entity models

### Tasks

#### 1.1 Project Setup & Dependencies
- [x] Verify .NET Core version (recommend .NET 8)
- [x] Install NuGet packages:
  - `Microsoft.EntityFrameworkCore`
  - `Pomelo.EntityFrameworkCore.MySql` (MySQL provider)
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.EntityFrameworkCore.Design`
- [x] Configure project structure (Models, Data, Controllers folders)

#### 1.2 Create Entity Models
- [x] Create `Utilisateur` model (User entity)
  - id, nom_complet, email, mot_de_passe, role
- [x] Create `ProfilSportif` model
  - user_id, date_naissance, genre, taille_cm, niveau_activite
  - objectif_principal, allergies, preferences_alim, problemes_sante
- [x] Create `Programme` model
  - id, coach_id, titre, duree_semaines
- [x] Create `Exercice` model
  - id, nom, video_url, muscle_cible
- [x] Create `Seance` model
  - id, programme_id, nom, jour_semaine
- [x] Create `DetailSeance` model
  - seance_id, exercice_id, series, repetitions, poids_conseille
- [x] Create `PlanNutrition` model
  - id, sportif_id, date_generation
  - objectif_calorique_journalier, objectif_proteines_g, objectif_glucides_g, objectif_lipides_g
  - est_actif
- [x] Create `Repas` model
  - id, plan_id, nom, heure_prevue
- [x] Create `Aliment` model
  - id, nom, calories_100g, proteines_100g, glucides_100g, lipides_100g
- [x] Create `CompositionRepas` model
  - repas_id, aliment_id, quantite_grammes
- [x] Create `BiometrieActuelle` model
  - id, sportif_id, date_mesure, poids_kg
  - taux_masse_grasse_percent, tour_de_taille_cm

#### 1.3 Configure DbContext
- [x] Create `YallaFitDbContext` class
- [x] Configure entity relationships (one-to-many, many-to-many)
- [x] Set up foreign keys and constraints
- [x] Configure indexes for performance
- [x] Add data annotations and fluent API configurations

#### 1.4 MySQL Database Configuration
- [x] Add connection string to `appsettings.json`
- [x] Configure MySQL options in `Program.cs`
- [x] Set up database connection pooling
- [x] Configure character set (utf8mb4) for French language support

#### 1.5 Create and Run Migrations
- [x] Create initial migration: `Add-Migration InitialCreate`
- [x] Review generated migration code
- [x] Apply migration: `Update-Database`
- [x] Verify all tables created in MySQL
- [x] Verify foreign key constraints
- [x] Verify indexes created

#### 1.6 Seed Initial Data
- [x] Create seed data for exercise library
- [x] Create seed data for common food items
- [x] Create admin user account
- [x] Run seeding on database initialization

**Deliverables**:
- ✅ All entity models created
- ✅ MySQL database configured and connected
- ✅ Migrations applied successfully
- ✅ Database schema matches design
- ✅ Seed data populated

---

## Sprint 2: Authentication & User Management
**Duration**: 1 week  
**Goal**: Implement secure user authentication and profile management

### Tasks

#### 2.1 Authentication Setup
- [ ] Install JWT authentication packages
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `System.IdentityModel.Tokens.Jwt`
- [ ] Configure JWT settings in `appsettings.json`
- [ ] Implement password hashing (BCrypt or ASP.NET Identity)
- [ ] Create JWT token generation service

#### 2.2 User Registration
- [ ] Create `AuthController`
- [ ] Implement POST `/api/auth/register` endpoint
  - Validate email format
  - Check email uniqueness
  - Hash password
  - Create user with default role
- [ ] Create DTOs for registration request/response
- [ ] Add validation attributes

#### 2.3 User Login
- [ ] Implement POST `/api/auth/login` endpoint
  - Validate credentials
  - Generate JWT token
  - Return user info + token
- [ ] Create login DTOs
- [ ] Implement refresh token mechanism (optional)

#### 2.4 User Profile Management
- [ ] Create `UserController`
- [ ] Implement GET `/api/users/profile` (get current user)
- [ ] Implement PUT `/api/users/profile` (update profile)
- [ ] Implement GET `/api/users/{id}/sportif-profile` (get athletic profile)
- [ ] Implement PUT `/api/users/{id}/sportif-profile` (update athletic profile)

#### 2.5 Role-Based Authorization
- [ ] Configure authorization policies (Sportif, Coach, Admin)
- [ ] Add `[Authorize]` attributes to controllers
- [ ] Implement role-based access control
- [ ] Test authorization for different roles

**Deliverables**:
- ✅ User registration working
- ✅ User login with JWT tokens
- ✅ Profile management endpoints
- ✅ Role-based authorization implemented

---

## Sprint 3: Sports Program Management
**Duration**: 1.5 weeks  
**Goal**: Implement training program creation and session tracking

### Tasks

#### 3.1 Exercise Library Management
- [ ] Create `ExerciseController`
- [ ] Implement GET `/api/exercises` (list all, with pagination)
- [ ] Implement GET `/api/exercises/{id}` (get single exercise)
- [ ] Implement POST `/api/exercises` (create - Admin/Coach only)
- [ ] Implement PUT `/api/exercises/{id}` (update - Admin/Coach only)
- [ ] Implement DELETE `/api/exercises/{id}` (delete - Admin only)
- [ ] Add filtering by muscle group

#### 3.2 Training Program CRUD
- [ ] Create `ProgrammeController`
- [ ] Implement POST `/api/programmes` (create program - Coach only)
- [ ] Implement GET `/api/programmes/{id}` (get program details)
- [ ] Implement PUT `/api/programmes/{id}` (update program - Coach only)
- [ ] Implement DELETE `/api/programmes/{id}` (delete - Coach only)
- [ ] Implement GET `/api/programmes/coach/{coachId}` (list coach's programs)

#### 3.3 Session Management
- [ ] Implement POST `/api/programmes/{id}/seances` (add session to program)
- [ ] Implement GET `/api/seances/{id}` (get session details)
- [ ] Implement PUT `/api/seances/{id}` (update session)
- [ ] Implement DELETE `/api/seances/{id}` (delete session)

#### 3.4 Session Details (Exercise Sets)
- [ ] Implement POST `/api/seances/{id}/details` (add exercise to session)
- [ ] Implement PUT `/api/seances/details/{id}` (update sets/reps/weight)
- [ ] Implement DELETE `/api/seances/details/{id}` (remove exercise)
- [ ] Implement GET `/api/seances/{id}/details` (get all exercises in session)

#### 3.5 Program Assignment
- [ ] Implement POST `/api/programmes/{id}/assign/{sportifId}` (assign to athlete)
- [ ] Implement GET `/api/users/{sportifId}/programmes` (get athlete's programs)
- [ ] Create notification system for new assignments (optional)

#### 3.6 Session Logging
- [ ] Create `SessionLogController`
- [ ] Implement POST `/api/session-logs` (log completed workout)
- [ ] Implement GET `/api/users/{sportifId}/session-logs` (get workout history)
- [ ] Track actual vs planned performance

**Deliverables**:
- ✅ Exercise library fully functional
- ✅ Program creation and management
- ✅ Session and exercise detail management
- ✅ Program assignment to athletes
- ✅ Workout logging capability

---

## Sprint 4: Nutrition AI Integration
**Duration**: 2 weeks  
**Goal**: Integrate AI for personalized nutrition plans

### Tasks

#### 4.1 AI Service Setup - DeepSeek Fine-tuning
- [ ] Set up DeepSeek API access
- [ ] Research DeepSeek fine-tuning capabilities
- [ ] Prepare nutrition training dataset:
  - French nutrition guidelines
  - Meal planning examples
  - Calorie and macro calculations
  - Food composition data
- [ ] Fine-tune DeepSeek model as nutritionist
- [ ] Test model with sample nutrition queries
- [ ] Configure API credentials in application

#### 4.2 AI Service Integration
- [ ] Create `INutritionAIService` interface
- [ ] Implement AI service wrapper class
- [ ] Configure API credentials in `appsettings.json`
- [ ] Create DTOs for AI requests/responses
- [ ] Implement error handling and retry logic

#### 4.3 Calorie Calculation
- [ ] Implement basal metabolic rate (BMR) calculation
  - Use Mifflin-St Jeor equation
  - Factor in age, gender, weight, height
- [ ] Implement total daily energy expenditure (TDEE)
  - Factor in activity level
  - Adjust for goals (weight loss, gain, maintenance)
- [ ] Create `/api/nutrition/calculate-needs` endpoint

#### 4.4 Nutrition Plan Generation
- [ ] Create `NutritionController`
- [ ] Implement POST `/api/nutrition/plans/generate`
  - Use AI to generate personalized meal plan
  - Consider allergies and preferences
  - Meet caloric and macro targets
- [ ] Implement GET `/api/nutrition/plans/{sportifId}/active`
- [ ] Implement GET `/api/nutrition/plans/{id}`

#### 4.5 Food Database Management
- [ ] Implement GET `/api/aliments` (list foods with search)
- [ ] Implement GET `/api/aliments/{id}` (get food details)
- [ ] Implement POST `/api/aliments` (add custom food - Admin)
- [ ] Integrate with external food database API (optional)

#### 4.6 Meal Management
- [ ] Implement GET `/api/nutrition/plans/{planId}/repas` (get all meals)
- [ ] Implement POST `/api/nutrition/plans/{planId}/repas` (add meal)
- [ ] Implement PUT `/api/repas/{id}` (update meal)
- [ ] Implement DELETE `/api/repas/{id}` (delete meal)

#### 4.7 Meal Composition
- [ ] Implement POST `/api/repas/{id}/aliments` (add food to meal)
- [ ] Implement PUT `/api/repas/{repasId}/aliments/{alimentId}` (update quantity)
- [ ] Implement DELETE `/api/repas/{repasId}/aliments/{alimentId}` (remove food)
- [ ] Calculate meal totals (calories, macros)

#### 4.8 Photo Analysis (AI Feature)
- [ ] Implement POST `/api/nutrition/analyze-photo`
  - Use AI vision to identify food items
  - Estimate portion sizes
  - Return nutritional information
- [ ] Handle image upload and processing
- [ ] Return structured food data

**Deliverables**:
- ✅ AI service integrated and functional
- ✅ Calorie calculation working
- ✅ Personalized meal plan generation
- ✅ Food database accessible
- ✅ Meal composition tracking
- ✅ Photo analysis feature (bonus)

---

## Sprint 5: Biometric & Progress Tracking
**Duration**: 1 week  
**Goal**: Track athlete progress and biometric data

### Tasks

#### 5.1 Biometric Data Management
- [ ] Create `BiometricController`
- [ ] Implement POST `/api/biometrics` (record new measurement)
- [ ] Implement GET `/api/users/{sportifId}/biometrics` (get history)
- [ ] Implement GET `/api/users/{sportifId}/biometrics/latest` (current stats)
- [ ] Add data validation (reasonable ranges)

#### 5.2 Progress Analytics
- [ ] Create analytics service
- [ ] Implement GET `/api/users/{sportifId}/progress`
  - Weight change over time
  - Body fat percentage trends
  - Strength progression
- [ ] Calculate progress percentages
- [ ] Generate insights and recommendations

#### 5.3 Dashboard Data Aggregation
- [ ] Create `DashboardController`
- [ ] Implement GET `/api/dashboard/sportif/{id}`
  - Current biometrics
  - Active nutrition plan summary
  - Active training program
  - Recent workout logs
  - Progress charts data
- [ ] Implement GET `/api/dashboard/coach/{id}`
  - List of assigned athletes
  - Recent activity overview
  - Pending feedback requests

#### 5.4 Goal Tracking
- [ ] Add goal setting endpoints
- [ ] Track goal progress automatically
- [ ] Send notifications on milestones (optional)

**Deliverables**:
- ✅ Biometric data recording and history
- ✅ Progress analytics and trends
- ✅ Dashboard endpoints for athletes and coaches
- ✅ Goal tracking system

---

## Sprint 6: Coach Features
**Duration**: 1 week  
**Goal**: Enable coach-athlete interaction and monitoring

### Tasks

#### 6.1 Athlete Management
- [ ] Create `CoachController`
- [ ] Implement GET `/api/coach/athletes` (list assigned athletes)
- [ ] Implement POST `/api/coach/athletes/{sportifId}/assign` (assign athlete)
- [ ] Implement DELETE `/api/coach/athletes/{sportifId}/unassign`
- [ ] Implement GET `/api/coach/athletes/{sportifId}/overview`

#### 6.2 Activity Journal Viewing
- [ ] Implement GET `/api/coach/athletes/{sportifId}/activity-journal`
  - View workout logs
  - View nutrition adherence
  - View biometric changes
- [ ] Add filtering by date range
- [ ] Add export functionality (PDF/CSV)

#### 6.3 Feedback System
- [ ] Create `Feedback` model and migration
- [ ] Implement POST `/api/feedback` (coach sends feedback)
- [ ] Implement GET `/api/users/{sportifId}/feedback` (athlete views feedback)
- [ ] Add notification for new feedback

#### 6.4 Program Templates
- [ ] Implement program template system
- [ ] Allow coaches to save programs as templates
- [ ] Enable quick program creation from templates

**Deliverables**:
- ✅ Coach can manage assigned athletes
- ✅ Coach can view athlete activity journals
- ✅ Feedback system functional
- ✅ Program templates available

---

## Sprint 7: Admin Features & Final Integration
**Duration**: 1.5 weeks  
**Goal**: Complete admin functionality and final testing

### Tasks

#### 7.1 User Management (Admin)
- [ ] Create `AdminController`
- [ ] Implement GET `/api/admin/users` (list all users with filters)
- [ ] Implement PUT `/api/admin/users/{id}/role` (change user role)
- [ ] Implement DELETE `/api/admin/users/{id}` (soft delete user)
- [ ] Implement POST `/api/admin/users/{id}/activate` (reactivate user)

#### 7.2 Exercise Library Management (Admin)
- [ ] Full CRUD for exercise library (already in Sprint 3)
- [ ] Bulk import exercises from CSV
- [ ] Exercise approval workflow (if coaches can suggest)

#### 7.3 AI Configuration (Admin)
- [ ] Implement GET `/api/admin/ai-config` (get current AI settings)
- [ ] Implement PUT `/api/admin/ai-config` (update AI parameters)
  - Model selection
  - Prompt templates
  - API keys
- [ ] Test AI configuration changes

#### 7.4 Subscription Management (Optional)
- [ ] Create `Subscription` model
- [ ] Implement subscription tiers (Free, Premium, Coach)
- [ ] Add subscription validation middleware
- [ ] Implement payment integration (Stripe/PayPal) - if needed

#### 7.5 API Documentation
- [ ] Install Swagger/OpenAPI
- [ ] Document all endpoints
- [ ] Add example requests/responses
- [ ] Create API usage guide

#### 7.6 Testing & Quality Assurance
- [ ] Write unit tests for services
- [ ] Write integration tests for controllers
- [ ] Test all user flows end-to-end
- [ ] Performance testing (load testing)
- [ ] Security audit (SQL injection, XSS, etc.)

#### 7.7 Deployment Preparation
- [ ] Configure production database
- [ ] Set up environment variables
- [ ] Configure CORS for frontend
- [ ] Set up logging and monitoring
- [ ] Create deployment documentation

**Deliverables**:
- ✅ Admin panel fully functional
- ✅ AI configuration manageable
- ✅ API fully documented
- ✅ All tests passing
- ✅ Ready for deployment

---

## Technology Stack Details

### Backend Framework
- **.NET 8** (or .NET 7)
- **ASP.NET Core Web API**

### Database
- **MySQL 8.0+**
- **Entity Framework Core** (ORM)
- **Pomelo.EntityFrameworkCore.MySql** (MySQL provider)

### Authentication
- **JWT (JSON Web Tokens)**
- **BCrypt** for password hashing

### AI Integration
**Selected Solution**: **DeepSeek** - Fine-tuned as nutritionist
- Custom fine-tuning for French nutrition recommendations
- Personalized meal planning based on user goals
- Calorie and macro calculations
- Food photo analysis capabilities

### Additional Packages
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Serilog** - Logging
- **Swashbuckle** - API documentation (Swagger)

---

## Development Workflow

### For Each Sprint:
1. **Planning**: Review tasks and acceptance criteria
2. **Development**: 
   - Create models
   - Create services/repositories
   - Create controllers
   - Add validation
3. **Testing**: Test each endpoint with Postman/Swagger
4. **Review**: Code review and refactoring
5. **Migration**: Update database if needed
6. **Documentation**: Update API docs

### Quality Checkpoints:
- ✅ Code compiles without errors
- ✅ All endpoints tested and working
- ✅ Database migrations applied successfully
- ✅ No security vulnerabilities
- ✅ API documentation updated
- ✅ Error handling implemented
- ✅ Logging added for debugging

---

## Current Status

✅ **Sprint 1 COMPLETE** - Database Models & Configuration
- All 11 entity models created
- MySQL database configured and connected
- Migrations applied successfully
- Seed data populated (admin user, 15 exercises, 27 foods)

## Next Steps

**Sprint 2**: Authentication & User Management
- Implement JWT authentication
- Create user registration and login endpoints
- Add role-based authorization
- Build user profile management

Would you like to proceed with Sprint 1, or would you like any adjustments to this plan?

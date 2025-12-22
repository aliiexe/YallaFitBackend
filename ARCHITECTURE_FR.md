# Documentation de l'Architecture Technique - YallaFitBackend

Ce document détaille l'architecture technique du projet backend YallaFit. Il s'agit d'une application ASP.NET Core utilisant Entity Framework Core pour l'accès aux données et exposant une API RESTful consommée par un frontend (Next.js).

## 1. Vue d'ensemble

Le projet suit une architecture classique **MVC (Model-View-Controller)** adaptée pour une API Web.
- **Framework** : .NET 8 / ASP.NET Core
- **Base de données** : MySQL (géré via Entity Framework Core)
- **Authentification** : JWT (JSON Web Tokens)
- **IA** : Intégration avec Mistral AI pour la génération de contenu et l'analyse d'images.

## 2. Structure des Dossiers

### `Controllers/`
Contient les points d'entrée de l'API. Chaque contrôleur gère un domaine fonctionnel spécifique.

*   **`AuthController.cs`** : Gère l'inscription et la connexion des utilisateurs. Génère les tokens JWT.
*   **`AdminController.cs`** : Endpoints réservés aux administrateurs (gestion globale).
*   **`CoachController.cs`** : Gestion des athlètes et des programmes par les coachs.
*   **`DashboardController.cs`** : Agrège les données pour les tableaux de bord (KPIs, résumés).
*   **`BiometricsController.cs`** : Gestion des données physiologiques des utilisateurs (poids, taille, etc.).
*   **`NutritionController.cs`** : Gestion des plans nutritionnels, des repas et des aliments.
*   **`FoodAnalysisController.cs`** : Analyse de photos de repas via IA (Mistral Vision) pour estimer les calories/macros.
*   **`ProgrammeController.cs`** : Gestion des programmes d'entraînement (CRUD, assignation).
*   **`TrainingController.cs`** : Gestion de l'exécution des séances d'entraînement par les utilisateurs.
*   **`ExerciceController.cs`** : Gestion de la bibliothèque d'exercices.
*   **`UserController.cs`** : Gestion du profil utilisateur.

### `Models/`
Définit les entités de la base de données (Code-First).

*   **`Utilisateur.cs`** : L'utilisateur central (Admin, Coach, ou Membre).
*   **`ProfilSportif.cs`** : Détails athlétiques de l'utilisateur (niveau, objectifs, blessures).
*   **`BiometrieActuelle.cs`** : Suivi des mesures corporelles.
*   **`Programme.cs`** : Un programme d'entraînement complet.
*   **`Seance.cs`** : Une séance d'entraînement dans un programme.
*   **`Exercice.cs`** : La définition d'un mouvement (ex: "Squat").
*   **`TrainingSession.cs` / `TrainingExercise.cs` / `TrainingSet.cs`** : Enregistrement de l'historique d'entraînement (ce que l'utilisateur a réellement fait).
*   **`PlanNutrition.cs`** : Un plan alimentaire assigné à un utilisateur.
*   **`Repas.cs`** : Un repas dans un plan (ex: "Petit-déjeuner").
*   **`Aliment.cs`** : Base de données des aliments.
*   **`AnalyseRepasPhoto.cs`** : Stocke les résultats des analyses IA de photos.

### `Services/`
Contient la logique métier complexe et les intégrations externes.

*   **`JwtService.cs`** : Service responsable de la création et de la validation des tokens de sécurité.
*   **`MistralAIService.cs`** : Client HTTP pour communiquer avec l'API Mistral AI (génération de texte/conseils).
*   **`MistralVisionService.cs`** : Service spécialisé pour envoyer des images à l'IA et interpréter le contenu (reconnaissance d'aliments).
*   **`MacroCalculationService.cs`** : Algorithmes pour calculer les besoins nutritionnels (TDEE, répartition des macros).
*   **`DatabaseSeeder.cs`** : Peuple la base de données avec des données initiales (catégories, exercices de base).

### `Data/`
Couche d'accès aux données.

*   **`YallaFitDbContext.cs`** : Le contexte Entity Framework qui fait le lien entre les objets C# et la base de données MySQL. Configure les relations (Foreign Keys) et les tables.

### `DTOs/` (Data Transfer Objects)
Non listé explicitement mais généralement présent pour définir les structures de données envoyées/reçues par l'API sans exposer directement les modèles de base de données.

## 3. Configuration (`Program.cs`)

Le fichier `Program.cs` est le point d'entrée de l'application. Il configure :
1.  **L'injection de dépendances (DI)** : Enregistrement des services (`DbContext`, `JwtService`, `MistralAIService`, etc.).
2.  **La base de données** : Configuration de la chaîne de connexion MySQL.
3.  **L'authentification** : Configuration du middleware JWT Bearer.
4.  **CORS** : Autorise le frontend (localhost:3000) à appeler l'API.
5.  **Le Pipeline HTTP** : Ordre des middlewares (Auth, Controllers, etc.).

## 4. Flux de Données Typique

1.  **Requête** : Le frontend envoie une requête HTTP (ex: `POST /api/nutrition/analyze-photo`).
2.  **Auth** : Le middleware vérifie le Token JWT.
3.  **Controller** : `FoodAnalysisController` reçoit la requête.
4.  **Service** : Le contrôleur appelle `MistralVisionService` pour analyser l'image.
5.  **Data** : Le résultat est sauvegardé via `YallaFitDbContext` dans la table `AnalyseRepasPhoto`.
6.  **Réponse** : Le contrôleur renvoie le résultat JSON au frontend.

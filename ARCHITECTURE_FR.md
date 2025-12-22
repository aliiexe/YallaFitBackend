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
*   **`PublicController.cs`** : Endpoints publics accessibles sans authentification (ex: stats landing page).
*   **`TestController.cs`** : Endpoints de diagnostic pour vérifier la santé de l'API.

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

## 5. Détail des Endpoints API

Voici la liste complète des routes disponibles par contrôleur.

### 🔐 Authentification (`AuthController`)
| Méthode | Route | Description |
|:---|:---|:---|
| `POST` | `/api/auth/login` | Connexion utilisateur (email/password). Retourne un Token JWT. |
| `POST` | `/api/auth/register` | Inscription nouvel utilisateur. Crée le compte et retourne un Token. |

### 👤 Utilisateur (`UserController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/user/profile` | Auth | Récupère le profil de l'utilisateur connecté. |
| `PUT` | `/api/user/profile` | Auth | Met à jour les infos de base (Nom, Email). |
| `GET` | `/api/user/sportif-profile` | Auth | Récupère le profil sportif (poids, taille, objectifs). |
| `PUT` | `/api/user/sportif-profile` | Auth | Met à jour le profil sportif. |
| `PUT` | `/api/user/change-password` | Auth | Change le mot de passe utilisateur. |
| `DELETE` | `/api/user/delete-account` | Auth | Supprime le compte de l'utilisateur connecté. |
| `GET` | `/api/user/all` | **Admin** | Liste tous les utilisateurs. |
| `POST` | `/api/user` | **Admin** | Crée un utilisateur via admin. |
| `GET` | `/api/user/{id}` | **Admin** | Détails d'un utilisateur spécifique. |
| `PUT` | `/api/user/{id}` | **Admin** | Modifie un utilisateur spécifique. |
| `DELETE` | `/api/user/{id}` | **Admin** | Supprime un utilisateur spécifique. |

### 🛠️ Admin (`AdminController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/admin/stats` | **Admin** | Statistiques globales (nombre d'utilisateurs, programmes, etc.). |
| `GET` | `/api/admin/recent-users` | **Admin** | Liste des derniers inscrits. |
| `GET` | `/api/admin/top-programs` | **Admin** | Liste des programmes les plus populaires. |

### 🏋️ Coach (`CoachController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/coach/stats` | **Coach/Admin** | Stats spécifiques du coach (ses athlètes, ses programmes). |
| `GET` | `/api/coach/athletes` | **Coach/Admin** | Liste les athlètes (`?filter=my` ou `all`). |
| `GET` | `/api/coach/athletes/{id}` | **Coach/Admin** | Détails complets d'un athlète (progression, bio). |
| `POST` | `/api/coach/assign-program` | **Coach/Admin** | Assigne un programme à un athlète. |

### 📊 Dashboard (`DashboardController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/dashboard` | Auth | Données générales pour le dashboard principal. |
| `GET` | `/api/dashboard/sportif` | Auth | Dashboard personnalisé pour le sportif (suivi, prochains entraînements). |
| `GET` | `/api/dashboard/coach` | **Coach/Admin** | Dashboard pour le coach. |

### 📋 Programmes (`ProgrammeController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/programme` | Auth | Liste tous les programmes disponibles. |
| `GET` | `/api/programme/public` | Public* | Liste les programmes publics. |
| `GET` | `/api/programme/assigned` | Auth | Récupère le programme actuellement assigné à l'utilisateur. |
| `GET` | `/api/programme/my-enrolled` | **Sportif** | Liste les programmes où le sportif est inscrit. |
| `GET` | `/api/programme/my-programs` | **Coach/Admin** | Liste les programmes créés par le coach connecté. |
| `GET` | `/api/programme/{id}` | Auth | Détails complets d'un programme (séances, exercices). |
| `POST` | `/api/programme` | **Coach/Admin** | Crée un nouveau programme. |
| `PUT` | `/api/programme/{id}` | **Coach/Admin** | Modifie un programme existant. |
| `DELETE` | `/api/programme/{id}` | **Coach/Admin** | Supprime un programme. |
| `POST` | `/api/programme/{id}/enroll` | Auth | S'inscrire à un programme public. |
| `GET` | `/api/programme/{id}/enrollment-status`| Auth | Vérifie si l'utilisateur est inscrit. |
| `POST` | `/api/programme/{id}/seance` | **Coach/Admin** | Ajoute une séance à un programme. |
| `PUT` | `/api/programme/{id}/seance/{sid}` | **Coach/Admin** | Modifie une séance. |
| `DELETE` | `/api/programme/{id}/seance/{sid}` | **Coach/Admin** | Supprime une séance. |

### 💪 Entraînement (`TrainingController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `POST` | `/api/training/sessions` | Auth | Enregistre une séance réalisée (log workout). |
| `GET` | `/api/training/sessions` | Auth | Historique des séances de l'utilisateur. |
| `GET` | `/api/training/sessions/{id}` | Auth | Détails d'une séance passée. |
| `GET` | `/api/training/stats` | Auth | Stats d'entraînement (volume, fréquence). |
| `GET` | `/api/training/progress/{exoId}`| Auth | Progression sur un exercice spécifique (poids max estimé, etc.). |
| `GET` | `/api/training/exercises-with-history`| Auth | Liste des exercices que l'utilisateur a déjà pratiqués. |

### 🏋️‍♀️ Exercices (`ExerciceController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/exercice` | Auth | Liste tous les exercices de la bibliothèque. |
| `GET` | `/api/exercice/{id}` | Auth | Détails d'un exercice. |
| `GET` | `/api/exercice/category/{cat}` | Auth | Filtre par catégorie (ex: Cardio, Musculation). |
| `GET` | `/api/exercice/muscle/{mus}` | Auth | Filtre par muscle (ex: Pectoraux). |
| `POST` | `/api/exercice` | **Coach/Admin** | Ajoute un exercice à la bibliothèque. |
| `PUT` | `/api/exercice/{id}` | **Coach/Admin** | Modifie un exercice. |
| `DELETE` | `/api/exercice/{id}` | **Coach/Admin** | Supprime un exercice. |

### 🧬 Biométrie (`BiometricsController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/biometrics` | Auth | Historique des mesures (poids, etc.). |
| `GET` | `/api/biometrics/latest` | Auth | Dernière mesure enregistrée. |
| `POST` | `/api/biometrics` | Auth | Ajoute une nouvelle mesure. |
| `DELETE` | `/api/biometrics/{id}` | Auth | Supprime une mesure. |

### 🥗 Nutrition & IA (`NutritionController` & `FoodAnalysisController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `POST` | `/api/nutrition/generate-plan` | Auth | **IA** : Génère un plan nutritionnel complet personnalisé. |
| `POST` | `/api/nutrition/calculate-macros`| Auth | **IA** : Calcule les besoins en macronutriments. |
| `GET` | `/api/nutrition/my-plans` | **Sportif** | Liste les plans nutritionnels de l'utilisateur. |
| `GET` | `/api/nutrition/plan/{id}` | **Sportif** | Détails d'un plan nutritionnel. |
| `POST` | `/api/foodanalysis/analyze-photo`| Auth | **AI Vision** : Analyse une photo de plat pour estimer les calories. |
| `GET` | `/api/foodanalysis/my-analyses` | **Sportif** | Historique des analyses photo. |
| `GET` | `/api/foodanalysis/today` | Auth | Résumé nutritionnel du jour. |
| `GET` | `/api/foodanalysis/history` | Auth | Historique nutritionnel sur une période. |

### 🌐 Public (`PublicController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/public/landing-stats` | Public | Statistiques pour la page d'accueil (Landing Page). |

### 🧪 Test (`TestController`)
| Méthode | Route | Accès | Description |
|:---|:---|:---|:---|
| `GET` | `/api/test` | Public | Vérifie si l'API répond ("API is working!"). |
| `GET` | `/api/test/health` | Public | Check de santé ("status": "healthy"). |

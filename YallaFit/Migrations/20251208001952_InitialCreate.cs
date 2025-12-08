using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Aliment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    calories_100g = table.Column<int>(type: "int", nullable: false),
                    proteines_100g = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: false),
                    glucides_100g = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: false),
                    lipides_100g = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aliment", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Exercice",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    video_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    muscle_cible = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercice", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom_complet = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(191)", maxLength: 191, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mot_de_passe = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Biometrie_Actuelle",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sportif_id = table.Column<int>(type: "int", nullable: false),
                    date_mesure = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    poids_kg = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: false),
                    taux_masse_grasse_percent = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: true),
                    tour_de_taille_cm = table.Column<float>(type: "float", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biometrie_Actuelle", x => x.id);
                    table.ForeignKey(
                        name: "FK_Biometrie_Actuelle_Utilisateur_sportif_id",
                        column: x => x.sportif_id,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Plan_Nutrition",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sportif_id = table.Column<int>(type: "int", nullable: false),
                    date_generation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    objectif_calorique_journalier = table.Column<int>(type: "int", nullable: false),
                    objectif_proteines_g = table.Column<int>(type: "int", nullable: false),
                    objectif_glucides_g = table.Column<int>(type: "int", nullable: false),
                    objectif_lipides_g = table.Column<int>(type: "int", nullable: false),
                    est_actif = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan_Nutrition", x => x.id);
                    table.ForeignKey(
                        name: "FK_Plan_Nutrition_Utilisateur_sportif_id",
                        column: x => x.sportif_id,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Profil_Sportif",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    date_naissance = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    genre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    taille_cm = table.Column<int>(type: "int", nullable: true),
                    niveau_activite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    objectif_principal = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    allergies = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    preferences_alim = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    problemes_sante = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profil_Sportif", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Profil_Sportif_Utilisateur_user_id",
                        column: x => x.user_id,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Programme",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    coach_id = table.Column<int>(type: "int", nullable: false),
                    titre = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duree_semaines = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programme", x => x.id);
                    table.ForeignKey(
                        name: "FK_Programme_Utilisateur_coach_id",
                        column: x => x.coach_id,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Repas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    heure_prevue = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Repas_Plan_Nutrition_plan_id",
                        column: x => x.plan_id,
                        principalTable: "Plan_Nutrition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Seance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    programme_id = table.Column<int>(type: "int", nullable: false),
                    nom = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    jour_semaine = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seance", x => x.id);
                    table.ForeignKey(
                        name: "FK_Seance_Programme_programme_id",
                        column: x => x.programme_id,
                        principalTable: "Programme",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Composition_Repas",
                columns: table => new
                {
                    repas_id = table.Column<int>(type: "int", nullable: false),
                    aliment_id = table.Column<int>(type: "int", nullable: false),
                    quantite_grammes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Composition_Repas", x => new { x.repas_id, x.aliment_id });
                    table.ForeignKey(
                        name: "FK_Composition_Repas_Aliment_aliment_id",
                        column: x => x.aliment_id,
                        principalTable: "Aliment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Composition_Repas_Repas_repas_id",
                        column: x => x.repas_id,
                        principalTable: "Repas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Detail_Seance",
                columns: table => new
                {
                    seance_id = table.Column<int>(type: "int", nullable: false),
                    exercice_id = table.Column<int>(type: "int", nullable: false),
                    series = table.Column<int>(type: "int", nullable: false),
                    repetitions = table.Column<int>(type: "int", nullable: false),
                    poids_conseille = table.Column<float>(type: "float", precision: 6, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detail_Seance", x => new { x.seance_id, x.exercice_id });
                    table.ForeignKey(
                        name: "FK_Detail_Seance_Exercice_exercice_id",
                        column: x => x.exercice_id,
                        principalTable: "Exercice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Detail_Seance_Seance_seance_id",
                        column: x => x.seance_id,
                        principalTable: "Seance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Biometrie_Actuelle_sportif_id_date_mesure",
                table: "Biometrie_Actuelle",
                columns: new[] { "sportif_id", "date_mesure" });

            migrationBuilder.CreateIndex(
                name: "IX_Composition_Repas_aliment_id",
                table: "Composition_Repas",
                column: "aliment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Detail_Seance_exercice_id",
                table: "Detail_Seance",
                column: "exercice_id");

            migrationBuilder.CreateIndex(
                name: "IX_Plan_Nutrition_sportif_id",
                table: "Plan_Nutrition",
                column: "sportif_id");

            migrationBuilder.CreateIndex(
                name: "IX_Programme_coach_id",
                table: "Programme",
                column: "coach_id");

            migrationBuilder.CreateIndex(
                name: "IX_Repas_plan_id",
                table: "Repas",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_Seance_programme_id",
                table: "Seance",
                column: "programme_id");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_email",
                table: "Utilisateur",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Biometrie_Actuelle");

            migrationBuilder.DropTable(
                name: "Composition_Repas");

            migrationBuilder.DropTable(
                name: "Detail_Seance");

            migrationBuilder.DropTable(
                name: "Profil_Sportif");

            migrationBuilder.DropTable(
                name: "Aliment");

            migrationBuilder.DropTable(
                name: "Repas");

            migrationBuilder.DropTable(
                name: "Exercice");

            migrationBuilder.DropTable(
                name: "Seance");

            migrationBuilder.DropTable(
                name: "Plan_Nutrition");

            migrationBuilder.DropTable(
                name: "Programme");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Programme_Enrollment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sportif_id = table.Column<int>(type: "int", nullable: false),
                    programme_id = table.Column<int>(type: "int", nullable: false),
                    enrolled_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programme_Enrollment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Programme_Enrollment_Profil_Sportif_sportif_id",
                        column: x => x.sportif_id,
                        principalTable: "Profil_Sportif",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Programme_Enrollment_Programme_programme_id",
                        column: x => x.programme_id,
                        principalTable: "Programme",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Training_Session",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sportif_id = table.Column<int>(type: "int", nullable: false),
                    programme_id = table.Column<int>(type: "int", nullable: false),
                    seance_id = table.Column<int>(type: "int", nullable: false),
                    date_completed = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Session", x => x.id);
                    table.ForeignKey(
                        name: "FK_Training_Session_Profil_Sportif_sportif_id",
                        column: x => x.sportif_id,
                        principalTable: "Profil_Sportif",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_Session_Programme_programme_id",
                        column: x => x.programme_id,
                        principalTable: "Programme",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_Session_Seance_seance_id",
                        column: x => x.seance_id,
                        principalTable: "Seance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Training_Exercise",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    training_session_id = table.Column<int>(type: "int", nullable: false),
                    exercice_id = table.Column<int>(type: "int", nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Exercise", x => x.id);
                    table.ForeignKey(
                        name: "FK_Training_Exercise_Exercice_exercice_id",
                        column: x => x.exercice_id,
                        principalTable: "Exercice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_Exercise_Training_Session_training_session_id",
                        column: x => x.training_session_id,
                        principalTable: "Training_Session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Training_Set",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    training_exercise_id = table.Column<int>(type: "int", nullable: false),
                    set_number = table.Column<int>(type: "int", nullable: false),
                    reps = table.Column<int>(type: "int", nullable: false),
                    weight_kg = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    completed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Set", x => x.id);
                    table.ForeignKey(
                        name: "FK_Training_Set_Training_Exercise_training_exercise_id",
                        column: x => x.training_exercise_id,
                        principalTable: "Training_Exercise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Programme_Enrollment_programme_id",
                table: "Programme_Enrollment",
                column: "programme_id");

            migrationBuilder.CreateIndex(
                name: "IX_Programme_Enrollment_sportif_id",
                table: "Programme_Enrollment",
                column: "sportif_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Exercise_exercice_id",
                table: "Training_Exercise",
                column: "exercice_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Exercise_training_session_id",
                table: "Training_Exercise",
                column: "training_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Session_programme_id",
                table: "Training_Session",
                column: "programme_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Session_seance_id",
                table: "Training_Session",
                column: "seance_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Session_sportif_id",
                table: "Training_Session",
                column: "sportif_id");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Set_training_exercise_id",
                table: "Training_Set",
                column: "training_exercise_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Programme_Enrollment");

            migrationBuilder.DropTable(
                name: "Training_Set");

            migrationBuilder.DropTable(
                name: "Training_Exercise");

            migrationBuilder.DropTable(
                name: "Training_Session");
        }
    }
}

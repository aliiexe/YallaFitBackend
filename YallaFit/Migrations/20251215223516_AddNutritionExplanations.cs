using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionExplanations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "benefices_nutritionnels",
                table: "Repas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "explication",
                table: "Repas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "analyse_globale",
                table: "Plan_Nutrition",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "conseils_personnalises",
                table: "Plan_Nutrition",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Analyse_Repas_Photo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sportif_id = table.Column<int>(type: "int", nullable: false),
                    date_analyse = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    chemin_photo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aliments_detectes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    calories_estimees = table.Column<int>(type: "int", nullable: false),
                    proteines_estimees = table.Column<float>(type: "float", nullable: false),
                    glucides_estimees = table.Column<float>(type: "float", nullable: false),
                    lipides_estimees = table.Column<float>(type: "float", nullable: false),
                    analyse_ia = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recommandations = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyse_Repas_Photo", x => x.id);
                    table.ForeignKey(
                        name: "FK_Analyse_Repas_Photo_Utilisateur_sportif_id",
                        column: x => x.sportif_id,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Analyse_Repas_Photo_sportif_id",
                table: "Analyse_Repas_Photo",
                column: "sportif_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analyse_Repas_Photo");

            migrationBuilder.DropColumn(
                name: "benefices_nutritionnels",
                table: "Repas");

            migrationBuilder.DropColumn(
                name: "explication",
                table: "Repas");

            migrationBuilder.DropColumn(
                name: "analyse_globale",
                table: "Plan_Nutrition");

            migrationBuilder.DropColumn(
                name: "conseils_personnalises",
                table: "Plan_Nutrition");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Changes already applied to DB manually or via failed previous run
            /*
            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "Profil_Sportif",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "poids",
                table: "Profil_Sportif",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sexe",
                table: "Profil_Sportif",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "taille",
                table: "Profil_Sportif",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "categorie",
                table: "Exercice",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "muscles_cibles",
                table: "Exercice",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age",
                table: "Profil_Sportif");

            migrationBuilder.DropColumn(
                name: "poids",
                table: "Profil_Sportif");

            migrationBuilder.DropColumn(
                name: "sexe",
                table: "Profil_Sportif");

            migrationBuilder.DropColumn(
                name: "taille",
                table: "Profil_Sportif");

            migrationBuilder.DropColumn(
                name: "categorie",
                table: "Exercice");

            migrationBuilder.DropColumn(
                name: "muscles_cibles",
                table: "Exercice");
        }
    }
}

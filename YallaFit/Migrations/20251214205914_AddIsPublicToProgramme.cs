using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublicToProgramme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "Programme",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "programme_id",
                table: "Profil_Sportif",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profil_Sportif_programme_id",
                table: "Profil_Sportif",
                column: "programme_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profil_Sportif_Programme_programme_id",
                table: "Profil_Sportif",
                column: "programme_id",
                principalTable: "Programme",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profil_Sportif_Programme_programme_id",
                table: "Profil_Sportif");

            migrationBuilder.DropIndex(
                name: "IX_Profil_Sportif_programme_id",
                table: "Profil_Sportif");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "Programme");

            migrationBuilder.DropColumn(
                name: "programme_id",
                table: "Profil_Sportif");
        }
    }
}

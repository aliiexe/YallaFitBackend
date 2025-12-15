using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToTrainingSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "Training_Set",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notes",
                table: "Training_Set");
        }
    }
}

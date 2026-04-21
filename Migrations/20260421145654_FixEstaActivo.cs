using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiPrimeraAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixEstaActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaActivo",
                table: "Productos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaActivo",
                table: "Productos");
        }
    }
}

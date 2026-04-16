using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiPrimeraAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixVentaColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DireccionId",
                table: "Ventas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MetodoPagoId",
                table: "Ventas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_DireccionId",
                table: "Ventas",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_MetodoPagoId",
                table: "Ventas",
                column: "MetodoPagoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Direcciones_DireccionId",
                table: "Ventas",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodoPago_MetodoPagoId",
                table: "Ventas",
                column: "MetodoPagoId",
                principalTable: "MetodoPago",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Direcciones_DireccionId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodoPago_MetodoPagoId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_DireccionId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_MetodoPagoId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "DireccionId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "MetodoPagoId",
                table: "Ventas");
        }
    }
}

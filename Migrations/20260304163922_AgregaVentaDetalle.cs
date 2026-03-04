using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiPrimeraAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregaVentaDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VentaDetalles_Productos_ProductoId",
                table: "VentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaDetalles_Ventas_VentaId",
                table: "VentaDetalles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VentaDetalles",
                table: "VentaDetalles");

            migrationBuilder.RenameTable(
                name: "VentaDetalles",
                newName: "VentaDetalle");

            migrationBuilder.RenameIndex(
                name: "IX_VentaDetalles_VentaId",
                table: "VentaDetalle",
                newName: "IX_VentaDetalle_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_VentaDetalles_ProductoId",
                table: "VentaDetalle",
                newName: "IX_VentaDetalle_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VentaDetalle",
                table: "VentaDetalle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaDetalle_Productos_ProductoId",
                table: "VentaDetalle",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaDetalle_Ventas_VentaId",
                table: "VentaDetalle",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VentaDetalle_Productos_ProductoId",
                table: "VentaDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaDetalle_Ventas_VentaId",
                table: "VentaDetalle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VentaDetalle",
                table: "VentaDetalle");

            migrationBuilder.RenameTable(
                name: "VentaDetalle",
                newName: "VentaDetalles");

            migrationBuilder.RenameIndex(
                name: "IX_VentaDetalle_VentaId",
                table: "VentaDetalles",
                newName: "IX_VentaDetalles_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_VentaDetalle_ProductoId",
                table: "VentaDetalles",
                newName: "IX_VentaDetalles_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VentaDetalles",
                table: "VentaDetalles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaDetalles_Productos_ProductoId",
                table: "VentaDetalles",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaDetalles_Ventas_VentaId",
                table: "VentaDetalles",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiPrimeraAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregaProductoArtistas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoArtista_Artistas_ArtistaId",
                table: "ProductoArtista");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoArtista_Productos_ProductoId",
                table: "ProductoArtista");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoArtista",
                table: "ProductoArtista");

            migrationBuilder.RenameTable(
                name: "ProductoArtista",
                newName: "ProductoArtistas");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoArtista_ArtistaId",
                table: "ProductoArtistas",
                newName: "IX_ProductoArtistas_ArtistaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoArtistas",
                table: "ProductoArtistas",
                columns: new[] { "ProductoId", "ArtistaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoArtistas_Artistas_ArtistaId",
                table: "ProductoArtistas",
                column: "ArtistaId",
                principalTable: "Artistas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoArtistas_Productos_ProductoId",
                table: "ProductoArtistas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoArtistas_Artistas_ArtistaId",
                table: "ProductoArtistas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoArtistas_Productos_ProductoId",
                table: "ProductoArtistas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoArtistas",
                table: "ProductoArtistas");

            migrationBuilder.RenameTable(
                name: "ProductoArtistas",
                newName: "ProductoArtista");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoArtistas_ArtistaId",
                table: "ProductoArtista",
                newName: "IX_ProductoArtista_ArtistaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoArtista",
                table: "ProductoArtista",
                columns: new[] { "ProductoId", "ArtistaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoArtista_Artistas_ArtistaId",
                table: "ProductoArtista",
                column: "ArtistaId",
                principalTable: "Artistas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoArtista_Productos_ProductoId",
                table: "ProductoArtista",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

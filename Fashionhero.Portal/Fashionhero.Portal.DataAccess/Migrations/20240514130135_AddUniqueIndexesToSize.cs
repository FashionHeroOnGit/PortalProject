using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashionhero.Portal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesToSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_ProductId",
                table: "Sizes");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ProductId_Primary_Secondary",
                table: "Sizes",
                columns: new[] { "ProductId", "Primary", "Secondary" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_ProductId_Primary_Secondary",
                table: "Sizes");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ProductId",
                table: "Sizes",
                column: "ProductId");
        }
    }
}

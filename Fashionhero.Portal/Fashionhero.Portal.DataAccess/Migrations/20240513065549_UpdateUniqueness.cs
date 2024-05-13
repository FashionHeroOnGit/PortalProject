using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashionhero.Portal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ReferenceId",
                table: "Sizes",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocaleProducts_ReferenceId_IsoName",
                table: "LocaleProducts",
                columns: new[] { "ReferenceId", "IsoName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_ReferenceId",
                table: "Sizes");

            migrationBuilder.DropIndex(
                name: "IX_LocaleProducts_ReferenceId_IsoName",
                table: "LocaleProducts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashionhero.Portal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceIdsToRemainingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Prices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name_ReferenceId",
                table: "Tags",
                columns: new[] { "Name", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ReferenceId_Currency",
                table: "Prices",
                columns: new[] { "ReferenceId", "Currency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ReferenceId_Url",
                table: "Images",
                columns: new[] { "ReferenceId", "Url" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name_ReferenceId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ReferenceId_Currency",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Images_ReferenceId_Url",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Images");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashionhero.Portal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddModelProductNumberPropertyToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelProductNumber",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelProductNumber",
                table: "Products");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetCategoriesAndStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCategoryId",
                table: "Assets",
                column: "AssetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetCategories_AssetCategoryId",
                table: "Assets",
                column: "AssetCategoryId",
                principalTable: "AssetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetCategories_AssetCategoryId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetCategoryId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "Assets");
        }
    }
}

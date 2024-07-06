using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouse.Data.Migrations
{
    public partial class addedProducts2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_ProductTypeId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpedialTagID",
                table: "Products",
                column: "SpedialTagID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpedialTagID",
                table: "Products",
                column: "SpedialTagID",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpedialTagID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpedialTagID",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

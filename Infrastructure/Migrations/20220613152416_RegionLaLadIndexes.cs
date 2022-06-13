using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RegionLaLadIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_Name",
                table: "LocalAuthorityDistricts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_Name",
                table: "LocalAuthority",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Regions_Name",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistricts_Name",
                table: "LocalAuthorityDistricts");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthority_Name",
                table: "LocalAuthority");
        }
    }
}

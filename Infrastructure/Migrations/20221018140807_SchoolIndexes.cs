using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class SchoolIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Schools_Postcode",
                table: "Schools",
                column: "Postcode");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_Urn",
                table: "Schools",
                column: "Urn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentStatus_Name",
                table: "EstablishmentStatus",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schools_Postcode",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Schools_Urn",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_EstablishmentStatus_Name",
                table: "EstablishmentStatus");
        }
    }
}

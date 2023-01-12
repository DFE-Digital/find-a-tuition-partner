using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class OrganisationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Delete all the data - since Organisation Type needs to be re-added by the import process
            migrationBuilder.Sql("DELETE FROM \"LocalAuthorityDistrictCoverage\";", true);
            migrationBuilder.Sql("DELETE FROM \"Prices\";", true);
            migrationBuilder.Sql("DELETE FROM \"SubjectCoverage\";", true);
            migrationBuilder.Sql("DELETE FROM \"TuitionPartners\";", true);

            migrationBuilder.DropColumn(
                name: "LegalStatus",
                table: "TuitionPartners");

            migrationBuilder.AddColumn<int>(
                name: "OrganisationTypeId",
                table: "TuitionPartners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrganisationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsTypeOfCharity = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationType", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "OrganisationType",
                columns: new[] { "Id", "IsTypeOfCharity", "Name" },
                values: new object[,]
                {
                    { 1, false, "Private company" },
                    { 2, false, "Limited company" },
                    { 3, false, "Limited liability partnership" },
                    { 4, true, "Private company limited by guarantee" },
                    { 5, true, "Charity/charities" },
                    { 6, true, "Non-profit" },
                    { 7, true, "Community interest company" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_OrganisationTypeId",
                table: "TuitionPartners",
                column: "OrganisationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationType_IsTypeOfCharity",
                table: "OrganisationType",
                column: "IsTypeOfCharity");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationType_Name",
                table: "OrganisationType",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartners_OrganisationType_OrganisationTypeId",
                table: "TuitionPartners",
                column: "OrganisationTypeId",
                principalTable: "OrganisationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartners_OrganisationType_OrganisationTypeId",
                table: "TuitionPartners");

            migrationBuilder.DropTable(
                name: "OrganisationType");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartners_OrganisationTypeId",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "OrganisationTypeId",
                table: "TuitionPartners");

            migrationBuilder.AddColumn<string>(
                name: "LegalStatus",
                table: "TuitionPartners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

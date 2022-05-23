using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerCoverage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TuitionPartnerCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_LocalAuthorityDistricts_LocalAuthori~",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_LocalAuthorityDistrictId",
                table: "TuitionPartnerCoverage",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_SubjectId",
                table: "TuitionPartnerCoverage",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionPartnerId",
                table: "TuitionPartnerCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionTypeId",
                table: "TuitionPartnerCoverage",
                column: "TuitionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionPartnerCoverage");
        }
    }
}

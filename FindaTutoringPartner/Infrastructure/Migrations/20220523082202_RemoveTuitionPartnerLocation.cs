using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemoveTuitionPartnerLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectTuitionPartnerLocation");

            migrationBuilder.DropTable(
                name: "TuitionPartnerLocationTutorType");

            migrationBuilder.DropTable(
                name: "TuitionPartnerLocations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TuitionPartnerLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    CoverageRadius = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerLocations_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerLocations_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectTuitionPartnerLocation",
                columns: table => new
                {
                    SubjectsId = table.Column<int>(type: "integer", nullable: false),
                    TuitionPartnerLocationsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTuitionPartnerLocation", x => new { x.SubjectsId, x.TuitionPartnerLocationsId });
                    table.ForeignKey(
                        name: "FK_SubjectTuitionPartnerLocation_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectTuitionPartnerLocation_TuitionPartnerLocations_Tuiti~",
                        column: x => x.TuitionPartnerLocationsId,
                        principalTable: "TuitionPartnerLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartnerLocationTutorType",
                columns: table => new
                {
                    TuitionPartnerLocationsId = table.Column<int>(type: "integer", nullable: false),
                    TutorTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerLocationTutorType", x => new { x.TuitionPartnerLocationsId, x.TutorTypesId });
                    table.ForeignKey(
                        name: "FK_TuitionPartnerLocationTutorType_TuitionPartnerLocations_Tui~",
                        column: x => x.TuitionPartnerLocationsId,
                        principalTable: "TuitionPartnerLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerLocationTutorType_TutorTypes_TutorTypesId",
                        column: x => x.TutorTypesId,
                        principalTable: "TutorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTuitionPartnerLocation_TuitionPartnerLocationsId",
                table: "SubjectTuitionPartnerLocation",
                column: "TuitionPartnerLocationsId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerLocations_AddressId",
                table: "TuitionPartnerLocations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerLocations_TuitionPartnerId",
                table: "TuitionPartnerLocations",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerLocationTutorType_TutorTypesId",
                table: "TuitionPartnerLocationTutorType",
                column: "TutorTypesId");
        }
    }
}

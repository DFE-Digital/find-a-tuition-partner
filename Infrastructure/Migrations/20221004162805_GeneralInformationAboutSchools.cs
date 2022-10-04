using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class GeneralInformationAboutSchools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstablishmentStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentTypeGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentTypeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhaseOfEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseOfEducation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralInformationAboutSchools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Urn = table.Column<int>(type: "integer", nullable: false),
                    EstablishmentName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    EstablishmentTypeGroupId = table.Column<int>(type: "integer", nullable: false),
                    EstablishmentStatusId = table.Column<int>(type: "integer", nullable: false),
                    PhaseOfEducationId = table.Column<int>(type: "integer", nullable: false),
                    LocalEducationAuthorityId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralInformationAboutSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralInformationAboutSchools_EstablishmentStatus_Establis~",
                        column: x => x.EstablishmentStatusId,
                        principalTable: "EstablishmentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralInformationAboutSchools_EstablishmentTypeGroup_Estab~",
                        column: x => x.EstablishmentTypeGroupId,
                        principalTable: "EstablishmentTypeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralInformationAboutSchools_LocalAuthority_LocalEducatio~",
                        column: x => x.LocalEducationAuthorityId,
                        principalTable: "LocalAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralInformationAboutSchools_LocalAuthorityDistricts_Loca~",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralInformationAboutSchools_PhaseOfEducation_PhaseOfEduc~",
                        column: x => x.PhaseOfEducationId,
                        principalTable: "PhaseOfEducation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EstablishmentStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Open" },
                    { 2, "Closed" },
                    { 3, "Open but proposed to close" },
                    { 4, "Proposed to open" }
                });

            migrationBuilder.InsertData(
                table: "EstablishmentTypeGroup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Colleges" },
                    { 2, "Universities" },
                    { 3, "Independent schools" },
                    { 4, "Local authority maintained schools" },
                    { 5, "Special schools" },
                    { 6, "Welsh schools" },
                    { 9, "Other types" },
                    { 10, "Academies" },
                    { 11, "Free schools" }
                });

            migrationBuilder.InsertData(
                table: "PhaseOfEducation",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Nursery" },
                    { 2, "Primary" },
                    { 3, "Middle deemed primary" },
                    { 4, "Secondary" },
                    { 5, "Middle deemed secondary" },
                    { 6, "16 Plus" },
                    { 7, "All-through" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTypeGroup_Name",
                table: "EstablishmentTypeGroup",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralInformationAboutSchools_EstablishmentStatusId",
                table: "GeneralInformationAboutSchools",
                column: "EstablishmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralInformationAboutSchools_EstablishmentTypeGroupId",
                table: "GeneralInformationAboutSchools",
                column: "EstablishmentTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralInformationAboutSchools_LocalAuthorityDistrictId",
                table: "GeneralInformationAboutSchools",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralInformationAboutSchools_LocalEducationAuthorityId",
                table: "GeneralInformationAboutSchools",
                column: "LocalEducationAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralInformationAboutSchools_PhaseOfEducationId",
                table: "GeneralInformationAboutSchools",
                column: "PhaseOfEducationId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseOfEducation_Name",
                table: "PhaseOfEducation",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralInformationAboutSchools");

            migrationBuilder.DropTable(
                name: "EstablishmentStatus");

            migrationBuilder.DropTable(
                name: "EstablishmentTypeGroup");

            migrationBuilder.DropTable(
                name: "PhaseOfEducation");
        }
    }
}

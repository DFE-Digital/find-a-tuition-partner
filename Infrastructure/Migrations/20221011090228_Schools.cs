using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Schools : Migration
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
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Urn = table.Column<int>(type: "integer", nullable: false),
                    EstablishmentName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Postcode = table.Column<string>(type: "text", nullable: false),
                    EstablishmentTypeGroupId = table.Column<int>(type: "integer", nullable: false),
                    EstablishmentStatusId = table.Column<int>(type: "integer", nullable: false),
                    PhaseOfEducationId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schools_EstablishmentStatus_EstablishmentStatusId",
                        column: x => x.EstablishmentStatusId,
                        principalTable: "EstablishmentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schools_EstablishmentTypeGroup_EstablishmentTypeGroupId",
                        column: x => x.EstablishmentTypeGroupId,
                        principalTable: "EstablishmentTypeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schools_LocalAuthority_LocalAuthorityId",
                        column: x => x.LocalAuthorityId,
                        principalTable: "LocalAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schools_LocalAuthorityDistricts_LocalAuthorityDistrictId",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schools_PhaseOfEducation_PhaseOfEducationId",
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
                    { 7, "All-through" },
                    { 9999, "Not Applicable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTypeGroup_Name",
                table: "EstablishmentTypeGroup",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseOfEducation_Name",
                table: "PhaseOfEducation",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_EstablishmentStatusId",
                table: "Schools",
                column: "EstablishmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_EstablishmentTypeGroupId",
                table: "Schools",
                column: "EstablishmentTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_LocalAuthorityDistrictId",
                table: "Schools",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_LocalAuthorityId",
                table: "Schools",
                column: "LocalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_PhaseOfEducationId",
                table: "Schools",
                column: "PhaseOfEducationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropTable(
                name: "EstablishmentStatus");

            migrationBuilder.DropTable(
                name: "EstablishmentTypeGroup");

            migrationBuilder.DropTable(
                name: "PhaseOfEducation");
        }
    }
}

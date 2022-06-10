using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Line1 = table.Column<string>(type: "text", nullable: false),
                    Line2 = table.Column<string>(type: "text", nullable: true),
                    Line3 = table.Column<string>(type: "text", nullable: true),
                    Line4 = table.Column<string>(type: "text", nullable: true),
                    Postcode = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TutorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartnerLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    CoverageRadius = table.Column<int>(type: "integer", nullable: false)
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
                name: "UserSearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SearchJson = table.Column<string>(type: "jsonb", nullable: false),
                    UserSessionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSearches_UserSessions_UserSessionId",
                        column: x => x.UserSessionId,
                        principalTable: "UserSessions",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Primary - Literacy" },
                    { 2, "Primary - Numeracy" },
                    { 3, "Primary - Science" },
                    { 4, "Secondary - English" },
                    { 5, "Secondary - Humanities" },
                    { 6, "Secondary - Maths" },
                    { 7, "Secondary - Modern Foreign Languages" },
                    { 8, "Secondary - Science" }
                });

            migrationBuilder.InsertData(
                table: "TutorTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Qualified Teachers" },
                    { 2, "Professional Tutors" },
                    { 3, "SEN Specialists" },
                    { 4, "Higher Level Teaching Assistants" },
                    { 5, "University Students" },
                    { 6, "Volunteer tutors" },
                    { 7, "No preference" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Latitude_Longitude",
                table: "Addresses",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Postcode",
                table: "Addresses",
                column: "Postcode");

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

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_Name",
                table: "TuitionPartners",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserSearches_UserSessionId",
                table: "UserSearches",
                column: "UserSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectTuitionPartnerLocation");

            migrationBuilder.DropTable(
                name: "TuitionPartnerLocationTutorType");

            migrationBuilder.DropTable(
                name: "UserSearches");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "TuitionPartnerLocations");

            migrationBuilder.DropTable(
                name: "TutorTypes");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "TuitionPartners");
        }
    }
}

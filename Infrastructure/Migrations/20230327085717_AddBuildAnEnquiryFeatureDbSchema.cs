using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddBuildAnEnquiryFeatureDbSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DROP TABLES If they exist mostly for main steel thread branch
            migrationBuilder.Sql("DROP TABLE IF EXISTS  \"TuitionPartnersEnquiry\";", true);
            migrationBuilder.Sql("DROP TABLE IF EXISTS  \"EnquiryResponses\";", true);
            migrationBuilder.Sql("DROP TABLE IF EXISTS  \"KeyStageSubjectsEnquiry\";", true);
            migrationBuilder.Sql("DROP TABLE IF EXISTS  \"MagicLinks\";", true);
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"Enquiries\";", true);

            migrationBuilder.CreateTable(
                name: "EnquiryResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyStageAndSubjectsText = table.Column<string>(type: "text", nullable: false),
                    TuitionTypeText = table.Column<string>(type: "text", nullable: false),
                    TutoringLogisticsText = table.Column<string>(type: "text", nullable: false),
                    SENDRequirementsText = table.Column<string>(type: "text", nullable: true),
                    AdditionalInformationText = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TutoringLogistics = table.Column<string>(type: "text", nullable: false),
                    SENDRequirements = table.Column<string>(type: "text", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SupportReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    PostCode = table.Column<string>(type: "text", nullable: false),
                    LocalAuthorityDistrict = table.Column<string>(type: "text", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: true),
                    MagicLinkId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enquiries_MagicLinks_MagicLinkId",
                        column: x => x.MagicLinkId,
                        principalTable: "MagicLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enquiries_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeyStageSubjectsEnquiry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnquiryId = table.Column<int>(type: "integer", nullable: false),
                    KeyStageId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyStageSubjectsEnquiry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyStageSubjectsEnquiry_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyStageSubjectsEnquiry_KeyStage_KeyStageId",
                        column: x => x.KeyStageId,
                        principalTable: "KeyStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyStageSubjectsEnquiry_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartnersEnquiry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnquiryId = table.Column<int>(type: "integer", nullable: false),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    MagicLinkId = table.Column<int>(type: "integer", nullable: false),
                    EnquiryResponseId = table.Column<int>(type: "integer", nullable: true),
                    ResponseCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnersEnquiry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_EnquiryResponses_EnquiryResponseId",
                        column: x => x.EnquiryResponseId,
                        principalTable: "EnquiryResponses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_MagicLinks_MagicLinkId",
                        column: x => x.MagicLinkId,
                        principalTable: "MagicLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_Email",
                table: "Enquiries",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_MagicLinkId",
                table: "Enquiries",
                column: "MagicLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_SupportReferenceNumber",
                table: "Enquiries",
                column: "SupportReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_TuitionTypeId",
                table: "Enquiries",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyStageSubjectsEnquiry_EnquiryId",
                table: "KeyStageSubjectsEnquiry",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyStageSubjectsEnquiry_KeyStageId",
                table: "KeyStageSubjectsEnquiry",
                column: "KeyStageId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyStageSubjectsEnquiry_SubjectId",
                table: "KeyStageSubjectsEnquiry",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_EnquiryId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_EnquiryResponseId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_MagicLinkId",
                table: "TuitionPartnersEnquiry",
                column: "MagicLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_TuitionPartnerId",
                table: "TuitionPartnersEnquiry",
                column: "TuitionPartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyStageSubjectsEnquiry");

            migrationBuilder.DropTable(
                name: "TuitionPartnersEnquiry");

            migrationBuilder.DropTable(
                name: "Enquiries");

            migrationBuilder.DropTable(
                name: "EnquiryResponses");

            migrationBuilder.DropTable(
                name: "MagicLinks");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class KeyStageSubjectEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyStageSubjectsEnquiry");
        }
    }
}

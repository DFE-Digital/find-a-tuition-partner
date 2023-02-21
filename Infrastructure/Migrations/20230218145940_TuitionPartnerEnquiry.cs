using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionPartnerEnquirySeoUrls");

            migrationBuilder.CreateTable(
                name: "TuitionPartnersEnquiry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    EnquiryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnersEnquiry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TuitionPartnersEnquiry_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_EnquiryId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_TuitionPartnerId",
                table: "TuitionPartnersEnquiry",
                column: "TuitionPartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionPartnersEnquiry");

            migrationBuilder.CreateTable(
                name: "TuitionPartnerEnquirySeoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnquiryId = table.Column<int>(type: "integer", nullable: true),
                    SeoUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerEnquirySeoUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerEnquirySeoUrls_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerEnquirySeoUrls_EnquiryId",
                table: "TuitionPartnerEnquirySeoUrls",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerEnquirySeoUrls_SeoUrl",
                table: "TuitionPartnerEnquirySeoUrls",
                column: "SeoUrl");
        }
    }
}

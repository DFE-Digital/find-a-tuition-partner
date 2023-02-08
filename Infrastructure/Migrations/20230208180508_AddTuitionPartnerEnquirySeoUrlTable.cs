using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddTuitionPartnerEnquirySeoUrlTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TuitionPartners",
                table: "Enquiries");

            migrationBuilder.CreateTable(
                name: "TuitionPartnerEnquirySeoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    EnquiryId = table.Column<int>(type: "integer", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionPartnerEnquirySeoUrls");

            migrationBuilder.AddColumn<List<string>>(
                name: "TuitionPartners",
                table: "Enquiries",
                type: "text[]",
                nullable: false);
        }
    }
}

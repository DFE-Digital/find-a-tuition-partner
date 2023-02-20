using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddMagicLinkType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicLinks_EnquiryResponses_EnquiryResponseId",
                table: "MagicLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.RenameColumn(
                name: "EnquiryResponseId",
                table: "MagicLinks",
                newName: "MagicLinkTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MagicLinks_EnquiryResponseId",
                table: "MagicLinks",
                newName: "IX_MagicLinks_MagicLinkTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "EnquiryId",
                table: "TuitionPartnersEnquiry",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MagicLinkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicLinkTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MagicLinkTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "EnquiryRequest" },
                    { 2, "EnquirerViewResponse" },
                    { 3, "EnquirerViewAllResponses" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MagicLinkTypes_Name",
                table: "MagicLinkTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicLinks_MagicLinkTypes_MagicLinkTypeId",
                table: "MagicLinks",
                column: "MagicLinkTypeId",
                principalTable: "MagicLinkTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryId",
                principalTable: "Enquiries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicLinks_MagicLinkTypes_MagicLinkTypeId",
                table: "MagicLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropTable(
                name: "MagicLinkTypes");

            migrationBuilder.RenameColumn(
                name: "MagicLinkTypeId",
                table: "MagicLinks",
                newName: "EnquiryResponseId");

            migrationBuilder.RenameIndex(
                name: "IX_MagicLinks_MagicLinkTypeId",
                table: "MagicLinks",
                newName: "IX_MagicLinks_EnquiryResponseId");

            migrationBuilder.AlterColumn<int>(
                name: "EnquiryId",
                table: "TuitionPartnersEnquiry",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicLinks_EnquiryResponses_EnquiryResponseId",
                table: "MagicLinks",
                column: "EnquiryResponseId",
                principalTable: "EnquiryResponses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnersEnquiry_Enquiries_EnquiryId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryId",
                principalTable: "Enquiries",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EnquiryResponseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnquirerNotInterestedReasonAdditionalInfo",
                table: "EnquiryResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnquirerNotInterestedReasonId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnquiryResponseStatusId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnquiryResponseStatusLastUpdated",
                table: "EnquiryResponses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TuitionPartnerResponseNotInterestedEmailLogId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EnquirerNotInterestedReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CollectAdditionalInfoIfSelected = table.Column<bool>(type: "boolean", nullable: false),
                    OrderBy = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquirerNotInterestedReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnquiryResponseStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OrderBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryResponseStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EnquirerNotInterestedReasons",
                columns: new[] { "Id", "CollectAdditionalInfoIfSelected", "Description", "IsActive", "OrderBy" },
                values: new object[,]
                {
                    { 1, false, "The response does not adequately cover my tuition plan needs", true, 1 },
                    { 2, false, "The response does not adequately cover support for our pupils with SEND", true, 2 },
                    { 3, false, "The response is too generic and doesn’t offer enough information", true, 3 },
                    { 4, true, "Other", true, 4 }
                });

            migrationBuilder.InsertData(
                table: "EnquiryResponseStatus",
                columns: new[] { "Id", "Description", "OrderBy", "Status" },
                values: new object[,]
                {
                    { 1, "The enquirer has indicated that they are interested in the tuition partner response", 1, "INTERESTED" },
                    { 2, "The enquirer has opened the tuition partner response, but has not confirmed if they are interested or not", 2, "UNDECIDED" },
                    { 3, "The enquirer has not yet viewed the tuition partner response", 3, "UNREAD" },
                    { 4, "Status that is used for enquries that are historical and we don't have the latest status for", 4, "NOT SET" },
                    { 5, "The enquirer has indicated that they are not interested in the tuition partner response", 5, "NOT INTERESTED" }
                });

            //MANUAL CHANGES - START
            migrationBuilder.Sql("UPDATE \"EnquiryResponses\" SET \"EnquiryResponseStatusId\" = 4;", true);
            //MANUAL CHANGES - END

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_EnquirerNotInterestedReasonId",
                table: "EnquiryResponses",
                column: "EnquirerNotInterestedReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_EnquiryResponseStatusId",
                table: "EnquiryResponses",
                column: "EnquiryResponseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_TuitionPartnerResponseNotInterestedEmailLo~",
                table: "EnquiryResponses",
                column: "TuitionPartnerResponseNotInterestedEmailLogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnquirerNotInterestedReasons_Description",
                table: "EnquirerNotInterestedReasons",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponseStatus_Status",
                table: "EnquiryResponseStatus",
                column: "Status",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EmailLog_TuitionPartnerResponseNotInterest~",
                table: "EnquiryResponses",
                column: "TuitionPartnerResponseNotInterestedEmailLogId",
                principalTable: "EmailLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EnquirerNotInterestedReasons_EnquirerNotIn~",
                table: "EnquiryResponses",
                column: "EnquirerNotInterestedReasonId",
                principalTable: "EnquirerNotInterestedReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EnquiryResponseStatus_EnquiryResponseStatu~",
                table: "EnquiryResponses",
                column: "EnquiryResponseStatusId",
                principalTable: "EnquiryResponseStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EmailLog_TuitionPartnerResponseNotInterest~",
                table: "EnquiryResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EnquirerNotInterestedReasons_EnquirerNotIn~",
                table: "EnquiryResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EnquiryResponseStatus_EnquiryResponseStatu~",
                table: "EnquiryResponses");

            migrationBuilder.DropTable(
                name: "EnquirerNotInterestedReasons");

            migrationBuilder.DropTable(
                name: "EnquiryResponseStatus");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_EnquirerNotInterestedReasonId",
                table: "EnquiryResponses");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_EnquiryResponseStatusId",
                table: "EnquiryResponses");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_TuitionPartnerResponseNotInterestedEmailLo~",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquirerNotInterestedReasonAdditionalInfo",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquirerNotInterestedReasonId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquiryResponseStatusId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquiryResponseStatusLastUpdated",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "TuitionPartnerResponseNotInterestedEmailLogId",
                table: "EnquiryResponses");
        }
    }
}

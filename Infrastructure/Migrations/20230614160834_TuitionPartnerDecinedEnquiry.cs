using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerDecinedEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TuitionPartnerDecinedEnquiry",
                table: "TuitionPartnersEnquiry",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TuitionPartnerDecinedEnquiryDate",
                table: "TuitionPartnersEnquiry",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TuitionPartnerDecinedEnquiry",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropColumn(
                name: "TuitionPartnerDecinedEnquiryDate",
                table: "TuitionPartnersEnquiry");
        }
    }
}

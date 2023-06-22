using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerDeclinedEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TuitionPartnerDeclinedEnquiry",
                table: "TuitionPartnersEnquiry",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TuitionPartnerDeclinedEnquiryDate",
                table: "TuitionPartnersEnquiry",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TuitionPartnerDeclinedEnquiry",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropColumn(
                name: "TuitionPartnerDeclinedEnquiryDate",
                table: "TuitionPartnersEnquiry");
        }
    }
}

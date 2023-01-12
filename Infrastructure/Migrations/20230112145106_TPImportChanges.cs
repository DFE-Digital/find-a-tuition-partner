using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TPImportChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalServiceOfferings",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "HasSenProvision",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "TuitionPartners");

            migrationBuilder.AddColumn<DateTime>(
                name: "TPLastUpdatedData",
                table: "TuitionPartners",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TPLastUpdatedData",
                table: "TuitionPartners");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalServiceOfferings",
                table: "TuitionPartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "TuitionPartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HasSenProvision",
                table: "TuitionPartners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastUpdated",
                table: "TuitionPartners",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}

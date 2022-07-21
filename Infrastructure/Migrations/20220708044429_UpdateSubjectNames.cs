using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateSubjectNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "English", "key-stage-1-english" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Maths", "key-stage-1-maths" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "English", "key-stage-2-english" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Maths", "key-stage-2-maths" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "English");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Literacy", "key-stage-1-literacy" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Numeracy", "key-stage-1-numeracy" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Literacy", "key-stage-2-literacy" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "SeoUrl" },
                values: new object[] { "Numeracy", "key-stage-2-numeracy" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Literacy");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class KeyStageSubjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeyStageId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KeyStage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyStage", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "KeyStage",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Key Stage 1" },
                    { 2, "Key Stage 2" },
                    { 3, "Key Stage 3" },
                    { 4, "Key Stage 4" }
                });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "KeyStageId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                column: "KeyStageId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                column: "KeyStageId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                column: "KeyStageId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                column: "KeyStageId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "KeyStageId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                column: "KeyStageId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                column: "KeyStageId",
                value: 3);

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "KeyStageId", "Name" },
                values: new object[,]
                {
                    { 9, 1, "Literacy" },
                    { 10, 1, "Numeracy" },
                    { 11, 1, "Science" },
                    { 12, 2, "Literacy" },
                    { 13, 2, "Numeracy" },
                    { 14, 2, "Literacy" },
                    { 15, 3, "English" },
                    { 16, 3, "Humanities" },
                    { 17, 3, "Maths" },
                    { 18, 3, "Modern Foreign Languages" },
                    { 19, 3, "Science" },
                    { 20, 4, "English" },
                    { 21, 4, "Humanities" },
                    { 22, 4, "Maths" },
                    { 23, 4, "Modern Foreign Languages" },
                    { 24, 4, "Science" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_KeyStageId",
                table: "Subjects",
                column: "KeyStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_KeyStage_KeyStageId",
                table: "Subjects",
                column: "KeyStageId",
                principalTable: "KeyStage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_KeyStage_KeyStageId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "KeyStage");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_KeyStageId",
                table: "Subjects");

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DropColumn(
                name: "KeyStageId",
                table: "Subjects");
        }
    }
}

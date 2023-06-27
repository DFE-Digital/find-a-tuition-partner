using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateLAsAndLADs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 835);

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 390,
                column: "Code",
                value: "E08000037");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 391,
                column: "Name",
                value: "Newcastle upon Tyne");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 420,
                column: "Name",
                value: "Isles Of Scilly");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 825,
                column: "Code",
                value: "E06000060");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 839,
                column: "Name",
                value: "Bournemouth, Christchurch and Poole");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 840,
                column: "Name",
                value: "County Durham");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 884,
                column: "Name",
                value: "Herefordshire, County of");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 929,
                column: "Code",
                value: "E06000057");

            migrationBuilder.InsertData(
                table: "LocalAuthority",
                columns: new[] { "Id", "Code", "Name", "RegionId" },
                values: new object[] { 838, "E06000059", "Dorset", 9 });

            migrationBuilder.InsertData(
                table: "LocalAuthorityDistricts",
                columns: new[] { "Id", "Code", "LocalAuthorityId", "Name", "RegionId" },
                values: new object[,]
                {
                    { 310, "E06000063", 909, "Cumberland", 2 },
                    { 311, "E06000065", 815, "North Yorkshire", 3 },
                    { 312, "E06000066", 933, "Somerset", 9 },
                    { 313, "E06000064", 909, "Westmorland and Furness", 2 }
                });

            //MANUAL CHANGES - START (since deleted above do to ref integrity issues)
            migrationBuilder.InsertData(
                table: "LocalAuthorityDistricts",
                columns: new[] { "Id", "Code", "LocalAuthorityId", "Name", "RegionId" },
                values: new object[,]
                {
                    { 63, "E06000059", 838, "Dorset", 9 }
                });
            //MANUAL CHANGES - END

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 63,
                column: "LocalAuthorityId",
                value: 838);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 838);

            migrationBuilder.DeleteData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 310);

            migrationBuilder.DeleteData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 311);

            migrationBuilder.DeleteData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 312);

            migrationBuilder.DeleteData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 313);

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 390,
                column: "Code",
                value: "E08000020");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 391,
                column: "Name",
                value: "Newcastle Upon Tyne");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 420,
                column: "Name",
                value: "Isles of Scilly");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 825,
                column: "Code",
                value: "E10000002");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 839,
                column: "Name",
                value: "Bournemouth, Christchurch & Poole");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 840,
                column: "Name",
                value: "Durham");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 884,
                column: "Name",
                value: "Herefordshire");

            migrationBuilder.UpdateData(
                table: "LocalAuthority",
                keyColumn: "Id",
                keyValue: 929,
                column: "Code",
                value: "E06000048");

            migrationBuilder.InsertData(
                table: "LocalAuthority",
                columns: new[] { "Id", "Code", "Name", "RegionId" },
                values: new object[] { 835, "E10000009", "Dorset", 9 });

            //MANUAL CHANGES - START (since deleted above do to ref integrity issues)
            migrationBuilder.InsertData(
                table: "LocalAuthorityDistricts",
                columns: new[] { "Id", "Code", "LocalAuthorityId", "Name", "RegionId" },
                values: new object[,]
                {
                    { 63, "E06000059", 835, "Dorset", 9 }
                });
            //MANUAL CHANGES - END

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 63,
                column: "LocalAuthorityId",
                value: 835);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionTypeToTuitionSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_TuitionTypes_TuitionTypeId",
                table: "Enquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalAuthorityDistrictCoverage_TuitionTypes_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_TuitionTypes_TuitionTypeId",
                table: "Prices");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectCoverage_TuitionTypes_TuitionTypeId",
                table: "SubjectCoverage");

            migrationBuilder.DropTable(
                name: "TuitionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_TuitionTypeId",
                table: "Enquiries");

            //Manual change, moved to lower in script:
            //migrationBuilder.DropColumn(
            //    name: "TuitionTypeId",
            //    table: "Enquiries");

            migrationBuilder.RenameColumn(
                name: "TuitionTypeId",
                table: "SubjectCoverage",
                newName: "TuitionSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectCoverage_TuitionTypeId_SubjectId",
                table: "SubjectCoverage",
                newName: "IX_SubjectCoverage_TuitionSettingId_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_TuitionTypeId_SubjectId",
                table: "SubjectCoverage",
                newName: "IX_SubjectCoverage_TuitionPartnerId_TuitionSettingId_SubjectId");

            migrationBuilder.RenameColumn(
                name: "TuitionTypeId",
                table: "Prices",
                newName: "TuitionSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_TuitionTypeId",
                table: "Prices",
                newName: "IX_Prices_TuitionSettingId");

            migrationBuilder.RenameColumn(
                name: "TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage",
                newName: "TuitionSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId_LocalAuthority~",
                table: "LocalAuthorityDistrictCoverage",
                newName: "IX_LocalAuthorityDistrictCoverage_TuitionSettingId_LocalAuthor~");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionType~",
                table: "LocalAuthorityDistrictCoverage",
                newName: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionSett~");

            migrationBuilder.RenameColumn(
                name: "TuitionTypeText",
                table: "EnquiryResponses",
                newName: "TuitionSettingText");

            migrationBuilder.CreateTable(
                name: "TuitionSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnquiryTuitionSetting",
                columns: table => new
                {
                    EnquiriesId = table.Column<int>(type: "integer", nullable: false),
                    TuitionSettingsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryTuitionSetting", x => new { x.EnquiriesId, x.TuitionSettingsId });
                    table.ForeignKey(
                        name: "FK_EnquiryTuitionSetting_Enquiries_EnquiriesId",
                        column: x => x.EnquiriesId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnquiryTuitionSetting_TuitionSettings_TuitionSettingsId",
                        column: x => x.TuitionSettingsId,
                        principalTable: "TuitionSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TuitionSettings",
                columns: new[] { "Id", "Name", "SeoUrl" },
                values: new object[,]
                {
                    { 1, "Online", "online" },
                    { 2, "Face-to-face", "face-to-face" }
                });

            //MANUAL CHANGES START
            migrationBuilder.Sql(@"INSERT INTO ""EnquiryTuitionSetting""
                                    SELECT ""Id"", ""TuitionTypeId""
                                    FROM ""Enquiries""
                                    WHERE ""TuitionTypeId"" IS NOT NULL", true);

            migrationBuilder.DropColumn(
                name: "TuitionTypeId",
                table: "Enquiries");
            //MANUAL CHANGES END

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryTuitionSetting_TuitionSettingsId",
                table: "EnquiryTuitionSetting",
                column: "TuitionSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionSettings_Name",
                table: "TuitionSettings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionSettings_SeoUrl",
                table: "TuitionSettings",
                column: "SeoUrl",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAuthorityDistrictCoverage_TuitionSettings_TuitionSetti~",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionSettingId",
                principalTable: "TuitionSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_TuitionSettings_TuitionSettingId",
                table: "Prices",
                column: "TuitionSettingId",
                principalTable: "TuitionSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectCoverage_TuitionSettings_TuitionSettingId",
                table: "SubjectCoverage",
                column: "TuitionSettingId",
                principalTable: "TuitionSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAuthorityDistrictCoverage_TuitionSettings_TuitionSetti~",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_TuitionSettings_TuitionSettingId",
                table: "Prices");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectCoverage_TuitionSettings_TuitionSettingId",
                table: "SubjectCoverage");

            //MANUAL CHANGES START
            migrationBuilder.AddColumn<int>(
                name: "TuitionTypeId",
                table: "Enquiries",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ""Enquiries""
                                    SET ""TuitionTypeId"" = subquery.""TuitionSettingsId""
                                    FROM (SELECT ""EnquiriesId"", ""TuitionSettingsId"" FROM ""EnquiryTuitionSetting"") AS subquery
                                    WHERE ""Enquiries"".""Id""=subquery.""EnquiriesId""", true);
            //MANUAL CHANGES END

            migrationBuilder.DropTable(
                name: "EnquiryTuitionSetting");

            migrationBuilder.DropTable(
                name: "TuitionSettings");

            migrationBuilder.RenameColumn(
                name: "TuitionSettingId",
                table: "SubjectCoverage",
                newName: "TuitionTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectCoverage_TuitionSettingId_SubjectId",
                table: "SubjectCoverage",
                newName: "IX_SubjectCoverage_TuitionTypeId_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_TuitionSettingId_SubjectId",
                table: "SubjectCoverage",
                newName: "IX_SubjectCoverage_TuitionPartnerId_TuitionTypeId_SubjectId");

            migrationBuilder.RenameColumn(
                name: "TuitionSettingId",
                table: "Prices",
                newName: "TuitionTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_TuitionSettingId",
                table: "Prices",
                newName: "IX_Prices_TuitionTypeId");

            migrationBuilder.RenameColumn(
                name: "TuitionSettingId",
                table: "LocalAuthorityDistrictCoverage",
                newName: "TuitionTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionSettingId_LocalAuthor~",
                table: "LocalAuthorityDistrictCoverage",
                newName: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId_LocalAuthority~");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionSett~",
                table: "LocalAuthorityDistrictCoverage",
                newName: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionType~");

            migrationBuilder.RenameColumn(
                name: "TuitionSettingText",
                table: "EnquiryResponses",
                newName: "TuitionTypeText");

            //MANUALLY MOVED ABOVE
            //migrationBuilder.AddColumn<int>(
            //    name: "TuitionTypeId",
            //    table: "Enquiries",
            //    type: "integer",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "TuitionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SeoUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TuitionTypes",
                columns: new[] { "Id", "Name", "SeoUrl" },
                values: new object[,]
                {
                    { 1, "Online", "online" },
                    { 2, "In School", "in-school" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_TuitionTypeId",
                table: "Enquiries",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionTypes_Name",
                table: "TuitionTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionTypes_SeoUrl",
                table: "TuitionTypes",
                column: "SeoUrl",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_TuitionTypes_TuitionTypeId",
                table: "Enquiries",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAuthorityDistrictCoverage_TuitionTypes_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_TuitionTypes_TuitionTypeId",
                table: "Prices",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectCoverage_TuitionTypes_TuitionTypeId",
                table: "SubjectCoverage",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

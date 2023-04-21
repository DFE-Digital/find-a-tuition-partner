using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EmailLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TuitionPartnerEnquirySubmittedEmailLogId",
                table: "TuitionPartnersEnquiry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnquirerResponseEmailLogId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmailStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AllowEmailSending = table.Column<bool>(type: "boolean", nullable: false),
                    PollForStatusUpdateIfSent = table.Column<bool>(type: "boolean", nullable: false),
                    RetrySendInSeconds = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledProcessingInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScheduleName = table.Column<string>(type: "text", nullable: false),
                    LastStartedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledProcessingInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessFromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastEmailSendAttemptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinishProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastStatusChangedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    EmailTemplateShortName = table.Column<string>(type: "text", nullable: false),
                    ClientReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    EmailAddressUsedForTesting = table.Column<string>(type: "text", nullable: true),
                    EmailStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLog_EmailStatus_EmailStatusId",
                        column: x => x.EmailStatusId,
                        principalTable: "EmailStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessFromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastEmailSendAttemptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailStatusId = table.Column<int>(type: "integer", nullable: false),
                    EmailLogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogHistory_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailNotifyResponseLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotifyId = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    Uri = table.Column<string>(type: "text", nullable: true),
                    TemplateId = table.Column<string>(type: "text", nullable: true),
                    TemplateUri = table.Column<string>(type: "text", nullable: true),
                    TemplateVersion = table.Column<int>(type: "integer", nullable: true),
                    EmailResponseContentFrom = table.Column<string>(type: "text", nullable: true),
                    EmailResponseContentBody = table.Column<string>(type: "text", nullable: true),
                    EmailResponseContentSubject = table.Column<string>(type: "text", nullable: true),
                    ExceptionCode = table.Column<string>(type: "text", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "text", nullable: true),
                    EmailLogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailNotifyResponseLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailNotifyResponseLog_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailPersonalisationLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    EmailLogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPersonalisationLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailPersonalisationLog_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailTriggerActivation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmailLogId = table.Column<int>(type: "integer", nullable: false),
                    ActivateEmailLogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTriggerActivation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTriggerActivation_EmailLog_ActivateEmailLogId",
                        column: x => x.ActivateEmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailTriggerActivation_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EmailStatus",
                columns: new[] { "Id", "AllowEmailSending", "Description", "PollForStatusUpdateIfSent", "RetrySendInSeconds", "Status" },
                values: new object[,]
                {
                    { 1, true, "Has been newly added to log, will be processed next time the email processing is run", true, null, "to-be-processed" },
                    { 2, true, "Is waiting for a chained email to be delivered (e.g. TP emails are only sent once the enquirer email has been delivered)", true, null, "waiting-to-be-triggered" },
                    { 3, true, "The email is to be sent is to be sent in the future (e.g. send notification emails daily)", true, null, "delayed-email" },
                    { 4, false, "The email has been processed and sent to GOV.UK Notify to be delivered", true, null, "been-processed" },
                    { 5, false, "GOV.UK Notify status: has placed the message in a queue, ready to be sent to the provider. It should only remain in this state for a few seconds.", true, null, "created" },
                    { 6, false, "GOV.UK Notify status: has sent the message to the provider. The provider will try to deliver the message to the recipient for up to 72 hours. GOV.UK Notify is waiting for delivery information.", true, null, "sending" },
                    { 7, false, "GOV.UK Notify status: the message was successfully delivered.", false, null, "delivered" },
                    { 8, false, "GOV.UK Notify status: the provider could not deliver the message because the email address was wrong. You should remove these email addresses from your database.", false, null, "permanent-failure" },
                    { 9, true, "GOV.UK Notify status: the provider could not deliver the message. This can happen when the recipient’s inbox is full or their anti-spam filter rejects your email. Check your content does not look like spam before you try to send the message again.", true, 600, "temporary-failure" },
                    { 10, true, "GOV.UK Notify status: your message was not sent because there was a problem between Notify and the provider. You’ll have to try sending your messages again.", true, 60, "technical-failure" },
                    { 11, true, "Error when calling the GOV.UK Notify SendEmailAsync()", false, 600, "processing-failure" }
                });

            //MANUAL CHANGES - START
            migrationBuilder.Sql("UPDATE \"TuitionPartnersEnquiry\" SET \"TuitionPartnerEnquirySubmittedEmailLogId\" = 1;", true);
            migrationBuilder.Sql("UPDATE \"EnquiryResponses\" SET \"EnquirerResponseEmailLogId\" = 1;", true);
            migrationBuilder.Sql("UPDATE \"EnquiryResponses\" SET \"TuitionPartnerResponseEmailLogId\" = 1;", true);
            migrationBuilder.Sql("UPDATE \"Enquiries\" SET \"EnquirerEnquirySubmittedEmailLogId\" = 1;", true);

            migrationBuilder.Sql(
            @"CREATE OR REPLACE FUNCTION fn_email_log_history_on_insert()
              RETURNS TRIGGER 
              LANGUAGE PLPGSQL
              AS
            $$
            BEGIN
				INSERT INTO ""EmailLogHistory""(""EmailLogId"", ""CreatedAt"", ""ProcessFromDate"", ""LastEmailSendAttemptDate"", ""EmailStatusId"")
				VALUES(NEW.""Id"", current_timestamp, NEW.""ProcessFromDate"", NEW.""LastEmailSendAttemptDate"", NEW.""EmailStatusId"");

	            RETURN NEW;
            END;
            $$", true);

            migrationBuilder.Sql(
            @"CREATE OR REPLACE FUNCTION fn_email_log_history_on_update()
              RETURNS TRIGGER 
              LANGUAGE PLPGSQL
              AS
            $$
            BEGIN
	            IF (COALESCE(NEW.""ProcessFromDate"", CURRENT_DATE) <> COALESCE(OLD.""ProcessFromDate"", CURRENT_DATE) OR
		            COALESCE(NEW.""LastEmailSendAttemptDate"", CURRENT_DATE) <> COALESCE(OLD.""LastEmailSendAttemptDate"", CURRENT_DATE) OR
		            NEW.""EmailStatusId"" <> OLD.""EmailStatusId"") THEN
					INSERT INTO ""EmailLogHistory""(""EmailLogId"", ""CreatedAt"", ""ProcessFromDate"", ""LastEmailSendAttemptDate"", ""EmailStatusId"")
					VALUES(NEW.""Id"", current_timestamp, NEW.""ProcessFromDate"", NEW.""LastEmailSendAttemptDate"", NEW.""EmailStatusId"");
	            END IF;

	            RETURN NEW;
            END;
            $$", true);

            migrationBuilder.Sql(
            @"CREATE TRIGGER TR_EmailLog_AfterInsert
              AFTER INSERT
              ON ""EmailLog""
              FOR EACH ROW
              EXECUTE PROCEDURE fn_email_log_history_on_insert();", true);

            migrationBuilder.Sql(
            @"CREATE TRIGGER TR_EmailLog_AfterUpdate
              AFTER UPDATE
              ON ""EmailLog""
              FOR EACH ROW
              EXECUTE PROCEDURE fn_email_log_history_on_update();", true);
            //MANUAL CHANGES - END

            migrationBuilder.InsertData(
                table: "EmailLog",
                columns: new[] { "Id", "ClientReferenceNumber", "CreatedDate", "EmailAddress", "EmailAddressUsedForTesting", "EmailStatusId", "EmailTemplateShortName", "FinishProcessingDate", "LastEmailSendAttemptDate", "LastStatusChangedDate", "ProcessFromDate" },
                values: new object[] { 1, "historical_emails_when_log_implemented", new DateTime(2023, 4, 21, 11, 20, 34, 309, DateTimeKind.Utc).AddTicks(4253), "historical_emails_when_log_implemented", null, 7, "historical_emails_when_log_implemented", new DateTime(2023, 4, 21, 11, 20, 34, 309, DateTimeKind.Utc).AddTicks(4253), null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_TuitionPartnerEnquirySubmittedEmailL~",
                table: "TuitionPartnersEnquiry",
                column: "TuitionPartnerEnquirySubmittedEmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_EnquirerResponseEmailLogId",
                table: "EnquiryResponses",
                column: "EnquirerResponseEmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses",
                column: "TuitionPartnerResponseEmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries",
                column: "EnquirerEnquirySubmittedEmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ClientReferenceNumber",
                table: "EmailLog",
                column: "ClientReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_EmailStatusId",
                table: "EmailLog",
                column: "EmailStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_FinishProcessingDate",
                table: "EmailLog",
                column: "FinishProcessingDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_LastEmailSendAttemptDate",
                table: "EmailLog",
                column: "LastEmailSendAttemptDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ProcessFromDate",
                table: "EmailLog",
                column: "ProcessFromDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogHistory_EmailLogId",
                table: "EmailLogHistory",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifyResponseLog_EmailLogId",
                table: "EmailNotifyResponseLog",
                column: "EmailLogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifyResponseLog_NotifyId",
                table: "EmailNotifyResponseLog",
                column: "NotifyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailPersonalisationLog_EmailLogId_Key",
                table: "EmailPersonalisationLog",
                columns: new[] { "EmailLogId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_Status",
                table: "EmailStatus",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTriggerActivation_ActivateEmailLogId",
                table: "EmailTriggerActivation",
                column: "ActivateEmailLogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTriggerActivation_EmailLogId_ActivateEmailLogId",
                table: "EmailTriggerActivation",
                columns: new[] { "EmailLogId", "ActivateEmailLogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledProcessingInfo_ScheduleName",
                table: "ScheduledProcessingInfo",
                column: "ScheduleName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_EmailLog_EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries",
                column: "EnquirerEnquirySubmittedEmailLogId",
                principalTable: "EmailLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EmailLog_EnquirerResponseEmailLogId",
                table: "EnquiryResponses",
                column: "EnquirerResponseEmailLogId",
                principalTable: "EmailLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EmailLog_TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses",
                column: "TuitionPartnerResponseEmailLogId",
                principalTable: "EmailLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnersEnquiry_EmailLog_TuitionPartnerEnquirySubmit~",
                table: "TuitionPartnersEnquiry",
                column: "TuitionPartnerEnquirySubmittedEmailLogId",
                principalTable: "EmailLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //MANUAL CHANGES - START
            migrationBuilder.Sql("DROP TRIGGER TR_EmailLog_AfterUpdate ON \"EmailLog\";", true);
            migrationBuilder.Sql("DROP TRIGGER TR_EmailLog_AfterInsert ON \"EmailLog\";", true);
            migrationBuilder.Sql("DROP FUNCTION fn_email_log_history_on_update();", true);
            migrationBuilder.Sql("DROP FUNCTION fn_email_log_history_on_insert();", true);
            //MANUAL CHANGES - END

            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_EmailLog_EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EmailLog_EnquirerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EmailLog_TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnersEnquiry_EmailLog_TuitionPartnerEnquirySubmit~",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropTable(
                name: "EmailLogHistory");

            migrationBuilder.DropTable(
                name: "EmailNotifyResponseLog");

            migrationBuilder.DropTable(
                name: "EmailPersonalisationLog");

            migrationBuilder.DropTable(
                name: "EmailTriggerActivation");

            migrationBuilder.DropTable(
                name: "ScheduledProcessingInfo");

            migrationBuilder.DropTable(
                name: "EmailLog");

            migrationBuilder.DropTable(
                name: "EmailStatus");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnersEnquiry_TuitionPartnerEnquirySubmittedEmailL~",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_EnquirerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "TuitionPartnerEnquirySubmittedEmailLogId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropColumn(
                name: "EnquirerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "TuitionPartnerResponseEmailLogId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquirerEnquirySubmittedEmailLogId",
                table: "Enquiries");
        }
    }
}

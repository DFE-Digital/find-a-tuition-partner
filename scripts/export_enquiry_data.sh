#!/bin/bash
ENVIRONMENT=${1:-"production"}
CF_SPACE="ntp-$ENVIRONMENT"
DB_SERVICE="find-a-tuition-partner-$ENVIRONMENT-postgres-db"

DATE_DIR=$(date +%Y-%m-%d_%H%M%S)
EXPORT_DIR="./exports/$ENVIRONMENT/$DATE_DIR"

mkdir -p $EXPORT_DIR

cf target -s $CF_SPACE

cf conduit $DB_SERVICE -c '{"read_only": true}' -- psql \
    -c '\COPY ( 
					SELECT 
						"SupportReferenceNumber" AS "Reference",
						"Schools"."EstablishmentName" AS "School Name",
						"Schools"."Urn" AS "School URN",
						"Postcode",
						"LocalAuthorityDistrict" AS "Local Authority District",
						(
							SELECT STRING_AGG("KeyStage"."Name" || '\'': '\'' || "Subjects"."Name", '\''; '\'' ORDER BY "KeyStage"."Id", "Subjects"."Name")
							FROM "KeyStageSubjectsEnquiry"
								INNER JOIN "Subjects" ON "KeyStageSubjectsEnquiry"."SubjectId" = "Subjects"."Id"
								INNER JOIN "KeyStage" ON "KeyStageSubjectsEnquiry"."KeyStageId" = "KeyStage"."Id"
							WHERE "Enquiries"."Id" = "KeyStageSubjectsEnquiry"."EnquiryId"
							GROUP BY "Enquiries"."Id"
						) AS "Key Stage And Subjects",
						CASE (
								SELECT count(*)
								FROM "EnquiryTuitionSetting"
								WHERE "Enquiries"."Id" = "EnquiryTuitionSetting"."EnquiriesId"
							)
							WHEN 0
							THEN '\''No preference'\''
							WHEN 1
							THEN (
								SELECT "TuitionSettings"."Name"
								FROM "EnquiryTuitionSetting" 
									INNER JOIN "TuitionSettings" ON "TuitionSettings"."Id" = "EnquiryTuitionSetting"."TuitionSettingsId"
								WHERE "Enquiries"."Id" = "EnquiryTuitionSetting"."EnquiriesId"
							)
							ELSE '\''Both face-to-face and online'\''
						END AS "Tuition Setting",
						"TutoringLogistics" AS "Tuition Plan - Full",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''NumberOfPupils'\'')::text) 
							ELSE NULL 
						END AS "Number Of Pupils (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''StartDate'\'')::text) 
							ELSE NULL 
						END AS "Start Date (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''TuitionDuration'\'')::text) 
							ELSE NULL 
						END AS "Tuition Duration (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''TimeOfDay'\'')::text) 
							ELSE NULL 
						END AS "Time Of Day (Tuition Plan)",
						"SENDRequirements" AS "SEND and additional requirements",
						"AdditionalInformation" AS "Other tuition requirements",
						"CreatedAt" AS "Enquiry Created At",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
						) AS "Number Of TPs Enquiry Sent To",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "TuitionPartnerDeclinedEnquiry" = true
						) AS "Number Of TPs Declined To Respond",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponseId" IS NOT NULL
						) AS "Number Of TP Responses",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
								INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponses"."EnquiryResponseStatusId" = 4
						) AS "Number Of Responses With Status - Not Set",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
								INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponses"."EnquiryResponseStatusId" = 3
						) AS "Number Of Responses With Status - Unread",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
								INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponses"."EnquiryResponseStatusId" = 2
						) AS "Number Of Responses With Status - Undecided",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
								INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponses"."EnquiryResponseStatusId" = 1
						) AS "Number Of Responses With Status - Interested",
						(
							SELECT count(*)
							FROM "TuitionPartnersEnquiry"
								INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
							WHERE "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
								AND "EnquiryResponses"."EnquiryResponseStatusId" = 5
						) AS "Number Of Responses With Status - Rejected",
						"ClientReferenceNumber" AS "Notify Ref - To Enquirer - Enquiry Submitted",
						"EmailStatus"."Status" AS "Email Status - To Enquirer - Enquiry Submitted",
						"EmailStatus"."Description" AS "Email Status Description - To Enquirer - Enquiry Submitted"
					FROM "Enquiries" 
						LEFT JOIN "Schools" ON "Schools"."Id" = "Enquiries"."SchoolId"
						LEFT JOIN "EmailLog" ON "EmailLog"."Id" = "Enquiries"."EnquirerEnquirySubmittedEmailLogId"
						LEFT JOIN "EmailStatus" ON "EmailStatus"."Id" = "EmailLog"."EmailStatusId"
					ORDER BY "CreatedAt" ASC
        )
        TO '\'$EXPORT_DIR'/enquiries.csv'\''
        WITH ( FORMAT CSV, HEADER );' \
    \
    -c '\COPY ( 
					SELECT 
						"Enquiries"."SupportReferenceNumber" AS "Reference",
						"TuitionPartners"."Name" AS "Tuition Partner Name",
						(
							SELECT STRING_AGG("KeyStage"."Name" || '\'': '\'' || "Subjects"."Name", '\''; '\'' ORDER BY "KeyStage"."Id", "Subjects"."Name")
							FROM "KeyStageSubjectsEnquiry"
								INNER JOIN "Subjects" ON "KeyStageSubjectsEnquiry"."SubjectId" = "Subjects"."Id"
								INNER JOIN "KeyStage" ON "KeyStageSubjectsEnquiry"."KeyStageId" = "KeyStage"."Id"
							WHERE "Enquiries"."Id" = "KeyStageSubjectsEnquiry"."EnquiryId"
							GROUP BY "Enquiries"."Id"
						) AS "Enquiry - Key Stage And Subjects",
						"EnquiryResponses"."KeyStageAndSubjectsText" AS "Response - Key Stage And Subjects",
						CASE (
								SELECT count(*)
								FROM "EnquiryTuitionSetting"
								WHERE "Enquiries"."Id" = "EnquiryTuitionSetting"."EnquiriesId"
							)
							WHEN 0
							THEN '\''No preference'\''
							WHEN 1
							THEN (
								SELECT "TuitionSettings"."Name"
								FROM "EnquiryTuitionSetting" 
									INNER JOIN "TuitionSettings" ON "TuitionSettings"."Id" = "EnquiryTuitionSetting"."TuitionSettingsId"
								WHERE "Enquiries"."Id" = "EnquiryTuitionSetting"."EnquiriesId"
							)
							ELSE '\''Both face-to-face and online'\''
						END AS "Enquiry - Tuition Setting",
						"EnquiryResponses"."TuitionSettingText" AS "Response - Tuition Setting",
						"TutoringLogistics" AS "Enquiry - Tuition Plan - Full",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''NumberOfPupils'\'')::text) 
							ELSE NULL 
						END AS "Enquiry - Number Of Pupils (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''StartDate'\'')::text) 
							ELSE NULL 
						END AS "Enquiry - Start Date (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''TuitionDuration'\'')::text) 
							ELSE NULL 
						END AS "Enquiry - Tuition Duration (Tuition Plan)",
						CASE 
							WHEN "TutoringLogistics" like '\''{"NumberOfPupils"%'\'' 
							THEN trim('\''"'\'' FROM ("TutoringLogistics"::json->'\''TimeOfDay'\'')::text) 
							ELSE NULL 
						END AS "Enquiry - Time Of Day (Tuition Plan)",
						"TutoringLogisticsText" AS "Response - Tuition Pla",
						"SENDRequirements" AS "Enquiry - SEND and additional requirements",
						"SENDRequirementsText" AS "Response - SEND and additional requirements",
						"AdditionalInformation" AS "Enquiry - Other tuition requirements",
						"AdditionalInformationText" AS "Response - Other tuition requirements",
						"Enquiries"."CreatedAt" AS "Enquiry - Created At",
						"CompletedAt" AS "Response - Completed At",
						"TuitionPartnerDeclinedEnquiryDate" AS "Tuition Partner Declined Enquiry At",
						"EnquiryResponseStatus"."Status" AS "Response Status For Enquirer",
						"EnquiryResponses"."EnquiryResponseStatusLastUpdated" AS "Response Status For Enquirer Last Updated At",
						"EnquirerNotInterestedReasons"."Description" AS "Enquirer Not Interested Reason",
						"EnquiryResponses"."EnquirerNotInterestedReasonAdditionalInfo" AS "Enquirer Not Interested Reason Additional Information",
						"TuitionPartnerEnquirySubmittedEmailLog"."ClientReferenceNumber" AS "Notify Ref - To Tuition Partner - Enquiry Submitted",
						"TuitionPartnerEnquirySubmittedEmailStatus"."Status" AS "Email Status - To Tuition Partner - Enquiry Submitted",
						"TuitionPartnerEnquirySubmittedEmailStatus"."Description" AS "Email Status Description - To Tuition Partner - Enquiry Submitted",
						"TuitionPartnerResponseEmailLog"."ClientReferenceNumber" AS "Notify Ref - To Tuition Partner - Response Submitted",
						"TuitionPartnerResponseEmailStatus"."Status" AS "Email Status - To Tuition Partner - Response Submitted",
						"TuitionPartnerResponseEmailStatus"."Description" AS "Email Status Description - To Tuition Partner - Response Submitted",
						"EnquirerResponseEmailLog"."ClientReferenceNumber" AS "Notify Ref - To Enquirer - Response Submitted",
						"EnquirerResponseEmailStatus"."Status" AS "Email Status - To Enquirer - Response Submitted",
						"EnquirerResponseEmailStatus"."Description" AS "Email Status Description - To Enquirer - Response Submitted",
						"TuitionPartnerResponseNotInterestedEmailLog"."ClientReferenceNumber" AS "Notify Ref - To Tuition Partner - Enquirer Not Interested",
						"TuitionPartnerResponseNotInterestedEmailStatus"."Status" AS "Email Status - To Tuition Partner - Enquirer Not Interested",
						"TuitionPartnerResponseNotInterestedEmailStatus"."Description" AS "Email Status Description - To Tuition Partner - Enquirer Not Interested"
					FROM "Enquiries"
						INNER JOIN "TuitionPartnersEnquiry" ON "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
						INNER JOIN "TuitionPartners" ON "TuitionPartnersEnquiry"."TuitionPartnerId" = "TuitionPartners"."Id"
						LEFT JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
						LEFT JOIN "EnquiryResponseStatus" ON "EnquiryResponseStatus"."Id" = "EnquiryResponses"."EnquiryResponseStatusId"
						LEFT JOIN "EnquirerNotInterestedReasons" ON "EnquirerNotInterestedReasons"."Id" = "EnquiryResponses"."EnquirerNotInterestedReasonId"
						LEFT JOIN "EmailLog" AS "TuitionPartnerEnquirySubmittedEmailLog" ON "TuitionPartnerEnquirySubmittedEmailLog"."Id" = "TuitionPartnersEnquiry"."TuitionPartnerEnquirySubmittedEmailLogId"
						LEFT JOIN "EmailStatus" AS "TuitionPartnerEnquirySubmittedEmailStatus" ON "TuitionPartnerEnquirySubmittedEmailStatus"."Id" = "TuitionPartnerEnquirySubmittedEmailLog"."EmailStatusId"
						LEFT JOIN "EmailLog" AS "TuitionPartnerResponseEmailLog" ON "TuitionPartnerResponseEmailLog"."Id" = "EnquiryResponses"."TuitionPartnerResponseEmailLogId"
						LEFT JOIN "EmailStatus" AS "TuitionPartnerResponseEmailStatus" ON "TuitionPartnerResponseEmailStatus"."Id" = "TuitionPartnerResponseEmailLog"."EmailStatusId"
						LEFT JOIN "EmailLog" AS "EnquirerResponseEmailLog" ON "EnquirerResponseEmailLog"."Id" = "EnquiryResponses"."EnquirerResponseEmailLogId"
						LEFT JOIN "EmailStatus" AS "EnquirerResponseEmailStatus" ON "EnquirerResponseEmailStatus"."Id" = "EnquirerResponseEmailLog"."EmailStatusId"
						LEFT JOIN "EmailLog" AS "TuitionPartnerResponseNotInterestedEmailLog" ON "TuitionPartnerResponseNotInterestedEmailLog"."Id" = "EnquiryResponses"."TuitionPartnerResponseNotInterestedEmailLogId"
						LEFT JOIN "EmailStatus" AS "TuitionPartnerResponseNotInterestedEmailStatus" ON "TuitionPartnerResponseNotInterestedEmailStatus"."Id" = "TuitionPartnerResponseNotInterestedEmailLog"."EmailStatusId"
					--Commented out, so includes the details of all the TPs the enquiry was sent to
					--WHERE "EnquiryResponses"."Id" IS NOT NULL OR "TuitionPartnersEnquiry"."TuitionPartnerDeclinedEnquiry" = true
					ORDER BY 
						"Enquiries"."CreatedAt" ASC, 
						COALESCE ("EnquiryResponses"."CompletedAt", "TuitionPartnersEnquiry"."TuitionPartnerDeclinedEnquiryDate") ASC,
						"TuitionPartners"."Name"
        )
        TO '\'$EXPORT_DIR'/responses.csv'\''
        WITH ( FORMAT CSV, HEADER );'

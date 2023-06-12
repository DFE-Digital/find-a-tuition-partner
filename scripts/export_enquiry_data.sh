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
            SELECT "Enquiries"."SupportReferenceNumber" AS "Reference",
            "TutoringLogistics",
            "SENDRequirements",
            "AdditionalInformation",
            "TuitionSettings"."Name" AS "TuitionSetting",
            "CreatedAt"
            FROM "Enquiries" 
            LEFT JOIN "EnquiryTuitionSetting" ON "Enquiries"."Id" = "EnquiryTuitionSetting"."EnquiriesId"
            LEFT JOIN "TuitionSettings" ON "TuitionSettings"."Id" = "EnquiryTuitionSetting"."TuitionSettingsId"
            ORDER BY "CreatedAt" ASC 
        )
        TO '\'$EXPORT_DIR'/enquiries.csv'\''
        WITH ( FORMAT CSV, HEADER );' \
    \
    -c '\COPY ( 
        SELECT "Enquiries"."SupportReferenceNumber" AS "Reference",
            "TuitionPartners"."Name" AS "TuitionPartner",
            "EnquiryResponses"."KeyStageAndSubjectsText" AS "KeyStageAndSubjects",
            "EnquiryResponses"."TuitionSettingText" AS "TuitionSetting",
            "EnquiryResponses"."TutoringLogisticsText" AS "TutoringLogistics",
            "EnquiryResponses"."SENDRequirementsText" AS "SENDRequirements",
            "EnquiryResponses"."AdditionalInformationText" AS "AdditionalInformation",
            "EnquiryResponses"."CompletedAt"
            FROM "Enquiries"
            INNER JOIN "TuitionPartnersEnquiry" ON "Enquiries"."Id" = "TuitionPartnersEnquiry"."EnquiryId"
            INNER JOIN "TuitionPartners" ON "TuitionPartnersEnquiry"."TuitionPartnerId" = "TuitionPartners"."Id"
            INNER JOIN "EnquiryResponses" ON "TuitionPartnersEnquiry"."EnquiryResponseId" = "EnquiryResponses"."Id"
            ORDER BY "Enquiries"."CreatedAt" ASC, "EnquiryResponses"."CompletedAt" ASC
        )
        TO '\'$EXPORT_DIR'/responses.csv'\''
        WITH ( FORMAT CSV, HEADER );'

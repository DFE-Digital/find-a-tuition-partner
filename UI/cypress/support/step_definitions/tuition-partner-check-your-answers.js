import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given("Tuition partner has journeyed to the check your answers page", () => {
  cy.visit(
    "/enquiry/respond/response?token=YbyXWr3a39wY7Ah7atZ0%2BlGuYsqx21KDv6E4%2BMhPpnH1vv6kT9tbrbb/dbwcBTVwSf3v6R%2BzlSTwgogSK0Ab%2BhHyegPFXppNgqZVWCgzkQ5kqDq6lmBCioHQJT5Ds1u21Ei/s346p3oKGCn6NFxRWQ=="
  );
  cy.get("#Data_KeyStageAndSubjectsText").clear().invoke("val", 80);
  cy.get("#Data_TuitionTypeText").clear().invoke("val", 80);
  cy.get("#Data_TutoringLogisticsText").clear().invoke("val", 80);
  cy.get("#Data_SENDRequirementsText").clear().invoke("val", 80);
  cy.get("#Data_AdditionalInformationText").clear().invoke("val", 80);
  cy.get('[data-testid="call-to-action"]').click();
});

Then("the heading of the page is {string}", (heading) => {
  cy.get(".govuk-heading-l > span").should("contain.text", heading);
});

Then(
  "the Tuition Partner Check Your Answers page displays the following:",
  (dataTable) => {
    dataTable.hashes().forEach((row) => {
      switch (row["Section Name"]) {
        case "Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths":
          cy.checkTextContent(
            ":nth-child(1) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Type type:":
          cy.checkTextContent(
            ":nth-child(2) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Tuition plan:":
          cy.checkTextContent(
            ":nth-child(3) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "SEND requirements:":
          cy.checkTextContent(
            ":nth-child(4) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Other school considerations:":
          cy.checkTextContent(
            ":nth-child(5) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        default:
          throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
      }
    });
  }
);

When("they click the contact us link", () => {
  cy.get('[data-testid="contact-us-link"]').click();
});

Then("the page has title {string}", (Title) => {
  cy.get(".govuk-panel__title").should("contain.text", Title);
});

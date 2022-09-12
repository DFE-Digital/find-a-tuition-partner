import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

When("the link ‘read our guidance’ is selected", () => {
  cy.get('[data-testid="report-issues-link"]').click();
});

Then("they will see the back link", () => {
  cy.get('[data-testid="back-link"]').should("have.attr", "href", "");
});

Then("they will see link to tutoring mail address", () => {
  cy.get('[data-testid="mailto-ntp-link"]').should(
    "have.attr",
    "href",
    "mailto:tutoring.support@service.education.gov.uk"
  );
});

Then("they will not see contact us link", () => {
  cy.get('[data-testid="contact-us-link"]').should("not.exist");
});

Then("they will see feedback form link", () => {
  cy.get('[data-testid="feedback-form-link"]').should(
    "have.attr",
    "href",
    "https://forms.gle/44KfQyg1YUidrDCi7"
  );
});

Then("the feedback form link opens in a new window", () => {
  cy.get('[data-testid="feedback-form-link"]').should(
    "have.attr",
    "target",
    "_blank"
  );
});

Then("they will be taken to the 'Report issues' page", () => {
  cy.location("pathname").should("eq", "/report-issues");
});

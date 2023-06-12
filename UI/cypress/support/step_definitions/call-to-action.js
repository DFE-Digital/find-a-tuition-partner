import { When, Then } from "@badeball/cypress-cucumber-preprocessor";

When("they click the {string} service name link", (serviceName) => {
  cy.get('[data-testid="service-name-link"]')
    .should("contain.text", serviceName)
    .click();
});

When("they click 'Back'", () => {
  cy.get(".govuk-back-link").click();
});

When("they click 'Home'", () => {
  cy.get('[data-testid="home-link"]').click();
});

When("they click 'Continue'", () => {
  cy.get('[data-testid="call-to-action"]').click();
});

When("they click 'Start now'", () => {
  cy.get(".enquire-nudge > .govuk-button").click();
});

When("they click 'Search'", () => {
  cy.get('[data-testid="call-to-action"]').click();
});

Then("the 'Back' link is not displayed", () => {
  cy.get(".govuk-back-link").should("not.exist");
});

Then("the back link's text is {string}", ($text) => {
  cy.get(".govuk-back-link").should("contain.text", $text);
});

When("they click 'Continue' through enquiry", () => {
  cy.get(".app-print-hide > .govuk-button").click();
});

When("they click back on the browser", () => {
  cy.go("back");
});

When("they click send enquiry", () => {
  cy.get(".govuk-grid-column-full > .govuk-button").click();
});

When("they click 'Submit'", () => {
  cy.get("form > .govuk-button").click();
});

When("they click continue the on guidance page", () => {
  cy.get(".govuk-grid-column-two-thirds > .govuk-button").click();
});

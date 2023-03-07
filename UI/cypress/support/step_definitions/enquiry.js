import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Then("they click 'Continue' on enquiry", () => {
  cy.get(".app-print-hide > .govuk-button").click();
});

Then("they enter an invalid email address", () => {
  cy.get('[data-testid="enquirer-email-input-box"]').type("email.email.com");
});

Then("they enter an valid email address", () => {
  cy.get('[data-testid="enquirer-email-input-box"]').type("email@email.com");
});

Then("the email address is visible in input field", () => {
  cy.get('[data-testid="enquirer-email-input-box"]').should(
    "have.value",
    "email@email.com"
  );
});

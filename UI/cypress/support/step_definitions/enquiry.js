import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Then("they click 'Continue' on enquiry", () => {
  cy.get(".app-print-hide > .govuk-button").click();
});

Then("they enter an invalid email address", () => {
  cy.get("#Data_Email").type("email.email.com");
});

Then("they enter an valid email address", () => {
  cy.get("#Data_Email").type("email@email.com");
});

Then("the email address is visible in input field", () => {
  cy.get("#Data_Email").should("have.value", "email@email.com");
});

Then("click the link on text {string}", (linkText) => {
  cy.get(".govuk-details__summary-text").should("contain.text", linkText);
});

Then(
  "they will see the correct error message for an invalid email address",
  () => {
    cy.get(".govuk-error-summary__body").should(
      "contain.text",
      "You must enter an email address in the correct format.  Emails are usually in a format, like, username@example.com"
    );
  }
);

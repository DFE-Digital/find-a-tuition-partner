import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given("User navigates to the email address page", () => {
  Step(
    this,
    "a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'"
  );
  Step(this, "they click 'Start now'");
  Step(this, "they click 'Continue' on enquiry");
  Step(this, "they will be taken to the single school selection page");
  Step(this, "the user clicks yes and continue");
  Step(this, "user is redirected to the enter email address page");
});

Then("their is an input field for the verification code", () => {
  cy.get(".govuk-form-group").should("contain.text", "Enter passcode");
});

Then("a {string} link is displayed", (text) => {
  cy.get(".govuk-details__summary-text").should("contain.text", text);
});

Then("they enter a valid passcode", () => {
  cy.get("form > :nth-child(14)").then(($code) => {
    const codeOnly = parseInt($code.text().replace("Code:", ""));
    cy.get("#Data_Passcode").type(codeOnly);
  });
});

Then("they enter the valid passcode", () => {
  cy.get("form > :nth-child(18)").then(($code) => {
    const codeOnly = parseInt($code.text().replace("Code:", ""));
    cy.get("#Data_Passcode").type(codeOnly);
  });
});

Then("they enter the valid passcode on mobile", () => {
  cy.get("form > :nth-child(12)").then(($code) => {
    const codeOnly = parseInt($code.text().replace("Code:", ""));
    cy.get("#Data_Passcode").type(codeOnly);
  });
});

When("they enter a passcode with letters and symbols", () => {
  cy.get("#Data_Passcode").type("1234a@");
});

Then("they enter an invalid passcode", () => {
  cy.get("#Data_Passcode").type("123456");
});

Then("they click I have not received my passcode", () => {
  cy.get(".govuk-details__summary-text").click();
});

Then("a drop down menu is displayed with a request new passcode button", () => {
  cy.get(".govuk-details__text").should("be.visible");
});

Then("they click request new passcode", () => {
  cy.get(".govuk-details__text > .govuk-button").click();
});

Then(" a success message is displayed with the date and time", () => {
  cy.get(".govuk-notification-banner__heading").should(
    "contain",
    "We have sent you a new passcode"
  );
});

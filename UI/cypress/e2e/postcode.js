import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has tried to continue without entering a postcode", () => {
  Step(this, "a user has started the 'Find a tuition partner' journey");
  Step(this, "they click 'Continue'");
});

When("they click 'What is a quality assured tuition partner?'", () => {
  cy.get('[data-testid="qatp-details"]').click();
});

When("they enter {string} as the school's postcode", (postcode) => {
  cy.get('input[name="Data.Postcode').type(postcode);
});

When("they click on the postcode error", () => {
  cy.get('a[href="#Data_Postcode"]').click();
});

Then("the quality assured tuition partner details are hidden", () => {
  cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("the quality assured tuition partner details are shown", () => {
  cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
});

Then("the school's postcode text input is focused", () => {
  cy.focused().should("have.attr", "name", "Data.Postcode");
});
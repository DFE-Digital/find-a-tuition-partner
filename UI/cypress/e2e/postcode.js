import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has tried to continue without entering a postcode", () => {
  Step(this, "a user has started the 'Find a tuition partner' journey");
  Step(this, "they click 'Continue'");
});

When("they click 'What is a quality assured tuition partner?'", () => {
  cy.get('[data-testid="qatp-details"]').click();
});

Then("the quality assured tuition partner details are hidden", () => {
  cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("the quality assured tuition partner details are shown", () => {
  cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
});

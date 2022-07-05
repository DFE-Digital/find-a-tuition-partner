import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the homepage", () => {
  cy.visit("/");
});

When("a user visits the homepage", () => {
  Step(this, "a user has arrived on the homepage");
});

Then("the heading should say {string}", (heading) => {
  cy.get("h1").should("have.text", heading);
});

When("they click 'Start now'", () => {
  cy.get('[data-testid="call-to-action"]').click();
});
import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Then("they will see the phase banner", () => {
  cy.get('[data-testid="phase-banner"]').should("exist");
});

Then("the current phase is {string}", (phase) => {
  cy.get('[data-testid="phase-banner"]').should("contain.text", phase);
});

Then(
  "the phase banner feedback link {string} links to the feedback page",
  (text) => {
    cy.get('[data-testid="phase-banner-feedback-link"]')
      .should("contain.text", text)
      .should("have.attr", "href")
      .and("match", /^\/feedback\?FromReturnUrl=.+$/);
  }
);

import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the funding and reporting page", () => {
  cy.visit(`/funding-and-reporting`);
});

Then("they will see the funding and report header", () => {
  cy.get('[data-testid="funding-reporting-header"]').should(
    "contain.text",
    "Funding and Reporting"
  );
});

Then("they will see the back link", () => {
  cy.get('[data-testid="back-link"]').should("have.attr", "href", "");
});

Then("the academic mentor tutoring rates details are hidden", () => {
  cy.get('[data-testid="academic-mentor-tutoring-rates-details-link"]').should(
    "not.have.attr",
    "open"
  );
});

When("they click academic mentor tutoring rates", () => {
  cy.get('[data-testid="academic-mentor-tutoring-rates-details"]').click();
});

Then("the academic mentor tutoring rates are shown", () => {
  cy.get('[data-testid="academic-mentor-tutoring-rates-details-link"]').should(
    "have.attr",
    "open"
  );
});

Then("they will see example 1 rates in moj ticket panel", () => {
  cy.get('[data-testid="inset-text-example1"]').should(
    "have.class",
    "moj-ticket-panel"
  );
});

import {Given, Then} from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the {string} website", (name) => {
    cy.visit(`/`);
});

Then("they should see {string} page links at the footer", (numberOfLinks) => {
    cy.get('[data-testid="govuk-footer-link"]').children().should('have.length', numberOfLinks);
});
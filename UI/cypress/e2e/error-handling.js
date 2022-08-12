
import { When, Then } from "@badeball/cypress-cucumber-preprocessor";

When("a service page has not been found", () => {
    cy.request({url: '/page-does-not-exist', failOnStatusCode: false}).its('status').should('equal', 404)
    cy.visit('/page-does-not-exist', {failOnStatusCode: false})
});

Then("the 'page not found' error page is displayed", () => {
    cy.get('[data-testid="error-page"').should('exist');
});
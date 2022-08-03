import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has arrived on the ", () => {
    cy.visit(`/accessibility`);
});

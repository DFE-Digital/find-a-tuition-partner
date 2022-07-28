import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Then("they see the support link at the bottom of the page", () => {
    cy.get('[data-testid="support-link"]').should("exist");
});

Then("the support link will open a new email to {string}", emailAddress => {
    cy.get('[data-testid="support-link"]').within(() => {
        cy.get('a')
            .should("contain.text", emailAddress)
            .and("have.attr", 'href', `mailto:${emailAddress}`)
    });
});

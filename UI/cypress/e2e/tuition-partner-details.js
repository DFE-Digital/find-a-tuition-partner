import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the 'Tuition Partner' page for {string}", name => {
    cy.visit(`/tuition-partner/${name}`);
});

Then("TP has not provided the information in the {string} section", details => {
    cy.get('[data-testid="contact-details"]').should('not.contain.text', details, { matchCase: true });
});

Then("TP has provided full contact details", () => {
    cy.get('[data-testid="contact-details"]').should('contain.text', 'Website', { matchCase: true })
    .and('contain.text', 'Phone number', { matchCase: true })
    .and('contain.text', 'Email address', { matchCase: true })
    .and('contain.text', 'Address', { matchCase: true });
});


Then("the tuition partner locations covered table is not displayed", () => {
    cy.get('[data-testid="locations-covered-table"]')
        .should('not.exist');
});

Then("the tuition partner locations covered table is displayed", () => {
    cy.get('[data-testid="locations-covered-table"]')
        .should('exist');
});

Then("the tuition partner pricing table is not displayed", () => {
    cy.get('[data-testid="pricing-table"]')
        .should('not.exist');
});

Then("the tuition partner pricing table is displayed", () => {
    cy.get('[data-testid="pricing-table"]')
        .should('exist');
});

Then("the tuition partner full pricing tables are not displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(`[data-testid="full-pricing-table-in-school-key-stage-${i}"]`)
            .should('not.exist');
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`)
            .should('not.exist');
    }    
});

Then("the tuition partner full pricing tables are displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(`[data-testid="full-pricing-table-in-school-key-stage-${i}"]`)
            .should('exist');
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`)
            .should('exist');
    }
});
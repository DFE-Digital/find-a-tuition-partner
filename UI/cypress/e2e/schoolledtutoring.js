import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the school led page", () => {
    cy.visit(`/school-led-tutoring`);
});

Then("they will see the school led header", () => {
    cy.get('[data-testid="school-led-header"]').should('contain.text', "Employ your own tutor")
});

Then("they will see the dbs check link", () => {
    cy.get('[data-testid="dbs-check-link"]').should('have.attr', 'href', 'https://www.gov.uk/find-out-dbs-check/y/caring-for-or-working-with-children-under-18-or-working-in-a-school/teaching-or-caring-for-children/working-in-a-school-nursery-children-s-centre-or-home-detention-service-young-offender-institution-or-childcare-premises/yes')
});

Then("they will see the funding allocation link", () => {
    cy.get('[data-testid="funding-allocation-link"]').should('have.attr', 'href', '/funding-and-reporting')
});

Then("they will see the home link", () => {
    cy.get('[data-testid="home-link"]').should('have.attr', 'href', '/')
});
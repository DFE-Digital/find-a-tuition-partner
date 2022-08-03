import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has arrived on the academic mentors page", () => {
    cy.visit(`/academic-mentors`);
});

Then("they will see the academic mentor header", () => {
    cy.get('[data-testid="academic-mentors-header"]').should('contain.text', "Employ an academic mentor")
});

Then("they will see the login link", () => {
    cy.get('[data-testid="login-link"]').should('have.attr', 'href', 'https://tuitionhub.nationaltutoring.org.uk/NTP/s/login/')
});

Then("they will see the funding allocation link", () => {
    cy.get('[data-testid="funding-allocation-link"]').should('have.attr', 'href', '/funding-and-reporting')
});

Then("they will see the home link", () => {
    cy.get('[data-testid="home-link"]').should('have.attr', 'href', '/')
});

Then("they will see the dbs check link", () => {
    cy.get('[data-testid="dbs-check-link"]').should('have.attr', 'href', 'https://www.gov.uk/find-out-dbs-check/y/caring-for-or-working-with-children-under-18-or-working-in-a-school/teaching-or-caring-for-children/working-in-a-school-nursery-children-s-centre-or-home-detention-service-young-offender-institution-or-childcare-premises/yes')
});
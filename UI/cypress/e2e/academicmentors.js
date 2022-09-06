import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the academic mentors page", () => {
    cy.visit(`/academic-mentors`);
});

Then("they will see the academic mentor header", () => {
    cy.get('[data-testid="academic-mentors-header"]').should('contain.text', "Employ an academic mentor")
});

Then("they will see the book training link", () => {
    cy.get('[data-testid="book-training"]').should('have.attr', 'href', 'http://nominations.tutortraining.co.uk/registration')
});

Then("the book training link opens in a new window", () => {
    cy.get('[data-testid="book-training"]')
        .should('have.attr', 'target', '_blank')
        .invoke('attr', 'href')
        .then(($href) => {
            cy.request($href).then((resp) => {
                expect(resp.status).to.eq(200)
              })});
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


Then("the dbs check link opens in a new window", () => {
    cy.get('[data-testid="dbs-check-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see the funding and reporting link", () => {
    cy.get('[data-testid="funding-reporting-link"]').should('have.attr', 'href', '/funding-and-reporting')
});

When("they click funding and reporting link", () => {
    cy.get('[data-testid="funding-reporting-link"]').click();
});

Then("they will see the funding reporting header", () => {
    cy.get('[data-testid="funding-reporting-header"]').should('contain.text', "Funding and Reporting")
});

Then("they will click the back link", () => {
    cy.get('[data-testid="back-link"]').click();
});

Then("they redirects to academic mentors page", () => {
    cy.location('pathname').should('eq', '/academic-mentors');
});
import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the contact us page", () => {
    cy.visit(`/Contactus`);
});

Then("they will see the back link", () => {
    cy.get('[data-testid="back-link"]').should('have.attr', 'href', '')
});

Then("they will see the contact us header", () => {
    cy.get('[data-testid="contact-us-header"]').should('contain.text', "Contact us")
});

Then("they will see link to tutoring mail address", () => {
    cy.get('[data-testid="mailto-ntp-link"]').should('have.attr', 'href', 'mailto:tutoring.support@service.education.gov.uk')
});

Then("they will not see contact us link", () => {
    cy.get('[data-testid="contact-us-link"]').should('not.exist');
});




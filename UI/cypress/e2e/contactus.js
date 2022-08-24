import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the contact us page", () => {
    cy.visit(`/contact-us`);
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

Then("they will see read our guidance link", () => {
    cy.get('[data-testid="read-guidance-link"]').should('have.attr', 'href', 'https://www.gov.uk/government/publications/national-tutoring-programme-guidance-for-schools-2022-to-2023')
});

Then("the read our guidance link opens in a new window", () => {
    cy.get('[data-testid="read-guidance-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see feedback form link", () => {
    cy.get('[data-testid="feedback-form-link"]').should('have.attr', 'href', 'https://forms.gle/44KfQyg1YUidrDCi7')
});

Then("the feedback form link opens in a new window", () => {
    cy.get('[data-testid="feedback-form-link"]').should('have.attr', 'target', '_blank')
});


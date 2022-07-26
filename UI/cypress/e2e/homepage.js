import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";


Then("check other tutoring options text is displayed", () => {
    cy.get('[data-testid="other-tutoring-options"]').should('contain.text', 'Other tutoring options')
});


Then("the other options academic links to {string}", (href) => {
    cy.get('[data-testid="academic-mentors-link"]')
        .should('have.attr', 'href', href)
});


Then("the other options school-led tutoring links to {string}", (href) => {
    cy.get('[data-testid="school-led-tutoring-link"]')
        .should('have.attr', 'href', href)
});
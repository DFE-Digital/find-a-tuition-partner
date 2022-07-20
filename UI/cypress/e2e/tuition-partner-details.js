import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the 'Tuition Partner' page for {string}", name => {
    cy.visit(`/tuition-partner/${name}`);
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after entering search details", name => {
    cy.visit(`/search-results?Data.Subjects=KeyStage1-English&Data.TuitionType=Any&Data.Postcode=sk11eb`);
    cy.get('.govuk-link').contains(name).click();
});

When("the home page is selected", () => {
    cy.get('[data-testid="home-link"]').click();
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

Then("the search details are correct", () => {
    cy.location('search').should('eq', '?Postcode=sk11eb&TuitionType=Any&Subjects=KeyStage1-English');
});





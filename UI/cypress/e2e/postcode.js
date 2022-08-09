import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has tried to continue without entering a postcode", () => {
  Step(this, "a user has started the 'Find a tuition partner' journey");
  Step(this, "they click 'Continue'");
});

When("they click 'What is a quality-assured tuition partner?'", () => {
  cy.get('[data-testid="qatp-details"]').click();
});

Then("the quality assured tuition partner details are hidden", () => {
  cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("they will see phase banner", () => {
    cy.get('[data-testid="phase-banner-title"]').should('contain.text', "private beta")
});

Then("the user should redirected to feedback form", () => {
    cy.get('[data-testid="feedback-link"]').should('have.attr', 'href', 'https://docs.google.com/document/d/1Iybtc7c9IVMVNUE2Hkj85WF8csEIdhD6XNZ0hd4ozOs/edit')
});

Then("the quality assured tuition partner details are shown", () => {
  cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
});

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

Then("the accessibility link {string} links to {string}", (text, href) => {
    cy.get('[data-testid="accessibility-link"]')
        .should('contain.text', text)
        .should('have.attr', 'href', href)
});

Then("the accessibility link opens in a new window", () => {
    cy.get('[data-testid="accessibility-link"]').should('have.attr', 'target', '_blank')
});

Then("the privacy link opens privacy page", () => {
    cy.get('[data-testid="privacy-link"]')
        .should('have.attr', 'href', '/privacy')
});
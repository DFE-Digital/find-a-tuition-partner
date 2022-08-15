import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the privacy page", () => {
    cy.visit(`/privacy`);
});

Then("they will see the privacy notice header", () => {
    cy.get('[data-testid="privacy-header"]').should('contain.text', "Find a tuition partner privacy notice")
});

Then("they will see personal information link", () => {
    cy.get('[data-testid="personal-information-link"]').should('have.attr', 'href', 'https://www.gov.uk/government/organisations/department-for-education/about/personal-information-charter')
});

Then("the personal information link opens in a new window", () => {
    cy.get('[data-testid="personal-information-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see contact form link", () => {
    cy.get('[data-testid="contact-form-link"]').should('have.attr', 'href', 'https://form.education.gov.uk/en/AchieveForms/?form_uri=sandbox-publish://AF-Process-f1453496-7d8a-463f-9f33-1da2ac47ed76/AF-Stage-1e64d4cc-25fb-499a-a8d7-74e98203ac00/definition.json&redirectlink=%2Fen&cancelRedirectLink=%2Fen')
});

Then("the contact form link opens in a new window", () => {
    cy.get('[data-testid="contact-form-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see contact form dfe link", () => {
    cy.get('[data-testid="contact-dfe-link"]').should('have.attr', 'href', 'https://www.gov.uk/contact-dfe')
});

Then("the contact dfe form link opens in a new window", () => {
    cy.get('[data-testid="contact-dfe-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see information commissioner link", () => {
    cy.get('[data-testid="data-matters-link"]').should('have.attr', 'href', 'https://ico.org.uk/your-data-matters/')
});

Then("the contact information commissioner link opens in a new window", () => {
    cy.get('[data-testid="contact-dfe-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see contact dfe contact link", () => {
    cy.get('[data-testid="contact-link"]').should('have.attr', 'href', 'https://www.gov.uk/contact-dfe')
});

Then("the contact dfe link opens in a new window", () => {
    cy.get('[data-testid="contact-link"]').should('have.attr', 'target', '_blank')
});


Then("they will see contact secure dfe form link", () => {
    cy.get('[data-testid="contact-form-secure-link"]').should('have.attr', 'href', 'https://www.gov.uk/contact-dfe')
});


Then("the contact secure dfe link opens in a new window", () => {
    cy.get('[data-testid="contact-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see contact secure dfe online form link", () => {
    cy.get('[data-testid="contact-form-secure-online-link"]').should('have.attr', 'href', 'https://form.education.gov.uk/service/Contact-the-Department-for-Education')
});


Then("the contact secure dfe online form link opens in a new window", () => {
    cy.get('[data-testid="contact-link"]').should('have.attr', 'target', '_blank')
});

Then("they will see the home link", () => {
    cy.get('[data-testid="home-link"]').should('have.attr', 'href', '/')
});


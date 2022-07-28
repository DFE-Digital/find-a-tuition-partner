import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the accessibility page", () => {
    cy.visit(`/accessibility`);
});

Then("they will see the Accessibility statement header", () => {
    cy.get('[data-testid="accessibility-header"]').should('contain.text', "Accessibility statement")
});

Then("they will see the design priniciples link", () => {
    cy.get('[data-testid="design-principles-link"]').should('have.attr', 'href', 'https://design-system.service.gov.uk/accessibility/')
});

Then("they will see cannot access part header", () => {
    cy.get('[data-testid="access-parts-header"]').should('contain.text', "What to do if you cannot access parts of this website")
});

Then("they will see link to ntp mail address", () => {
    cy.get('[data-testid="mailto-ntp-link"]').should('have.attr', 'href', 'mailto:ntp.digital@education.gov.uk')
});

Then("they will see reporting accesibility problems header", () => {
    cy.get('[data-testid="accessibility-reporting-header"]').should('contain.text', "Reporting accessibility problems with this website")
});

Then("they will see enforcement procedure header", () => {
    cy.get('[data-testid="accessibility-enforcement-header"]').should('contain.text', "Enforcement procedure")
});

Then("they will see link to equality advisory service", () => {
    cy.get('[data-testid="contact-quality-link"]').should('have.attr', 'href', 'https://www.equalityadvisoryservice.com/')
});

Then("they will see header improve accesibility", () => {
    cy.get('[data-testid="improve-accessibility-header"]').should('contain.text', "What we’re doing to improve accessibility")
});
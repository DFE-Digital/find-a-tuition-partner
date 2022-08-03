import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the accessibility page", () => {
    cy.visit(`/accessibility`);
});

Then("they will see the accessibility statement header", () => {
    cy.get('[data-testid="accessibility-header"]').should('contain.text', "Accessibility statement for Find a tution partner")
});

Then("they will see the legislation link", () => {
    cy.get('[data-testid="legislation-link"]').should('have.attr', 'href', 'https://www.legislation.gov.uk/uksi/2018/952/contents/made')
});

Then("they will see cannot access part header", () => {
    cy.get('[data-testid="access-parts-header"]').should('contain.text', "What to do if you cannot access parts of this website")
});

Then("they will see link to tutoring mail address", () => {
    cy.get('[data-testid="mailto-ntp-link"]').should('have.attr', 'href', 'mailto:tutoring.support@service.education.gov.uk')
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


Then("they will see header accessibility technical header", () => {
    cy.get('[data-testid="accessibility-technical-information-header"]').should('contain.text', "Technical information about this website’s accessibility")
});

Then("they will see header Compliance status", () => {
    cy.get('[data-testid="compliance-status-header"]').should('contain.text', "Compliance status")
});

Then("they will see header how we tested website", () => {
    cy.get('[data-testid="tested-website-header"]').should('contain.text', "How we tested this website")
});
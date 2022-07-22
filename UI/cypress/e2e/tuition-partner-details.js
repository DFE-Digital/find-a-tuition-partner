import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage, KeyStageSubjects } from "../support/utils";

Given("a user has arrived on the 'Tuition Partner' page for {string}", name => {
    cy.visit(`/tuition-partner/${name}`);
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after searching for {string}", (name, subjects) => {

    cy.visit(`/search-results?${KeyStageSubjects(subjects)}&Data.TuitionType=Any&Data.Postcode=sk11eb`);
    cy.get('.govuk-link').contains(name).click();
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after entering search details for multiple subjects", name => {
    cy.visit(`/search-results?Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage1-Maths&Data.TuitionType=Any&Data.Postcode=sk11eb`);
    cy.get('.govuk-link').contains(name).click();
});

When("the home page is selected", () => {
    cy.get('[data-testid="home-link"]').click();
});

When("they click 'What is a quality assured tuition partner?'", () => {
    cy.get('[data-testid="qatp-details"]').click();
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

Then("the search details include {string}", subjects => {
    subjects.split(',').forEach(element => {
        cy.get(`input[id="${kebabCase(element.trim())}"]`).should('be.checked');
    });
});

Then("the quality assured tuition partner details are hidden", () => {
    cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("the payment details are hidden", () => {
    cy.get('[data-testid="payment-details"]').should("not.have.attr", "open");
});

Then("the quality assured tuition partner details are shown", () => {
    cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
  });

Then("the tuition partners website link exist", () => {
    cy.get('[data-testid=tuition-partner-website-link]').should('have.prop', 'href');
});

Then("the funding guidance page is accessible", () => {
    cy.get('[data-testid=funding-guidance-1]').then(function ($a) {
        const href = $a.prop('href');
        cy.request(href).its('body').should('include', '</html>');
    })

    cy.get('[data-testid=funding-guidance-2]').then(function ($a) {
        const href = $a.prop('href');
        cy.request(href).its('body').should('include', '</html>');
    })
});

Then("the tuition partner locations covered table is not displayed", () => {
    cy.get('[data-testid="locations-covered-table"]')
        .should('not.exist');
});

Then("the tuition partner locations covered table is displayed", () => {
    cy.get('[data-testid="locations-covered-table"]')
        .should('exist');
});

Then("the tuition partner pricing table is not displayed", () => {
    cy.get('[data-testid="pricing-table"]')
        .should('not.exist');
});

Then("the tuition partner pricing table is displayed", () => {
    cy.get('[data-testid="pricing-table"]')
        .should('exist');
});

Then("the tuition partner full pricing tables are not displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(`[data-testid="full-pricing-table-in-school-key-stage-${i}"]`)
            .should('not.exist');
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`)
            .should('not.exist');
    }    
});

Then("the tuition partner full pricing tables are displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(`[data-testid="full-pricing-table-in-school-key-stage-${i}"]`)
            .should('exist');
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`)
            .should('exist');
    }
});

Then("the subjects covered by a tuition partner are in alphabetical order", () => {
   
    const stages = ['Key stage 1 - English, Maths and Science', 
                    'Key stage 2 - English, Maths and Science', 
                    'Key stage 3 - English, Humanities, Maths, Modern Foreign Languages and Science',
                    'Key stage 4 - English, Humanities, Maths, Modern Foreign Languages and Science'];

    stages.forEach(element => {
        cy.get('.govuk-list').first().contains(element);
    });
});


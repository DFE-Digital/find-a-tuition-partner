import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, KeyStageSubjects, KeyStageSubjects2 } from "../utils";

Given("a user has arrived on the 'Search results' page for {string}", keyStage => {
    cy.visit(`/search-results?postcode=AB12CD&key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Given("a user has arrived on the 'Search results' page for {string} without a postcode", keyStage => {
    cy.visit(`/search-results?key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Given("a user has arrived on the 'Search results' page for {string} for postcode {string}", (keystages, postcode) => {
    const query = keystages.split(',').map(s => KeyStageSubjects2('subjects', s.trim())).join('&');
    cy.visit(`/search-results?Postcode=${postcode}&${query}`);
});

Given("a user has arrived on the 'Search results' page", () => {
    cy.visit(`/search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&Subjects=KeyStage2-Science&Subjects=KeyStage3-English&Subjects=KeyStage3-Humanities&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage3-Science&Subjects=KeyStage4-English&Subjects=KeyStage4-Humanities&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&Subjects=KeyStage4-Science&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4`);
});

Then("they will see the tuition partner {string}", tp => {
    cy.contains('a', tp)
});

Given("a user has arrived on the 'Tuition Partner' page for {string}", name => {
    cy.visit(`/tuition-partner/${name}`);
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after searching for {string}", (name, subjects) => {
    Step(this, `a user has arrived on the 'Tuition Partner' page for '${name}' after searching for '${subjects}' in postcode 'sk11eb'`)
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after entering search details for multiple subjects", name => {
    Step(this, `a user has arrived on the 'Tuition Partner' page for '${name}' after searching for 'Key stage 1 English, Key stage 1 Maths' in postcode 'sk11eb'`)
});

Given("a user has arrived on the 'Tuition Partner' page for {string} after searching for {string} in postcode {string}", (name, subjects, postcode) => {
    cy.visit(`/search-results?${KeyStageSubjects(subjects)}&Data.TuitionType=Any&Data.Postcode=${postcode}`);
    cy.get('.govuk-link').contains(name).click();
});

When("they select {string} tuition type", tuition => {
    cy.get(`input[id="${kebabCase(tuition)}"]`).should('be.checked');
});

When("they select the tuition partner {string}", name => {
    cy.get('.govuk-link').contains(name).click();
});

Then("they see only tuition types available for postcode 'SK1 1EB'", () => {
    cy.get("[data-testid='type-of-tuition']").invoke('text').then(text => {
        expect(text.trim()).to.eq('Online')
    });
})
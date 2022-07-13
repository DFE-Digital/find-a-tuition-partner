import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, removeWhitespace } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Maths", "Science", "Humanities", "Modern foreign languages"],
    "Key stage 4": [ "English", "Maths", "Science", "Humanities", "Modern foreign languages"]
}

Given("a user has arrived on the 'Search results' page for {string}", keyStage => {
    cy.visit(`/search-results?postcode=AB12CD&key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Given("a user has arrived on the 'Search results' page for {string} without a postcode", keyStage => {
    cy.visit(`/search-results?key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Then("they will see all the subjects for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    
    stages.forEach(element => {

        const subjects = allSubjects[element]

        cy.get(`[data-testid=${kebabCase(keystage)}]`)
        .parent().parent()
        .within(() =>
        {
            cy.get('[data-testid="subject-name"]').each((item, index) => {
                cy.wrap(item).should('contain.text', subjects[index])
            });
        });

    });
});

Then("they will see a collapsed subject filter for {string}}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    
    stages.forEach(element => {

        const subjects = allSubjects[element]

        cy.get(`[data-testid=${kebabCase(keystage)}]`)
        .parent().parent()
        .within(() =>
        {
            cy.get('[data-testid="subject-name"]').each((item, index) => {
                cy.wrap(item).should('contain.text', subjects[index])
            });
        });

    });
});

Then("they will see the tuition type {string} is selected", tutionType => {
    cy.get(`input[id="${kebabCase(tutionType)}"]`).should('be.checked');
});
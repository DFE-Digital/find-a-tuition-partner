import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, removeWhitespace } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"],
    "Key stage 4": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"]
}

Given("a user has arrived on the 'Search results' page for {string}", keyStage => {
    cy.visit(`/search-results?postcode=AB12CD&key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Given("a user has arrived on the 'Search results' page for {string} without a postcode", keyStage => {
    cy.visit(`/search-results?key-subjects=KeyStage1&subjects=KeyStage1-English`);
});

Given("a user has arrived on the 'Search results' page", () => {
    cy.visit(`/search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&Subjects=KeyStage2-Science&Subjects=KeyStage3-English&Subjects=KeyStage3-Humanities&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage3-Science&Subjects=KeyStage4-English&Subjects=KeyStage4-Humanities&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&Subjects=KeyStage4-Science&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4`);
});

When("they click on the option heading for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`#option-select-title-${kebabCase(element)}`).click()
    });
})

Then("they will see all the subjects for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    
    stages.forEach(element => {

        const subjects = allSubjects[element]

        cy.get(`[data-testid=${kebabCase(element)}]`)
            .should('not.have.class', 'js-closed')

        cy.get(`[data-testid=${kebabCase(element)}]`)
        .within(() =>
        {
            cy.get('[data-testid="subject-name"]').each((item, index) => {
                cy.wrap(item).should('contain.text', subjects[index])
            });
        });

    });
});

Then("they will see a collapsed subject filter for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`[data-testid=${kebabCase(element)}]`).
            should('have.class', 'js-closed')
    });
});

Then("they will see an expanded subject filter for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`[data-testid=${kebabCase(element)}]`).
            should('not.have.class', 'js-closed')
    });
});

Then("they will see the tuition type {string} is selected", tutionType => {
    cy.get(`input[id="${kebabCase(tutionType)}"]`).should('be.checked');
});

Then("the subjects in the filter displayed in alphabetical order", () => {
   
    const stages = ['Key stage 1', 'Key stage 2', 'Key stage 3', 'Key stage 4'];
    stages.forEach(element => {

        const subjects = allSubjects[element]

        cy.get('legend')
        .contains(`${element}`)
        .parent()
        .within(() =>
        {
            cy.get('[data-testid="subject-name"]').each((item, index) => {
                cy.wrap(item).should('contain.text', subjects[index])
            });
        });

    });
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


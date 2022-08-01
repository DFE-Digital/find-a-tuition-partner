import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, KeyStageSubjects } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"],
    "Key stage 4": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"]
}

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

Then("they will see the results summary for {string}", location => {
    var expected = new RegExp(`\\d+ results for ${location} sorted by A-Z`)
    cy.get('[data-testid="results-summary"]')
        .invoke("text").invoke("trim")
        .should('match', expected)
});

Then("show all correct tuition partners that provide tuition in the postcode's location", () => {
    cy.get('[data-testid="results-list-item"]').should("exist")
});

Then("display all correct tuition partners that provide the selected subjects in any location", () => {
    cy.get('[data-testid="results-list-item"]').should("exist")
});

Then("display all correct tuition partners in any location", () => {
    cy.get('[data-testid="results-list-item"]').should("exist")
});
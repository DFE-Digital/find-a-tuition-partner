import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, KeyStageSubjects } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"],
    "Key stage 4": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"]
}

const stages = ['Key stage 1', 'Key stage 2', 'Key stage 3', 'Key stage 4'];

When("they click on the option heading for {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`#option-select-title-${kebabCase(element)}`).click()
    });
})

When("they clear all the filters", () => {
    cy.get('[type="checkbox"]').uncheck()
})

Then("the ‘clear filters’ button as been selected", () => {
    cy.get('[data-testid="clear-all-filters"]').click();
});

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
    var expected = new RegExp(`\\d+ results for ${location}`)
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

Then("the ‘clear filters’ button is displayed at the bottom of the filters section", () => {
    cy.get('[data-testid="clear-all-filters"]').should("exist").should('contain.text', 'Clear all filters');
});

Then("no subject should be shown as selected", () => {
    cy.get('.app-results-filter').contains('text', 'checked').should('not.exist');
});

Then("all subject filters are collapsed", () => {
    stages.forEach(element => 
        cy.get(`[data-testid=${kebabCase(element)}]`).
            should('have.class', 'js-closed')
)});

Then("results are updated after filters are cleared", () => {
    let countOfElements = 0;
    cy.get('[data-testid="results-list-item"]').then($elements => {
    countOfElements = $elements.length;
    cy.get('[data-testid="clear-all-filters"]').click();
    cy.get('[data-testid="results-list-item"]').should('have.length.greaterThan', countOfElements)
    })
});

Then("the postcode search parameter remains", () => {
    cy.get('[data-testid="postcode-input-box"]').should('not.be.null');
});















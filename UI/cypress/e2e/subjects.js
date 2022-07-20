import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Maths", "Science", "Humanities", "Modern foreign languages"],
    "Key stage 4": [ "English", "Maths", "Science", "Humanities", "Modern foreign languages"]
}

function CamelCaseKeyStage(s)
{
    switch (s) {
    case 'Key stage 1': return 'KeyStage1';
    case 'Key stage 2': return 'KeyStage2';
    case 'Key stage 3': return 'KeyStage3';
    case 'Key stage 4': return 'KeyStage4';
    default: return '';
    }
}

Given("a user has arrived on the 'Which key stages' page", () => {
    Step(this, "a user has arrived on the 'Which key stages' page for postcode 'AB12CD'");
});

Given("a user has arrived on the 'Which key stages' page for postcode {string}", postcode => {
    cy.visit(`/which-key-stages?Postcode=${postcode}`);
});

Given("a user has arrived on the 'Which subjects' page for {string}", keystage => {
    const query = keystage.split(',').map(s => `KeyStages=${CamelCaseKeyStage(s.trim())}`).join('&');
    cy.visit(`/which-subjects?${query}`);
});

When("they manually navigate to the 'Which subjects' page", () => {
    cy.visit("/which-subjects");
});

When("they select {string}", keystage => {
    const stages = keystage.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`input[id=${kebabCase(element)}]`).check();
    });
});

Then("they will see all the keys stages as options", () => {
    const keyStages = Object.keys(allSubjects)

    cy.get('[data-testid="key-stage-name"]')
        .should('have.length', keyStages.length);

    cy.get('[data-testid="key-stage-name"]').each((item, index) => {
        cy.wrap(item).should('contain.text', keyStages[index])
    })
});

Then("they are shown the subjects for {string}", keystage => {
    
    const stages = keystage.split(',').map(s => s.trim());
    
    stages.forEach(element => {

        const subjects = allSubjects[element]

        cy.get('h2')
        .contains(`${element} subjects`)
        .parent()
        .within(() =>
        {
            cy.get('[data-testid="subject-name"]').each((item, index) => {
                cy.wrap(item).should('contain.text', subjects[index])
            });
        });

    });
});

Then("they are shown all the subjects under all the keys stages", () => {
    Step(this, "they are shown the subjects for 'Key stage 1'")
    Step(this, "they are shown the subjects for 'Key stage 2'")
    Step(this, "they are shown the subjects for 'Key stage 3'")
    Step(this, "they are shown the subjects for 'Key stage 4'")
})

Then("they will see {string} entered for the postcode", postcode => {
    cy.get('[data-testid="postcode"]>input').should('contain.value', postcode);
});

Then("they will see {string} selected", keystages => {
    const stages = keystages.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`input[id="${kebabCase(element)}"]`).should('be.checked');
    });
});

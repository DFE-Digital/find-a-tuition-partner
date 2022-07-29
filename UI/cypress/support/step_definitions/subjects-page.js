import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage } from "../utils";

export const allSubjects = {
    "Key stage 1": [ "English", "Maths", "Science"],
    "Key stage 2": [ "English", "Maths", "Science"],
    "Key stage 3": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"],
    "Key stage 4": [ "English", "Humanities", "Maths", "Modern foreign languages", "Science"]
}

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

Then("they will see {string} selected", keystages => {
    const stages = keystages.split(',').map(s => s.trim());
    stages.forEach(element => {
        cy.get(`input[id="${kebabCase(element)}"]`).should('be.checked');
    });
});
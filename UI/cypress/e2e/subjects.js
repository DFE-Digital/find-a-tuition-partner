import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";
        
const allSubjects = {
    "Key stage 1": [ "Literacy", "Numeracy", "Science"],
    "Key stage 2": [ "Literacy", "Numeracy", "Science"],
    "Key stage 3": [ "Maths", "English", "Science", "Humanities", "Modern foreign languages"],
    "Key stage 4": [ "Maths", "English", "Science", "Humanities", "Modern foreign languages"]
}

Given("a user has arrived on the 'Which key stages' page", () => {
    Step(this, "a user has arrived on the 'Which key stages' page for postcode 'AB12CD'");
});

Given("a user has arrived on the 'Which key stages' page for postcode {string}", postcode => {
    cy.visit(`/find-a-tuition-partner/which-key-stages?Postcode=${postcode}`);
});

Given("a user has arrived on the 'Which subjects' page for Key stage 1", () => {
    cy.visit("/find-a-tuition-partner/which-subjects?KeyStages=KeyStage1");
});

When("they manually navigate to the 'Which subjects' page", () => {
    cy.visit("/find-a-tuition-partner/which-subjects");
});

When("they select {string}", keystage => {
    cy.get(`input[id=${kebabCase(keystage)}]`).check();
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
    
    const subjects = allSubjects[keystage]

    cy.get('h2')
    .contains(`${keystage} subjects`)
    .parent()
    .within(() =>
    {
        cy.get('[data-testid="subject-name"]').each((item, index) => {
            cy.wrap(item).should('contain.text', subjects[index])
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

Then("they will see 'Key stage 1' selected", () => {
    cy.get('input[id="key-stage-1"]').should('be.checked');
});

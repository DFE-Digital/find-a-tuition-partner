import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage } from "../support/utils";
import { allSubjects } from "../support/step_definitions/subjects-page";
        
Given("a user has arrived on the 'Which key stages' page", () => {
    Step(this, "a user has arrived on the 'Which key stages' page for postcode 'AB12CD'");
});

Given("a user has arrived on the 'Which key stages' page for postcode {string}", postcode => {
    cy.visit(`/which-key-stages?Postcode=${postcode}`);
});

Given("a user has arrived on the 'Which subjects' page for {string}", keystage => {
    const query = keystage.split(',').map(s => `KeyStages=${camelCaseKeyStage(s.trim())}`).join('&');
    cy.visit(`/which-subjects?${query}`);
});

Given("the 'Which subjects' page is displayed", () => {
    cy.visit("/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4");
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

Then("they are shown all the subjects under all the keys stages", () => {
    Step(this, "they are shown the subjects for 'Key stage 1'")
    Step(this, "they are shown the subjects for 'Key stage 2'")
    Step(this, "they are shown the subjects for 'Key stage 3'")
    Step(this, "they are shown the subjects for 'Key stage 4'")
})

Then("the subjects are displayed in alphabetical order", () => {
    Step(this, "they are shown the subjects for 'Key stage 1'")
    Step(this, "they are shown the subjects for 'Key stage 2'")
    Step(this, "they are shown the subjects for 'Key stage 3'")
    Step(this, "they are shown the subjects for 'Key stage 4'")
});

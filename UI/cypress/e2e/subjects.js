import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";
        
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
    cy.get('[data-testid="key-stage-name"]').then(items => {
        cy.wrap(items[0]).should('contain.text', 'Key stage 1')
        cy.wrap(items[1]).should('contain.text', 'Key stage 2')
        cy.wrap(items[2]).should('contain.text', 'Key stage 3')
        cy.wrap(items[3]).should('contain.text', 'Key stage 4')
    })
});

Then("they are shown the subjects for Key stage 1", () => {
    let allSubjects = [
        // KS1
        "English",
        "Maths",
        "Science"
    ]

    cy.get('[data-testid="subject-name"]').should('have.length.greaterThan', 0)
    cy.get('[data-testid="subject-name"]').each((item, index) => {
        cy.wrap(item).should('contain.text', allSubjects[index])
    })
});

Then("they are shown the subjects for Key stage 1 and Key stage 2", () => {
    let allSubjects = [
        // KS1
        "English",
        "Maths",
        "Science",
        // KS2
        "English",
        "Maths",
        "Science"
    ]

    cy.get('[data-testid="subject-name"]').should('have.length.greaterThan', 0)
    cy.get('[data-testid="subject-name"]').each((item, index) => {
        cy.wrap(item).should('contain.text', allSubjects[index])
    })
});

Then("they are shown all the subjects under all the keys stages", () => {
    let allSubjects = [
        // KS1
        "English", 
        "Maths",
        "Science",
        // KS2
        "English", 
        "Maths",
        "Science",
        // KS3
        "English",
        "Maths",
        "Science",
        "Humanities", 
        "Modern foreign languages",
        // KS4
        "English",
        "Maths",
        "Science",
        "Humanities", 
        "Modern foreign languages",
    ]

    cy.get('[data-testid="subject-name"]').should('have.length.greaterThan', 0)
    cy.get('[data-testid="subject-name"]').each((item, index) => {
        cy.wrap(item).should('contain.text', allSubjects[index])
    })
})

Then("they will see {string} entered for the postcode", postcode => {
    cy.get('[data-testid="postcode"]>input').should('contain.value', postcode);
});

Then("they will see 'Key stage 1' selected", () => {
    cy.get('input[id="key-stage-1"]').should('be.checked');
});

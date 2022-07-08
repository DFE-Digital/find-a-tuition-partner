import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the 'Which subjects' page for Key stage 1", () => {
    cy.visit("/find-a-tuition-partner/which-subjects?KeyStages=KeyStage1");
});

Given("a user has arrived on the 'Which key stages' page for postcode {string}", postcode => {
    cy.visit(`/find-a-tuition-partner/which-key-stages?Postcode=${postcode}`);
});

When("they manually navigate to the 'Which subjects' page", () => {
    cy.visit("/find-a-tuition-partner/which-subjects");
});

Then("they are shown the subjects for Key stage 1", () => {
    let allSubjects = [
        // KS1
        "Literacy",
        "Numeracy",
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
        "Literacy", 
        "Numeracy",
        "Science",
        // KS2
        "Literacy", 
        "Numeracy",
        "Science",
        // KS3
        "Maths",
        "English",
        "Science",
        "Humanities", 
        "Modern foreign languages",
        // KS4
        "Maths",
        "English",
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
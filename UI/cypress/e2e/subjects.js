import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

When("they manually navigate to the 'Which subjects' page", () => {
    cy.visit("/find-a-tuition-partner/which-subjects");
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
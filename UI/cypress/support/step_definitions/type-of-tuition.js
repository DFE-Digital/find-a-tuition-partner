
import {
    Given,
    When,
    Then,
} from "@badeball/cypress-cucumber-preprocessor";

Given("the 'Type of tuition' page is displayed", () => {
    cy.visit("which-tuition-types?From=SearchResults&Postcode=wc1h8hl&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&KeyStages=KeyStage3&KeyStages=KeyStage4")
})



Then("they select subjects for the key stages", () => {
    cy.get('#key-stage-3-maths').click()
    cy.get('#key-stage-3-modern-foreign-languages').click()
    cy.get('#key-stage-4-maths').click()
    cy.get('#key-stage-4-modern-foreign-languages').click()
})

Then("they will be taken to the type of tuition page", () => {
    cy.get("h1").should("contain.text", "What type of tuition do you need?")
})


Then("the correct options will display", () => {
    cy.get('.govuk-radios > :nth-child(1)').contains("Any")
    cy.get('.govuk-radios > :nth-child(2)').contains("Online")
    cy.get('.govuk-radios > :nth-child(3)').contains("In School")
    cy.get('#any').should("be.enabled")
    cy.get('#online').should("be.enabled")
    cy.get('#in-school').should("be.enabled")

})

When("user clicks the button with text {string}", (type) => {
    const formattedType = type.replace(/\s/g, "-").toLowerCase();
    cy.get(`#${formattedType}`).click();
})

When("the {string} button is selected and the other two buttons are not", (type) => {
    let formattedType = type.replace(/\s/g, "-").toLowerCase();
    if (formattedType == 'any') {
        cy.get(`#${formattedType}`).should("be.checked")
        cy.get('#online').should("not.be.checked")
        cy.get('#in-school').should("not.be.checked")
    } else if (formattedType == 'online') {
        cy.get(`#${formattedType}`).should("be.checked")
        cy.get('#any').should("not.be.checked")
        cy.get('#in-school').should("not.be.checked")
    } else if (formattedType == 'in-school') {
        cy.get(`#${formattedType}`).should("be.checked")
        cy.get('#any').should("not.be.checked")
        cy.get('#online').should("not.be.checked")
    }

})

Then("the filter results show the expected selection", () => {
    cy.get('#in-school').should("be.checked")
})

Then("they will be taken to the 'Type of tuition' page", () => {
    cy.visit("/which-tuition-types?Postcode=SK1%201EB&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&TuitionType=Any&KeyStages=KeyStage1&KeyStages=KeyStage2")
})

Then("they select Any", () => {
    cy.get('#any').check();
})

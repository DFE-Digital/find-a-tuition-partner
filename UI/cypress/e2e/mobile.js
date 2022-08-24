import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Then("a user is using a {string}", (device) => {
    cy.viewport(428, 926);
});

When("the search result page is displayed", () => {
     cy.visit('/search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&Subjects=KeyStage2-Science&Subjects=KeyStage3-English&Subjects=KeyStage3-Humanities&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage3-Science&Subjects=KeyStage4-English&Subjects=KeyStage4-Humanities&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&Subjects=KeyStage4-Science&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4');
});

When("a tuition partner page is displayed", () => {
    cy.visit("/tuition-partner/bright-heart-education");
});

Then("the subject list is bullet pointed", () => {
    cy.get('.govuk-list-bullets-mobile-view').first()
    .within(() => { 
        cy.window().then((win) => {
            cy.contains('li').then(($el) => {
                const marker = win.getComputedStyle($el[0], '::marker')
                const markerProperty = marker.getPropertyValue('list-style-type')
                expect(markerProperty).to.equal('disc')
            })
        })
    })
}); 

Then("the subject list is not bullet pointed", () => {
    cy.get('.govuk-list-bullets-mobile-view').first()
    .within(() => { 
        cy.window().then((win) => {
            cy.contains('li').then(($el) => {
                const marker = win.getComputedStyle($el[0], '::marker')
                const markerProperty = marker.getPropertyValue('list-style-type')
                expect(markerProperty).to.equal('none')
            })
        })
    })
}); 
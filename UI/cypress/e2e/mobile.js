import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Then("a user is using a {string}", (device) => {
   
    if (device == 'phone'){
        cy.viewport(321, 640);
    }
    else if (device == 'tablet')
    {
        cy.viewport(642, 1024);
    }
    else if (device == 'desktop')
    {
        cy.viewport(770, 1024);
    }
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
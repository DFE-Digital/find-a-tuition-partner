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

Then("the search filters, postcode and results sections are all displayed", () => {
    Step(this, "the search filters are displayed");
    Step(this, "the postcode search is displayed");
    Step(this, "the search results are displayed");
});

Then("only the postcode and results sections are displayed", () => {
    Step(this, "the search filters are not displayed");
    Step(this, "the postcode search is displayed");
    Step(this, "the search results are displayed");
});

Then("the search results filter heading is displayed", () => {
    cy.get('[data-testid="filter-results-heading"]').should('be.visible');
});

Then("the search results filter heading is {string}", (heading) => {
    cy.get('[data-testid="filter-results-heading"]').should("contain.text", heading);
});

Then("the overlay search results filter heading is displayed", () => {
    cy.get('[data-testid="filter-results-heading"]').should('be.visible');
});

Then("the overlay search results filter heading is be displayed", () => {
    cy.get('[data-testid="overlay-filter-results-heading"]').should('not.be.visible');
});

Then("the show filters button is displayed", () => {
    cy.get('[data-testid="show-filters-button"]').should('be.visible');
});

Then("the show filters button is not displayed", () => {
    cy.get('[data-testid="show-filters-button"]').should('not.be.visible');
});
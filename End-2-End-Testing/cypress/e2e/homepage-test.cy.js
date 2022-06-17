it('login Test', () => {
    let url = Cypress.env('baseUrl');
    let username = Cypress.env('username');
    let password = Cypress.env('password');
    cy.visit(url, {
        auth: {
            username: username,
            password: password,
        },
    })
    cy.get('h1').first().should('be.exist');
    cy.get('div').contains('Compare national tutoring options');
    cy.get('h1').contains('The National Tutoring Programme');
})

it('Navigate to options page check', () => {
    let url = Cypress.env('baseUrl');
    let username = Cypress.env('username');
    let password = Cypress.env('password');
    cy.visit(url, {
        auth: {
            username: username,
            password: password,
        },
    })
    cy.get('a[href*="/options"]').click();
    cy.location('pathname').should('match', /\/options$/);

})
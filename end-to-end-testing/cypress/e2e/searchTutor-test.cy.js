it('search Tutor Happy Path', () => {
    let url = Cypress.env('baseUrl');
    let username = Cypress.env('username');
    let password = Cypress.env('password');
    cy.visit(url + 'options', {
        auth: {
            username: username,
            password: password,
        },
    })
    cy.get('a[href*="/find-a-tuition-partner/start"]').click();
    cy.get('input[name="Postcode"]').type('WF84SW')
    cy.get('button').click()
    cy.contains('label', 'Primary - Literacy')  // find your text
        .parent('div')                      // move to parent div
        .find('input')                      // select it's input
        .check();
    cy.get('button').click()
    cy.get('h2').first().should('be.exist');
    cy.get('a[class="govuk-link"]').last().click();
    cy.get('h2').first().should('contain', 'Tuition details');
})
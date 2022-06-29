beforeEach(() => {
    let url = '/';
    let username = Cypress.env('username');
    let password = Cypress.env('password');
    if (username && password) {
        cy.visit(url, {
            auth: {
                username: username,
                password: password,
            },
        })
    }
    else {
        cy.visit(url)
    }
})

it('search Tutor Happy Path', () => {
    // Start page
    cy.contains('Start').click();
    
    // Options page
    cy.contains('Find a tuition partner')
        .parent()
        .contains('Continue')
        .click();

    // Location page
    cy.get('input[name="Data.Postcode"]').type('WF84SW{enter}')

    // Key stages page
    cy.contains('KeyStage1')
        .parent('div')                      // move to parent div
        .find('input')                      // select it's input
        .check();
    cy.get('form').submit();

    // Subjects page
    cy.contains('Literacy')
        .parent('div')                      // move to parent div
        .find('input')                      // select it's input
        .check();
    cy.get('form').submit();

    // Results page
    cy.get('h2').first().should('be.exist');

    // Tuition partner page
    cy.get('a[class="govuk-link"]').last().click();
    cy.get('h2').first().should('contain', 'Tuition details');
})
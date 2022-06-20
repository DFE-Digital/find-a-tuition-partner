beforeEach(() => {
    let url = Cypress.env('baseUrl') + 'options';
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
    cy.get('a[href*="/find-a-tuition-partner/start"]').click();
    cy.get('input[name="Postcode"]').type('WF84SW')
    cy.get('button').click()
    //To do Element Id Tags
    cy.contains('label', 'Primary - Literacy')  // find your text
        .parent('div')                      // move to parent div
        .find('input')                      // select it's input
        .check();
    cy.get('button').click()
    cy.get('h2').first().should('be.exist');
    cy.get('a[class="govuk-link"]').last().click();
    cy.get('h2').first().should('contain', 'Tuition details');
})
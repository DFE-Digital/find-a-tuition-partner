
describe('Verify Error Messages For Search Page', () => {
    beforeEach(() => {
        let url = '/options';
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

    it('check Error Message For Invalid Postcode', () => {
        cy.get('a[href*="/find-a-tuition-partner/start"]').click();
        cy.get('input[name="Postcode"]').type('WF84S1');
        cy.get('button').click();
        cy.get('[data-module="govuk-error-summary"]').type('Enter a valid postcode');
    })

    it('check Error Message For Not Entering Postcode', () => {
        cy.get('a[href*="/find-a-tuition-partner/start"]').click();
        cy.get('button').click();
        cy.get('[data-module="govuk-error-summary"]').type('Enter a postcode');
    })

    it('check Error Message For Non England Postcode', () => {

        cy.get('a[href*="/find-a-tuition-partner/start"]').click();
        cy.get('input[name="Postcode"]').type('EH12NG');
        cy.get('button').click();
        cy.get('[data-module="govuk-error-summary"]').type('This service covers England only');
    })
})
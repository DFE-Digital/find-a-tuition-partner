import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("the SEND status is {string} for tuition partner {string}", (send, tuitionPartnerName) => {
    cy.get('.govuk-link').contains(tuitionPartnerName).parent().parent().within(() =>
    {
        cy.get('div').contains('SEND support').parent().contains(send)
    });
});

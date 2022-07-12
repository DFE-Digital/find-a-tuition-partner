import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";

Then("they will see {string} as an error message for the {string}", (error, property) => {
    cy.get('[data-module="govuk-error-summary"] > div').should("contain.text", error);
});


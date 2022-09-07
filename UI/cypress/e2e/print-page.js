import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Then("the 'Print this page' link is displayed", () => {
  cy.get('[data-testid="print-this-page-link"]').should("exist");
});

Then("the 'Print this page' link is not displayed", () => {
  cy.get('[data-testid="print-this-page-link"]').should("not.exist");
});

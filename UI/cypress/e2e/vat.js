import { Then } from "@badeball/cypress-cucumber-preprocessor";

Then("the prices include VAT content is displayed", () => {
  cy.get('[data-testid="price-includes-vat"]')
    .should("exist")
    .and("contain.text", "Prices shown include VAT.");
});

Then("the prices include VAT content is not displayed", () => {
  cy.get('[data-testid="price-includes-vat"]').should("not.exist");
});

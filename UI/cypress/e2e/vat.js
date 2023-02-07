import { Then } from "@badeball/cypress-cucumber-preprocessor";

Then("the prices include VAT content is displayed", () => {
  cy.get('[data-testid="price-includes-vat"]')
    .should("exist")
    .and("contain.text", "Prices shown include VAT.");
});

Then(
  "the prices with VAT does not apply content is displayed for {string}",
  (name) => {
    cy.get('[data-testid="price-includes-vat"]').should("not.exist");
    cy.get('[data-testid="price-vat-not-applicable"]')
      .should("exist")
      .and(
        "contain.text",
        `VAT does not apply to these prices because ${name} is VAT exempt.'`
      );
  }
);

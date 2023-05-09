import { Then } from "@badeball/cypress-cucumber-preprocessor";

Then("the prices include VAT content is displayed", () => {
  cy.get(".govuk-grid-column-two-thirds > :nth-child(10)")
    .should("exist")
    .and("contain.text", "Tuition prices (including VAT)");
});

Then(
  "the prices with VAT does not apply content is displayed for {string}",
  (name) => {
    cy.get('[data-testid="price-includes-vat"]').should("not.exist");
    cy.get(".govuk-inset-text > :nth-child(1)")
      .should("exist")
      .and("contain.text", `This tuition partner does not charge VAT.`);
  }
);

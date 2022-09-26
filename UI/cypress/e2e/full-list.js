import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

When("they click the 'All quality-assured tuition partners' link", () => {
  cy.get('[data-testid="full-list-link"]')
    .should("exist")
    .should("have.text", "All quality-assured tuition partners")
    .click();
});

Then("they will see the 'All quality-assured tuition partners' page", () => {
  Step(this, "the page URL ends with '/full-list'");
  Step(this, "the page's title is 'Full List'");
  cy.get('[data-testid="full-list-header"]').should(
    "have.text",
    "All quality-assured tuition partners"
  );
  cy.get('[data-testid="cookie-banner"]').should("exist");
});

import { Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import "cypress-axe";

Then("the page should be accessible", () => {
  cy.injectAxe();
  cy.configureAxe({
    rules: [
      { id: "region", enabled: false },
      {
        id: "aria-allowed-attr",
        enabled: false,
      },
    ],
  });
  cy.checkA11y();
});

afterEach(() => {
  Step(this, "the page should be accessible");
});

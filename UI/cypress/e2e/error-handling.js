import { When, Then } from "@badeball/cypress-cucumber-preprocessor";

When("a service page has not been found", () => {
  const requestOptions = {
    url: "/page-does-not-exist",
    failOnStatusCode: false,
  };
  cy.requestWithBasicAuth(requestOptions).its("status").should("equal", 404);
  cy.visit("/page-does-not-exist", { failOnStatusCode: false });
});

Then("the 'page not found' error page is displayed", () => {
  cy.get('[data-testid="error-page"').should("exist");
  cy.get("head title").should("contain", "Page not found");
});

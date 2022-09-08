import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("the ‘contact us’ page is displayed", () => {
  cy.visit("/contact-us");
});

When("the link ‘read our guidance’ is selected", () => {
  cy.get('[data-testid="complaints-page-link"]').click();
});

When("the home link is selected", () => {
  cy.get('[data-testid="home-link"]').click();
});

Then("the 'complaints page' is displayed", () => {
  cy.visit("/complaints-page");
});

Then("page has link to tutoring support email address", () => {
  cy.get('[data-testid="mailto-ntp-link"]').should(
    "have.attr",
    "href",
    "mailto:tutoring.support@service.education.gov.uk"
  );
});

Then(
  "the 'report child abuse to local council' page is accessible in a new tab",
  () => {
    cy.get('[data-testid="local-council-link"]').should(
      "have.attr",
      "href",
      "https://www.gov.uk/report-child-abuse-to-local-council"
    );

    cy.get('[data-testid="local-council-link"]').should(
      "have.attr",
      "target",
      "_blank"
    );

    cy.get('[data-testid="local-council-link"]').then(function ($a) {
      const href = $a.prop("href");
      cy.request(href).should((response) => {
        expect(response.status).to.eq(200);
      });
    });
  }
);

Then("they will be taken to the 'complaints' page", () => {
  cy.location("pathname").should("eq", "/complaints-page");
});

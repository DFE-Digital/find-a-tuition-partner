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

When("the ‘local council’ link has been selected", () => {
  cy.get('[data-testid="local-council-link"]').click();
});

Then("the 'complaints page' is displayed", () => {
  cy.visit("/complaints-page");
});

Then("they will see link to tutoring mail address", () => {
  cy.get('[data-testid="mailto-ntp-link"]').should(
    "have.attr",
    "href",
    "mailto:tutoring.support@service.education.gov.uk"
  );
});

Then("the 'report child abuse to local council' page is accessible", () => {
  cy.get('[data-testid="local-council-link"]').should(
    "have.attr",
    "href",
    "https://www.gov.uk/report-child-abuse-to-local-council"
  );

  cy.request("https://www.gov.uk/report-child-abuse-to-local-council").should(
    (response) => {
      expect(response.status).to.eq(200);
    }
  );
});

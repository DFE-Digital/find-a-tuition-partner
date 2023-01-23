import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Then("they will see the back link", () => {
  cy.get('[data-testid="back-link"]').should("have.attr", "href", "");
});

Then("they will see link to tutoring mail address", () => {
  cy.get('[data-testid="mailto-ntp-link"]').should(
    "have.attr",
    "href",
    "mailto:tutoring.support@service.education.gov.uk"
  );
});

Then("they will see the telephone number {string}", (telephone) => {
  cy.get('[data-testid="contact-us-telephone"]').should("have.text", telephone);
});

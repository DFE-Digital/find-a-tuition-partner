import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user accesses a service page after accepting cookies", () => {
    Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are accepted");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("a user accesses a service page after rejecting cookies", () => {
    Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are rejected");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("the 'Which subjects' page is displayed", () => {
    cy.visit("/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4");
});
  
When("cookies are accepted", () => {
    cy.get('[data-testid="accept-cookies"]').click();
  });

When("cookies are rejected", () => {
    cy.get('[data-testid="reject-cookies"]').click();
  });

When("the ‘view cookies’ is selected", () => {
    cy.get('[data-testid="view-cookies"]').click();
  });

Then("the cookies banner is displayed", () => {
    cy.get('[data-testid="cookie-banner"]').should("exist");
  });

Then("user session is tracked", () => {
    cy.get('head script').should('contain', 'gtag')
  });

Then("the banner disappears", () => {
    cy.contains('[data-testid="cookie-banner"]').should('not.exist');;
  });
  
Then("user session is not tracked", () => {
    cy.contains('gtag').should('not.exist');;
  });

Then("the 'view cookies' page is loaded", () => {
    cy.location('pathname').should('eq', '/cookies');
  });

Then("the cookie banner is not displayed", () => {
    Step(this, "the banner disappears");
  });

  
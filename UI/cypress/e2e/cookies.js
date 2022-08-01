import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user accesses a service page after accepting cookies", () => {
    Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are accepted");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("a user accesses a service page after rejecting cookies", () => {Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are rejected");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("the 'Which subjects' page is displayed", () => {
    cy.visit("/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4");
});

Given("nothing is selected", () => {
  cy.get(`input[data-testid="cookie-consent-accept"]`).should('not.be.checked');
  cy.get(`input[data-testid="cookie-consent-deny"]`).should('not.be.checked');
});

Given("the success banner has been displayed", () => {
  Step(this, "the 'Which subjects' page is displayed");
  Step(this, "the ‘cookies' is selected from footer");
  Step(this, "a user opts-in");
  Step(this, "Saves Changes");
  Step(this, "a Success Banner is displayed");
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

  When("the ‘cookies' is selected from footer", () => {
    cy.get('[data-testid="view-footer-cookies"]').click();
  });

When("a user opts-in", () => {
  cy.get(`input[data-testid="cookie-consent-accept"]`).click();
  });

When("a user opts-out", () => {
    cy.get(`input[data-testid="cookie-consent-deny"]`).click();
    });

When("Saves Changes", () => {
    cy.get(`[data-testid="save-changes"]`).click();
    });

When("the link to previous page is clicked", () => {
      cy.get(`[data-testid="view-previous-page-link"]`).click();
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

Then("a Success Banner is displayed", () => {
    cy.contains('[data-testid="success-banner""]').should('exist');;
  });

Then("the previous page is displayed correctly (any previous settings are maintained)", () => {
    cy.location('search').should('eq', '/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4');
});

Then("the privacy policy is accessible in a new tab", () => {
  cy.get('[data-testid="privacy-page"]').then(function ($a) {
    const href = $a.prop('href');
    cy.request(href).its('body').should('include', '</html>');
})});

Given("opt-in is selected", () => {
  cy.get(`input[data-testid="cookie-consent-accept"]`).should('be.checked');
});


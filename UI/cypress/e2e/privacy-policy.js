import {When, Then} from "@badeball/cypress-cucumber-preprocessor";

When("they select the privacy policy in the footer", () => {
    cy.get(`[data-testid="privacy-policy-link"]`).click();
    });

Then("the privacy policy page is displayed", () => {
    cy.get('head title').should('contain', 'Privacy - Compare National Tutoring Options')
    });
  
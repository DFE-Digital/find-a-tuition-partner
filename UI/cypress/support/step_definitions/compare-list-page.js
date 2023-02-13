import { Given } from "@badeball/cypress-cucumber-preprocessor";

Given(
  "a user has arrived on the 'Compare tuition partner prices' page for postcode {string}",
  (postcode) => {
    cy.visit(`/compare-list?Postcode=${postcode}`);
  }
);

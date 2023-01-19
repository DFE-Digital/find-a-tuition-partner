import {
    Given,
    When,
    Then,
    Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given(
    "a user has arrived on the 'My shortlisted tuition partners' page for postcode {string}",
    (postcode) => {
        cy.visit(`/shortlist?Postcode=${postcode}`);
    }
);
import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the 'Tuition Partner' page for {string}", name => {
    cy.visit(`/tuition-partner/${name}`);
});
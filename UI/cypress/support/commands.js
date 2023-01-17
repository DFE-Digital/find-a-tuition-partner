// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })

import { getJumpToLocationId } from "./utils";

Cypress.Commands.overwrite("visit", (originalFn, url, options) => {
  const username = Cypress.env("username");
  const password = Cypress.env("password");

  if (username && password) {
    options = options || {};
    options.auth = {
      username: username,
      password: password,
    };
  }

  return originalFn(url, options);
});

Cypress.Commands.add("isWithinViewPort", { prevSubject: true }, (element) => {
  const { top } = element[0].getBoundingClientRect();
  expect(top).to.be.oneOf([-0.1875, -0.453125, 0, 0.234375, 0.28125, 63]);
  return element;
});

Cypress.Commands.add("isCorrectJumpToLocation", ($element) => {
  cy.visit($element.attr("href"));
  cy.get('[data-testid="type-of-tuition"]').first().should("not.be.empty");
  cy.get(".govuk-back-link").click();
  cy.get(`[id="${getJumpToLocationId($element)}"]`).isWithinViewPort();
});

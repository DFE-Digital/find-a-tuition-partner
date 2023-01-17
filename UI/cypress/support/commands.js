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

import { removeNewLine } from "./utils";

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

Cypress.Commands.add("checkTotalTps", (expectedTotal) => {
  cy.get('[id="totalShortlistedTuitionPartners"]')
    .invoke("text")
    .then((text) => {
      cy.wrap(removeNewLine(text)).should("equal", `${expectedTotal}`);
    });
});
Cypress.Commands.add("isCheckboxUnchecked", (checkboxSelector) => {
  cy.get(checkboxSelector).should("not.be.checked");
});
Cypress.Commands.add("isCheckboxchecked", (checkboxSelector) => {
  cy.get(checkboxSelector).should("be.checked");
});
Cypress.Commands.add("goToTpDetailPage", (tpName) => {
  cy.get(".govuk-link").contains(tpName).click();
  cy.get(".govuk-heading-l").should("contain.text", tpName);
});
Cypress.Commands.add("clickBack", () => {
  cy.get('[data-testid="back-link"]').click();
});
Cypress.Commands.add("checkLaLabelText", (expectedText) => {
  cy.get('[data-testid="la-name"]').should("contain.text", expectedText);
});
Cypress.Commands.add("isCookieNull", (cookieName) => {
  cy.getCookie(`${cookieName}`).should("equal", null);
});
Cypress.Commands.add("checkCookieValue", (cookieName, expectedCookieValue) => {
  cy.getCookie(`${cookieName}`).should(
    "have.property",
    "value",
    `${expectedCookieValue}`
  );
});

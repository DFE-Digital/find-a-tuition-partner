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

import { removeNewLine, getJumpToLocationId } from "./utils";

Cypress.Commands.overwrite("visit", (originalFn, url, options) => {
  console.log("Testing env variable retrieved from the Pipeline..");
  console.log(Cypress.env("ENV_VARIABLE_TEST"));
  console.log("End Testing env variable retrieved from the Pipeline..");

  const basicAuthCredentials = Cypress.env("BASIC_AUTH_CREDENTIALS");

  if (basicAuthCredentials) {
    const [username, password] = basicAuthCredentials.split(":");
    if (username && password) {
      options = options || {};
      options.auth = {
        username: username,
        password: password,
      };
    }
  }
  return originalFn(url, options);
});

Cypress.Commands.add("checkTotalTps", (expectedTotal) => {
  cy.get('[id="totalCompareListedTuitionPartners"]')
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

Cypress.Commands.add("isWithinViewPort", (element) => {
  const { top } = element[0].getBoundingClientRect();
  expect(top).to.be.greaterThan(-1);
  expect(top).to.be.lessThan(150);
  return element;
});

Cypress.Commands.add("validateTPPageAndReturnLink", ($element) => {
  cy.visit($element.attr("href"));
  cy.get('[data-testid="type-of-tuition"]').first().should("not.be.empty");
  cy.get('[data-testid="pricing-group-size-column"]')
    .first()
    .should("not.be.empty");
  const names = [];
  cy.get('[data-testid="pricing-group-size-column"]')
    .each(($element, index) => {
      names[index] = $element.text();
    })
    .then(() => {
      const sortedNames = names.slice().sort(function (a, b) {
        return a.localeCompare(b, "en", { sensitivity: "base" });
      });
      expect(names).to.deep.equal(sortedNames);
    });
  cy.get(".govuk-back-link").click();
  cy.get(`[id="${getJumpToLocationId($element)}"]`).then(($el) => {
    cy.isWithinViewPort($el);
  });
});

Cypress.Commands.add("checkTextContent", (selector, expectedText) => {
  cy.get(selector)
    .invoke("text")
    .then((text) => expect(text.trim()).to.equal(expectedText));
});

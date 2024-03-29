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
  expect(top).to.be.lessThan(160);

  return element;
});

Cypress.Commands.add("validateTPPageAndReturnLink", () => {
  // cy.get('[data-testid="tuition-setting"]').first().should("not.be.empty");

  // Check if 'pricing-group-size-column' exists before interacting with it
  cy.get("body").then(($body) => {
    if ($body.find('[data-testid="pricing-group-size-column"]').length > 0) {
      // 'pricing-group-size-column' was found, perform actions here
      const names = [];
      cy.get('[data-testid="pricing-group-size-column"]')
        .first()
        .should("not.be.empty");
      cy.get('[data-testid="pricing-group-size-column"]')
        .each(($element, index) => {
          names[index] = $element.text().trim();
        })
        .then(() => {
          const sortedNames = names.slice().sort(function (a, b) {
            return a.toLowerCase().localeCompare(b.toLowerCase(), "en", {
              ignorePunctuation: true,
              sensitivity: "base",
            });
          });
          const sortedSets = names
            .map((name) => {
              const [start, end] = name.split(" to ");
              return [parseInt(start), parseInt(end)].sort();
            })
            .sort();
          const sortedSetsFromSortedNames = sortedNames
            .map((name) => {
              const [start, end] = name.split(" to ");
              return [parseInt(start), parseInt(end)].sort();
            })
            .sort();
          expect(sortedSets).to.deep.equal(sortedSetsFromSortedNames);
          const countMap = {};
          names.forEach((name) => {
            if (!countMap[name]) {
              countMap[name] = 1;
            } else {
              countMap[name]++;
            }
          });
          Object.values(countMap).forEach((count) => {
            assert(
              count === 1 || count === 2,
              `Expected count to be 1 or 2, but got ${count}`
            );
          });
        });
    }
  });
  // cy.get(".govuk-back-link").click();
  // This may need to be updated, since `$element` is no longer available
  // I'll comment it out for now
  // cy.get(`[id="${getJumpToLocationId($element)}"]`).then(($el) => {
  //   cy.isWithinViewPort($el);
  // });
});

Cypress.Commands.add("checkTextContent", (selector, expectedText) => {
  cy.get(selector)
    .invoke("text")
    .then((text) => expect(text.trim()).to.equal(expectedText));
});

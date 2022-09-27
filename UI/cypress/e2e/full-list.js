import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";

When("they click the 'All quality-assured tuition partners' link", () => {
  cy.get('[data-testid="full-list-link"]')
    .should("exist")
    .should("have.text", "All quality-assured tuition partners")
    .click();
});

When("they click on the {string} tuition partner's name", ($position) => {
  const index = $position.match(/\d+/);
  cy.get('[data-testid="tuition-partner-name-link"]').eq(index).click();
});

Then("they will see the 'All quality-assured tuition partners' page", () => {
  Step(this, "the page URL ends with '/full-list'");
  Step(this, "the page's title is 'Full List'");
  cy.get('[data-testid="full-list-header"]').should(
    "have.text",
    "All quality-assured tuition partners"
  );
  cy.get('[data-testid="cookie-banner"]').should("exist");
});

Then(
  "the full list of quality-assured tuition partners is in alphabetical order by name",
  () => {
    const names = [];
    cy.get('[data-testid="tuition-partner-name-link"]')
      .each(($element, index) => {
        names[index] = $element.text();
      })
      .then(() => {
        const sortedNames = names.slice().sort(function (a, b) {
          return a.localeCompare(b, "en", { sensitivity: "base" });
        });
        expect(names).to.deep.equal(sortedNames);
      });
  }
);

Then(
  "the user is only shown the name, website, phone number and email address for each tuition partner",
  () => {
    cy.get('[data-testid="tuition-partner-summary"]').each(($element) => {
      $element = cy.wrap($element);
      $element.get('[data-testid="tuition-partner-name-link"]').should("exist");
      $element
        .get('[data-testid="tuition-partner-website-link"]')
        .should("exist");
      $element
        .get('[data-testid="tuition-partner-phone-number-link"]')
        .should("exist");
      $element
        .get('[data-testid="tuition-partner-email-link"]')
        .should("exist");
    });
  }
);

Then("the name of each tuition partner links to their details page", () => {
  cy.get('[data-testid="tuition-partner-name-link"]').each(($element) => {
    cy.wrap($element).should(
      "have.attr",
      "href",
      `/tuition-partner/${kebabCase($element.text()).replace(
        /'/g,
        "%27"
      )}?from=FullList`
    );
  });
});

Then(
  "the website link for each tuition partner opens their website in a new tab",
  () => {
    cy.get('[data-testid="tuition-partner-website-link"]').each(($element) => {
      cy.wrap($element).should("have.attr", "href");
      cy.wrap($element).should("have.attr", "target", "_blank");
    });
  }
);

Then(
  "the phone number link for each tuition partner initiates their device's calling options",
  () => {
    cy.get('a[data-testid="tuition-partner-phone-number-link"]').each(
      ($element) => {
        cy.wrap($element).should("have.attr", "href", `tel:${$element.text()}`);
      }
    );
  }
);

Then(
  "the email link for each tuition partner initiates their email client options",
  () => {
    cy.get('a[data-testid="tuition-partner-email-link"]').each(($element) => {
      cy.wrap($element).should(
        "have.attr",
        "href",
        `mailto:${$element.text()}`
      );
    });
  }
);

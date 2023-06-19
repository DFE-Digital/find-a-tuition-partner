import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given("a mobile user has opened the mobile filters overlay", () => {
  Step(
    this,
    "a user has arrived on the 'Search results' page for 'Key stage 1 English'"
  );
  Step(this, "a user is using a 'phone'");
  Step(this, "they click 'Show filters'");
});

When("they click 'Show filters'", () => {
  cy.get('[data-testid="show-filters-button"]').click();
});

When("they select the 'Return to results' link", () => {
  cy.get('[data-testid="return-to-results-link"]').click();
});

When("they select the 'Show search results' button", () => {
  cy.get('[data-testid="show-search-results-button"]').click();
});

Then("the subject list is bullet pointed", () => {
  cy.get(".govuk-summary-list > :nth-child(1)")
    .first()
    .within(() => {
      cy.window().then((win) => {
        cy.contains("li").then(($el) => {
          const marker = win.getComputedStyle($el[0], "::marker");
          const markerProperty = marker.getPropertyValue("list-style-type");
          expect(markerProperty).to.equal("disc");
        });
      });
    });
});

Then("the subject list is not bullet pointed", () => {
  cy.get(".govuk-list-bullets-mobile-view")
    .first()
    .within(() => {
      cy.window().then((win) => {
        cy.contains("li").then(($el) => {
          const marker = win.getComputedStyle($el[0], "::marker");
          const markerProperty = marker.getPropertyValue("list-style-type");
          expect(markerProperty).to.equal("none");
        });
      });
    });
});

Then(
  "the search filters, postcode and results sections are all displayed",
  () => {
    Step(this, "the search filters are displayed");
    Step(this, "the postcode search is displayed");
    Step(this, "the search results are displayed");
  }
);

Then("only the postcode and results sections are displayed", () => {
  Step(this, "the search filters are not displayed");
  Step(this, "the postcode search is displayed");
  Step(this, "the search results are displayed");
});

Then("the search results filter heading is displayed", () => {
  cy.get('[data-testid="filter-results-heading"]').should("be.visible");
});

Then("the search results filter heading is {string}", (heading) => {
  cy.get('[data-testid="filter-results-heading"]').should(
    "contain.text",
    heading
  );
});

Then("the overlay search results filter heading is displayed", () => {
  cy.get('[data-testid="filter-results-heading"]').should("be.visible");
});

Then("the overlay search results filter heading is not displayed", () => {
  cy.get('[data-testid="overlay-filter-results-heading"]').should(
    "not.be.visible"
  );
});

Then("the show filters button is displayed", () => {
  cy.get('[data-testid="show-filters-button"]')
    .should("be.visible")
    .should("have.text", "Show filters");
});

Then("the show filters button is not displayed", () => {
  cy.get('[data-testid="show-filters-button"]').should("not.be.visible");
});

Then("the overlay search results filter heading is {string}", (heading) => {
  cy.get('[data-testid="overlay-filter-results-heading"]')
    .find("h1")
    .should("have.text", heading);
});

Then("the overlay search results filter heading has the results count", () => {
  cy.get('[data-testid="overlay-filter-results-count"]').contains(
    /\d+ results/
  );
});

Then(
  "the overlay search results filter heading has the subjects header",
  () => {
    cy.get('[data-testid="overlay-filter-results-heading"]')
      .find("h2")
      .should("have.text", "Subjects");
  }
);

Then("the return to results link is displayed", () => {
  cy.get('[data-testid="return-to-results-link"]').should("be.visible");
});

Then("the show search results button is displayed", () => {
  cy.get('[data-testid="show-search-results-button"]')
    .scrollIntoView()
    .should("be.visible");
});

Then("subject {string} is selected", (subject) => {
  cy.url().should("contain", subject);
});

Then("subject {string} is selected on the filter", (subject) => {
  cy.get('[data-testid="subject-name"] :checked')
    .should("be.checked")
    .check(subject, { force: true });
});

Then("subject {string} is no longer selected", (subject) => {
  cy.url().should("not.contain", subject);
});

Given("a user begins journey from a mobile", () => {
  Step(this, "a user has started the 'Find a tuition partner' journey");
  Step(this, "a user is using a 'phone'");
});

When("the postcode is edited in the start page", () => {
  Step(this, "they enter 'YO11 1AA' as the school's postcode");
  Step(this, "they click 'Continue'");
});

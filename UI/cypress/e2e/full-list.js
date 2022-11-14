import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";

Given(
  "a user has searched the all quality-assured tuition partners by name {string}",
  ($name) => {
    Step(
      this,
      "a user has arrived on the all quality-assured tuition partners page"
    );
    Step(this, `they search by tuition partner name '${$name}'`);
  }
);

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

When("they search by tuition partner name {string}", ($name) => {
  cy.get('[data-testid="name-input-box"]').type($name);
  cy.get('[data-testid="call-to-action"]').click();
});

Then("they will see the 'All quality-assured tuition partners' page", () => {
  Step(this, "the page URL ends with '/all-tuition-partners'");
  Step(this, "the page's title is 'All Tuition Partners'");
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
  "they will see the list of quality-assured tuition partners with names containing {string} is in alphabetical order by name",
  ($name) => {
    cy.get('[data-testid="tuition-partner-name-link"]').each(($element) => {
      cy.wrap($element).contains($name, { matchCase: false });
    });
    Step(
      this,
      "the full list of quality-assured tuition partners is in alphabetical order by name"
    );
  }
);

Then(
  "the user is only shown the name, website, phone number and email address for each tuition partner",
  () => {
    cy.get('[data-testid="tuition-partner-summary"]').each(
      ($element, $index) => {
        $element = cy.wrap($element);
        $element
          .get('[data-testid="tuition-partner-name-link"]')
          .should("exist");
        $element
          .get('[data-testid="tuition-partner-website-link"]')
          .should("exist");
        $element
          .get('[data-testid="tuition-partner-phone-number-link"]')
          .should("exist");
        $element
          .get('[data-testid="tuition-partner-email-link"]')
          .should("exist");
        if ($index >= 5) return false;
      }
    );
  }
);

Then("the name of each tuition partner links to their details page", () => {
  cy.get('[data-testid="tuition-partner-name-link"]').each(($element) => {
    cy.wrap($element).should(
      "have.attr",
      "href",
      `/tuition-partner/${kebabCase($element.text()).replace(
        /'/g,
        ""
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

Then("search by tuition partner name is empty", () => {
  cy.get('[data-testid="name-input-box"]').should("not.have.value");
});

Then("search by tuition partner name is {string}", ($name) => {
  cy.get('[data-testid="name-input-box"]').should("have.value", $name);
});

Then("they will see there are no search results for {string}", ($name) => {
  cy.get('[data-testid="no-search-results-message"]')
    .should("be.visible")
    .should("contain.text", `there are no search results for '${$name}'.`);
});

Then("they will not see there are no search results for name", () => {
  cy.get('[data-testid="no-search-results-message"]').should("not.exist");
});

Then(
  "the number of tuition partners displayed matches the displayed count",
  () => {
    let countOfElements = 0;
    cy.get('[data-testid="tuition-partner-summary"]').then(($elements) => {
      countOfElements = $elements.length;
      cy.get('[data-testid="result-count"]')
        .invoke("text")
        .then(parseInt)
        .should("equal", countOfElements);
    });
  }
);

Then("logos are shown for tuition partners", () => {
  cy.get('[data-tuitionpartner="nudge-education"]')
    .find("img")
    .should("be.visible")
    .and("have.attr", "src", "/tuition-partner-logo/nudge-education")
    .and("have.attr", "alt", "The company logo for Nudge Education")
    .and(($img) => {
      expect($img[0].width).to.be.equal(120);
    });
});

Then("logos are not shown for tuition partners", () => {
  cy.get('[data-tuitionpartner="nudge-education"]>img').should(
    "not.be.visible"
  );
});

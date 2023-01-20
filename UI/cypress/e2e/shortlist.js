import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import {
  kebabCase,
  removeExcessWhitespaces,
  removeNewLine,
} from "../support/utils";

When("they add {string} to their shortlist on the results page", (tpName) => {
  cy.get(`#shortlist-cb-${kebabCase(tpName)}`).check();
  cy.wait(500);
});

When(
  "they remove {string} from their shortlist on the results page",
  (tpName) => {
    cy.get(`#shortlist-cb-${kebabCase(tpName)}`).uncheck();
    cy.wait(500);
  }
);

Then("{string} is marked as shortlisted on the results page", (tpName) => {
  cy.get(`#shortlist-cb-${kebabCase(tpName)}`).should("be.checked");
});

Then("{string} is not marked as shortlisted on the results page", (tpName) => {
  cy.get(`#shortlist-cb-${kebabCase(tpName)}`).should("not.be.checked");
});

Then(
  "the shortlist shows as having {int} entries on the results page",
  (numEntries) => {
    cy.get("#totalShortlistedTuitionPartners").should((el) =>
      expect(parseInt(el.text().trim())).to.equal(numEntries)
    );
  }
);

When("they choose to view their shortlist from the results page", () => {
  cy.get('[data-testid="my-shortlisted-tuition-partners-link"]').click();
});

Then("{string} is entry {int} on the shortlist page", (tpName, entry) => {
  tpName = removeExcessWhitespaces(removeNewLine(tpName));
  cy.get("tbody th")
    .eq(entry - 1)
    .then(($tbodyHeader) => {
      return removeExcessWhitespaces(removeNewLine($tbodyHeader.text()));
    })
    .should("equal", tpName);
});

Then("there are {int} entries on the shortlist page", (count) => {
  cy.get("tbody th").should("have.length", count);
});

Then(
  "{string} is entry {int} on the not available list on the shortlist page",
  (tpName, entry) => {
    cy.get('[data-testid="unavailable-tps"] li')
      .eq(entry - 1)
      .should("contain.text", tpName);
  }
);

Then(
  "entry {int} on the shortlist is the row {string}, {string}, {string}, {string}, {string}",
  (entry, name, keyStages, subjects, tuitionType, price) => {
    cy.get("tbody tr")
      .eq(entry - 1)
      .within(() => {
        cy.get("th").should((el) => expect(el.text().trim()).to.equal(name));
        cy.get("td")
          .eq(0)
          .should((el) => expect(el.text().trim()).to.equal(keyStages));
        cy.get("td")
          .eq(1)
          .should((el) => expect(el.text().trim()).to.equal(subjects));
        cy.get("td")
          .eq(2)
          .should((el) => expect(el.text().trim()).to.equal(tuitionType));
        cy.get("td")
          .eq(3)
          .should((el) => expect(el.text().trim()).to.equal(price));
      });
  }
);

When(
  "they choose to view the {string} details from the shortlist",
  (tpName) => {
    cy.get("a").contains(tpName).click();
  }
);

Then("they choose to sort the shortlist by price", () => {
  cy.get('[data-testid="shortlist-price-sort"]').click();
});

Then("they choose to remove entry {int} on the shortlist page", (entry) => {
  cy.get('tbody td [data-testid="remove-tp"]')
    .eq(entry - 1)
    .click();
});

Then("they choose to click on clear shortlist link", () => {
  cy.get('[data-testid="clear-shortlist-link"]').click();
});

Then("they are taken to the clear shortlist confirmation page", () => {
  cy.get("h1").should("have.text", "You're about to clear your shortlist");
});

Then("they click the cancel link", () => {
  cy.get('[data-testid="cancel-link"]').click();
});

Then("they click confirm button", () => {
  cy.get('[data-testid="call-to-action"]').click();
});

When(
  "they programmatically add the first {int} results to their shortlist on the results page",
  (num) => {
    // We will do this programmatically so that it happens quickly enough to potentially cause a race condition
    cy.window().then((win) => {
      [
        ...win.document.querySelectorAll(
          `input[name="ShortlistedTuitionPartners"]`
        ),
      ]
        .slice(0, num)
        .forEach((_) => _.click());
    });
    cy.wait(500);
  }
);

Then("total amount of shortlisted TPs is {int}", (expectedTotal) => {
  cy.checkTotalTps(expectedTotal);
});
Then("{string} checkbox is unchecked", (tpName) => {
  cy.isCheckboxUnchecked(`[id="shortlist-cb-${kebabCase(tpName)}"]`);
});
Then("{string} checkbox is unchecked on its detail page", (tpName) => {
  cy.isCheckboxUnchecked(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`);
});
Then("the LA label text is {string}", (laLabelText) =>
  cy.checkLaLabelText(laLabelText)
);
Then("{string} checkbox is checked on its detail page", (tpName) => {
  cy.isCheckboxchecked(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`);
});

Then("there is {int} entry on the shortlist page", (count) => {
  cy.get("tbody th").should("have.length", count);
});

Then("{string} name link is clicked", (tpName) => cy.goToTpDetailPage(tpName));

Then("{string} is removed from the shortlist", (tpName) => {
  cy.get(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`).uncheck();
  cy.wait(200);
});

Then("they click Back to go back to the shortlist page", () => cy.clickBack());

Then("the shortlist page displays {string}", (expectedText) => {
  cy.get("[id='shortlist-no-tp-shortlisted']").should(
    "contain.text",
    removeExcessWhitespaces(removeNewLine(expectedText))
  );
});

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

When(
  "they add tp name {int} to their price comparison list on the results page",
  (entry) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get(
        `#compare-list-cb-${kebabCase(tplist.tpnames[`${entry}`])}`
      ).check();
      cy.wait(500);
    });
  }
);

When(
  "they add {string} to their price comparison list on the results page",
  (tpName) => {
    cy.get(`#compare-list-cb-${kebabCase(tpName)}`).check();
    cy.wait(500);
  }
);

When(
  "they remove tp name {int} from their price comparison list on the results page",
  (entry) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get(
        `#compare-list-cb-${kebabCase(tplist.tpnames[`${entry}`])}`
      ).uncheck();
    });
    cy.wait(500);
  }
);

Then(
  "tp name {int} is marked as added to the price comparison list on the results page",
  (entry) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get(
        `#compare-list-cb-${kebabCase(tplist.tpnames[`${entry}`])}`
      ).should("be.checked");
    });
  }
);

Then(
  "tp name {int} is not marked as added to the price comparison list on the results page",
  (entry) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get(
        `#compare-list-cb-${kebabCase(tplist.tpnames[`${entry}`])}`
      ).should("not.be.checked");
    });
  }
);

Then(
  "the price comparison list shows as having {int} entries on the results page",
  (numEntries) => {
    cy.get("#totalCompareListedTuitionPartners").should((el) =>
      expect(parseInt(el.text().trim())).to.equal(numEntries)
    );
  }
);

When(
  "they choose to view their price comparison list from the results page",
  () => {
    cy.get('[data-testid="my-compare-listed-tuition-partners-link"]').click({
      force: true,
    });
  }
);

Then(
  "the correct Local Authority District is shown for {string}",
  (district) => {
    const tuitionPartnersText = `Tuition partner for ${district}`;
    const tuitionPartnersTextPlural = `Tuition partners for ${district}`;
    cy.get('[data-testid="la-name"]')
      .invoke("text")
      .then((text) => {
        if (text.includes(tuitionPartnersText)) {
          expect(text).to.include(tuitionPartnersText);
        } else if (text.includes(tuitionPartnersTextPlural)) {
          expect(text).to.include(tuitionPartnersTextPlural);
        } else {
          throw new Error(
            `Neither "${tuitionPartnersText}" nor "${tuitionPartnersTextPlural}" found in text: "${text}"`
          );
        }
      });
  }
);

Then("tp name is entry {int} on the price comparison list page", (entry) => {
  cy.fixture("tplist").then(function (tplist) {
    const tpName = removeExcessWhitespaces(
      removeNewLine(tplist.tpnames[`${entry}`])
    );
    cy.get("tbody th")
      .eq(entry - 1)
      .then(($tbodyHeader) => {
        return removeExcessWhitespaces(removeNewLine($tbodyHeader.text()));
      })
      .should("equal", tpName);
  });
});

Then(
  "tp name {int} is entry {int} on the price comparison list page",
  (tpName, entry) => {
    cy.fixture("tplist").then(function (tplist) {
      const tp = removeExcessWhitespaces(
        removeNewLine(tplist.tpnames[`${tpName}`])
      );
      cy.get("tbody th")
        .eq(entry - 1)
        .then(($tbodyHeader) => {
          return removeExcessWhitespaces(removeNewLine($tbodyHeader.text()));
        })
        .should("equal", tp);
    });
  }
);

Then(
  "{string} is entry {int} on the price comparison list page",
  (tpName, entry) => {
    tpName = removeExcessWhitespaces(removeNewLine(tpName));
    cy.get("tbody th")
      .eq(entry - 1)
      .then(($tbodyHeader) => {
        return removeExcessWhitespaces(removeNewLine($tbodyHeader.text()));
      })
      .should("equal", tpName);
  }
);

Then("there are {int} entries on the price comparison list page", (count) => {
  Step(this, "the heading is 'Compare tuition partner prices'");
  cy.get("tbody th").should("have.length", count);
});

Then(
  "tp name {int} is entry {int} on the not available list on the price comparison list page",
  (tpName, entry) => {
    cy.fixture("tplist").then(function (tplist) {
      tpName = removeExcessWhitespaces(
        removeNewLine(tplist.tpnames[`${tpName}`])
      );
      cy.get('[data-testid="unavailable-tps"] li')
        .eq(entry - 1)
        .should("contain.text", tpName);
    });
  }
);

Then(
  "entry {int} on the price comparison list is the row tp name {int}, {string}, {string}, {string}",
  (entry, name, groupSizes, tuitionSetting, price) => {
    cy.fixture("tplist").then(function (tplist) {
      name = removeExcessWhitespaces(removeNewLine(tplist.tpnames[`${name}`]));
    });
    cy.get("tbody tr")
      .eq(entry - 1)
      .within(() => {
        cy.get("th").should((el) => expect(el.text().trim()).to.equal(name));
        cy.get("td")
          .eq(0)
          .should((el) => expect(el.text().trim()).to.equal(groupSizes));
        cy.get("td")
          .eq(1)
          .should((el) => expect(el.text().trim()).to.equal(tuitionSetting));
        cy.get("td")
          .eq(2)
          .should((el) => expect(el.text().trim()).to.equal(price));
      });
  }
);

When(
  "they choose to view the tp name {int} details from the price comparison list",
  (entry) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get("a").contains(tplist.tpnames[`${entry}`]).click();
    });
  }
);

Then("they choose to sort the price comparison list by price", () => {
  cy.get('[data-testid="compare-list-price-sort"]').click();
});

Then(
  "they choose to remove entry {int} on the price comparison list page",
  (entry) => {
    cy.get('tbody td [data-testid="remove-tp"]')
      .eq(entry - 1)
      .click();
  }
);

Then("they choose to click on clear price comparison list link", () => {
  cy.get('[data-testid="clear-compare-list-link"]').click();
});

Then(
  "they are taken to the clear price comparison list confirmation page",
  () => {
    cy.get("h1").should(
      "have.text",
      "You're about to clear your price comparison list"
    );
  }
);

Then("they click the cancel link", () => {
  cy.get('[data-testid="cancel-link"]').click();
});

Then("they click confirm button", () => {
  cy.get('[data-testid="call-to-action"]').click();
});

When(
  "they programmatically add the first {int} results to their price comparison list on the results page",
  (num) => {
    // We will do this programmatically so that it happens quickly enough to potentially cause a race condition
    cy.window().then((win) => {
      [
        ...win.document.querySelectorAll(
          `input[name="CompareListedTuitionPartners"]`
        ),
      ]
        .slice(0, num)
        .forEach((_) => _.click());
    });
    cy.wait(500);
  }
);

Then("total amount of price comparison list TPs is {int}", (expectedTotal) => {
  cy.checkTotalTps(expectedTotal);
});

Then("tp name {int} checkbox is unchecked", (entry) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.isCheckboxUnchecked(
      `#compare-list-cb-${kebabCase(tplist.tpnames[`${entry}`])}`
    );
  });
});

Then("tp name {int} checkbox is unchecked on its detail page", (tpName) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.isCheckboxUnchecked(
      `#compare-list-tpInfo-cb-${kebabCase(tplist.tpnames[`${tpName}`])}`
    );
  });
});

Then("the LA label text is {string}", (laLabelText) =>
  cy.checkLaLabelText(laLabelText)
);
Then("tp name {int} checkbox is checked on its detail page", (tpName) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.isCheckboxchecked(
      `#compare-list-tpInfo-cb-${kebabCase(tplist.tpnames[`${tpName}`])}`
    );
  });
});

Then("there is {int} entry on the price comparison list page", (count) => {
  cy.get("tbody th").should("have.length", count);
});

Then("tp name {int} name link is clicked", (tpName) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.goToTpDetailPage(tplist.tpnames[`${tpName}`]);
  });
});

Then("tp name {int} is removed from the price comparison list", (tpName) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.get(
      `[id="compare-list-tpInfo-cb-${kebabCase(tplist.tpnames[`${tpName}`])}"]`
    ).uncheck();
    cy.wait(200);
  });
});

Then("they click Back to go back to the price comparison list page", () =>
  cy.clickBack()
);

Then("the price comparison list page displays {string}", (expectedText) => {
  cy.get("[id='compare-list-no-tp-compare-listed']").should(
    "contain.text",
    removeExcessWhitespaces(removeNewLine(expectedText))
  );
});

Then(
  "{string} group size price comparison list refinement option is selected",
  (optionText) => {
    cy.get("[data-testid='compare-list-group-size-refine'] select").select(
      `${optionText}`
    );
    cy.wait(1000);
  }
);

Then(
  "{string} tuition setting price comparison list refinement option is selected",
  (optionText) => {
    cy.get("[data-testid='compare-list-tuition-setting-refine'] select").select(
      `${optionText}`
    );
    cy.wait(1000);
  }
);

Then(
  "{string} VAT price comparison list refinement option is selected",
  (optionText) => {
    cy.get("[data-testid='compare-list-show-vat-toggle'] select").select(
      `${optionText}`
    );
    cy.wait(1000);
  }
);

Then("the group size select option is {string}", (optionText) => {
  cy.get(
    "[data-testid='compare-list-group-size-refine'] select option:selected"
  ).should("contain.text", removeExcessWhitespaces(removeNewLine(optionText)));
});

Then("the tuition setting select option is {string}", (optionText) => {
  cy.get(
    "[data-testid='compare-list-tuition-setting-refine'] select option:selected"
  ).should("contain.text", removeExcessWhitespaces(removeNewLine(optionText)));
});

Then("the VAT select option is {string}", (optionText) => {
  cy.get(
    "[data-testid='compare-list-show-vat-toggle'] select option:selected"
  ).should("contain.text", removeExcessWhitespaces(removeNewLine(optionText)));
});

Then("the tp name {int} price is {string}", (entry, priceString) => {
  cy.fixture("tplist").then(function (tplist) {
    cy.get(
      `[data-testid="compare-list-price-${kebabCase(
        tplist.tpnames[`${entry}`]
      )}`
    ).should(
      "contain.text",
      removeExcessWhitespaces(removeNewLine(priceString))
    );
  });
});

Then(
  "the tp name {int} empty data reason is {string}",
  (entry, priceString) => {
    cy.fixture("tplist").then(function (tplist) {
      cy.get(
        `[data-testid="compare-list-empty-data-reason-${kebabCase(
          tplist.tpnames[`${entry}`]
        )}`
      ).should(
        "contain.text",
        removeExcessWhitespaces(removeNewLine(priceString))
      );
    });
  }
);

Given(
  "a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page",
  () => {
    Step(
      this,
      "a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'"
    );
    Step(
      this,
      "they add tp name 5 to their price comparison list on the results page"
    );
    Step(
      this,
      "they add tp name 1 to their price comparison list on the results page"
    );
    Step(
      this,
      "they add tp name 2 to their price comparison list on the results page"
    );
    Step(
      this,
      "they add tp name 6 to their price comparison list on the results page"
    );
    Step(
      this,
      "they add tp name 7 to their price comparison list on the results page"
    );
    Step(
      this,
      "they add tp name 8 to their price comparison list on the results page"
    );
    Step(
      this,
      "they choose to view their price comparison list from the results page"
    );
    Step(this, "there are 6 entries on the price comparison list page");
  }
);

Then(
  "the price comparison list key stage subjects label number {int} is {string}",
  (entry, keyStageAndSubjectsLabel) => {
    keyStageAndSubjectsLabel = removeExcessWhitespaces(
      removeNewLine(keyStageAndSubjectsLabel)
    );
    cy.get("[data-testid='compare-list-key-stage-subjects-label']")
      .eq(entry - 1)
      .then(($paragraph) => {
        return removeExcessWhitespaces(removeNewLine($paragraph.text()));
      })
      .should("equal", keyStageAndSubjectsLabel);
  }
);

Then("the price comparison list key stage subjects header is not shown", () => {
  cy.get("[data-testid='compare-list-key-stage-subjects-header']").should(
    "not.exist"
  );
});

When(
  "they scroll to the top of in page price comparison list page link",
  () => {
    cy.get(
      '[data-testid="my-compare-listed-tuition-partners-link"]'
    ).scrollIntoView();
  }
);

Then("the top fixed price comparison list page link is not visible", () => {
  cy.get('[data-testid="my-compare-listed-tuition-partners-link-top"]').should(
    "not.be.visible"
  );
});

When("they scroll to the first result", () => {
  cy.get('[data-testid="results-list-item"]').first().scrollIntoView();
});

Then("the top fixed price comparison list page link is visible", () => {
  cy.get('[data-testid="my-compare-listed-tuition-partners-link-top"]').should(
    "be.visible"
  );
});

Then(
  "the top fixed price comparison list shows as having {int} entries on the results page",
  (numEntries) => {
    cy.get("#total-compare-listed-tuition-partners-badge-top").should((el) =>
      expect(parseInt(el.text().trim())).to.equal(numEntries)
    );
  }
);

When(
  "they choose to view their price comparison list from the results page using the fixed price comparison list page link",
  () => {
    cy.get('[data-testid="my-compare-listed-tuition-partners-link-top"]').click(
      {
        force: true,
      }
    );
  }
);

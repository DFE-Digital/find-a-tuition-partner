import { When, Then } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase } from "../support/utils";

Then(
  "TP has not provided the information in the {string} section",
  (details) => {
    cy.get('[data-testid="contact-details"]').should(
      "not.contain.text",
      details,
      { matchCase: true }
    );
  }
);

Then("TP has provided full contact details", () => {
  cy.get('[data-testid="contact-details"]')
    .should("contain.text", "Website", { matchCase: true })
    .and("contain.text", "Phone number", { matchCase: true })
    .and("contain.text", "Email address", { matchCase: true })
    .and("contain.text", "Address", { matchCase: true });
});

Then("the search details are correct", () => {
  cy.location("search").should(
    "eq",
    "?Postcode=sk11eb&TuitionType=Any&Subjects=KeyStage1-English"
  );
});

Then("the search details include {string}", (subjects) => {
  subjects.split(",").forEach((element) => {
    cy.get(`input[id="${kebabCase(element.trim())}"]`).should("be.checked");
  });
});

Then("the payment details are hidden", () => {
  cy.get('[data-testid="payment-details"]').should("not.have.attr", "open");
});

Then("the tuition partner's website link is displayed", () => {
  cy.get("[data-testid=tuition-partner-website-link]").should("exist");
});

Then("the tuition partners website link starts with {string}", (prefix) => {
  cy.get("[data-testid=tuition-partner-website-link]")
    .invoke("attr", "href")
    .should("match", new RegExp(`^${prefix}`));
});

When("they click funding and reporting link", () => {
  cy.get('[data-testid="funding-guidance-1"]').click();
});

Then("they will see the funding reporting header", () => {
  cy.get('[data-testid="funding-reporting-header"]').should(
    "contain.text",
    "Funding and Reporting"
  );
});

Then("they will click the back link", () => {
  cy.get('[data-testid="back-link"]').click();
});

Then(
  "they redirects to the tuition partners website link with bright-heart-education",
  () => {
    cy.location("pathname").should(
      "eq",
      `/tuition-partner/bright-heart-education`
    );
  }
);

Then("the tuition partner locations covered table is not displayed", () => {
  cy.get('[data-testid="locations-covered-table"]').should("not.exist");
});

Then("the tuition partner locations covered table is displayed", () => {
  cy.get('[data-testid="locations-covered-table"]').should("exist");
});

Then("the tuition partner pricing table is not displayed", () => {
  cy.get('[data-testid="pricing-table"]').should("not.exist");
});

Then("the tuition partner pricing table is displayed", () => {
  cy.get('[data-testid="pricing-table"]').should("exist");
});

Then(
  "the tuition partner pricing table is displayed for {string}",
  (tuitionTypes) => {
    tuitionTypes.split(",").forEach((tuitionType) => {
      cy.get('[data-testid="pricing-table"]').should(
        "contain.text",
        tuitionType
      );
    });
  }
);

Then("the tuition partner full pricing tables are not displayed", () => {
  for (let i = 1; i < 5; i++) {
    cy.get(
      `[data-testid="full-pricing-table-in-school-key-stage-${i}"]`
    ).should("not.exist");
    cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`).should(
      "not.exist"
    );
  }
});

Then("the tuition partner full pricing tables are displayed", () => {
  for (let i = 1; i < 5; i++) {
    cy.get(
      `[data-testid="full-pricing-table-in-school-key-stage-${i}"]`
    ).should("exist");
    cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`).should(
      "exist"
    );
  }
});

Then(
  "the subjects covered by a tuition partner are in alphabetical order",
  () => {
    const stages = [
      "Key stage 1 - English, Maths and Science",
      "Key stage 2 - English, Maths and Science",
      "Key stage 3 - English, Humanities, Maths, Modern Foreign Languages and Science",
      "Key stage 4 - English, Humanities, Maths, Modern Foreign Languages and Science",
    ];

    stages.forEach((element) => {
      cy.get('[data-testid="results-subjects"]').first().contains(element);
    });
  }
);

Then("the tuition cost information states declares no differences", () => {
  cy.get('[data-testid="pricing-same-for-subjects').should("exist");
  cy.get('[data-testid="pricing-differences-for-subjects').should("not.exist");
});

Then("the tuition cost information states declares differences", () => {
  cy.get('[data-testid="pricing-differences-for-subjects').should("exist");
  cy.get('[data-testid="pricing-same-for-subjects').should("not.exist");
});

Then("all tuition partner details are populated correctly", () => {
  cy.get('[data-testid="results-subjects"] > li:first')
    .first()
    .invoke("text")
    .should("match", new RegExp("^Key stage \\d\\s\\-"));
  cy.get('[data-testid="type-of-tuition"]').first().should("not.be.empty");
  cy.get('[data-testid="results-description"]').first().should("not.be.empty");
  cy.get('[data-testid="organisation-type"]').first().should("not.be.empty");
});

Then("the logo is shown", () => {
  cy.get('[data-testid="tuition-partner-website-logo"]')
    .should("be.visible")
    .and(($img) => {
      expect($img[0].naturalWidth).to.be.greaterThan(0);
      expect($img[0].naturalWidth).to.be.equal(180);
    });
});

Then("the logo is not shown", () => {
  cy.get('[data-testid="tuition-partner-website-logo"]').should(
    "not.be.visible"
  );
});

Then("the LA name is not shown", () => {
  cy.get('[data-testid="la-name"]').should("not.exist");
});

Then("the LA name displayed is {string}", (laName) => {
  cy.get('[data-testid="la-name"]').should("contain.text", laName);
});

Then("the LA label text is {string}", (laLabelText) => {
  cy.checkLaLabelText(laLabelText);
});

Then("the user checks the {string} checkbox on its detail page", (tpName) => {
  cy.get(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`).check();
});

Then("the user unchecks the {string} checkbox", (tpName) => {
  cy.get(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`).uncheck();
});

Then("{string} checkbox is unchecked on its detail page", (tpName) => {
  cy.isCheckboxUnchecked(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`);
});

Then("{string} checkbox is checked on its detail page", (tpName) => {
  cy.isCheckboxchecked(`[id="shortlist-tpInfo-cb-${kebabCase(tpName)}"]`);
});

Then("the 'TuitionPartner' - {string} checkbox is checked", (tpName) => {
  cy.isCheckboxchecked(`[id="shortlist-cb-${kebabCase(tpName)}"]`);
});

Then("the 'TuitionPartner' - {string} checkbox is unchecked", (tpName) => {
  cy.isCheckboxUnchecked(`[id="shortlist-cb-${kebabCase(tpName)}"]`);
});

Then("total amount of shortlisted TPs is {int}", (expectedTotal) => {
  cy.checkTotalTps(expectedTotal);
});
Then("{string} checkbox is unchecked", (tpName) => {
  cy.isCheckboxUnchecked(`[id="shortlist-cb-${kebabCase(tpName)}"]`);
});

Then("{string} name link is clicked", (tpName) => {
  cy.goToTpDetailPage(tpName);
});

Then("they click Back to go back to the search results page", () => {
  cy.clickBack();
});

import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import { getJumpToLocationId, kebabCase, KeyStageSubjects } from "../utils";

Given(
  "a user has arrived on the 'Search results' page for {string}",
  (keyStageSubject) => {
    cy.visit(
      `/search-results?postcode=sk11eb&${KeyStageSubjects(
        "subjects",
        keyStageSubject
      )}`
    );
  }
);

Given(
  "a user has arrived on the 'Search results' page for {string} without a postcode",
  (keyStage) => {
    cy.visit(
      `/search-results?key-subjects=KeyStage1&subjects=KeyStage1-English`
    );
  }
);

Given(
  "a user has arrived on the 'Search results' page for {string} for postcode {string}",
  (keystages, postcode) => {
    const query = keystages
      .split(",")
      .map((s) => KeyStageSubjects("subjects", s.trim()))
      .join("&");
    cy.visit(`/search-results?Postcode=${postcode}&${query}`);
  }
);

Given(
  "a user has arrived on the 'Search results' page without subjects",
  () => {
    Step(
      this,
      "a user has arrived on the 'Search results' page without subjects for postcode 'sk11eb'"
    );
  }
);

Given(
  "a user has arrived on the 'Search results' page without subjects for postcode {string}",
  (postcode) => {
    cy.visit(`/search-results?Postcode=${postcode}`);
  }
);

Given(
  "a user has arrived on the 'Search results' page without subjects or postcode",
  () => {
    cy.visit(`/search-results`);
  }
);

Given("a user has arrived on the 'Search results' page", () => {
  cy.visit(
    `/search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&Subjects=KeyStage2-Science&Subjects=KeyStage3-English&Subjects=KeyStage3-Humanities&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage3-Science&Subjects=KeyStage4-English&Subjects=KeyStage4-Humanities&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&Subjects=KeyStage4-Science&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4`
  );
});

Then("they will see the tuition partner {string}", (tp) => {
  cy.contains("a", tp);
});

Given(
  "a user has arrived on the 'Tuition Partner' page for {string}",
  (name) => {
    cy.visit(`/tuition-partner/${kebabCase(name)}`);
  }
);

Given(
  "a user has arrived on the 'Tuition Partner' page for {string} after searching for {string}",
  (name, subjects) => {
    Step(
      this,
      `a user has arrived on the 'Tuition Partner' page for '${name}' after searching for '${subjects}' in postcode 'sk11eb'`
    );
  }
);

Given(
  "a user has arrived on the 'Tuition Partner' page for {string} after entering search details for multiple subjects",
  (name) => {
    Step(
      this,
      `a user has arrived on the 'Tuition Partner' page for '${name}' after searching for 'Key stage 1 English, Key stage 1 Maths' in postcode 'sk11eb'`
    );
  }
);

Given(
  "a user has arrived on the 'Tuition Partner' page for {string} after searching for {string} in postcode {string}",
  (name, subjects, postcode) => {
    cy.visit(
      `/search-results?${KeyStageSubjects(
        "Data.Subjects",
        subjects
      )}&Data.TuitionType=Any&Data.Postcode=${postcode}`
    );
    cy.get(".govuk-link").contains(name).click();
  }
);

When("they select subject {string}", (subject) => {
  cy.get(`input[id="${kebabCase(subject)}"]`).click();
});

When("the user selects tuition type {string}", (tutionType) => {
  cy.get(`input[id="${kebabCase(tutionType)}-tuition-types"]`).click();
});

When(
  "the user selects organisation grouping type {string}",
  (organisationTypeGrouping) => {
    cy.get(
      `input[id="${kebabCase(
        organisationTypeGrouping
      )}-organisation-types-grouping"]`
    ).click();
  }
);

When("they select the tuition partner {string}", (name) => {
  cy.get(".govuk-link").contains(name).click();
});

Then("they see the tuition types {string}", (tuitionTypes) => {
  const tuitionArray = tuitionTypes.split(",").map((s) => s.trim());

  cy.get("[data-testid='type-of-tuition']")
    .invoke("text")
    .then((text) => {
      var listedTuitionTypes = text
        .split("\n")
        .map((s) => s.trim())
        .filter((s) => s);
      expect(listedTuitionTypes).to.deep.equal(tuitionArray);
    });
});

Then("they see the cost for tuition type {string}", (tuitionTypes) => {
  const tuitionArray = tuitionTypes.split(",").map((s) => s.trim());

  tuitionArray.forEach((element) => {
    cy.get("[data-testid='pricing-table'] thead").contains("th", element);
  });

  cy.get("[data-testid='pricing-table'] thead th").should(
    "have.length",
    tuitionArray.length + 1
  );
});

Then("the search filters are displayed", () => {
  cy.get('[data-testid="results-filter"]').should("be.visible");
});

Then("the search filters are not displayed", () => {
  cy.get('[data-testid="results-filter"]').should("not.be.visible");
});

Then("the postcode search is displayed", () => {
  cy.get('[data-testid="postcode"]').should("be.visible");
});

Then("the search results are displayed", () => {
  cy.get('[data-testid="results-summary"]').should("be.visible");
  cy.get('[data-testid="results-list-item"]').should("be.visible");
});

Then("the search results page heading is {string}", (heading) => {
  cy.get("h1").find("span").filter(":visible").should("have.text", heading);
});

Then("all tuition partner parameters are populated correctly", () => {
  cy.get('[data-testid="results-subjects"] > li:first')
    .first()
    .should("contain.text", "Key");
  cy.get('[data-testid="type-of-tuition"]').first().should("not.be.empty");
  cy.get('[data-testid="results-description"]').first().should("not.be.empty");
});

When("they click on the option heading for {string}", (keystage) => {
  const stages = keystage.split(",").map((s) => s.trim());
  stages.forEach((element) => {
    cy.get(`#option-select-title-${kebabCase(element)}`).click();
  });
});

Then(
  "they can visit each TP details page and be returned back to the correct TP location",
  () => {
    cy.get('[data-testid="tuition-partner-name-link"]').each(($element) => {
      cy.isCorrectJumpToLocation($element);
    });
  }
);

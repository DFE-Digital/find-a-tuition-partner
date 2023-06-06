import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import "cypress-axe";

Given(
  "user navigates to the postcode confirmation page with postcode {string}",
  (postcode) => {
    cy.visit("/");
    Step(this, `they enter '${postcode}' as the school's postcode`);
    Step(this, "they click 'Continue'");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they select Any");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
  }
);

Then("they will be taken to the school selection page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/confirm-school");
  cy.get(".govuk-fieldset__heading").should(
    "contain.text",
    "Confirm which school you need tuition for"
  );
});

Then("they will be taken to the single school selection page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/confirm-school");
  cy.get(".govuk-heading-l").should(
    "contain.text",
    "Confirm this is the school you need tuition for"
  );
});

Then("they will see a list of schools with the same postcode", () => {
  cy.get(":nth-child(1) > .govuk-radios__label > .govuk-heading-m").should(
    "contain.text",
    "139242, Oasis Academy Boulton, B21 0RE"
  );
  cy.get(":nth-child(3) > .govuk-radios__label > .govuk-heading-m").should(
    "contain.text",
    "134102, James Watt Primary School, B21 0RE"
  );
});

Then("the user will select the first school in the list", () => {
  cy.get("#Data_SchoolId").check();
});

Then("more information about the school selected will appear", () => {
  cy.get("#conditional-Data_SchoolId").should("be.visible");
});

Then("the schools additional information shown will be:", (dataTable) => {
  dataTable.hashes().forEach((row, index) => {
    const expectedValues = {
      nameAndAddress: row["Name and address"],
      localAuthority: row["Local authority"],
      phaseOfEducation: row["Phase of education"],
      ids: row["IDs"],
    };

    const selectors = {
      nameAndAddress:
        "#conditional-Data_SchoolId > .govuk-summary-list > :nth-child(1) > .govuk-summary-list__value",
      localAuthority:
        "#conditional-Data_SchoolId > .govuk-summary-list > :nth-child(2) > .govuk-summary-list__value",
      phaseOfEducation:
        "#conditional-Data_SchoolId > .govuk-summary-list > :nth-child(3) > .govuk-summary-list__value",
      ids: "#conditional-Data_SchoolId > .govuk-summary-list > :nth-child(4) > .govuk-summary-list__value",
    };

    for (const [key, value] of Object.entries(expectedValues)) {
      cy.get(selectors[key]).should((element) => {
        const actualText = element
          .text()
          .replace(/\s+/g, " ")
          .replace(/[^\w\s]/g, "")
          .trim();
        const expectedText = value
          .replace(/\s+/g, " ")
          .replace(/[^\w\s]/g, "")
          .trim();
        expect(actualText).to.equal(expectedText);
      });
    }
  });
});

Then(
  "the second schools additional information shown will be:",
  (dataTable) => {
    dataTable.hashes().forEach((row, index) => {
      const expectedValues = {
        nameAndAddress: row["Name and address"],
        localAuthority: row["Local authority"],
        phaseOfEducation: row["Phase of education"],
        ids: row["IDs"],
      };

      const selectors = {
        nameAndAddress: `#conditional-Data_SchoolId-2 > .govuk-summary-list > :nth-child(1) > .govuk-summary-list__value`,
        localAuthority: `#conditional-Data_SchoolId-2 > .govuk-summary-list > :nth-child(2) > .govuk-summary-list__value`,
        phaseOfEducation: `#conditional-Data_SchoolId-2 > .govuk-summary-list > :nth-child(3) > .govuk-summary-list__value`,
        ids: `#conditional-Data_SchoolId-2 > .govuk-summary-list > :nth-child(4) > .govuk-summary-list__value`,
      };

      for (const [key, value] of Object.entries(expectedValues)) {
        cy.get(selectors[key]).should((element) => {
          const actualText = element
            .text()
            .replace(/\s+/g, " ")
            .replace(/[^\w\s]/g, "")
            .trim();
          const expectedText = value
            .replace(/\s+/g, " ")
            .replace(/[^\w\s]/g, "")
            .trim();
          expect(actualText).to.equal(expectedText);
        });
      }
    });
  }
);

When("the user selects the second school in the list", () => {
  cy.get("#Data_SchoolId-2").check();
});

Then("more information about the second school selected will appear", () => {
  cy.get("#conditional-Data_SchoolId-2").should("be.visible");
});

Then("there is an option to confirm the school", () => {
  cy.get(".govuk-fieldset").should("contain", "Is this your school?");
});

Then("the user clicks yes and continue", () => {
  cy.get("#Data_ConfirmedIsSchool").check();
  cy.get("form > .govuk-button-group > .govuk-button").click();
});

Then("the user clicks No I need to choose another school and continue", () => {
  cy.get("#Data_ConfirmedIsSchool-2").check();
  cy.get("form > .govuk-button-group > .govuk-button").click();
});

Then(
  "the user clicks I need to choose another school link and continues",
  () => {
    cy.get("form > .govuk-link").click();
  }
);

When("the user clicks continue", () => {
  cy.get("form > .govuk-button-group > .govuk-button").click();
});

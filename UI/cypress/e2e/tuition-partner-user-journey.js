import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import {
  kebabCase,
  camelCaseKeyStage,
  KeyStageSubjects,
} from "../support/utils";

When("the key stages are edited in the key stages page", () => {
  Step(this, "they select 'Key stage 1, Key stage 2, Key stage 3'");
  Step(this, "they click 'Continue'");
});

When(
  "the subjects are edited in the subjects page after key stage has been edited",
  () => {
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths, Key stage 3 Science'"
    );
    Step(this, "they click 'Continue'");
  }
);

When("the postcode is edited in the start page", () => {
  Step(this, "they enter 'YO11 1AA' as the school's postcode");
  Step(this, "they click 'Continue'");
});

When("the tuition type is changed", () => {
  cy.get(`input[id="in-school"]`).check();
});

Then("user has journeyed forward to a selected tuition partner page", () => {
  Step(this, "they enter 'SK1 1EB' as the school's postcode");
  Step(this, "they click 'Continue'");
  Step(this, "they will be taken to the 'Which key stages' page");
  Step(this, "they will see all the keys stages as options");
  Step(this, "they select 'Key stage 1, Key stage 2'");
  Step(this, "they click 'Continue'");
  Step(this, "they will be taken to the 'Which subjects' page");
  Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
  Step(
    this,
    "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
  );
  Step(this, "they click 'Continue'");
  Step(this, "they will be taken to the 'Search Results' page");
  Step(this, "the filter section will be correctly displayed");
  Step(this, "they select the tuition partner 'Tute Education'");
  Step(this, "the page's title is 'Tute Education'");
});

Then("the filter section will be correctly displayed", () => {
  const subjects = [
    "Key stage 1 English",
    "Key stage 1 Maths",
    "Key stage 2 English",
    "Key stage 2 Maths",
  ];
  subjects.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).check();
  });
  cy.get('[data-testid="postcode-input-box"]').should("have.value", "SK1 1EB");
  cy.get(`input[id="any"]`).should("be.checked");
});

Then("they will be journey back to the page they started from", () => {
  Step(this, "they click 'Back'");
  Step(this, "they will be taken to the 'Search Results' page");
  Step(this, "they click 'Back'");
  Step(this, "they will be taken to the 'Which subjects' page");
  Step(this, "they click 'Back'");
  Step(this, "they will be taken to the 'Which key stages' page");
  Step(this, "they click 'Back'");
  Step(this, "the page's title is 'Find a tuition partner'");
});

Then(
  "starting the journey again the postcode is correct in the Find a tuition partner page",
  () => {
    cy.get('[data-testid="postcode-input-box"]').should(
      "have.value",
      "SK1 1EB"
    );
  }
);

Then("the key stages are correct in the key stages page", () => {
  Step(this, "they click 'Continue'");
  Step(this, "stages 'Key stage 1, Key stage 2' are selected");
  Step(this, "stages 'Key stage 3, Key stage 4' are not selected");
  Step(this, "they click 'Continue'");
});

Then("stages 'Key stage 1, Key stage 2' are selected", () => {
  const subjects = ["Key stage 1", "Key stage 2"];
  subjects.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).check();
  });
});

Then("stages 'Key stage 3, Key stage 4' are not selected", () => {
  const subjects = ["Key stage 3", "Key stage 4"];
  subjects.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).should("not.be.checked");
  });
});

Then("the subjects are correct in the subjects page", () => {
  const subjects = [
    "Key stage 1 English",
    "Key stage 1 Maths",
    "Key stage 2 English",
    "Key stage 2 Maths",
  ];
  subjects.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).check();
  });

  const subjectsNotSelected = ["Key stage 1 Science", "Key stage 2 Science"];
  subjectsNotSelected.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).should("not.be.checked");
  });

  const subjectsDoNotExist = [
    "Key stage 3 English",
    "Key stage 3 Maths",
    "Key stage 3 Science",
    "Key stage 4 English",
    "Key stage 4 Maths",
    "Key stage 4 Science",
  ];
  subjectsDoNotExist.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).should("not.exist");
  });
});

Then("the filter selections are correct in the search results page", () => {
  Step(this, "they click 'Back'");
  Step(this, "stages 'Key stage 1, Key stage 2' are selected");
});

Then(
  "the filter selections are correct in the search results page with the edited selections",
  () => {
    const subjects = [
      "Key stage 1 English",
      "Key stage 1 Maths",
      "Key stage 2 English",
      "Key stage 2 Maths",
      "Key stage 3 Science",
    ];
    subjects.forEach((element) => {
      cy.get(`input[id=${kebabCase(element)}]`).check();
    });
    cy.get('[data-testid="postcode-input-box"]').should(
      "have.value",
      "YO11 1AA"
    );
    cy.get(`input[id="in-school"]`).should("be.checked");
  }
);

Then("user has entered {string} and journeyed forward to the {string} tuition partner page", (postcode, tp) => {
    Step(this, "they enter '" + postcode + "' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which key stages' page");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which subjects' page");
    Step(this, "they are shown the subjects for 'Key stage 2'");
    Step(
        this,
        "they select 'Key stage 2 English'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "they select the tuition partner '" + tp + "'");
    Step(this, "the page's title is '" + tp + "'");
});

Then("they journey back to the search page", () => {
    Step(this, "they click 'Back'");
    Step(this, "they will be taken to the 'Search Results' page");
});

Then("they update the postcode on the search page to {string} and go to a selected tuition partner page", (postcode) => {
    Step(this, "they enter '" + postcode + "' as the school's postcode");
    Step(this, "they click 'Search'");
    Step(this, "they select the tuition partner 'Tute Education'");
});

Then("they select the {string} tuition partner page", (tp) => {
    Step(this, "they select the tuition partner '" + tp + "'");
    Step(this, "the page's title is '" + tp + "'");
});

When("they click the 'All quality-assured tuition partners' link", () => {
    cy.get('[data-testid="full-list-link"]')
        .should("exist")
        .should("have.text", "All quality-assured tuition partners")
        .click();
});

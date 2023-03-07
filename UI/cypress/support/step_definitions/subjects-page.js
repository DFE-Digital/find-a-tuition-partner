import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage } from "../utils";

export const allSubjects = {
  "Key stage 1": ["English", "Maths", "Science"],
  "Key stage 2": ["English", "Maths", "Science"],
  "Key stage 3": [
    "English",
    "Humanities",
    "Maths",
    "Modern foreign languages",
    "Science",
  ],
  "Key stage 4": [
    "English",
    "Humanities",
    "Maths",
    "Modern foreign languages",
    "Science",
  ],
};

Given("a user has arrived on the 'Which key stages' page", () => {
  Step(
    this,
    "a user has arrived on the 'Which key stages' page for postcode 'AB12CD'"
  );
});

Given(
  "a user has arrived on the 'Which key stages' page for postcode {string}",
  (postcode) => {
    cy.visit(`/which-key-stages?Postcode=${postcode}`);
  }
);

Given(
  "a user has arrived on the 'Which subjects' page for {string}",
  (keystage) => {
    const query = keystage
      .split(",")
      .map((s) => `KeyStages=${camelCaseKeyStage(s.trim())}`)
      .join("&");
    cy.visit(`/which-subjects?${query}`);
  }
);

When("they select {string}", (keystage) => {
  const stages = keystage.split(",").map((s) => s.trim());
  stages.forEach((element) => {
    cy.get(`input[id=${kebabCase(element)}]`).check();
  });
});

Then("they are shown the subjects for {string}", (keystage) => {
  const stages = keystage.split(",").map((s) => s.trim());
  stages.forEach((element) => {
    const subjects = allSubjects[element];

    cy.get("legend")
      .contains(`${element} subjects`)
      .parent()
      .within(() => {
        cy.get('[data-testid="subject-name"]').each((item, index) => {
          cy.wrap(item).should("contain.text", subjects[index]);
        });
      });
  });
});

Then("they will see {string} selected", (keystages) => {
  const stages = keystages.split(",").map((s) => s.trim());
  stages.forEach((element) => {
    cy.get(`input[id="${kebabCase(element)}"]`).should("be.checked");
  });
});

Then("they will see all the keys stages as options", () => {
  const keyStages = Object.keys(allSubjects);

  cy.get('[data-testid="key-stage-name"]').should(
    "have.length",
    keyStages.length
  );

  cy.get('[data-testid="key-stage-name"]').each((item, index) => {
    cy.wrap(item).should("contain.text", keyStages[index]);
  });
});



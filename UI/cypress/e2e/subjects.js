import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage } from "../support/utils";
import { allSubjects } from "../support/step_definitions/subjects-page";

Given("the 'Which subjects' page is displayed", () => {
  cy.visit(
    "/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4"
  );
});


When("they manually navigate to the 'Which subjects' page", () => {
  cy.visit("/which-subjects");
});

Then("they are shown all the subjects under all the keys stages", () => {
  Step(this, "they are shown the subjects for 'Key stage 1'");
  Step(this, "they are shown the subjects for 'Key stage 2'");
  Step(this, "they are shown the subjects for 'Key stage 3'");
  Step(this, "they are shown the subjects for 'Key stage 4'");
});

Then("the subjects are displayed in alphabetical order", () => {
  Step(this, "they are shown the subjects for 'Key stage 1'");
  Step(this, "they are shown the subjects for 'Key stage 2'");
  Step(this, "they are shown the subjects for 'Key stage 3'");
  Step(this, "they are shown the subjects for 'Key stage 4'");
});

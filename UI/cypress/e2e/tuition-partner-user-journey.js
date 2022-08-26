import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";
import { kebabCase, camelCaseKeyStage, KeyStageSubjects } from "../support/utils";

Then("they will be able journey forward to a selected tuition partner page", () => {
    Step(this, "they enter 'SK1 1EB' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which key stages' page");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which subjects' page");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(this, "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "the filter section will be correctly displayed");
    Step(this, "they select the tuition partner 'Tute Education'");
    Step(this, "the page's title is 'Tute Education'");
  });

  Then("the filter section will be correctly displayed", () => {

    const subjects = ['Key stage 1 English', 'Key stage 1 Maths', 'Key stage 2 English', 'Key stage 2 Maths'];
    subjects.forEach(element => {
        cy.get(`input[id=${kebabCase(element)}]`).check();
    });

    cy.get('[data-testid="postcode-input-box"]').should('have.value', 'SK1 1EB');
    cy.get(`input[id="any"]`).should('be.checked');
});

Then("they will be journey back to the page the started from", () => {
    Step(this, "they click 'Back'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "they click 'Back'");
    Step(this, "they will be taken to the 'Which subjects' page");
    Step(this, "they click 'Back'");
    Step(this, "they will be taken to the 'Which key stages' page");
    Step(this, "they click 'Back'");
    Step(this, "the page's title is 'Find a tuition partner'");
});

Then("starting the journey again the postcode is correct in the Find a tuition partner page", () => {
  cy.get('[data-testid="postcode-input-box"]').should('have.value', 'SK1 1EB');
});

Then("And the key stages are correct in the key stages page", () => {
  Step(this, "they click 'Continue'");
  Step(this, "stages 'Key stage 1, Key stage 2' are selected");
  Step(this, "they click 'Continue'");
});

  


import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

When("user has journeyed forward to the check your answers page", () => {
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
  Step(this, "they will be taken to the 'Type of tuition' page");
  Step(this, "they select Any");
  Step(this, "they click 'Continue'");
  Step(this, "they will be taken to the 'Search Results' page");
  Step(this, "they click 'Make an enquiry' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they enter a valid email address");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for tuition plan");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for SEND requirements");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for other requirements");
  Step(this, "they click 'Continue'");
});

Then("they click {string} button", () => {
  cy.get(".app-print-hide > .govuk-button").click();
});

Then("they enter a valid email address", () => {
  cy.get("#Data_Email").type("email@email.com");
});

Then("they click 'Send enquiry'", () => {
  cy.get("form > .govuk-button").click();
});

let refNumOne;
Then("a unique reference number is shown", () => {
  cy.get(".govuk-panel__body > strong")
    .invoke("text")
    .then((referenceNumber) => {
      const pattern = /^[A-Z]{2}\d{4}$/;
      expect(pattern.test(referenceNumber)).to.be.true;
      refNumOne = referenceNumber;
    });
});

Then("the page has title Request sent", () => {
  cy.get(".govuk-panel__title").should("contain.text", "Request sent");
});

Then("they enter an answer for tuition plan", () => {
  cy.get('.govuk-label').should("contain.text", "What type of tuition plan do you need?")
  cy.get('#Data_TutoringLogistics').type("enquiry");
});

Then("they enter an answer for SEND requirements", () => {
  cy.get('.govuk-label').should("contain.text", "Do you need tuition partners who can support pupils with SEND? (optional)")
  cy.get('#Data_SENDRequirements').type("enquiry");
});


Then("they enter an answer for other requirements", () => {
  cy.get('.govuk-label').should("contain.text", "Is there anything else that you want tuition partners to consider? (optional)")
  cy.get('#Data_AdditionalInformation').type("enquiry");
});


Then("click first change button", () => {
  cy.get(":nth-child(1) > .govuk-summary-list__actions > .govuk-link").click();
});

When("they click back on browser", () => {
  cy.go("back");
});

Then("the session timeout page is shown", () => {
  cy.location("pathname").should("eq", "/session/timeout");
});

When("user creates another enquiry", () => {
  cy.visit("/");
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
  Step(this, "they will be taken to the 'Type of tuition' page");
  Step(this, "they select Any");
  Step(this, "they click 'Continue'");
  Step(this, "they will be taken to the 'Search Results' page");
  Step(this, "they click 'Make an enquiry' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they enter a valid email address");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for tuition plan");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for SEND requirements");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for other requirements");
  Step(this, "they click 'Continue'");
});

Then("the second reference number is dfferent to the first", () => {
  cy.get(".govuk-panel__body > strong")
    .invoke("text")
    .then((referenceNumber) => {
      const pattern = /^[A-Z]{2}\d{4}$/;
      expect(pattern.test(referenceNumber)).to.be.true;
      refNumOne !== referenceNumber;
    });
});

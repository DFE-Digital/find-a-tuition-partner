import {
  Given,
  When,
  And,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

When("user has journeyed forward to the check your answers page", () => {
  Step(this, "they enter 'OX4 2AU' as the school's postcode");
  Step(this, "they click 'Continue'");
  Step(this, "they select 'Key stage 1, Key stage 2'");
  Step(this, "they click 'Continue'");
  Step(
    this,
    "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
  );
  Step(this, "they click 'Continue'");
  Step(this, "they select Any");
  Step(this, "they click 'Continue'");
  Step(this, "they click 'Start now' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they will be taken to the single school selection page");
  Step(this, "the user clicks yes and continue");
  Step(this, "they enter a valid email address");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for tuition plan");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for SEND requirements");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for other requirements");
  Step(this, "they click 'Continue'");
});

Then("they click {string} button", (buttonText) => {
  cy.get(".govuk-button").contains(buttonText).click();
});

Then("they enter a valid email address", () => {
  cy.get("#Data_Email")
    .clear()
    .type("simulate-delivered@notifications.service.gov.uk");
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
  cy.get(".govuk-label").should(
    "contain.text",
    "What type of tuition plan do you need?"
  );
  cy.get("#Data_TutoringLogistics").type("enquiry");
});

Then("they enter an answer for SEND requirements", () => {
  cy.get(".govuk-label").should(
    "contain.text",
    "Do you need tuition partners who can support pupils with SEND? (optional)"
  );
  cy.get("#Data_SENDRequirements").type("enquiry");
});

Then("they enter an answer for other requirements", () => {
  cy.get(".govuk-label").should(
    "contain.text",
    "Is there anything else that you want tuition partners to consider? (optional)"
  );
  cy.get("#Data_AdditionalInformation").type("enquiry");
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
  Step(this, "they enter 'OX4 2AU' as the school's postcode");
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
  Step(this, "they click 'Start now' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they will be taken to the single school selection page");
  Step(this, "the user clicks yes and continue");
  Step(this, "they click 'Continue'");
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

Then("they click 'Continue' on enquiry", () => {
  cy.get(".govuk-grid-column-two-thirds > .govuk-button").click();
});

Then("they enter an invalid email address", () => {
  cy.get("#Data_Email").clear().type("email.email.com");
});

Then("the email address is visible in input field", () => {
  cy.get("#Data_Email").should(
    "have.value",
    "simulate-delivered@notifications.service.gov.uk"
  );
});

Then("click the link on text {string}", (linkText) => {
  cy.get(".govuk-details__summary-text").should("contain.text", linkText);
});

Then(
  "they will see the correct error message for an invalid email address",
  () => {
    cy.get(".govuk-error-summary__body").should(
      "contain.text",
      "You must enter an email address in the correct format.  Emails are usually in a format, like, username@example.com"
    );
  }
);

When("user navigates to the first enquiry question", () => {
  cy.visit("/");
  Step(this, "they enter 'OX4 2AU' as the school's postcode");
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
  Step(this, "they click 'Start now' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they will be taken to the single school selection page");
  Step(this, "the user clicks yes and continue");
  Step(this, "they enter a valid email address");
  Step(this, "they click 'Continue'");
});

When("they type {string} characters for question 1", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_TutoringLogistics").clear().invoke("val", totalText);
});

When("they type {string} characters for question 2", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_SENDRequirements").clear().invoke("val", totalText);
});

When("they type {string} characters for question 3", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_AdditionalInformation").clear().invoke("val", totalText);
});

Then("the text by the second and third questions is {string}", (text) => {
  cy.get(":nth-child(4) > .govuk-summary-list__value").should(
    "contain.text",
    "Not specified"
  );
  cy.get(":nth-child(5) > .govuk-summary-list__value").should(
    "contain.text",
    "Not specified"
  );
});

Then(
  "the warning should be displayed showing they have {string} characters left",
  (numOfCharsLeft) => {
    const expectedText = `You have ${numOfCharsLeft} characters remaining`;

    cy.get(".govuk-character-count__status")
      .invoke("text")
      .should((text) => {
        const cleanText = text.replace(/,\s*/g, "");
        expect(cleanText).to.contain(expectedText);
      });
  }
);

Then(
  "the warning should be displayed showing they are over by {string} characters",
  (numOfCharsOver) => {
    const expectedText = `You have ${numOfCharsOver} characters too many`;

    cy.get(".govuk-character-count__status")
      .invoke("text")
      .should((text) => {
        const cleanText = text.replace(/,\s*/g, "");
        expect(cleanText).to.contain(expectedText);
      });
  }
);

Then("they select terms and conditions", () => {
  cy.get("#Data_ConfirmTermsAndConditions").check();
});

When("user navigates to check your answers unselecting filter results", () => {
  cy.visit("/");
  Step(this, "they enter 'OX4 2AU' as the school's postcode");
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
  Step(this, "they unselect filter results");
  Step(this, "they click 'Start now' button");
  Step(this, "they click 'Continue' button");
  Step(this, "they will be taken to the single school selection page");
  Step(this, "the user clicks yes and continue");
  Step(this, "they enter a valid email address");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for tuition plan");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for SEND requirements");
  Step(this, "they click 'Continue'");
  Step(this, "they enter an answer for other requirements");
  Step(this, "they click 'Continue'");
});

Then("they unselect filter results", () => {
  cy.get("#key-stage-1-english").click();
  cy.get("#key-stage-2-english").click();
  cy.get("#key-stage-1-maths").click();
  cy.get("#key-stage-2-maths").click();
  cy.wait(500);
});

When("they enter an email address causing an error", () => {
  cy.get("#Data_Email").clear().type("email+1@invalid");
});

When(
  "user has journeyed forward to the check your answers page for an invalid email address",
  () => {
    Step(this, "they enter 'OX4 2AU' as the school's postcode");
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
    Step(this, "they click 'Start now'");
    Step(this, "they click 'Continue' button");
    Step(this, "they will be taken to the single school selection page");
    Step(this, "the user clicks yes and continue");
    Step(this, "they enter an email address causing an error");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for tuition plan");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for SEND requirements");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for other requirements");
    Step(this, "they click 'Continue'");
    Step(this, "they select terms and conditions");
    Step(this, "they click send enquiry");
  }
);

import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("the 'Tuition setting' page is displayed", () => {
  cy.visit(
    "which-tuition-settings?From=SearchResults&Postcode=wc1h8hl&Subjects=KeyStage3-Maths&Subjects=KeyStage3-Modern%20foreign%20languages&Subjects=KeyStage4-Maths&Subjects=KeyStage4-Modern%20foreign%20languages&KeyStages=KeyStage3&KeyStages=KeyStage4"
  );
});

Then("they select subjects for the key stages", () => {
  cy.get("#key-stage-3-maths").click();
  cy.get("#key-stage-3-modern-foreign-languages").click();
  cy.get("#key-stage-4-maths").click();
  cy.get("#key-stage-4-modern-foreign-languages").click();
});

Then("they will be taken to the tuition setting page", () => {
  cy.get("h1")
    .invoke("text")
    .then((text) => {
      const trimmedText = text.trim();
      expect(trimmedText).to.equal("What tuition setting do you prefer?");
    });
});

Then("the correct options will display", () => {
  cy.get(".govuk-radios > :nth-child(1)").contains("Face-to-face");
  cy.get(".govuk-radios > :nth-child(2)").contains("Online");
  cy.get(".govuk-radios > :nth-child(3)").contains(
    "Both face-to-face and online"
  );
  cy.get(".govuk-radios > :nth-child(4)").contains("No preference");
  cy.get("#no-preference").should("be.enabled");
  cy.get("#online").should("be.enabled");
  cy.get("#face-to-face").should("be.enabled");
});

When("user clicks the button with text {string}", (type) => {
  const formattedType = type.replace(/\s/g, "-").toLowerCase();
  cy.get(`#${formattedType}`).click();
});

When(
  "the {string} button is selected and the other two buttons are not",
  (type) => {
    let formattedType = type.replace(/\s/g, "-").toLowerCase();
    if (formattedType == "no-preference") {
      cy.get(`#${formattedType}`).should("be.checked");
      cy.get("#online").should("not.be.checked");
      cy.get("#face-to-face").should("not.be.checked");
    } else if (formattedType == "online") {
      cy.get(`#${formattedType}`).should("be.checked");
      cy.get("#no-preference").should("not.be.checked");
      cy.get("#face-to-face").should("not.be.checked");
    } else if (formattedType == "face-to-face") {
      cy.get(`#${formattedType}`).should("be.checked");
      cy.get("#no-preference").should("not.be.checked");
      cy.get("#online").should("not.be.checked");
    }
  }
);

Then("the filter results show the expected selection", () => {
  cy.get("#face-to-face").should("be.checked");
});

Then("they will be taken to the 'Tuition setting' page", () => {
  cy.visit(
    "/which-tuition-settings?Postcode=SK1%201EB&TuitionSetting=NoPreference&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage2-English&Subjects=KeyStage2-Maths&TuitionSetting=Any&KeyStages=KeyStage1&KeyStages=KeyStage2"
  );
});

Then("they select No preference", () => {
  cy.get("#no-preference").check();
});

import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";
import "../commands";

Then("the Check Your Answers page displays the following:", (dataTable) => {
  dataTable.hashes().forEach((row) => {
    switch (row["Section Name"]) {
      case "Key stages and subjects":
        cy.checkTextContent(
          ":nth-child(1) > .govuk-summary-list__key",
          "Key stages and subjects"
        );
        cy.checkTextContent(
          ".govuk-list > :nth-child(1)",
          "Key stage 1 - English and Maths"
        );
        cy.checkTextContent(
          ".govuk-list > :nth-child(2)",
          "Key stage 2 - English and Maths"
        );
        break;
      case "Type of Tuition":
        cy.checkTextContent(
          ":nth-child(2) > .govuk-summary-list__value",
          row["Expected Content"]
        );
        break;
      case "Email Address":
        cy.checkTextContent(
          ":nth-child(3) > .govuk-summary-list__value",
          row["Expected Content"]
        );
        break;
      case "Tuition Plan":
        cy.checkTextContent(
          ":nth-child(4) > .govuk-summary-list__value",
          row["Expected Content"]
        );
        break;
      case "SEND Support":
        cy.checkTextContent(
          ":nth-child(5) > .govuk-summary-list__value",
          row["Expected Content"]
        );
        break;
      case "Other Considerations":
        cy.checkTextContent(
          ":nth-child(6) > .govuk-summary-list__value",
          row["Expected Content"]
        );
        break;
      default:
        throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
    }
  });
});

When("the user clicks the change button {string}", (x) => {
  const num = x;
  cy.get(
    `:nth-child(${x}) > .govuk-summary-list__actions > .govuk-link`
  ).click();
});

When(
  "the user is taken back to the key-stage page to change their selection",
  () => {
    cy.location("pathname").should("eq", "/which-key-stages");
    cy.get("#key-stage-1").should("be.checked");
    cy.get("#key-stage-2").should("be.checked");
  }
);

When("they select Key stage 3", () => {
  cy.get("#key-stage-3").check();
});

Then("the user is taken to the subjects page to change their selection", () => {
  cy.location("pathname").should("eq", "/which-subjects");
  cy.get("#key-stage-1-english").should("be.checked");
  cy.get("#key-stage-2-english").should("be.checked");
  cy.get("#key-stage-1-maths").should("be.checked");
  cy.get("#key-stage-2-maths").should("be.checked");
});

Then("they select science for all three key stages", () => {
  cy.get("#key-stage-1-science").check();
  cy.get("#key-stage-2-science").check();
  cy.get("#key-stage-3-science").check();
});

Then(
  "the Check Your Answers page displays the following with the key stage and subjects updates:",
  (dataTable) => {
    dataTable.hashes().forEach((row) => {
      switch (row["Section Name"]) {
        case "Key stages and subjects":
          cy.checkTextContent(
            ":nth-child(1) > .govuk-summary-list__key",
            "Key stages and subjects"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(1)",
            "Key stage 1 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(2)",
            "Key stage 2 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(3)",
            "Key stage 3 - Science"
          );
          break;
        case "Type of Tuition":
          cy.checkTextContent(
            ":nth-child(2) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Email Address":
          cy.checkTextContent(
            ":nth-child(3) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Tuition Plan":
          cy.checkTextContent(
            ":nth-child(4) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "SEND Support":
          cy.checkTextContent(
            ":nth-child(5) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Other Considerations":
          cy.checkTextContent(
            ":nth-child(6) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        default:
          throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
      }
    });
  }
);

Then("the user is taken back to the type of tuition page", () => {
  cy.location("pathname").should("eq", "/which-tuition-types");
  cy.get("#any").should("be.checked");
});

Then(
  "the Check Your Answers page displays the following with the type of tuition update:",
  (dataTable) => {
    dataTable.hashes().forEach((row) => {
      switch (row["Section Name"]) {
        case "Key stages and subjects":
          cy.checkTextContent(
            ":nth-child(1) > .govuk-summary-list__key",
            "Key stages and subjects"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(1)",
            "Key stage 1 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(2)",
            "Key stage 2 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(3)",
            "Key stage 3 - Science"
          );
          break;
        case "Type of Tuition":
          cy.checkTextContent(
            ":nth-child(2) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Email Address":
          cy.checkTextContent(
            ":nth-child(3) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Tuition Plan":
          cy.checkTextContent(
            ":nth-child(4) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "SEND Support":
          cy.checkTextContent(
            ":nth-child(5) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Other Considerations":
          cy.checkTextContent(
            ":nth-child(6) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        default:
          throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
      }
    });
  }
);

Then("the user is taken back to the email address page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/enquirer-email");
  cy.get("#Data_Email").should("have.value", "email@email.com");
});

Then(
  "the Check Your Answers page displays the following with the email address update:",
  (dataTable) => {
    dataTable.hashes().forEach((row) => {
      switch (row["Section Name"]) {
        case "Key stages and subjects":
          cy.checkTextContent(
            ":nth-child(1) > .govuk-summary-list__key",
            "Key stages and subjects"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(1)",
            "Key stage 1 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(2)",
            "Key stage 2 - English, Maths and Science"
          );
          cy.checkTextContent(
            ".govuk-list > :nth-child(3)",
            "Key stage 3 - Science"
          );
          break;
        case "Type of Tuition":
          cy.checkTextContent(
            ":nth-child(2) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Email Address":
          cy.checkTextContent(
            ":nth-child(3) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Tuition Plan":
          cy.checkTextContent(
            ":nth-child(4) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "SEND Support":
          cy.checkTextContent(
            ":nth-child(5) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        case "Other Considerations":
          cy.checkTextContent(
            ":nth-child(6) > .govuk-summary-list__value",
            row["Expected Content"]
          );
          break;
        default:
          throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
      }
    });
  }
);

When("they enter another email address", () => {
  cy.get("#Data_Email").clear().type("new@new.com");
});

Then("the user clears the current sessions data", () => {
  Cypress.session.clearCurrentSessionData();
  Cypress.session.clearCurrentSessionData();
  cy.wait(500)
});

Then("they will see the correct error for not checking the t and c", () => {
  const expectedErrorText =
    "Select to confirm that you have not included any information that would allow anyone to identify pupils, such as names or specific characteristics";
  cy.get(".govuk-error-summary__list li a")
    .contains(expectedErrorText)
    .first()
    .should("be.visible")
    .invoke("text")
    .then((text) => expect(text.trim()).to.equal(expectedErrorText));
});

Then(
  "the correct error message should display for no keystage and subjects selected",
  () => {
    cy.get('.govuk-error-summary__list > li > a').should("contain.text", "Select at least one key stage and related subject")

  }
);

Then("the key stages and subjects show {string}", (keystageandsubjects) => {
  cy.get(":nth-child(1) > .govuk-summary-list__value").should(
    "contain.text",
    keystageandsubjects
  );
});


import {
  Given,
  When,
  Then,
  And,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

let firstStatementUrl;

Given(
  "a tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
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
    Step(this, "they select terms and conditions");
    Step(this, "they click send enquiry");
    cy.get(":nth-child(11) > a").click();
    cy.url().then((url) => {
      firstStatementUrl = url;
    });
  }
);

Given("a tuition partner has arrived on respond to an enquiry page", () => {
  cy.visit(firstStatementUrl);
});

Given("A school has arrived on view all responses page", () => {
  console.log(firstStatementUrl);
  cy.visit(firstStatementUrl);
});

Then(
  "the page heading should show School Enquiry from {string} area",
  (LAD) => {
    cy.contains(".govuk-caption-l", LAD);
    cy.contains(
      ".govuk-heading-l > span",
      "View the school’s tuition requirements"
    );
  }
);

Then("the page should display You have {string} days to respond", (x) => {
  cy.get(".govuk-inset-text > .govuk-body")
    .invoke("text")
    .then((text) =>
      expect(text).to.equal(` You have  ${x} days  to respond to this enquiry`)
    );
});

Then("the responses should have heading {string}", (respondHeading) => {
  cy.contains(".govuk-heading-m > span", respondHeading);
});

Then("the first response section is to be Key stage and subjects", () => {
  cy.contains(
    ".govuk-grid-column-two-thirds-from-desktop > :nth-child(2) > span",
    "Key stage and subjects:"
  );
});

Then("the key stages and subjects should match the request:", (dataTable) => {
  dataTable.hashes().forEach((row) => {
    switch (row["Section Name"]) {
      case "Key stages and subjects":
        cy.checkTextContent(
          ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(1)",
          "Key stage 1: English and Maths"
        );
        cy.checkTextContent(
          ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(2)",
          "Key stage 2: English and Maths"
        );
    }
  });
});

Then(
  "the second response section is to be {string} with Type {string}",
  (header, text) => {
    cy.contains(":nth-child(6) > span", header);
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(7)",
      text
    );
  }
);

Then(
  "the third response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(":nth-child(10) > span", header);
    cy.contains(":nth-child(11) > .display-pre-wrap", text);
  }
);

Then(
  "the fourth response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(":nth-child(14) > span", header);
    cy.contains(":nth-child(15) > .display-pre-wrap", text);
  }
);

Then(
  "the last response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(":nth-child(18) > span", header);
    cy.contains(":nth-child(19) > .display-pre-wrap", text);
  }
);

Then(
  "the Other considerations section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(":nth-child(16) > span", header);
    cy.contains(":nth-child(17) > .display-pre-wrap", text);
  }
);

Then("they type {string} characters for section 1", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_KeyStageAndSubjectsText").clear().invoke("val", totalText);
});

Then("they type {string} characters for section 2", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_TuitionTypeText").clear().invoke("val", totalText);
});

Then("they type {string} characters for section 3", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_TutoringLogisticsText").clear().invoke("val", totalText);
});

Then("they type {string} characters for section 4", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_SENDRequirementsText").clear().invoke("val", totalText);
});

Then("they type {string} characters for section 5", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_AdditionalInformationText").clear().invoke("val", totalText);
});

Then("the error message shows {string}", (errorText) => {
  cy.get(".govuk-error-summary__list > li > a").should(
    "contain.text",
    errorText
  );
});

Given(
  "a tuition partner clicks a magic link with no info for optional inputs",
  () => {
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
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Continue'");
    Step(this, "they select terms and conditions");
    Step(this, "they click send enquiry");
    cy.get(":nth-child(11) > a").click();
  }
);

Then(
  "the check your answers page does not include SEND and Other considerations",
  () => {
    cy.get(".govuk-summary-list__key").should(
      "not.have.text",
      "SEND requirements"
    );
    cy.get(".govuk-summary-list__key").should(
      "not.have.text",
      "Other school considerations"
    );
  }
);

Then("the page has the correct content information", () => {
  cy.get(":nth-child(5) > strong").should(
    "contain.text",
    "Contact information"
  );
});

And(
  "the page shows contact information such as the following:",
  (dataTable) => {
    dataTable.hashes().forEach((column) => {
      switch (column["Section Name"]) {
        case "email:":
          cy.contains(column["Value"]).should("be.visible");
          break;
        case "contact number:":
          cy.contains(column["Value"]).should("be.visible");
          break;
        default:
          throw new Error(
            `Unrecognized section name: ${column["Section Name"]}`
          );
      }
    });
  }
);

When("they click return to your enquiry list", () => {
  cy.get(":nth-child(8) > .govuk-link").click();
});

let secondStatementUrl;
Then("the user has arrived on the tuition response confirmation page", () => {
  cy.location("pathname").should(
    "eq",
    "/enquiry/respond/response-confirmation"
  );
  cy.url().then((url) => {
    secondStatementUrl = url;
  });
});

Given("A school has arrived on view all responses page with a response", () => {
  cy.visit(secondStatementUrl);
  cy.get(":nth-child(12) > a").click();
});

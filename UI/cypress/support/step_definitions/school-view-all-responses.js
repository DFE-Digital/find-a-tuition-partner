import {
  Given,
  When,
  And,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given("a school views a tuition partners response", () => {
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
  Step(this, "they select terms and conditions");
  Step(this, "they click send enquiry");
  cy.get(":nth-child(11) > a").click();
  cy.get("#Data_KeyStageAndSubjectsText").clear().invoke("val", 80);
  cy.get("#Data_TuitionTypeText").clear().invoke("val", 80);
  cy.get("#Data_TutoringLogisticsText").clear().invoke("val", 80);
  cy.get("#Data_SENDRequirementsText").clear().invoke("val", 80);
  cy.get("#Data_AdditionalInformationText").clear().invoke("val", 80);
  cy.get('[data-testid="call-to-action"]').click();
  Step("this", "they click 'Submit'");
  cy.get("p a").eq(1).click();
});

And("there is text {string}", (text) => {
  cy.get(".govuk-details__summary-text").should("contain.text", text);
});

When("the user clicks Your tuition requirements", () => {
  cy.get(".govuk-details__summary-text").click();
});

Then("The correct enquiry information is shown as follows:", (dataTable) => {
  dataTable.hashes().forEach((row, index) => {
    switch (row["Enquiry information"]) {
      case "This is the tuition requirements you asked for:":
        cy.get(".govuk-details__text").should(
          "contain.text",
          row["Enquiry information"]
        );
        break;
      case "Key stage 1: English and Maths":
        cy.get(".govuk-list > :nth-child(1)").should(
          "contain.text",
          row["Enquiry information"]
        );
        break;
      case "Key stage 2: English and Maths":
        cy.get(".govuk-list > :nth-child(2)").should(
          "contain.text",
          row["Enquiry information"]
        );
        break;
      case "Any":
        cy.get(".govuk-list > :nth-child(3)").should(
          "contain.text",
          row["Enquiry information"]
        );
        break;
      case "enquiry":
        cy.get(".govuk-list > :nth-child(4)").should(
          "contain.text",
          row["Enquiry information"]
        );
        break;
      default:
        throw new Error(`Unexpected SectionName: ${row["Section Name"]}`);
    }
  });
});

And(
  "there should be text stating the amount of tuition partners that responded",
  () => {
    cy.contains(
      "#main-content > :nth-child(3) > :nth-child(1)",
      "1 out of 46 tuition partners have responded at the moment."
    );
  }
);

Then(
  "The tuition partner responses are shown at the bottom of the page as follows:",
  (dataTable) => {
    dataTable.hashes().forEach((column) => {
      switch (column["Column"]) {
        case "Date":
          const currentDate = new Date();
          const currentDay = currentDate.getDate();
          const currentMonth = currentDate.getMonth() + 1; // Month is zero-indexed, so add 1
          const currentYear = currentDate.getFullYear();
          // Get the date value from the table
          cy.get(".govuk-table__body > .govuk-table__row > :nth-child(1)").then(
            ($cell) => {
              const tableDate = $cell.text();
              // Extract the day, month, and year values from the table date
              const tableDay = parseInt(tableDate.split("/")[0], 10);
              const tableMonth = parseInt(tableDate.split("/")[1], 10);
              const tableYear = parseInt(tableDate.split("/")[2], 10);
              // Assert that the table date matches the current date
              expect(tableDay).to.equal(currentDay);
              expect(tableMonth).to.equal(currentMonth);
              expect(tableYear).to.equal(currentYear);
            }
          );
          break;
        case "Tuition Partner":
          cy.get(
            ".govuk-table__body > .govuk-table__row > :nth-child(2)"
          ).should("contain.text", column["Value"]);
          break;
        case "Their Response":
          cy.get(
            ".govuk-table__body > .govuk-table__row > :nth-child(3)"
          ).should("contain.text", column["Value"]);
          break;
        default:
          throw new Error(`Unexpected column name: ${column["Column"]}`);
      }
    });
  }
);

When("they click View response from a tuition partner", () => {
  cy.get(".govuk-table__body > .govuk-table__row > :nth-child(3)").click();
});

When(
  "they click all the view your tuition requirement links the text shows",
  () => {
    cy.get(
      ":nth-child(14) > .govuk-details__summary > .govuk-details__summary-text"
    ).click();
    cy.get(
      ":nth-child(19) > .govuk-details__summary > .govuk-details__summary-text"
    ).click();
    cy.get(
      ":nth-child(24) > .govuk-details__summary > .govuk-details__summary-text"
    ).click();
  }
);

Then("the heading of the page is View Sherpa Online's response", () => {
  cy.contains(".govuk-heading-l > span", "View Sherpa response");
});

Then("the response page has the following information:", (dataTable) => {
  dataTable.hashes().forEach((row) => {
    const requirement = row["Requirement"];
    const yourResponse = row["Your Response"];
    const sherpaResponse = row["Sherpa Online's Response"];

    switch (requirement) {
      case "Key stage and subjects:":
        cy.get(
          ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(1)"
        )
          .should("contain.text", "Key stage 1: English and Maths")
          .should("be.visible");
        cy.get(
          ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(2)"
        )
          .should("contain.text", "Key stage 2: English and Maths")
          .should("be.visible");
        cy.get(
          ":nth-child(3) > .govuk-grid-row > .govuk-grid-column-two-thirds-from-desktop > :nth-child(6)"
        )
          .should("contain.text", sherpaResponse)
          .should("be.visible");
        break;
      case "Tuition type:":
        cy.get(".govuk-grid-column-two-thirds-from-desktop > :nth-child(9)")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(".govuk-grid-column-two-thirds-from-desktop > :nth-child(11)")
          .should("contain.text", sherpaResponse)
          .should("be.visible");
        break;
      case "Tuition plan:":
        cy.get('[open=""] > .govuk-details__text > .display-pre-wrap')
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(".govuk-grid-column-two-thirds-from-desktop > :nth-child(16)")
          .should("contain.text", sherpaResponse)
          .should("be.visible");
        break;
      case "Can you support SEND?:":
        cy.get(":nth-child(19) > .govuk-details__text")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(21)")
          .should("contain.text", sherpaResponse)
          .should("be.visible");
        break;
      case "Other tuition considerations?:":
        cy.get(":nth-child(24) > .govuk-details__text")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(26)")
          .should("contain.text", sherpaResponse)
          .should("be.visible");
        break;
      default:
        throw new Error(`Unexpected requirement: ${requirement}`);
    }
  });
});

And("they click contact tuition partner", () => {
  cy.get(".govuk-grid-column-two-thirds-from-desktop > .govuk-button").click();
});

And("they click cancel on the response page", () => {
  cy.get(".govuk-grid-column-two-thirds-from-desktop > .govuk-link").click();
});

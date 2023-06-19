import {
  Given,
  When,
  And,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given("a school views a tuition partners response", () => {
  Step(this, "a school clicks the magic link to view their enquiry");
});

And("there is text {string}", (text) => {
  cy.get(".govuk-details__summary-text").should("contain.text", text);
});

When("the user clicks Your tuition requirements", () => {
  cy.get(".govuk-details__summary-text").click();
});

Then("The correct enquiry information is shown as follows:", (dataTable) => {
  dataTable.hashes().forEach((row, index) => {
    switch (row["Question"]) {
      case "Key stage and subjects":
        cy.get(".govuk-summary-list__value > ul > :nth-child(1)").should(
          "contain.text",
          "Key stage 1: English and Maths"
        );

        cy.get(".govuk-summary-list__value > ul > :nth-child(2)").should(
          "contain.text",
          "Key stage 2: English and Maths"
        );
        break;
      case "Tuition setting":
        cy.get(":nth-child(2) > .govuk-summary-list__value").should(
          "contain.text",
          row["Your Requirements"]
        );
        break;
      case "Tuition plan":
        cy.get(":nth-child(3) > .govuk-summary-list__value").should(
          "contain.text",
          row["Your Requirements"]
        );
        break;
      case "Can you support SEND":
        cy.get(":nth-child(4) > .govuk-summary-list__value").should(
          "contain.text",
          row["Your Requirements"]
        );
        break;
      case "Other tuition considerations":
        cy.get(":nth-child(5) > .govuk-summary-list__value").should(
          "contain.text",
          row["Your Requirements"]
        );
        break;
      default:
        throw new Error(`Unexpected Question: ${row["Question"]}`);
    }
  });
});

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
          let tpName1;
          let tpName2;
          cy.get(".govuk-table__body > .govuk-table__row > :nth-child(2)")
            .invoke("text")
            .then((text) => {
              tpName1 = text.trim();
            });
          cy.get(".govuk-table__body > .govuk-table__row > :nth-child(3)")
            .invoke("text")
            .then((text) => {
              tpName2 = text.trim();
              expect(tpName2).to.contain(tpName1);
            });
          break;
        default:
          throw new Error(`Unexpected column name: ${column["Column"]}`);
      }
    });
  }
);

When("they click View response from a tuition partner", () => {
  cy.get('[data-testid="view-enquiry-response-link"]').click();
});

Then("the heading of the page is View Sherpa Online's response", () => {
  cy.contains(".govuk-heading-l > span", "View Sherpa response");
});

Then("the response page has the following information:", (dataTable) => {
  dataTable.hashes().forEach((row) => {
    const requirement = row["Requirement"];
    const yourResponse = row["Your Response"];
    const tpResponse = row["{TP's name}'s Response"];

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
          ":nth-child(2) > .govuk-grid-row > .govuk-grid-column-two-thirds-from-desktop > :nth-child(7)"
        )
          .should("contain.text", tpResponse)
          .should("be.visible");
        break;
      case "Tuition setting:":
        cy.get(".govuk-grid-column-two-thirds-from-desktop > :nth-child(11)")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(".govuk-grid-column-two-thirds-from-desktop > :nth-child(13)")
          .should("contain.text", tpResponse)
          .should("be.visible");
        break;
      case "Tuition plan:":
        cy.get(":nth-child(1) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(2) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(3) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(4) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(19) > .display-pre-wrap")
          .should("contain.text", tpResponse)
          .should("be.visible");
        break;
      case "Can you support SEND?:":
        cy.get(":nth-child(23) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(25) > .display-pre-wrap")
          .should("contain.text", tpResponse)
          .should("be.visible");
        break;
      case "Other tuition considerations?:":
        cy.get(":nth-child(29) > .display-pre-wrap")
          .should("contain.text", yourResponse)
          .should("be.visible");
        cy.get(":nth-child(31) > .display-pre-wrap")
          .should("contain.text", tpResponse)
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

When("they click return to your enquiry list", () => {
  cy.get(".govuk-body.app-print-hide > .govuk-link").click();
});

import {
  Given,
  When,
  And,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

let refrenceNumber;
Given(
  "a school clicks the magic link to respond to a view all responses",
  () => {
    cy.visit(
      "https://localhost:7036/enquiry/respond/response-confirmation?SupportReferenceNumber=SJ4798&EnquirerMagicLink=FmeOs4T1Y9uTU0Q8rR3Qo9xDopWyHBBD5GKuukQHsUoevwBuM3ftXccCD22NBmdLITdbrgKl84xRd%2FOCFvcRReP0w3Nv%2BKVrsehSIboFCk4f%2FVYPMLNbPxTij7fnvEkED7mce6UnnPga%2FfEYNbLyMQ%3D%3D"
    );
    cy.get(".govuk-panel__body > strong")
      .invoke("text")
      .then((text) => (refrenceNumber = text));
    cy.get(":nth-child(12) > a").click();
  }
);

And("the enquiry response page contains the same reference number", () => {
  cy.get(".govuk-caption-l").should("contain.text", refrenceNumber);
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
          cy.get(
            ".govuk-table__body > .govuk-table__row > :nth-child(1)"
          ).should("contain.text", column["Value"]);
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

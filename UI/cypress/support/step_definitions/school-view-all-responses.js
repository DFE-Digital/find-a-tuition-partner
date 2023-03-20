import {
  Given,
  When,
  And,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Given(
  "a school clicks the magic link to respond to a view all responses",
  () => {
    cy.visit(
      "https://localhost:7036/enquiry/respond/all-enquirer-responses?token=FmeOs4T1Y9uTU0Q8rR3Qo9xDopWyHBBD5GKuukQHsUoevwBuM3ftXccCD22NBmdLA/KFHjsJoDJowJOBMIyZ1e6%2BmTfB4oOhXiEoPRaWQkdDctDHdjwbWf101abpTHwmZslOxT8/2VJ68PzrpztsrA=="
    );
  }
);

Then(
  "the page should have text stating the date and time the enquiry was made",
  () => {
    cy.get(".govuk-grid-column-full > :nth-child(5)")
      .should("be.visible")
      .should("have.text", "Created on 1:42pm on Friday 17 March");
  }
);

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

import {
  Given,
  When,
  Then,
  And,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Then(
  "the read only page heading should show School Enquiry from {string} area",
  (LAD) => {
    cy.contains(".govuk-caption-l > strong", LAD);
    cy.contains(".govuk-heading-l", "View the school");
    cy.contains(".govuk-heading-l", "s tuition requirements");
  }
);

Then("the page should display the reference number for the enquiry", () => {
  const refNumRegex = /Reference number: ([a-zA-Z]{2})(\d{4})/;

  cy.get("[data-testid='ref-num']")
    .invoke("text")
    .then((text) => {
      expect(text).to.match(refNumRegex); // check that the text matches the regex
    });
});

Then("the page should display the number of TPs sent to", () => {
  const numberOfTPsRegex = /(\d{1,2}) tuition partners received this enquiry/;

  cy.get("[data-testid='sent-to']")
    .invoke("text")
    .then((text) => {
      expect(text).to.match(numberOfTPsRegex); // check that the text matches the regex
    });
});

Then(
  "the confirm decline page heading should show School Enquiry from {string} area",
  (LAD) => {
    cy.contains(".govuk-caption-l > strong", LAD);
    cy.contains(".govuk-heading-l", "You are about to decline this enquiry");
  }
);

Then(
  "the declined confirmation page heading should show School Enquiry from {string} area",
  (LAD) => {
    cy.contains(".govuk-caption-l > strong", LAD);
    cy.contains(".govuk-heading-l", "You have declined this enquiry");
  }
);

Then(
  "the edit page heading should show School Enquiry from {string} area",
  (LAD) => {
    cy.contains(".govuk-caption-l > strong", LAD);
    cy.contains(
      ".govuk-heading-l",
      "Responding to the schools tuition requirements"
    );
  }
);

Then(
  "the page should display the correct date format for the response deadline",
  () => {
    const deadlineRegex =
      /You have until (\d{1,2}):(\d{2})(am|pm|AM|PM) on ([a-zA-Z]+) (\d{1,2}) ([a-zA-Z]+) (\d{4}) to respond to this enquiry/;

    cy.get(".govuk-inset-text")
      .invoke("text")
      .then((text) => {
        expect(text).to.match(deadlineRegex); // check that the text matches the regex

        const matches = text.match(deadlineRegex); // extract the parts of the date from the text
        const hour = parseInt(matches[1]);
        const minute = parseInt(matches[2]);
        const ampm = matches[3];
        const day = parseInt(matches[5]);
        const month = matches[6];
        const year = parseInt(matches[7]);

        expect(hour).to.be.within(1, 12); // check that the hour is valid
        expect(minute).to.be.within(0, 59); // check that the minute is valid
        expect(ampm).to.be.oneOf(["am", "pm", "AM", "PM"]); // check that the am/pm indicator is valid
        expect(day).to.be.within(1, 31); // check that the day is valid
        expect(month).to.be.oneOf([
          "January",
          "February",
          "March",
          "April",
          "May",
          "June",
          "July",
          "August",
          "September",
          "October",
          "November",
          "December",
        ]); // check that the month is valid
        expect(year).to.equal(new Date().getFullYear()); // check that the year is the current year
      });
  }
);

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
          ".govuk-grid-column-two-thirds-from-desktop > :nth-child(3) > :nth-child(1)",
          "Key stage 1: English and Maths"
        );
        cy.checkTextContent(
          ":nth-child(3) > :nth-child(2)",
          "Key stage 2: English and Maths"
        );
    }
  });
});

Then(
  "the second response section is to be {string} with Type {string}",
  (header, text) => {
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(6)",
      header
    );
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(7)",
      text
    );
  }
);

Then(
  "the third response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(10)",
      header
    );
    cy.contains(":nth-child(1) > .display-pre-wrap", text);
    cy.contains(":nth-child(2) > .display-pre-wrap", text);
    cy.contains(":nth-child(3) > .display-pre-wrap", text);
    cy.contains(":nth-child(4) > .display-pre-wrap", text);
  }
);

Then(
  "the fourth response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(14)",
      header
    );
    cy.contains(":nth-child(15) > .display-pre-wrap", text);
  }
);

Then(
  "the last response section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(18)",
      header
    );
    cy.contains(":nth-child(19) > .display-pre-wrap", text);
  }
);

Then(
  "the Other considerations section is to be {string} with text {string}",
  (header, text) => {
    cy.contains(
      ".govuk-grid-column-two-thirds-from-desktop > :nth-child(16)",
      header
    );
    cy.contains(":nth-child(17) > .display-pre-wrap", text);
  }
);

Then("they type {string} characters for section 1", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_KeyStageAndSubjectsText").clear().invoke("val", totalText);
});

Then("they type {string} characters for section 2", (numOfChars) => {
  const totalText = "a".repeat(numOfChars);
  cy.get("#Data_TuitionSettingText").clear().invoke("val", totalText);
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
  cy.get(".govuk-caption-l").should(
    "contain.text",
    `Reference number ${
      Cypress.$(".govuk-caption-l")
        .text()
        .match(/[A-Z]{2}\d{4}/)[0]
    }`
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

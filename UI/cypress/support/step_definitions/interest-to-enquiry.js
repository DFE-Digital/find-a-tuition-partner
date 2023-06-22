import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

When("a school navigates to the tp response page", () => {
  cy.get('[data-testid="enquirer-magic-link"]').click();
});

Then(
  "the school should see the tuition partner has responded with a defaut unread status",
  () => {
    cy.get(":nth-child(3) > .table-column-sort").should(
      "contain.text",
      "Your interest"
    );
    cy.get(".enquiry-status-box").should("contain.text", "UNREAD");
  }
);

Then(
  "the school selects the first tuition partner and should see the tuition partners response",
  () => {
    cy.get(
      ':nth-child(1) > :nth-child(2) > [data-testid="view-enquiry-response-link"]'
    ).click();
  }
);

Then(
  "the school should have options to select {string}, {string} or {string}",
  (interested, notInterested, undecided) => {
    cy.get("button.govuk-button").should("contain.text", interested);
    cy.get(".govuk-button--secondary").should("contain.text", notInterested);
    cy.get("form > .govuk-body > .govuk-link").should(
      "contain.text",
      undecided
    );
  }
);

Then("the school clicks {string}", (interested) => {
  cy.get("button.govuk-button").should("contain.text", interested).click();
  cy.get(".govuk-body.app-print-hide > .govuk-link").click();
});

Then("the status of the response should be updated to {string}", (status) => {
  cy.get(":nth-child(1) > :nth-child(3) > .enquiry-status-box").should(
    "contain.text",
    status
  );
});

Then("the school has selected {string} for a response", (notInterested) => {
  cy.get(".govuk-button--secondary")
    .should("contain.text", notInterested)
    .click();
});

When("the school clicks the {string} button", () => {
  cy.get("form > .govuk-button-group > .govuk-button").click();
});

Then("the school submits feedback", () => {
  cy.get("#Data_NotInterestedFeedback").type("This is a test feedback");
  cy.get("form > .govuk-button-group > .govuk-button").click();
});

Then("there is text {string} visible", (text) => {
  cy.get("#main-content > :nth-child(6)").should("contain.text", text);
});

Then("the school clicks the {string} link", () => {
  cy.get("form > .govuk-body > .govuk-link").click();
});

Then("the status of the response is {string}", (text) => {
  cy.get(":nth-child(1) > :nth-child(3) > .enquiry-status-box").should(
    "contain.text",
    text
  );
});

Then("the school chooses to skip the feedback", () => {
  cy.get("form > .govuk-button-group > .govuk-link").click();
});

Then("the school selects to filter options by tuition partner response", () => {
  cy.get(":nth-child(2) > .table-column-sort").click();
});

Then("the tuition partners should be in ascending alphabetical order", () => {
  cy.get('[data-testid="view-enquiry-response-link"]').then(($el) => {
    // Create an array of tuition partner names
    const partnerNames = $el
      .text()
      .trim()
      .split("\n")
      .map((name) => name.trim())
      .filter((name) => name !== ""); // Remove any empty strings

    // Create a sorted copy of this array in descending order
    const sortedNames = [...partnerNames].sort();

    // Check that the original array is sorted in descending order
    for (let i = 0; i < partnerNames.length; i++) {
      expect(partnerNames[i]).to.equal(sortedNames[i]);
    }
  });
});

Then("the tuition partners should be in descending alphabetical order", () => {
  cy.get('[data-testid="view-enquiry-response-link"]').then(($el) => {
    // Create an array of tuition partner names
    const partnerNames = $el
      .text()
      .trim()
      .split("\n")
      .map((name) => name.trim())
      .filter((name) => name !== ""); // Remove any empty strings

    // Create a sorted copy of this array in descending order
    const sortedNames = [...partnerNames].sort().reverse();

    // Check that the original array is sorted in descending order
    for (let i = 0; i < partnerNames.length; i++) {
      expect(partnerNames[i]).to.equal(sortedNames[i]);
    }
  });
});

When("the school selects to filter options by your interest", () => {
  cy.get(":nth-child(3) > .table-column-sort").click();
});

Then("the tuition partners who responded positively show first", () => {
  cy.get(".enquiry-status-box").then(($els) => {
    const allResponses = $els
      .map((index, el) => Cypress.$(el).text().trim())
      .get();
    const firstResponse = allResponses[0];
    expect(firstResponse).to.equal("INTERESTED");
    const lastResponse = allResponses[allResponses.length - 1];
    expect(lastResponse).to.equal("UNREAD");
  });
});

Then("the tuition partners who haven't read show first", () => {
  cy.get(".enquiry-status-box").then(($els) => {
    const allResponses = $els
      .map((index, el) => Cypress.$(el).text().trim())
      .get();
    const firstResponse = allResponses[0];
    expect(firstResponse).to.equal("UNREAD");
    const lastResponse = allResponses[allResponses.length - 1];
    expect(lastResponse).to.equal("INTERESTED");
  });
});

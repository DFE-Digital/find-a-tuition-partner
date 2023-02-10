import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

Then("the heading caption is {string}", (expected) => {
  cy.get(".govuk-caption-l")
    .first()
    .should((caption) => expect(caption.text().trim()).to.equal(expected));
});

Then("the heading is {string}", (expected) => {
  cy.get(".govuk-heading-l")
    .first()
    .should((caption) => expect(caption.text().trim()).to.equal(expected));
});

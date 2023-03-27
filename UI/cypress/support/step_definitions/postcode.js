import {
  Given,
  When,
  Then,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

When("they enter {string} as the school's postcode", (postcode) => {
  cy.get("#Data_Postcode").type(`{selectAll}{del}${postcode}`);
});

When("they click on the postcode error", () => {
  cy.get('a[href="#Data_Postcode"]').click();
});

Then("the school's postcode text input is focused", () => {
  cy.focused().should("have.attr", "name", "Data.Postcode");
});

Then("they will see {string} entered for the postcode", (postcode) => {
  cy.get('[data-testid="postcode"]>input').should("contain.value", postcode);
});

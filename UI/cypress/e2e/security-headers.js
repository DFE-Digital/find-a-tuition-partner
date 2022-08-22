import {Then} from "@badeball/cypress-cucumber-preprocessor";

Then("header {string} is added to the displayed page", (header) => {
    cy.intercept('/which-subjects?Postcode=sk11eb&KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3&KeyStages=KeyStage4', (req) => {
      req.on('response', (res) => {
              expect(res.headers).to.have.property(header) //assert Request header
      })
    })
});

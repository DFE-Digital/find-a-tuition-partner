import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has started the 'Find a tuition partner' journey", () => {
    cy.visit("/");
});

When("they set the {string} query string parameter value to {string}", (key, value) => {
    cy.location('pathname').then((pathName) => {
        const separator = pathName.includes('?') === true ? '&' : '?';
        const url = `${pathName}${separator}${key}=${value}`;
        cy.visit(url);
    });
});

Then("they will be taken to the 'Compare national tutoring options' page", () => {
    cy.location('pathname').should('eq', '/options');
});

Then("they will be taken to the 'Find a tuition partner' journey start page", () => {
    cy.location('pathname').should('eq', '/');
});

Then("they will be taken to the 'Which key stages' page", () => {
    cy.location('pathname').should('eq', '/which-key-stages');
});

Then("they will be taken to the 'Which subjects' page", () => {
    cy.location('pathname').should('eq', '/which-subjects');
});

Then("the page URL ends with {string}", url => {
    cy.location('pathname').should('match', new RegExp(`${url}$`));
});

Then("the heading should say {string}", (heading) => {
    cy.get("h1").should("have.text", heading);
});

Then("the page's title is {string}", (title) => {
    cy.title().should("eq", title);
});

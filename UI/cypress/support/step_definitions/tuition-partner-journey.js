import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a tuition partner clicks the magic link to respond to a schools enquiry", () => {
    cy.visit("/enquiry/respond/response?token=YbyXWr3a39wY7Ah7atZ0%2BlGuYsqx21KDv6E4%2BMhPpnH1vv6kT9tbrbb/dbwcBTVwSf3v6R%2BzlSTwgogSK0Ab%2BhHyegPFXppNgqZVWCgzkQ5kqDq6lmBCioHQJT5Ds1u21Ei/s346p3oKGCn6NFxRWQ==")
})


Then("the page heading should show School Enquiry from {string} area", (LAD) => {
    cy.contains('.govuk-caption-l', LAD)
    cy.contains('.govuk-heading-l > span', "View the school’s tuition requirements")
})

Then("the page should display You have {string} days to respond", (x) => {
    cy.get('.govuk-inset-text > .govuk-body').invoke("text").then((text) => expect(text).to.equal(` You have  ${x} days  to respond to this enquiry`))
})

Then("the responses should have heading {string}", (respondHeading) => {
    cy.contains('.govuk-heading-m > span', respondHeading)
})

Then("the first response section is to be Key stage and subjects", () => {
    cy.contains('.govuk-grid-column-two-thirds-from-desktop > :nth-child(2) > span', "Key stage and subjects:")
})

Then(
    "the key stages and subjects should match the request:",
    (dataTable) => {
        dataTable.hashes().forEach((row) => {
            switch (row["Section Name"]) {
                case "Key stages and subjects":
                    cy.checkTextContent(
                        ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(1)",
                        "Key stage 1: English and Maths"
                    );
                    cy.checkTextContent(
                        ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(2)",
                        "Key stage 2: English and Maths"
                    )
            }
        })
    })

Then(
    "the key stages and subjects should match the new request:",
    (dataTable) => {
        dataTable.hashes().forEach((row) => {
            switch (row["Section Name"]) {
                case "Key stages and subjects":
                    cy.checkTextContent(
                        ".govuk-grid-column-two-thirds-from-desktop > .govuk-list > :nth-child(1)",
                        "Key stage 4: Science"
                    );

            }
        })
    })

Then("the second response section is to be {string} with Type {string}", (header, text) => {
    cy.contains(':nth-child(6) > span', header)
    cy.contains('.govuk-grid-column-two-thirds-from-desktop > :nth-child(7)', text)
})

Then("the third response section is to be {string} with text {string}", (header, text) => {
    cy.contains(':nth-child(10) > span', header)
    cy.contains(':nth-child(11) > .display-pre-wrap', text)
})

Then("the fourth response section is to be {string} with text {string}", (header, text) => {
    cy.contains(':nth-child(14) > span', header)
    cy.contains(':nth-child(15) > .display-pre-wrap', text)
})

Then("the last response section is to be {string} with text {string}", (header, text) => {
    cy.contains(':nth-child(18) > span', header)
    cy.contains(':nth-child(19) > .display-pre-wrap', text)
})

Then("the Other considerations section is to be {string} with text {string}", (header, text) => {
    cy.contains(':nth-child(16) > span', header)
    cy.contains(':nth-child(17) > .display-pre-wrap', text)
})

function typeCharactersInSection(sectionId, numOfChars) {
    const totalText = "a".repeat(numOfChars);
    cy.get(sectionId).clear().invoke("val", totalText);
}

Then("they type {string} characters for section 1", (numOfChars) => {
    const totalText = "a".repeat(numOfChars);
    cy.get('#Data_KeyStageAndSubjectsText').clear().invoke("val", totalText);
})

Then("they type {string} characters for section 2", (numOfChars) => {
    const totalText = "a".repeat(numOfChars);
    cy.get('#Data_TuitionTypeText').clear().invoke("val", totalText);
})

Then("they type {string} characters for section 3", (numOfChars) => {
    const totalText = "a".repeat(numOfChars);
    cy.get('#Data_TutoringLogisticsText').clear().invoke("val", totalText);
})

Then("they type {string} characters for section 4", (numOfChars) => {
    const totalText = "a".repeat(numOfChars);
    cy.get('#Data_SENDRequirementsText').clear().invoke("val", totalText);
})

Then("they type {string} characters for section 5", (numOfChars) => {
    const totalText = "a".repeat(numOfChars);
    cy.get('#Data_AdditionalInformationText').clear().invoke("val", totalText);
})


Then("the error message shows {string}", (errorText) => {
    cy.get('.govuk-error-summary__list > li > a').should("contain.text", errorText)
})

Given("a tuition partner clicks a magic link with no info for optional inputs", () => {
    cy.visit("https://localhost:7036/enquiry/respond/response?token=YbyXWr3a39wY7Ah7atZ0%2BlGuYsqx21KDv6E4%2BMhPpnFqg2LchHfmOUh%2BkQRNlYzN20cNUQHH5iSbQJ2qGC2f1bRvD9uyxwuQVuPHjnf7Rcu4UuGjI6htRPzT9wPWFcrMGa0HpbTt0V/SIZOpoq7mqbGBMre%2B0w8jy1diHTCJcxflLLi7RhPHrEMOldP08rFx")
})

Then("the check your answers page does not include SEND and Other considerations", () => {
    cy.get(".govuk-summary-list__key").should("not.have.text", "SEND requirements")
    cy.get(".govuk-summary-list__key").should("not.have.text", "Other school considerations")

})
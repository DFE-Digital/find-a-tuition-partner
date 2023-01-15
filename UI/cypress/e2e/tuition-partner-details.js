import {
    Given,
    When,
    Then,
    Step,
} from "@badeball/cypress-cucumber-preprocessor";
import {
    kebabCase,
    camelCaseKeyStage,
    KeyStageSubjects, removeNewLine,
} from "../support/utils";

When("they click 'What is a quality assured tuition partner?'", () => {
    cy.get('[data-testid="qatp-details"]').click();
});

Then(
    "TP has not provided the information in the {string} section",
    (details) => {
        cy.get('[data-testid="contact-details"]').should(
            "not.contain.text",
            details,
            {matchCase: true}
        );
    }
);

Then("TP has provided full contact details", () => {
    cy.get('[data-testid="contact-details"]')
        .should("contain.text", "Website", {matchCase: true})
        .and("contain.text", "Phone number", {matchCase: true})
        .and("contain.text", "Email address", {matchCase: true})
        .and("contain.text", "Address", {matchCase: true});
});

Then("the search details are correct", () => {
    cy.location("search").should(
        "eq",
        "?Postcode=sk11eb&TuitionType=Any&Subjects=KeyStage1-English"
    );
});

Then("the search details include {string}", (subjects) => {
    subjects.split(",").forEach((element) => {
        cy.get(`input[id="${kebabCase(element.trim())}"]`).should("be.checked");
    });
});

Then("the quality assured tuition partner details are hidden", () => {
    cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("the payment details are hidden", () => {
    cy.get('[data-testid="payment-details"]').should("not.have.attr", "open");
});

Then("the quality assured tuition partner details are shown", () => {
    cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
});

Then("the tuition partner's website link is displayed", () => {
    cy.get("[data-testid=tuition-partner-website-link]").should("exist");
});

Then("the tuition partners website link starts with {string}", (prefix) => {
    cy.get("[data-testid=tuition-partner-website-link]")
        .invoke("attr", "href")
        .should("match", new RegExp(`^${prefix}`));
});

When("they click funding and reporting link", () => {
    cy.get('[data-testid="funding-guidance-1"]').click();
});

Then("they will see the funding reporting header", () => {
    cy.get('[data-testid="funding-reporting-header"]').should(
        "contain.text",
        "Funding and Reporting"
    );
});

Then("they will click the back link", () => {
    cy.get('[data-testid="back-link"]').click();
});

Then(
    "they redirects to the tuition partners website link with bright-heart-education",
    () => {
        cy.location("pathname").should(
            "eq",
            `/tuition-partner/bright-heart-education`
        );
    }
);

Then("the tuition partner locations covered table is not displayed", () => {
    cy.get('[data-testid="locations-covered-table"]').should("not.exist");
});

Then("the tuition partner locations covered table is displayed", () => {
    cy.get('[data-testid="locations-covered-table"]').should("exist");
});

Then("the tuition partner pricing table is not displayed", () => {
    cy.get('[data-testid="pricing-table"]').should("not.exist");
});

Then("the tuition partner pricing table is displayed", () => {
    cy.get('[data-testid="pricing-table"]').should("exist");
});

Then(
    "the tuition partner pricing table is displayed for {string}",
    (tuitionTypes) => {
        tuitionTypes.split(",").forEach((tuitionType) => {
            cy.get('[data-testid="pricing-table"]').should(
                "contain.text",
                tuitionType
            );
        });
    }
);

Then("the tuition partner full pricing tables are not displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(
            `[data-testid="full-pricing-table-in-school-key-stage-${i}"]`
        ).should("not.exist");
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`).should(
            "not.exist"
        );
    }
});

Then("the tuition partner full pricing tables are displayed", () => {
    for (let i = 1; i < 5; i++) {
        cy.get(
            `[data-testid="full-pricing-table-in-school-key-stage-${i}"]`
        ).should("exist");
        cy.get(`[data-testid="full-pricing-table-online-key-stage-${i}"]`).should(
            "exist"
        );
    }
});

Then(
    "the subjects covered by a tuition partner are in alphabetical order",
    () => {
        const stages = [
            "Key stage 1 - English, Maths and Science",
            "Key stage 2 - English, Maths and Science",
            "Key stage 3 - English, Humanities, Maths, Modern Foreign Languages and Science",
            "Key stage 4 - English, Humanities, Maths, Modern Foreign Languages and Science",
        ];

        stages.forEach((element) => {
            cy.get(".govuk-list").first().contains(element);
        });
    }
);

Then("the tuition cost information states declares no differences", () => {
    cy.get('[data-testid="pricing-same-for-subjects').should("exist");
    cy.get('[data-testid="pricing-differences-for-subjects').should("not.exist");
});

Then("the tuition cost information states declares differences", () => {
    cy.get('[data-testid="pricing-differences-for-subjects').should("exist");
    cy.get('[data-testid="pricing-same-for-subjects').should("not.exist");
});

Then("all tuition partner details are populated correctly", () => {
    cy.get('[data-testid="results-subjects"] > li:first')
        .first()
        .invoke("text")
        .should("match", new RegExp("^Key stage \\d\\s\\-"));
    cy.get('[data-testid="type-of-tuition"]').first().should("not.be.empty");
    cy.get('[data-testid="results-description"]').first().should("not.be.empty");
    cy.get('[data-testid="organisation-type"]').first().should("not.be.empty");
});

Then("the logo is shown", () => {
    cy.get('[data-testid="tuition-partner-website-logo"]')
        .should("be.visible")
        .and(($img) => {
            expect($img[0].naturalWidth).to.be.greaterThan(0);
            expect($img[0].naturalWidth).to.be.equal(180);
        });
});

Then("the logo is not shown", () => {
    cy.get('[data-testid="tuition-partner-website-logo"]').should(
        "not.be.visible"
    );
});

Then("the LA name is not shown", () => {
    cy.get('[data-testid="la-name"]').should("not.exist");
});

Then("the LA name displayed is {string}", (laName) => {
    cy.get('[data-testid="la-name"]').should("contain.text", laName);
});

Then("the LA label text is {string}", (laLabelText) => {
    cy.get('[data-testid="la-name"]').should("contain.text", laLabelText);
});

Then(
    "the LA span text and .FindATuitionPartner.Shortlist are updated correctly when the checkbox is clicked",
    () => {
        const unCheckedSpanText = "Tuition partner for Stockport";
        const cookieName = ".FindATuitionPartner.Shortlist";
        const laElement = '[data-testid="la-name"]';
        const checkboxSelector='[id="shortlist-tpInfo-cb-bright-heart-education"]';
        const options = {
            cookieName: cookieName,
            tpSeoUrl: "bright-heart-education",
            laElement: laElement,
            laName: "Stockport",
            checkboxSelector:checkboxSelector
        }
        const opt = {
            cookieName: cookieName,
            expectedCookieValue: "",
            laElement: laElement,
            expectedText: unCheckedSpanText,
            checkboxSelector:checkboxSelector
        }
        cy.get(laElement).should("contain.text", `${unCheckedSpanText}`);
        cy.getCookie(cookieName).should("equal", null);
        cy.get(checkboxSelector).then(
            ($checkbox) => {
                cy.wrap($checkbox).check();
                cy.isTpShortlistedOnTpDetailPage(options);

                cy.wrap($checkbox).uncheck();
                cy.isTpNotShortlistedOnTpDetailPage(opt);
            }
        );
    }
);

Then(
    "shortlisted changes on TP 'Seven Springs Education' detail page should be reflected on search results page",
    () => {
        const totalShortlistedTpElementSelector = '[id="totalShortlistedTuitionPartners"]';
        const searchResultPageTpCheckboxSelector = '[id="shortlist-cb-seven-springs-education"]';
        const tpName = "Seven Springs Education";
        const tpDetailPageCheckboxSelector = '[id="shortlist-tpInfo-cb-seven-springs-education"]';
        const backlink = '[data-testid="back-link"]';

        cy.checkTotalShortlistedTp(totalShortlistedTpElementSelector, "0");
        cy.get(searchResultPageTpCheckboxSelector).should("not.be.checked");
        cy.goToSpecificTpDetailPage(tpName);

        cy.get(tpDetailPageCheckboxSelector).check();
        cy.get(backlink).click();
        cy.checkTotalShortlistedTp(totalShortlistedTpElementSelector, "1");
        cy.get(searchResultPageTpCheckboxSelector).should("be.checked");
        cy.goToSpecificTpDetailPage(tpName);

        cy.get(tpDetailPageCheckboxSelector).uncheck();
        cy.get(backlink).click();
        cy.checkTotalShortlistedTp(totalShortlistedTpElementSelector, "0");
        cy.get(searchResultPageTpCheckboxSelector).should("not.be.checked");
    }
);

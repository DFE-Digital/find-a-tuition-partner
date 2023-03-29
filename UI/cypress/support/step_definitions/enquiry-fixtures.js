import {
  Given,
  When,
  Then,
  And,
  Step,
} from "@badeball/cypress-cucumber-preprocessor";

let enquiry, enquiryNoInfo;

// Returns the first valid link in the array, also removing any which are now invalid
const getFirstValidLink = async (links) => {
  while (links.length) {
    const link = links[0];
    if ((await fetch(link)).ok) {
      return link;
    }
    links.shift();
  }
};

Given("An enquiry has been submitted", async () => {
  if (!enquiry) {
    Step(this, "they enter 'SK1 1EB' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which key stages' page");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which subjects' page");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Type of tuition' page");
    Step(this, "they select Any");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
    Step(this, "they enter a valid email address");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for tuition plan");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for SEND requirements");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for other requirements");
    Step(this, "they click 'Continue'");
    Step(this, "they select terms and conditions");
    Step(this, "they click send enquiry");

    cy.get("main").then(($el) => {
      enquiry = {
        enquirerHref: $el.find('a:contains("Enquirer link")').attr("href"),
        tpHrefs: [],
      };

      $el.find('a:contains("Response link")').each((i, responseEl) => {
        enquiry.tpHrefs.push(responseEl.getAttribute("href"));
      });
    });
  }
});

Given(
  "a tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
    Step(this, "An enquiry has been submitted");
    cy.then(async () => {
      cy.visit(await getFirstValidLink(enquiry.tpHrefs));
    });
  }
);

Given("a school clicks the magic link to view their enquiry", () => {
  Step(this, "An enquiry has been submitted");
  cy.then(async () => {
    cy.visit(enquiry.enquirerHref);
  });
});

Given("An enquiry with no optional info has been submitted", async () => {
  if (!enquiryNoInfo) {
    Step(this, "they enter 'SK1 1EB' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which key stages' page");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Which subjects' page");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Type of tuition' page");
    Step(this, "they select Any");
    Step(this, "they click 'Continue'");
    Step(this, "they will be taken to the 'Search Results' page");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
    Step(this, "they enter a valid email address");
    Step(this, "they click 'Continue'");
    Step(this, "they enter an answer for tuition plan");
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Continue'");
    Step(this, "they select terms and conditions");
    Step(this, "they click send enquiry");

    cy.get("main").then(($el) => {
      enquiryNoInfo = {
        enquirerHref: $el.find('a:contains("Enquirer link")').attr("href"),
        tpHrefs: [],
      };

      $el.find('a:contains("Response link")').each((i, responseEl) => {
        enquiryNoInfo.tpHrefs.push(responseEl.getAttribute("href"));
      });
    });
  }
});

Given(
  "a tuition partner clicks a magic link with no info for optional inputs",
  () => {
    Step(this, "An enquiry with no optional info has been submitted");
    cy.then(async () => {
      cy.log(enquiryNoInfo.tpHrefs);
      cy.visit(await getFirstValidLink(enquiryNoInfo.tpHrefs));
    });
  }
);

Given(
  "a school clicks the magic link to view their enquiry with no info for optional inputs",
  () => {
    Step(this, "An enquiry with no optional info has been submitted");
    cy.then(async () => {
      cy.visit(enquiryNoInfo.enquirerHref);
    });
  }
);

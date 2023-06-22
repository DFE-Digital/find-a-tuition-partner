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
const getSecondValidLink = async (links) => {
  while (links.length) {
    const link = links[1];
    if ((await fetch(link)).ok) {
      return link;
    }
    links.shift();
  }
};
const getThirdValidLink = async (links) => {
  while (links.length) {
    const link = links[0];
    if ((await fetch(link)).ok) {
      return link;
    }
    links.shift();
  }
};
const getFourthValidLink = async (links) => {
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
    Step(this, "they enter 'OX4 2AU' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they select No preference");
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
    Step(this, "they will be taken to the single school selection page");
    Step(this, "the user clicks yes and continue");
    Step(this, "they enter a valid email address");
    Step(this, "they click 'Continue'");
    Step(this, "the email address verification page is displayed");
    Step(this, "their is an input field for the verification code");
    Step(this, "they enter the valid passcode");
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

Given(
  "the first tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
    Step(this, "An enquiry has been submitted");
    cy.then(async () => {
      cy.visit(await getFirstValidLink(enquiry.tpHrefs));
    });
  }
);

Given(
  "the second tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
    Step(this, "An enquiry has been submitted");
    cy.then(async () => {
      cy.visit(await getSecondValidLink(enquiry.tpHrefs));
    });
  }
);

Given(
  "the third tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
    Step(this, "An enquiry has been submitted");
    cy.then(async () => {
      cy.visit(await getThirdValidLink(enquiry.tpHrefs));
    });
  }
);

Given(
  "the fourth tuition partner clicks the magic link to respond to a schools enquiry",
  () => {
    Step(this, "An enquiry has been submitted");
    cy.then(async () => {
      cy.visit(await getFourthValidLink(enquiry.tpHrefs));
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
    Step(this, "they enter 'OX4 2AU' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they select No preference");
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
    Step(this, "they will be taken to the single school selection page");
    Step(this, "the user clicks yes and continue");
    Step(this, "they enter a valid email address");
    Step(this, "they click 'Continue'");
    Step(this, "the email address verification page is displayed");
    Step(this, "their is an input field for the verification code");
    Step(this, "they enter the valid passcode");
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

Given("many tuition partners respond to an enquiry", async () => {
  if (!enquiry) {
    Step(this, "they enter 'OX4 2AU' as the school's postcode");
    Step(this, "they click 'Continue'");
    Step(this, "they will see all the keys stages as options");
    Step(this, "they select 'Key stage 1, Key stage 2'");
    Step(this, "they click 'Continue'");
    Step(this, "they are shown the subjects for 'Key stage 1, Key stage 2'");
    Step(
      this,
      "they select 'Key stage 1 English, Key stage 1 Maths, Key stage 2 English, Key stage 2 Maths'"
    );
    Step(this, "they click 'Continue'");
    Step(this, "they select No preference");
    Step(this, "they click 'Continue'");
    Step(this, "they click 'Start now' button");
    Step(this, "they click 'Continue' button");
    Step(this, "they will be taken to the single school selection page");
    Step(this, "the user clicks yes and continue");
    Step(this, "they enter a valid email address");
    Step(this, "they click 'Continue'");
    Step(this, "the email address verification page is displayed");
    Step(this, "their is an input field for the verification code");
    Step(this, "they enter the valid passcode");
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

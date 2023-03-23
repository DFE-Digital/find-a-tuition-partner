import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";
import { mapTextToNumberIndexZeroToTenth } from "../utils";

Given("a user has started the 'Find a tuition partner' journey", () => {
  cy.visit(`/`);
});

Given("a user has arrived on the funding and reporting page", () => {
  cy.visit(`/funding-and-reporting`);
});

Given("a user has arrived on the academic mentors page", () => {
  cy.visit(`/academic-mentors`);
});

Given("a user has arrived on the school led tutoring page", () => {
  cy.visit(`/school-led-tutoring`);
});

Given("a user has arrived on the contact us page", () => {
  cy.visit(`/contact-us`);
});

Given("a user has arrived on the Report issues page", () => {
  cy.visit(`/report-issues`);
});

Given("a user has arrived on the accessibility page", () => {
  cy.visit(`/accessibility`);
});

Given("a user has arrived on the cookies page", () => {
  cy.visit(`/cookies`);
});

Given("a user has arrived on the privacy page", () => {
  cy.visit(`/privacy`);
});

Given(
  "a user has arrived on the all quality-assured tuition partners page",
  () => {
    cy.visit(`/all-tuition-partners`);
  }
);

Then("they will be taken to the 'Which key stages' page", () => {
  cy.visit("/which-key-stages?KeyStages=KeyStage1&KeyStages=KeyStage2");
});

When(
  "they set the {string} query string parameter value to {string}",
  (key, value) => {
    cy.location("pathname").then((pathName) => {
      const separator = pathName.includes("?") === true ? "&" : "?";
      const url = `${pathName}${separator}${key}=${value}`;
      cy.visit(url);
    });
  }
);

Then(
  "they will be taken to the 'Find a tuition partner' journey start page",
  () => {
    cy.location("pathname").should("eq", "/");
  }
);

Then(
  "they will be taken to the 'What type of tuition do you need?' page",
  () => {
    cy.location("pathname").should("eq", "/which-tuition-types");
  }
);

Then("they will be taken to the 'Which subjects' page", () => {
  cy.location("pathname").should("eq", "/which-subjects");
});

Then("they will be taken to the 'Search Results' page", () => {
  cy.location("pathname").should("eq", "/search-results");
});

Then("they will be taken to the 'Price comparison list' page", () => {
  cy.location("pathname").should("eq", "/compare-list");
});

Then("the user will navigate to the guidance page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/guidance");
});

Then("user is redirected to the enter email address page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/enquirer-email");
});

Then("they are redirected to the enquiry question page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/tutoring-logistics");
});

Then("they are redirected to the SEND requirements page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/send-requirements");
});

Then("they are redirected to the other requirements page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/additional-information");
});

Then("they are redirected to the check your answers page", () => {
  cy.location("pathname").should("eq", "/enquiry/build/check-your-answers");
});

Then("the confirmation page is shown", () => {
  cy.location("pathname").should("eq", "/enquiry/build/submitted-confirmation");
});

Then("the user has arrived on the tuition response page", () => {
  cy.location("pathname").should("eq", "/enquiry/respond/response");
});

Then(
  "the user has arrived on the tuition response check your answers page",
  () => {
    cy.location("pathname").should("eq", "/enquiry/respond/check-your-answers");
  }
);


Then("the user has arrived on the tuition response confirmation page", () => {
  cy.location("pathname").should(
    "eq",
    "/enquiry/respond/response-confirmation"
  );
});

Then("the user has arrived on the view all enquiry responses page", () => {
  cy.location("pathname").should(
    "eq",
    "/enquiry/respond/all-enquirer-responses"
  );
});

Then("the tuition partners response page is shown", () => {
  cy.location("pathname").should("eq", "/enquiry/respond/enquirer-response");
});

Then("the user has arrived on the contact tuition partner page", () => {
  cy.location("pathname").should(
    "eq",
    "/enquiry/respond/enquirer-view-tuition-partner-details"
  );
});

Then("the page URL ends with {string}", (url) => {
  cy.location("pathname").should("match", new RegExp(`${url}$`));
});

Then("the heading should say {string}", (heading) => {
  cy.get("h1").should("have.text", heading);
});

Then("the page's title is {string}", (title) => {
  cy.title().should("eq", title);
});

Then("they will click the contact us link", () => {
  cy.get('[data-testid="contact-us-link"]').click();
});

Then("a user is using a {string}", (device) => {
  if (device == "phone") {
    cy.viewport(321, 640);
  } else if (device == "tablet") {
    cy.viewport(642, 1024);
  } else if (device == "desktop") {
    cy.viewport(770, 1024);
  }
});

Then("they will see link to {string} with test id {string}", (link, id) => {
  cy.get(`[data-testid="${id}"]`).should("have.attr", "href", link);
});

Then(
  "the {string} link opens a new window with test id {string}",
  (linkText, id) => {
    cy.get(`[data-testid="${id}"]`).should("have.attr", "target", "_blank");
  }
);

Then(
  "the {string} link they see is {string} with test id {string}",
  (linkTextLocation, linkText, testId) => {
    cy.get(`[data-testid="${testId}"]`)
      .find(">li")
      .eq(`${mapTextToNumberIndexZeroToTenth(linkTextLocation)}`)
      .should("contain", linkText);
  }
);

When("they click 'How are tuition partners quality-assured?'", () => {
  cy.get('[data-testid="qatp-details"]').click();
});

Then("the quality assured tuition partner details are hidden", () => {
  cy.get('[data-testid="qatp-details"]').should("not.have.attr", "open");
});

Then("the quality assured tuition partner details are shown", () => {
  cy.get('[data-testid="qatp-details"]').should("have.attr", "open");
});

Then("they will see the tribal link", () => {
  cy.get('[data-testid="tribal-link"]').should(
    "have.attr",
    "href",
    "https://www.tribalgroup.com/become-an-ntp-tuition-partner-0"
  );
});

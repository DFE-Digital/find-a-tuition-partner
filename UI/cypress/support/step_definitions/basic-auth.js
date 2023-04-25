describe("apply basic authentication", () => {
  before(() => {
    // set up authentication credentials
    const basicAuthCredentials = Cypress.env("BASIC_AUTH_CREDENTIALS");
    if (basicAuthCredentials) {
      const [username, password] = basicAuthCredentials.split(":");
      const auth = `${username}:${password}`;
      const authHeader = `Basic ${btoa(auth)}`;
      cy.request({
        url: "/",
        headers: {
          Authorization: authHeader,
        },
      });
    }
  });
});

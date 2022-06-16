const { defineConfig } = require("cypress");

module.exports = defineConfig({
    e2e: {
     baseUrl: 'https://national-tutoring-dev.london.cloudapps.digital/',
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
    },
   
});
 
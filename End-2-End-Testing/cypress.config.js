const { defineConfig } = require("cypress");

module.exports = defineConfig({
    env: {
        baseUrl: 'https://national-tutoring-dev.london.cloudapps.digital/',
        username: 'private',
        password: 'beta'
    },
    e2e: {
    
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
    },
   
});
 
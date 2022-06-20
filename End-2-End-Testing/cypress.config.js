const { defineConfig } = require("cypress");

module.exports = defineConfig({
    env: {
        baseUrl: 'https://localhost:7036/'
    },
    e2e: {
    
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
    },
   
});
 
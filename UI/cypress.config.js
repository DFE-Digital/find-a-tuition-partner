const { defineConfig } = require("cypress");
const webpack = require("@cypress/webpack-preprocessor");
const preprocessor = require("@badeball/cypress-cucumber-preprocessor");
const cypressSplit = require("cypress-split");

async function setupNodeEvents(on, config) {
  cypressSplit(on, config);
  on("task", {
    log(message) {
      console.log(message);
      return null;
    },
    table(message) {
      console.table(message);
      return null;
    },
    clearCookies() {
      cy.clearCookies(); // Add the clearCookies command here
      return null;
    },
  });

  await preprocessor.addCucumberPreprocessorPlugin(on, config);

  on(
    "file:preprocessor",
    webpack({
      webpackOptions: {
        resolve: {
          extensions: [".ts", ".js"],
        },
        module: {
          rules: [
            {
              test: /\.ts$/,
              exclude: [/node_modules/],
              use: [
                {
                  loader: "ts-loader",
                },
              ],
            },
            {
              test: /\.feature$/,
              use: [
                {
                  loader: "@badeball/cypress-cucumber-preprocessor/webpack",
                  options: config,
                },
              ],
            },
          ],
        },
      },
    })
  );

  return config;
}

module.exports = defineConfig({
  e2e: {
    baseUrl: "https://localhost:7036/",
    specPattern: "**/*.feature",
    setupNodeEvents,
    defaultCommandTimeout: 20000, // Command timeout overridden for E2E tests set to 20 seconds
  },
});

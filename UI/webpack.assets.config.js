const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: path.resolve(__dirname, "node_modules/govuk-frontend/govuk/assets"), to: path.resolve(__dirname, "wwwroot/assets") },
        { from: path.resolve(__dirname, "node_modules/govuk-frontend/govuk/all.js"), to: path.resolve(__dirname, "wwwroot/dist/govuk.js") }
      ],
    }),
  ],
};
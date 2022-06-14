const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: path.join(__dirname, 'node_modules/govuk-frontend/govuk/assets'), to: path.join(__dirname, 'assets') }
      ],
    }),
  ],
};
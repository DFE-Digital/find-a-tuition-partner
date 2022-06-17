const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
  entry: ["./src/index.js", "./src/index.scss"],
  output: {
    path: path.resolve(__dirname, 'wwwroot/dist'),
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: path.resolve(__dirname, "node_modules/govuk-frontend/govuk/assets"), to: path.resolve(__dirname, "wwwroot/assets") },
      ],
    }),
    new MiniCssExtractPlugin()
  ],
  resolve: {
    extensions: ['.mjs', '.js'],
  },
  module: {
    rules: [
      {
        test: /\.mjs$/,
        type: 'javascript/auto',
        resolve: {
          fullySpecified: false
        }
      },
      {
        test: /\.s[ac]ss$/i,
        use: [
          MiniCssExtractPlugin.loader,
          // Translates CSS into CommonJS
          "css-loader",
          // Compiles Sass to CSS
          "sass-loader",
        ],
      },
    ],
  },
};
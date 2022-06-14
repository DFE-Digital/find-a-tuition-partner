const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
  entry: ["./UI/assets/site.sass"],
  output: {
    path: path.resolve(__dirname, "UI/assets")
  },
  plugins: [new MiniCssExtractPlugin({ filename: '[name].css' })],
  module: {
    rules: [
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
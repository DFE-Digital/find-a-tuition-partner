import {
  Button,
  Checkboxes,
  Details,
  ErrorSummary,
  Radios,
  SkipLink,
  CharacterCount,
} from "govuk-frontend";

import HMRCFrontend from "hmrc-frontend/hmrc/all";

window.HMRCFrontend = HMRCFrontend;

var $buttons = document.querySelectorAll('[data-module="govuk-button"]');
if ($buttons) {
  for (var i = 0; i < $buttons.length; i++) {
    new Button($buttons[i]).init();
  }
}

var $checkboxes = document.querySelectorAll('[data-module="govuk-checkboxes"]');
if ($checkboxes) {
  for (var i = 0; i < $checkboxes.length; i++) {
    new Checkboxes($checkboxes[i]).init();
  }
}

var $details = document.querySelectorAll('[data-module="govuk-details"]');
if ($details) {
  for (var i = 0; i < $details.length; i++) {
    new Details($details[i]).init();
  }
}

var $errorSummary = document.querySelector(
  '[data-module="govuk-error-summary"]'
);
if ($errorSummary) {
  new ErrorSummary($errorSummary).init();
}

var $radios = document.querySelectorAll('[data-module="govuk-radios"]');
if ($radios) {
  for (var i = 0; i < $radios.length; i++) {
    new Radios($radios[i]).init();
  }
}

var $skipLink = document.querySelector('[data-module="govuk-skip-link"]');
if ($skipLink) {
  new SkipLink($skipLink).init();
}

var $characterCount = document.querySelectorAll(
  '[data-module="govuk-character-count"]'
);
if ($characterCount) {
  for (var i = 0; i < $characterCount.length; i++) {
    new CharacterCount($characterCount[i]).init();
  }
}

import ResultsFilter from "./javascript/results-filter";
var resultsFilter = new ResultsFilter();
resultsFilter.init();

import OptionSelect from "./javascript/option-select";
var $optionsSelect = document.querySelectorAll('[data-module="option-select"]');
if ($optionsSelect) {
  $optionsSelect.forEach((el) =>
    new window.GOVUK.Modules.OptionSelect(el).init()
  );
}

HMRCFrontend.initAll();

import PrintThisPage from "./javascript/print-this-page";
var printThisPage = new PrintThisPage();
printThisPage.init();

import CompareListCheckbox from "./javascript/compare-list-checkbox";
const compareListCheckboxes = document.querySelectorAll(
  '[data-module="compare-list-checkbox"]'
);
CompareListCheckbox.init(compareListCheckboxes);

import CompareListRefinement from "./javascript/compare-list-refinement";
var compareListRefinement = new CompareListRefinement();
compareListRefinement.init();

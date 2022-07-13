import {
    Button,
    Checkboxes,
    Details,
    ErrorSummary,
    Radios,
    SkipLink
} from 'govuk-frontend'

var $buttons = document.querySelectorAll('[data-module="govuk-button"]');
if ($buttons) {
    for (var i = 0; i < $buttons.length; i++) {
        new Button($buttons[i]).init();
    };
}

var $checkboxes = document.querySelectorAll('[data-module="govuk-checkboxes"]')
if ($checkboxes) {
    for (var i = 0; i < $checkboxes.length; i++) {
        new Checkboxes($checkboxes[i]).init()
    }
}

var $details = document.querySelectorAll('[data-module="govuk-details"]')
if ($details) {
    for (var i = 0; i < $details.length; i++) {
        new Details($details[i]).init()
    }
}

var $errorSummary = document.querySelector('[data-module="govuk-error-summary"]');
if ($errorSummary) {
    new ErrorSummary($errorSummary).init();
}

var $radios = document.querySelectorAll('[data-module="govuk-radios]')
if ($radios) {
    for (var i = 0; i < $radios.length; i++) {
        new Radios($radios[i]).init()
    }
}

var $skipLink = document.querySelector('[data-module="govuk-skip-link"]')
if ($skipLink) {
    new SkipLink($skipLink).init()
}

import ResultsFilter from './javascript/results-filter'
var resultsFilter = new ResultsFilter()
resultsFilter.init()

import OptionSelect from './javascript/option-select'
var $optionsSelect = document.querySelector('[data-module="option-select"]')
if ($optionsSelect) {
    new window.GOVUK.Modules.OptionSelect($optionsSelect).init()
}

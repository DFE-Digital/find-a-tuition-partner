function ResultsFilter() {
  this.$body = document.querySelector("body");
  this.$filters = document.querySelector('[data-module="results-filter"]');
  this.$overlayApplyFilters = document.querySelector(
    '[data-module="overlay-apply-filters"]'
  );
  this.$timeout = null;
}

ResultsFilter.prototype.init = function () {
  if (this.$filters) {
    document
      .querySelector('[data-module="return-to-results-link"]')
      .addEventListener("click", this.returnToResultsEvent.bind(this));

    this.$filters
      .querySelectorAll("input[type='checkbox']")
      .forEach((element) => {
        element.addEventListener("click", this.filterChangedEvent.bind(this));
      });

    this.$filters.querySelectorAll("input[type='radio']").forEach((element) => {
      element.addEventListener("click", this.filterChangedEvent.bind(this));
    });

    document
      .querySelector('[data-module="show-filters-button-group"]')
      .querySelector("button")
      .addEventListener("click", this.showFiltersEvent.bind(this));

    //If the error summary filter prompt is shown then when click on error link display filters
    var errorSummaryFilterPrompt = document.querySelector(
      ".error-summary-filter-prompt"
    );
    if (
      errorSummaryFilterPrompt &&
      window.getComputedStyle(errorSummaryFilterPrompt).display != "none"
    ) {
      document
        .querySelector(".govuk-error-summary")
        .querySelector("a")
        .addEventListener("click", this.showFiltersEvent.bind(this));
    }
  }
};

ResultsFilter.prototype.filterChangedEvent = function (e) {
  if (this.$timeout) {
    clearTimeout(this.$timeout);
  }

  if (this.$overlayApplyFilters.offsetParent == null) {
    this.$timeout = setTimeout(
      () => document.querySelector("form").submit(),
      500
    );
  }
};

ResultsFilter.prototype.returnToResultsEvent = function (e) {
  e.preventDefault();
  window.location.reload(false);
};

ResultsFilter.prototype.showFiltersEvent = function (e) {
  e.preventDefault();

  this.$body.style.top = "-" + window.scrollY + "px";
  this.$body.style.position = "fixed";

  this.$filters.classList.add("app-results-filter-overlay");
  this.$filters.setAttribute("tabindex", 0);
  this.$filters.focus();
};

export default ResultsFilter;

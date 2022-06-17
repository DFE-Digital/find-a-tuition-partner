function ResultsFilter() {
    this.$filters = document.querySelector('[data-module="results-filter"]')
    this.$timeout = null
}

ResultsFilter.prototype.init = function () {
    if (this.$filters) {
        document.querySelector('[data-module="results-filter-button"]').style.display = 'none'

        this.$filters.querySelectorAll("input[type='checkbox']").forEach(element => {
            element.addEventListener('click', this.filterChangedEvent.bind(this))
        });

        this.$filters.querySelectorAll("input[type='radio']").forEach(element => {
            element.addEventListener('click', this.filterChangedEvent.bind(this))
        });
    }
}

ResultsFilter.prototype.filterChangedEvent = function (e) {
    if (this.$timeout) {
        clearTimeout(this.$timeout);
    }

    this.$timeout = setTimeout(() => document.querySelector('form').submit(), 500);
}

export default ResultsFilter
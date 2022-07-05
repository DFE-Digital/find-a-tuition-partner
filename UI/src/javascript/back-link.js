function BackLink() {
    this.$links = document.querySelectorAll('.govuk-back-link[data-module="history-back-link"]')
}

BackLink.prototype.init = function () {
    if (this.$links) {
        this.$links.forEach(element => {
            element.addEventListener('click', this.linkClickedEvent.bind(this))
        });
    }
}

BackLink.prototype.linkClickedEvent = function (e) {
    e.preventDefault();
    history.back();
}

export default BackLink
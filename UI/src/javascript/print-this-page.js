function PrintThisPage() {
  this.$links = document.querySelectorAll(
    '[data-module="print-this-page-link"]'
  );
}

PrintThisPage.prototype.init = function () {
  if (this.$links) {
    this.$links.forEach((element) => {
      element.addEventListener("click", this.printEvent.bind(this, element));
    });
  }
};

PrintThisPage.prototype.printEvent = function (link, e) {
  e.preventDefault();
  link.blur();
  window.print();
};

export default PrintThisPage;

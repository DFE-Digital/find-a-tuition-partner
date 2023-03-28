function PrintThisPage() {
  this.$links = document.querySelectorAll(
    '[data-module="print-this-page-link"]'
  );

  window.addEventListener("beforeprint", () => {
    const $details = document.body.querySelectorAll("details");
    const detailsArray = Array.prototype.slice.call($details);
    for (let i = 0; i < detailsArray.length; i++)
      detailsArray[i].open
        ? (detailsArray[i].dataset.open = "1")
        : detailsArray[i].setAttribute("open", "");
  });
  window.addEventListener("afterprint", () => {
    const $details = document.body.querySelectorAll("details");
    const detailsArray = Array.prototype.slice.call($details);
    for (let i = 0; i < detailsArray.length; i++)
      detailsArray[i].dataset.open
        ? (detailsArray[i].dataset.open = "")
        : detailsArray[i].removeAttribute("open");
  });
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

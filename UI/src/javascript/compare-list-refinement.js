function CompareListRefinement() {
  this.$body = document.querySelector("body");
  this.$refinementSection = document.querySelector(
    '[data-module="compare-list-refinement"]'
  );
  this.$timeout = null;
}

CompareListRefinement.prototype.init = function () {
  if (this.$refinementSection) {
    this.$refinementSection.querySelectorAll("select").forEach((element) => {
      element.addEventListener(
        "change",
        this.refinementChangedEvent.bind(this)
      );
    });
  }
};

CompareListRefinement.prototype.refinementChangedEvent = function (e) {
  if (this.$timeout) {
    clearTimeout(this.$timeout);
  }

  this.$timeout = setTimeout(
    () => document.querySelector("form").submit(),
    500
  );
};

export default CompareListRefinement;

"use strict";

const BackToTop = (() => {
  const init = (backToTopBanner) => {
    if (backToTopBanner && backToTopBanner.length > 0) {
      window.addEventListener("scroll", onScroll);
      onScroll();
    }
  };

  const onScroll = async () => {
    var backToTopBanner = document.body.querySelector(
      ".app-back-to-top-banner"
    );

    if (backToTopBanner) {
      var windowScrolledFromTop =
        window.pageYOffset || document.documentElement.scrollTop;

      var showBanner = false;

      var myCompareListLink = document.body.querySelector(
        "#my-compare-list-link"
      );
      if (myCompareListLink) {
        var compareLocationGoesOffWindow =
          myCompareListLink.offsetTop + myCompareListLink.offsetHeight;
        if (windowScrolledFromTop > compareLocationGoesOffWindow) {
          showBanner = true;
        }
      } else {
        var topElement = document.body.querySelector("#top");
        if (topElement) {
          var topElementGoesOffWindow = topElement.offsetTop;
          if (windowScrolledFromTop > topElementGoesOffWindow) {
            showBanner = true;
          }
        }
      }

      if (showBanner) {
        backToTopBanner.classList.add("app-back-to-top-banner__fixed");
      } else {
        backToTopBanner.classList.remove("app-back-to-top-banner__fixed");
      }
    }
  };

  return {
    init,
  };
})();

export default BackToTop;

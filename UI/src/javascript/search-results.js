"use strict";

import SearchResultsClient from "./api/search-results-client";

const SearchResults = (() => {
  let client = new SearchResultsClient();
  const isFetchResponseDataValid = (response) =>
    response &&
    response.isCallSuccessful !== "undefined" &&
    response.totalShortlistedTuitionPartners !== "undefined";
  const init = (searchResultShortlistCheckboxes) => {
    if (
      searchResultShortlistCheckboxes &&
      searchResultShortlistCheckboxes.length > 0
    ) {
      searchResultShortlistCheckboxes.forEach((element) => {
        element
          .querySelector("input[type='checkbox']")
          .addEventListener("click", onCheckboxClick);
      });
    }
  };
  let lastCheckboxPromise = null;
  const onCheckboxClick = async (event) => {
    const checkbox = event.target;
    const checked = checkbox.checked;

    lastCheckboxPromise = (async (previousPromise) => {
      await previousPromise;

      try {
        const updateFunction = checked ? onChecked : onUnChecked;

        const result = await updateFunction(checkbox);
        updateSearchResultPage(result, checkbox, !checked);
      } catch (e) {
        checkbox.checked = checked;
      }
    })(lastCheckboxPromise);
  };
  const setShortlistedTuitionPartnersBadgeClass = (
    badgeElement,
    totalShortlistedTuitionPartners
  ) => {
    if (!totalShortlistedTuitionPartners) {
      return;
    }
    if (totalShortlistedTuitionPartners < 20) {
      badgeElement.classList.remove(
        "my-shortlisted-partners-badge--for-values-greater-than-nineteen"
      );
      badgeElement.classList.add("my-shortlisted-partners-badge");
    } else {
      badgeElement.classList.remove("my-shortlisted-partners-badge");
      badgeElement.classList.add(
        "my-shortlisted-partners-badge--for-values-greater-than-nineteen"
      );
    }
  };
  const updateSearchResultPage = (result, checkBox, elseCheckboxState) => {
    if (isFetchResponseDataValid(result) && result.isCallSuccessful) {
      const badgeElement = document.getElementById(
        "totalShortlistedTuitionPartners"
      );
      if (badgeElement) {
        badgeElement.textContent = result.totalShortlistedTuitionPartners;
        setShortlistedTuitionPartnersBadgeClass(
          badgeElement,
          result.totalShortlistedTuitionPartners
        );
      }
    } else {
      checkBox.checked = elseCheckboxState;
    }
  };
  const onChecked = async (checkbox) => {
    return await client.getAddShortlistedTuitionPartner(checkbox.id);
  };
  const onUnChecked = async (checkbox) => {
    return await client.getRemoveShortlistedTuitionPartner(checkbox.id);
  };

  return {
    init,
  };
})();

export default SearchResults;

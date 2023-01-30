"use strict";

import axios from "axios";

const SearchResults = (() => {
  const getAddShortlistTuitionPartnerFetchUrl = (seoUrl) =>
    `/search-results?handler=AddShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

  const getRemoveShortlistTuitionPartnerFetchUrl = (seoUrl) =>
    `/search-results?handler=RemoveShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

  const getSeoUrlFromCheckboxId = (id) => id && id.replace("shortlist-cb-", "");

  const isFetchResponseDataValid = (response) =>
    response &&
    response.isCallSuccessful !== "undefined" &&
    response.totalShortlistedTuitionPartners !== "undefined";

  const shortlistCheckboxes = document.querySelectorAll(
    '[data-module="search-results-tp-shortlist"]'
  );
  const throwError = (message = "Invalid result") => {
    throw new Error(message);
  };

  const init = () => {
    if (shortlistCheckboxes && shortlistCheckboxes.length > 0) {
      shortlistCheckboxes.forEach((element) => {
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
  const getResponseData = (response) => {
    if (response.status >= 200 && response.status <= 299) {
      return response.data;
    } else {
      throwError(response.statusText);
    }
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
    return await axios
      .get(
        getAddShortlistTuitionPartnerFetchUrl(
          getSeoUrlFromCheckboxId(checkbox.id)
        )
      )
      .then((response) => getResponseData(response))
      .catch((error) => {
        throwError(error);
      });
  };
  const onUnChecked = async (checkbox) => {
    return await axios
      .get(
        getRemoveShortlistTuitionPartnerFetchUrl(
          getSeoUrlFromCheckboxId(checkbox.id)
        )
      )
      .then((response) => getResponseData(response))
      .catch((error) => {
        throwError(error);
      });
  };

  return {
    init,
  };
})();

export default SearchResults;

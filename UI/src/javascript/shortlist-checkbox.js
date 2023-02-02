"use strict";

import ShortlistClient from "./api/shortlist-client";
import { throwError } from "./util";

const ShortlistCheckbox = (() => {
  let client = new ShortlistClient();
  const isResultValid = (result) =>
    result &&
    result.isCallSuccessful !== "undefined" &&
    result.totalShortlistedTuitionPartners !== "undefined";

  const init = (shortlistCheckboxes) => {
    if (shortlistCheckboxes && shortlistCheckboxes.length > 0) {
      shortlistCheckboxes.forEach((element) => {
        element
          .querySelector("input[type='checkbox']")
          .addEventListener("click", onCheckboxClick);
      });
    }
  };
  const onCheckboxClick = async (event) => {
    const checkbox = event.target;
    if (checkbox.checked) {
      try {
        const result = await onChecked(checkbox.id);
        if (!isResultValid(result)) throwError();
        updateSearchResultPage(result);
        setLocalAuthorityHeaderText();
      } catch (error) {
        checkbox.checked = false;
      }
    } else {
      try {
        const result = await onUnChecked(checkbox.id);
        if (!isResultValid(result)) throwError();
        updateSearchResultPage(result);
        setLocalAuthorityHeaderText("Tuition partner for ");
      } catch (error) {
        checkbox.checked = true;
      }
    }
  };
  const setLocalAuthorityHeaderText = (
    headerText = "Shortlisted tuition partner for "
  ) => {
    const localAuthHeader = document.getElementById(
      "tp-details-page--tp-localAuthHeader"
    );
    if (localAuthHeader != null) {
      localAuthHeader.innerHTML = headerText;
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
  const updateSearchResultPage = (result) => {
    if (isResultValid(result) && result.isCallSuccessful) {
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
    }
  };
  const onChecked = async (checkboxId) => {
    const totalShortlistedTuitionPartners = document.querySelectorAll(
      `input[name="ShortlistedTuitionPartners"]:checked`
    ).length;
    return await client.postAddToShortlist(
      checkboxId,
      totalShortlistedTuitionPartners
    );
  };
  const onUnChecked = async (checkboxId) => {
    const totalShortlistedTuitionPartners = document.querySelectorAll(
      `input[name="ShortlistedTuitionPartners"]:checked`
    ).length;
    return await client.postRemoveFromShortlist(
      checkboxId,
      totalShortlistedTuitionPartners
    );
  };

  return {
    init,
  };
})();

export default ShortlistCheckbox;

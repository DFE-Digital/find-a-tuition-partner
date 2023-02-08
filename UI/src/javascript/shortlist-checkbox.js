"use strict";

import ShortlistClient from "./api/shortlist-client";

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
        setLocalAuthorityHeaderText(checked);
      } catch (e) {
        checkbox.checked = checked;
      }
    })(lastCheckboxPromise);
  };
  const setLocalAuthorityHeaderText = (checked) => {
    let headerText = "Tuition partner for ";
    if (checked) {
      headerText = "Price comparison listed tuition partner for ";
    }
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
  const updateSearchResultPage = (result, checkBox, elseCheckboxState) => {
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
    } else {
      checkBox.checked = elseCheckboxState;
    }
  };
  const onChecked = async (checkbox) => {
    const totalShortlistedTuitionPartners = document.querySelectorAll(
      `input[name="ShortlistedTuitionPartners"]:checked`
    ).length;
    return await client.postAddToShortlist(
      checkbox.id,
      totalShortlistedTuitionPartners
    );
  };
  const onUnChecked = async (checkbox) => {
    const totalShortlistedTuitionPartners = document.querySelectorAll(
      `input[name="ShortlistedTuitionPartners"]:checked`
    ).length;
    return await client.postRemoveFromShortlist(
      checkbox.id,
      totalShortlistedTuitionPartners
    );
  };

  return {
    init,
  };
})();

export default ShortlistCheckbox;

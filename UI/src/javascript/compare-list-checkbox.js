"use strict";

import CompareListClient from "./api/compare-list-client";

const CompareListCheckbox = (() => {
  let client = new CompareListClient();
  const isResultValid = (result) =>
    result &&
    result.isCallSuccessful !== "undefined" &&
    result.totalCompareListedTuitionPartners !== "undefined";

  const init = (compareListCheckboxes) => {
    if (compareListCheckboxes && compareListCheckboxes.length > 0) {
      compareListCheckboxes.forEach((element) => {
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

  const setCompareListedTuitionPartnersBadgeClass = (
    badgeElement,
    totalCompareListedTuitionPartners
  ) => {
    if (!totalCompareListedTuitionPartners) {
      return;
    }
    if (totalCompareListedTuitionPartners < 20) {
      badgeElement.classList.remove(
        "my-compare-listed-partners-badge--for-values-greater-than-nineteen"
      );
      badgeElement.classList.add("my-compare-listed-partners-badge");
    } else {
      badgeElement.classList.remove("my-compare-listed-partners-badge");
      badgeElement.classList.add(
        "my-compare-listed-partners-badge--for-values-greater-than-nineteen"
      );
    }
  };
  const updateSearchResultPage = (result, checkBox, elseCheckboxState) => {
    if (isResultValid(result) && result.isCallSuccessful) {
      const badgeElement = document.getElementById(
        "totalCompareListedTuitionPartners"
      );
      if (badgeElement) {
        badgeElement.textContent = result.totalCompareListedTuitionPartners;
        setCompareListedTuitionPartnersBadgeClass(
          badgeElement,
          result.totalCompareListedTuitionPartners
        );
      }
    } else {
      checkBox.checked = elseCheckboxState;
    }
  };
  const onChecked = async (checkbox) => {
    const totalCompareListedTuitionPartners = document.querySelectorAll(
      `input[name="CompareListedTuitionPartners"]:checked`
    ).length;
    return await client.postAddToCompareList(
      checkbox.id,
      totalCompareListedTuitionPartners
    );
  };
  const onUnChecked = async (checkbox) => {
    const totalCompareListedTuitionPartners = document.querySelectorAll(
      `input[name="CompareListedTuitionPartners"]:checked`
    ).length;
    return await client.postRemoveFromCompareList(
      checkbox.id,
      totalCompareListedTuitionPartners
    );
  };

  return {
    init,
  };
})();

export default CompareListCheckbox;

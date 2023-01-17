"use strict";

const getAddShortlistTuitionPartnerFetchUrl = (seoUrl) =>
  `?handler=AddShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

const getRemoveShortlistTuitionPartnerFetchUrl = (seoUrl) =>
  `?handler=RemoveShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

const isFetchResponseDataValid = (response) =>
  response &&
  response.isCallSuccessful !== "undefined" &&
  response.totalShortlistedTuitionPartners !== "undefined";

const getFetchResponseData = (response) => {
  if (response.status >= 200 && response.status <= 299) {
    return response.json();
  } else {
    throw Error(response.statusText);
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

const getSeoUrlFromCheckboxId = (id) => id && id.replace("shortlist-cb-", "");

const onChecked = async (checkbox) => {
  return await fetch(
    getAddShortlistTuitionPartnerFetchUrl(getSeoUrlFromCheckboxId(checkbox.id))
  )
    .then((response) => getFetchResponseData(response))
    .catch((error) => {
      //What do we want to do with the error.
      throw Error(error);
    });
};

const onUnChecked = async (checkbox) => {
  return await fetch(
    getRemoveShortlistTuitionPartnerFetchUrl(
      getSeoUrlFromCheckboxId(checkbox.id)
    )
  )
    .then((response) => getFetchResponseData(response))
    .catch((error) => {
      //What do we want to do with the error.
      throw Error(error);
    });
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

const addClickEventListenerToTuitionPartnerCheckboxes = () => {
  // The if statement is to prevent using javascript for the checkboxes,
  // mainly in the case of if the client is using an IE browsers
  // as it doesn’t seem to support newer javascript keywords. e.g. 'let'.
  if (!document.body.className.includes("js-enabled")) return;
  const selectableTuitionPartners = document.getElementsByName(
    "ShortlistedTuitionPartners"
  );
  if (selectableTuitionPartners && selectableTuitionPartners.length > 0) {
    selectableTuitionPartners.forEach((checkBox) => {
      checkBox.addEventListener("click", onCheckboxClick);
    });
  }
};

addClickEventListenerToTuitionPartnerCheckboxes();

"use strict";

const getSeoUrlFromId = (id) => id && id.replace("shortlist-tpInfo-cb-", "");
const getAddToShortlistFetchUrl = () => `?handler=AddToShortlist`;
const getRemoveFromShortlistFetchUrl = () => `?handler=RemoveFromShortlist`;
const getAdditionalFetchParameter = (checkboxId) => ({
  method: "POST",
  body: JSON.stringify(`${getSeoUrlFromId(checkboxId)}`),
  headers: {
    RequestVerificationToken: document.getElementsByName(
      "__RequestVerificationToken"
    )[0].value,
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});
const getFetchResponseData = (response) => {
  if (response.status >= 200 && response.status <= 299) {
    return response.json();
  } else {
    throw Error(response.statusText);
  }
};
const isResultValid = (result) =>
  result && result.updated !== "undefined" && result.updated;
const getError = (message = "Invalid result") => Error(message);
const setLocalAuthorityHeaderText = (
  headerText = "Shortlisted tuition partner for "
) => {
  const localAuthHeader = document.getElementById(
    "tp-details-page--tp-localAuthHeader"
  );
  localAuthHeader.innerHTML = headerText;
};
const onChecked = async (checkboxId) => {
  return await fetch(
    getAddToShortlistFetchUrl(),
    getAdditionalFetchParameter(checkboxId)
  )
    .then((response) => getFetchResponseData(response))
    .catch((error) => {
      throw Error(error);
    });
};
const onUnChecked = async (checkboxId) => {
  return await fetch(
    getRemoveFromShortlistFetchUrl(),
    getAdditionalFetchParameter(checkboxId)
  )
    .then((response) => getFetchResponseData(response))
    .catch((error) => {
      throw Error(error);
    });
};
const onCheckboxClick = async (event) => {
  const checkbox = event.target;
  if (checkbox.checked) {
    try {
      const result = await onChecked(checkbox.id);
      if (!isResultValid(result)) throw getError();

      setLocalAuthorityHeaderText();
    } catch (error) {
      //What do we want to do with error?
      checkbox.checked = false;
    }
  } else {
    try {
      const result = await onUnChecked(checkbox.id);
      if (!isResultValid(result)) throw getError();

      setLocalAuthorityHeaderText("Tuition partner for ");
    } catch (error) {
      //What do we want to do with error?
      checkbox.checked = true;
    }
  }
};
const isCssJsEnabled = () => document.body.className.includes("js-enabled");

const addClickEventListenerToCheckbox = () => {
  // The if statement is to prevent using javascript for the checkboxes,
  // mainly in the case of if the client is using an IE browsers
  // as it doesn’t seem to support newer javascript keywords. e.g. 'let'.
  if (!isCssJsEnabled()) return;
  const checkboxes = document.getElementsByName("ShortlistedCheckbox");
  if (checkboxes)
    checkboxes.forEach((c) => c.addEventListener("click", onCheckboxClick));
};

addClickEventListenerToCheckbox();

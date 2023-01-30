"use strict";

import axios from "axios";

const TuitionPartner = (() => {
  const getSeoUrlFromId = (id) => id && id.replace("shortlist-tpInfo-cb-", "");
  const getAddToShortlistFetchUrl = (id) =>
    `/tuition-partner/${id}?handler=AddToShortlist`;
  const getRemoveFromShortlistFetchUrl = (id) =>
    `/tuition-partner/${id}?handler=RemoveFromShortlist`;

  const shortlistCheckboxes = document.querySelector(
    '[data-module="tuition-partner-shortlist"]'
  );

  const isResultValid = (result) =>
    result && result.updated !== "undefined" && result.updated;
  const throwError = (message = "Invalid result") => {
    throw new Error(message);
  };

  const init = () => {
    if (shortlistCheckboxes) {
      shortlistCheckboxes
        .querySelector("input[type='checkbox']")
        .addEventListener("click", onCheckboxClick);
    }
  };
  const onCheckboxClick = async (event) => {
    const checkbox = event.target;
    if (checkbox.checked) {
      try {
        const result = await onChecked(checkbox.id);
        if (!isResultValid(result)) throwError();

        setLocalAuthorityHeaderText();
      } catch (error) {
        //What do we want to do with error?
        checkbox.checked = false;
      }
    } else {
      try {
        const result = await onUnChecked(checkbox.id);
        if (!isResultValid(result)) throwError();

        setLocalAuthorityHeaderText("Tuition partner for ");
      } catch (error) {
        //What do we want to do with error?
        checkbox.checked = true;
      }
    }
  };
  const getAdditionalParameter = (checkboxId) => ({
    method: "POST",
    data: JSON.stringify(`${getSeoUrlFromId(checkboxId)}`),
    headers: {
      RequestVerificationToken: document.getElementsByName(
        "__RequestVerificationToken"
      )[0].value,
      "Content-Type": "application/json",
      Accept: "application/json",
    },
  });
  const getResponseData = (response) => {
    if (response.status >= 200 && response.status <= 299) {
      return response.data;
    } else {
      throwError(response.statusText);
    }
  };
  const setLocalAuthorityHeaderText = (
    headerText = "Shortlisted tuition partner for "
  ) => {
    const localAuthHeader = document.getElementById(
      "tp-details-page--tp-localAuthHeader"
    );
    localAuthHeader.innerHTML = headerText;
  };
  const onChecked = async (checkboxId) => {
    return await axios
      .post(
        getAddToShortlistFetchUrl(getSeoUrlFromId(checkboxId)),
        JSON.stringify(getSeoUrlFromId(checkboxId)),
        getAdditionalParameter(checkboxId)
      )
      .then((response) => getResponseData(response))
      .catch((error) => {
        throwError(error);
      });
  };
  const onUnChecked = async (checkboxId) => {
    return await axios
      .post(
        getRemoveFromShortlistFetchUrl(getSeoUrlFromId(checkboxId)),
        JSON.stringify(getSeoUrlFromId(checkboxId)),
        getAdditionalParameter(checkboxId)
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

export default TuitionPartner;

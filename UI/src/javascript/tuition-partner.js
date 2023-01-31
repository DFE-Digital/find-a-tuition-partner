"use strict";

import TuitionPartnerClient from "./api/tuition-partner-client";
import { throwError } from "./util";

const TuitionPartner = (() => {
  let client = new TuitionPartnerClient();
  const isResultValid = (result) =>
    result && result.updated !== "undefined" && result.updated;

  const init = (tpShortlistCheckboxes) => {
    if (tpShortlistCheckboxes) {
      tpShortlistCheckboxes
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
  const setLocalAuthorityHeaderText = (
    headerText = "Shortlisted tuition partner for "
  ) => {
    const localAuthHeader = document.getElementById(
      "tp-details-page--tp-localAuthHeader"
    );
    localAuthHeader.innerHTML = headerText;
  };
  const onChecked = async (checkboxId) => {
    return await client.postAddToShortlist(checkboxId);
  };
  const onUnChecked = async (checkboxId) => {
    return await client.postRemoveFromShortlist(checkboxId);
  };

  return {
    init,
  };
})();

export default TuitionPartner;

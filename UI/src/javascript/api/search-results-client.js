import BaseClient from "./base-client";
import { getSeoUrlFromId } from "../util";

const getAddShortlistTuitionPartnerUrl = (seoUrl) =>
  `/search-results?handler=AddShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

const getRemoveShortlistedTuitionPartnerUrl = (seoUrl) =>
  `/search-results?handler=RemoveShortlistedTuitionPartner&tuitionPartnerSeoUrl=${seoUrl}`;

const replaceIdString = "shortlist-cb-";
export default class SearchResultsClient extends BaseClient {
  constructor() {
    super();
  }
  getAddShortlistedTuitionPartner(id) {
    return this.getWithHandleError(
      getAddShortlistTuitionPartnerUrl(getSeoUrlFromId(id, replaceIdString))
    );
  }

  getRemoveShortlistedTuitionPartner(id) {
    return this.getWithHandleError(
      getRemoveShortlistedTuitionPartnerUrl(
        getSeoUrlFromId(id, replaceIdString)
      )
    );
  }
}

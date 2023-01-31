import BaseClient from "./base-client";
import { getSeoUrlFromId, addPostHeaderWithVerificationToken } from "../util";

const replaceIdString = "shortlist-tpInfo-cb-";

const postAddToShortlistUrl = (id) =>
  `/tuition-partner/${id}?handler=AddToShortlist`;
const postRemoveFromShortlistUrl = (id) =>
  `/tuition-partner/${id}?handler=RemoveFromShortlist`;
export default class TuitionPartnerClient extends BaseClient {
  constructor() {
    super();
  }

  commonShortlistPostLogic = (urlToCall, seoUrl) => {
    return this.postWithHandleError(
      urlToCall,
      JSON.stringify(seoUrl),
      addPostHeaderWithVerificationToken()
    );
  };
  postAddToShortlist(id) {
    const seoUrl = getSeoUrlFromId(id, replaceIdString);
    return this.commonShortlistPostLogic(postAddToShortlistUrl(seoUrl), seoUrl);
  }

  postRemoveFromShortlist(id) {
    const seoUrl = getSeoUrlFromId(id, replaceIdString);
    return this.commonShortlistPostLogic(
      postRemoveFromShortlistUrl(seoUrl),
      seoUrl
    );
  }
}

import BaseClient from "./base-client";
import { getSeoUrlFromId, addPostHeaderWithVerificationToken } from "../util";

const replaceIdStringArray = ["shortlist-cb-", "shortlist-tpInfo-cb-"];

const postAddToShortlistUrl = `/shortlist?handler=AddToShortlist`;
const postRemoveFromShortlistUrl = `/shortlist?handler=RemoveFromShortlist`;
export default class ShortlistClient extends BaseClient {
  constructor() {
    super();
  }

  commonShortlistPostLogic = (urlToCall, shortlistModel) => {
    return this.postWithHandleError(
      urlToCall,
      shortlistModel,
      addPostHeaderWithVerificationToken()
    );
  };
  postAddToShortlist(id, totalShortlistedTuitionPartners) {
    const seoUrl = getSeoUrlFromId(id, replaceIdStringArray);
    const addToShortlistModel = {
      seoUrl: seoUrl,
      totalShortlistedTuitionPartners: totalShortlistedTuitionPartners,
    };
    return this.commonShortlistPostLogic(
      postAddToShortlistUrl,
      addToShortlistModel
    );
  }

  postRemoveFromShortlist(id, totalShortlistedTuitionPartners) {
    const seoUrl = getSeoUrlFromId(id, replaceIdStringArray);
    const removeFromShortlistModel = {
      seoUrl: seoUrl,
      totalShortlistedTuitionPartners: totalShortlistedTuitionPartners,
    };
    return this.commonShortlistPostLogic(
      postRemoveFromShortlistUrl,
      removeFromShortlistModel
    );
  }
}

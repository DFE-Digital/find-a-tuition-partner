import BaseClient from "./base-client";
import { getSeoUrlFromId, addPostHeaderWithVerificationToken } from "../util";

const replaceIdStringArray = ["compare-list-cb-", "compare-list-tpInfo-cb-"];

const postAddToCompareListUrl = `/compare-list?handler=AddToCompareList`;
const postRemoveFromCompareListUrl = `/compare-list?handler=RemoveFromCompareList`;
export default class CompareListClient extends BaseClient {
  constructor() {
    super();
  }

  commonCompareListPostLogic = (urlToCall, compareListModel) => {
    return this.postWithHandleError(
      urlToCall,
      compareListModel,
      addPostHeaderWithVerificationToken()
    );
  };
  postAddToCompareList(id, totalCompareListedTuitionPartners) {
    const seoUrl = getSeoUrlFromId(id, replaceIdStringArray);
    const addToCompareListModel = {
      seoUrl: seoUrl,
      totalCompareListedTuitionPartners: totalCompareListedTuitionPartners,
    };
    return this.commonCompareListPostLogic(
      postAddToCompareListUrl,
      addToCompareListModel
    );
  }

  postRemoveFromCompareList(id, totalCompareListedTuitionPartners) {
    const seoUrl = getSeoUrlFromId(id, replaceIdStringArray);
    const removeFromCompareListModel = {
      seoUrl: seoUrl,
      totalCompareListedTuitionPartners: totalCompareListedTuitionPartners,
    };
    return this.commonCompareListPostLogic(
      postRemoveFromCompareListUrl,
      removeFromCompareListModel
    );
  }
}

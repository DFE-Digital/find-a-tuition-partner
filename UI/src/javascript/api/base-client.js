import axios from "axios";
import { throwError } from "../util";

export default class BaseClient {
  constructor() {
    this.client = axios.create({
      baseURL: "/",
    });
  }

  getWithHandleError(url) {
    return this.get({
      url: url,
    })
      .then((response) => response)
      .catch((error) => {
        throwError(error);
      });
  }

  postWithHandleError(url, body, headers = null) {
    const data =
      headers == null
        ? {
            url: url,
            data: body,
          }
        : {
            url: url,
            data: body,
            headers,
          };
    return this.post(data)
      .then((response) => response)
      .catch((error) => {
        throwError(error);
      });
  }

  get(config) {
    return this.makeRequest({
      ...config,
      method: "get",
    });
  }

  post(config) {
    return this.makeRequest({
      ...config,
      method: "post",
    });
  }

  makeRequest(config) {
    return this.client.request(config).then((x) => x.data);
  }
}

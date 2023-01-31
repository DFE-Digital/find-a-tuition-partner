export const getSeoUrlFromId = (id, replaceString) =>
  id && id.replace(replaceString, "");

export const throwError = (message = "Invalid result") => {
  throw new Error(message);
};

export const addPostHeaderWithVerificationToken = () => ({
  RequestVerificationToken: document.getElementsByName(
    "__RequestVerificationToken"
  )[0].value,
  "Content-Type": "application/json",
  Accept: "application/json",
});

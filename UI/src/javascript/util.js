export const getSeoUrlFromId = (id, replaceIdStringArray) => {
  if (!id) return;

  replaceIdStringArray.forEach((str) => {
    id = id.replace(str, "");
  });

  return id;
};

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

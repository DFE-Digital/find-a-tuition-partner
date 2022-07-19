export const kebabCase = string => string
        .replace(/([a-z])([A-Z])/g, "$1-$2")
        .replace(/[\s_]+/g, '-')
        .toLowerCase();

export const removeWhitespace = string =>
        string.trim().replace(/\s/, '')
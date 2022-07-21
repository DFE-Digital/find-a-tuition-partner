export const kebabCase = string => string
        .replace(/([a-z])([A-Z])/g, "$1-$2")
        .replace(/[\s_]+/g, '-')
        .toLowerCase();

export const removeWhitespace = string =>
        string.trim().replace(/\s/, '')

export const  camelCaseKeyStage = s => {
        switch (s) {
        case 'Key stage 1': return 'KeyStage1';
        case 'Key stage 2': return 'KeyStage2';
        case 'Key stage 3': return 'KeyStage3';
        case 'Key stage 4': return 'KeyStage4';
        default: return '';
        }
}
        
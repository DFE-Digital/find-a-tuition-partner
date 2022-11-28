namespace Application.Constants
{
    public static class StringConstants
    {
        public const string PostcodeRegExp = "^[a-zA-Z]{1,2}[0-9][a-zA-Z0-9]? ?[0-9][a-zA-Z]{2}$";

        public const string CamelCaseBoundaries = @"((?<=[a-z])[A-Z]|(?<=[^\-\W])[A-Z](?=[a-z])|(?<=[a-z])\d+|(?<=\d+)[a-z])";
        public const string SpacesAndUnderscore = @"[\s_]+";
        public const string UrlUnsafeCharacters = "[^a-zA-Z0-9_{}()\\-~/]";
        public const string MultipleUnderscores = @"[-]{2,}";
    }
}

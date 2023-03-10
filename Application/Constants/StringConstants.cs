namespace Application.Constants
{
    public static class StringConstants
    {
        //Using the gov notify email reg exp so validation matches: https://github.com/alphagov/notifications-utils/blob/main/notifications_utils/__init__.py
        public const string EmailRegExp = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~\-]+@([^.@][^@\s]+)$";
        public const string WebsiteURLRegExp = @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,256}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$";
        public const string PhoneNumberRegExp = @"\d{4}";
        public const string PostcodeRegExp = "^[a-zA-Z]{1,2}[0-9][a-zA-Z0-9]? ?[0-9][a-zA-Z]{2}$";

        public const string CamelCaseBoundaries = @"((?<=[a-z])[A-Z]|(?<=[^\-\W])[A-Z](?=[a-z])|(?<=[a-z])\d+|(?<=\d+)[a-z])";
        public const string SpacesAndUnderscore = @"[\s_]+";
        public const string UrlUnsafeCharacters = "[^a-zA-Z0-9_{}()\\-~/]";
        public const string MultipleUnderscores = @"[-]{2,}";

        public const string English = "English";

        public const string Maths = "Maths";

        public const string Science = "Science";

        public const string Humanities = "Humanities";

        public const string ModernForeignLanguages = "Modern foreign languages";

        public const string SessionCookieName = ".FindATuitionPartner.Session";

        public const string EnquirerEmail = "EnquirerEmail";

        public const string EnquiryTutoringLogistics = "EnquiryTutoringLogistics";

        public const string EnquirySENDRequirements = "EnquirySENDRequirements";

        public const string EnquiryAdditionalInformation = "EnquiryAdditionalInformation";
    }
}

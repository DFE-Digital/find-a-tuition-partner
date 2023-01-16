using System.ComponentModel;

namespace UI.Enums
{
    public enum OrganisationTypeGrouping
    {
        [Description("Any")]
        Any = 0,

        [Description("Charity and social enterprise")]
        Charity = 1,

        [Description("Non-charity and social enterprise")]
        NonCharity,
    }
}

using System.ComponentModel;

namespace Domain.Enums
{
    public enum TuitionType
    {
        Unspecified = 0,

        [Description("Any")]
        Any = 99,

        [Description("Online")]
        Online = 1,

        [Description("In School")]
        InSchool,
    }
}

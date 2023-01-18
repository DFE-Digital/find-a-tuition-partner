using System.ComponentModel;

namespace Domain.Enums
{
    public enum TuitionType
    {
        [Description("Any")]
        Any = 0,

        [Description("Online")]
        Online = 1,

        [Description("In School")]
        InSchool = 2,
    }
}

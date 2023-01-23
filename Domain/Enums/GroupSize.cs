using System.ComponentModel;

namespace Domain.Enums
{
    public enum GroupSize
    {
        [Description("Any")]
        Any = 0,

        [Description("1 to 1")]
        One = 1,

        [Description("1 to 2")]
        Two = 2,

        [Description("1 to 3")]
        Three = 3,

        [Description("1 to 4")]
        Four = 4,

        [Description("1 to 5")]
        Five = 5,

        [Description("1 to 6")]
        Six = 6,
    }
}
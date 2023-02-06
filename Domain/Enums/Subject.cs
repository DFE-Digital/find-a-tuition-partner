using System.ComponentModel;

namespace Domain.Enums
{
    public enum Subject
    {
        [Description("Unspecified")]
        Unspecified = 0,

        [Description("English")]
        English,

        [Description("Maths")]
        Maths,

        [Description("Science")]
        Science,

        [Description("Humanities")]
        Humanities,

        [Description("Modern foreign languages")]
        ModernForeignLanguages,
    }
}

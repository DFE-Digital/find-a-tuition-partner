using System.ComponentModel;

namespace Application.Enums
{
    public enum KeyStage
    {
        Unspecified = 0,

        [Description("Key stage 1")]
        KeyStage1 = 1,

        [Description("Key stage 2")]
        KeyStage2,

        [Description("Key stage 3")]
        KeyStage3,

        [Description("Key stage 4")]
        KeyStage4,
    }
}

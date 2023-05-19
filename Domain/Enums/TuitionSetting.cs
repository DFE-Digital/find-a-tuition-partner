using System.ComponentModel;

namespace Domain.Enums
{
    public enum TuitionSetting
    {
        [Description("No preference")]
        NoPreference = 0,

        [Description("Online")]
        Online = 1,

        [Description("Face-to-face")]
        FaceToFace = 2,

        [Description("Both face to face and online")]
        Both = 99,
    }
}

using System.ComponentModel;
using Domain.Attributes;

namespace Domain.Enums
{
    public enum TuitionSetting
    {
        [Order(4)]
        [Description("No preference")]
        NoPreference = 0,

        [Order(2)]
        [Description("Online")]
        Online = 1,

        [Order(1)]
        [Description("Face-to-face")]
        FaceToFace = 2,

        [Order(3)]
        [Description("Both face-to-face and online")]
        Both = 99,
    }
}

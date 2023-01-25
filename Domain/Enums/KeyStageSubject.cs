using System.ComponentModel;

namespace Domain.Enums
{
    //TODO - similar code in Domain.Constants.Subjects and Infrastructure.EntityConfigurations.SubjectConfiguration and UI.Models.KeyStageSubject and review existing ParseKeyStageSubjects - refactor
    public enum KeyStageSubject
    {
        [Description("Key stage 1 English")]
        KeyStage1English = 1,

        [Description("Key stage 1 Maths")]
        KeyStage1Maths = 2,

        [Description("Key stage 1 Science")]
        KeyStage1Science = 3,

        [Description("Key stage 2 English")]
        KeyStage2English = 4,

        [Description("Key stage 2 Maths")]
        KeyStage2Maths = 5,

        [Description("Key stage 2 Science")]
        KeyStage2Science = 6,

        [Description("Key stage 3 English")]
        KeyStage3English = 7,

        [Description("Key stage 3 Humanities")]
        KeyStage3Humanities = 8,

        [Description("Key stage 3 Maths")]
        KeyStage3Maths = 9,

        [Description("Key stage 3 Modern Foreign Languages")]
        KeyStage3ModernForeignLanguages = 10,

        [Description("Key stage 3 Science")]
        KeyStage3Science = 11,

        [Description("Key stage 4 English")]
        KeyStage4English = 12,

        [Description("Key stage 4 Humanities")]
        KeyStage4Humanities = 13,

        [Description("Key stage 4 Maths")]
        KeyStage4Maths = 14,

        [Description("Key stage 4 Modern Foreign Languages")]
        KeyStage4ModernForeignLanguages = 15,

        [Description("Key stage 4 Science")]
        KeyStage4Science = 16,
    }
}
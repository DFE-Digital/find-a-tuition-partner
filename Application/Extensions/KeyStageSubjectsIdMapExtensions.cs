using Application.Common.Models;
using Application.Constants;
using Domain.Constants;

namespace Application.Extensions;

public static class KeyStageSubjectsIdMapExtensions
{
    public static IEnumerable<(int, int)> GetIdsForKeyStageSubjects(this KeyStageSubject[] keyStageSubjects)
    {
        var keyStagesMap = new Dictionary<int, int>()
        {
            { 1, KeyStages.Id.One },
            { 2, KeyStages.Id.Two },
            { 3, KeyStages.Id.Three },
            { 4, KeyStages.Id.Four }
        };

        var subjectsMap = new Dictionary<string, Dictionary<int, int>>()
        {
            {
                StringConstants.English, new Dictionary<int, int>()
                {
                    { KeyStages.Id.One, Subjects.Id.KeyStage1English },
                    { KeyStages.Id.Two, Subjects.Id.KeyStage2English },
                    { KeyStages.Id.Three, Subjects.Id.KeyStage3English },
                    { KeyStages.Id.Four, Subjects.Id.KeyStage4English }
                }
            },
            {
                StringConstants.Maths, new Dictionary<int, int>()
                {
                    { KeyStages.Id.One, Subjects.Id.KeyStage1Maths },
                    { KeyStages.Id.Two, Subjects.Id.KeyStage2Maths },
                    { KeyStages.Id.Three, Subjects.Id.KeyStage3Maths },
                    { KeyStages.Id.Four, Subjects.Id.KeyStage4Maths }
                }
            },
            {
                StringConstants.Science, new Dictionary<int, int>()
                {
                    { KeyStages.Id.One, Subjects.Id.KeyStage1Science },
                    { KeyStages.Id.Two, Subjects.Id.KeyStage2Science },
                    { KeyStages.Id.Three, Subjects.Id.KeyStage3Science },
                    { KeyStages.Id.Four, Subjects.Id.KeyStage4Science }
                }
            },
            {
                StringConstants.Humanities, new Dictionary<int, int>()
                {
                    { KeyStages.Id.Three, Subjects.Id.KeyStage3Humanities },
                    { KeyStages.Id.Four, Subjects.Id.KeyStage4Humanities }
                }
            },
            {
                StringConstants.ModernForeignLanguages, new Dictionary<int, int>()
                {
                    { KeyStages.Id.Three, Subjects.Id.KeyStage3ModernForeignLanguages },
                    { KeyStages.Id.Four, Subjects.Id.KeyStage4ModernForeignLanguages }
                }
            }
        };

        var result = new List<(int, int)>();

        foreach (var keyStageSubject in keyStageSubjects)
        {
            var keyStageId = (int)keyStageSubject.KeyStage;

            var subject = keyStageSubject.Subject;

            if (!keyStagesMap.TryGetValue(keyStageId, out var keyStageIdValue)) continue;

            if (subjectsMap.TryGetValue(subject, out var subjectMap)
                && subjectMap.TryGetValue(keyStageId, out var subjectIdValue))
            {
                result.Add((keyStageIdValue, subjectIdValue));
            }
        }

        return result;
    }
}
using Domain;

namespace Application.Extensions;

public static class KeyStageExtensions
{
    public static string DisplayList(this IEnumerable<KeyStage> keyStages)
    {
        var keyStageStrings = keyStages.Select(e => e.Id.ToString()).Distinct().OrderBy(e => e);
        return keyStageStrings.DisplayList();
    }
}
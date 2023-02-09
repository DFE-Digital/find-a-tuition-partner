using Domain;

namespace Application.Extensions;

public static class KeyStageExtensions
{
    public static string DisplayList(this IEnumerable<KeyStage> keyStages)
    {
        return keyStages.Select(e => e.Id.ToString()).Distinct().OrderBy(e => e).DisplayList();
    }
}
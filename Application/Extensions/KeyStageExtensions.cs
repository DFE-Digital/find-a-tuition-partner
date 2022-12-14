using Domain;

namespace Application.Extensions;

public static class KeyStageExtensions
{
    public static string DisplayList(this IEnumerable<KeyStage> keyStages)
    {
        var commaSeparated = string.Join(", ", keyStages.Select(e => e.Id).Distinct().OrderBy(e => e));

        var lastCommaIndex = commaSeparated.LastIndexOf(",", StringComparison.Ordinal);

        if (lastCommaIndex == -1) return commaSeparated;

        return commaSeparated.Remove(lastCommaIndex, 1).Insert(lastCommaIndex, " and");
    }
}
using Domain;

namespace Application.Extensions;

public static class SubjectExtensions
{
    public static string DisplayList(this IEnumerable<Subject> subjects)
    {
        var commaSeparated = string.Join(", ", subjects.Distinct().OrderBy(e => e.Id).Select(e => e.Name).Distinct());

        var lastCommaIndex = commaSeparated.LastIndexOf(",", StringComparison.Ordinal);

        if (lastCommaIndex == -1) return commaSeparated;

        return commaSeparated.Remove(lastCommaIndex, 1).Insert(lastCommaIndex, " and");
    }
}
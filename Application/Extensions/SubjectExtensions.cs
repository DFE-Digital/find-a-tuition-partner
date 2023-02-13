using Domain;

namespace Application.Extensions;

public static class SubjectExtensions
{
    public static string DisplayList(this IEnumerable<Subject> subjects)
    {
        var subjectStrings = subjects.Distinct().OrderBy(e => e.Id).Select(e => e.Name).Distinct();
        return subjectStrings.DisplayList();
    }
}
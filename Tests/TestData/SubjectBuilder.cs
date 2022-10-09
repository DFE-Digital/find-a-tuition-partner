using Domain;

namespace Tests.TestData;

public record SubjectBuilder()
{
    internal List<SubjectLessonBuilder> Lessons { get; private init; } = new();

    internal SubjectBuilder Subject(int lessonId, Func<SubjectLessonBuilder, SubjectLessonBuilder> subject)
        => new SubjectBuilder(this) with
        {
            Lessons = new List<SubjectLessonBuilder>(Lessons)
            {
                subject(new(lessonId))
            }
        };

    public ICollection<SubjectCoverage> SubjectCoverage
        => Lessons.SelectMany(x => x.SubjectCoverage).ToList();

    public ICollection<Price> Prices
        => Lessons.SelectMany(x => x.Prices).ToList();
}

using Domain;
using Domain.Constants;
using Tests.TestData;

namespace Tests;

public class TestTheBuilders
{
    [Fact]
    public void Subjects()
    {
        var subjects = new SubjectBuilder().Subject(1, s => s.Online().ForGroups(12.12m, 2, 3));
        subjects.Prices.Should().BeEquivalentTo(new[]
        {
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 2, HourlyRate = 12.12m },
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 3, HourlyRate = 12.12m },
        });
    }

    [Fact]
    public void Subjects2()
    {
        var subjects = new SubjectBuilder()
            .Subject(1, s => s.Online().ForGroups(12.12m, 2, 3))
            .Subject(1, s => s.InSchool().ForGroups(15m, 4));

        subjects.Prices.Should().BeEquivalentTo(new[]
        {
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 2, HourlyRate = 12.12m },
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 3, HourlyRate = 12.12m },
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.InSchool, GroupSize = 4, HourlyRate = 15m },
        });
    }

    [Fact]
    public void Subjects3()
    {
        var subjects = new SubjectBuilder()
            .Subject(1, s => s
                .InSchool().ForGroups(12.34m, 2, 3))
            .Subject(2, s => s
                .InSchool().ForGroups(56.78m, 2, 3)
                .Online().ForGroups(12.34m, 2, 3));

        subjects.Prices.Should().BeEquivalentTo(new[]
        {
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.InSchool, GroupSize = 2, HourlyRate = 12.34m },
            new Price{ SubjectId = 1, TuitionTypeId = (int)TuitionTypes.InSchool, GroupSize = 3, HourlyRate = 12.34m },
            new Price{ SubjectId = 2, TuitionTypeId = (int)TuitionTypes.InSchool, GroupSize = 2, HourlyRate = 56.78m },
            new Price{ SubjectId = 2, TuitionTypeId = (int)TuitionTypes.InSchool, GroupSize = 3, HourlyRate = 56.78m },
            new Price{ SubjectId = 2, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 2, HourlyRate = 12.34m },
            new Price{ SubjectId = 2, TuitionTypeId = (int)TuitionTypes.Online, GroupSize = 3, HourlyRate = 12.34m },
        });
    }
}

using Domain;
using TuitionType = UI.Enums.TuitionType;

namespace Tests.TestData;

internal record TuitionCost(decimal Cost, int[] GroupSizes);

internal record SubjectLessonBuilder(int SubjectId)
{
    private TuitionType Configuring { get; init; } = TuitionType.Online;
    private decimal ConfiguringCost { get; init; }

    public Dictionary<TuitionType, TuitionCost> Cost { get; private init; } = new();

    internal SubjectLessonBuilder InSchool()
        => new SubjectLessonBuilder(this) with { Configuring = TuitionType.InSchool };

    internal SubjectLessonBuilder Online()
        => new SubjectLessonBuilder(this) with { Configuring = TuitionType.Online };

    internal SubjectLessonBuilder Costing(decimal cost)
        => new SubjectLessonBuilder(this) with
        {
            ConfiguringCost = cost,
            Cost = new Dictionary<TuitionType, TuitionCost>(Cost)
            {
                [Configuring] = new(ConfiguringCost, new[] { 2 })
            }
        };

    internal SubjectLessonBuilder ForGroupSizes(params int[] groupSizes)
        => new SubjectLessonBuilder(this) with
        {
            Cost = new Dictionary<TuitionType, TuitionCost>(Cost)
            {
                [Configuring] = new(ConfiguringCost, groupSizes)
            }
        };

    public IEnumerable<SubjectCoverage> SubjectCoverage
        => Cost.Select(x => new SubjectCoverage
        {
            SubjectId = SubjectId,
            TuitionTypeId = (int)x.Key
        });

    public IEnumerable<Price> Prices
        => Cost.SelectMany(x => x.Value.GroupSizes.Select(y => new Price
        {
            TuitionTypeId = (int)x.Key,
            SubjectId = SubjectId,
            GroupSize = y,
            HourlyRate = x.Value.Cost,
        }));
}
using Domain;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Tests.TestData;

internal record TuitionCost(decimal Cost, int[] GroupSizes);

internal record SubjectLessonBuilder(int SubjectId)
{
    private TuitionSetting Configuring { get; init; } = TuitionSetting.Online;
    private decimal ConfiguringCost { get; init; }

    public Dictionary<TuitionSetting, TuitionCost> Cost { get; private init; } = new();

    internal SubjectLessonBuilder FaceToFace()
        => new SubjectLessonBuilder(this) with { Configuring = TuitionSetting.FaceToFace };

    internal SubjectLessonBuilder Online()
        => new SubjectLessonBuilder(this) with { Configuring = TuitionSetting.Online };

    internal SubjectLessonBuilder Costing(decimal cost)
        => new SubjectLessonBuilder(this) with
        {
            ConfiguringCost = cost,
            Cost = new Dictionary<TuitionSetting, TuitionCost>(Cost)
            {
                [Configuring] = new(ConfiguringCost, new[] { 2 })
            }
        };

    internal SubjectLessonBuilder ForGroupSizes(params int[] groupSizes)
        => new SubjectLessonBuilder(this) with
        {
            Cost = new Dictionary<TuitionSetting, TuitionCost>(Cost)
            {
                [Configuring] = new(ConfiguringCost, groupSizes)
            }
        };

    public IEnumerable<SubjectCoverage> SubjectCoverage
        => Cost.Select(x => new SubjectCoverage
        {
            SubjectId = SubjectId,
            TuitionSettingId = (int)x.Key
        });

    public IEnumerable<Price> Prices
        => Cost.SelectMany(x => x.Value.GroupSizes.Select(y => new Price
        {
            TuitionSettingId = (int)x.Key,
            SubjectId = SubjectId,
            GroupSize = y,
            HourlyRate = x.Value.Cost,
        }));
}
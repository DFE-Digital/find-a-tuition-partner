using Application.Extensions;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class EnumExtensionsTests
{
    [Fact]
    public void GetAllEnums_WithOrderBy()
    {
        // Act
        var allTuitionSettings = EnumExtensions.GetAllEnums<TuitionSetting>();

        // Assert
        allTuitionSettings.Count.Should().Be(4);
        allTuitionSettings[0].Should().Be(TuitionSetting.FaceToFace);
        allTuitionSettings[1].Should().Be(TuitionSetting.Online);
        allTuitionSettings[2].Should().Be(TuitionSetting.Both);
        allTuitionSettings[3].Should().Be(TuitionSetting.NoPreference);
    }

    [Fact]
    public void GetAllEnums_NoOrderBy()
    {
        // Act
        var allGroupSizes = EnumExtensions.GetAllEnums<GroupSize>();

        // Assert
        allGroupSizes.Count.Should().Be(7);
        allGroupSizes[0].Should().Be(GroupSize.Any);
        allGroupSizes[1].Should().Be(GroupSize.One);
        allGroupSizes[2].Should().Be(GroupSize.Two);
        allGroupSizes[3].Should().Be(GroupSize.Three);
        allGroupSizes[4].Should().Be(GroupSize.Four);
        allGroupSizes[5].Should().Be(GroupSize.Five);
        allGroupSizes[6].Should().Be(GroupSize.Six);
    }
}
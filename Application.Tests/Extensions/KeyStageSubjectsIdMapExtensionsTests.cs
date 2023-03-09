using System.Collections.Generic;
using Application.Common.Models;
using Application.Constants;
using Application.Extensions;
using Domain.Constants;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class KeyStageSubjectsIdMapExtensionsTests
{
    [Fact]
    public void GetIdsForKeyStageSubjects_ReturnsCorrectMapping()
    {
        // Arrange
        var keyStageSubjects = new[]
        {
            new KeyStageSubject(KeyStage.KeyStage1, StringConstants.English),
            new KeyStageSubject(KeyStage.KeyStage2, StringConstants.Math),
            new KeyStageSubject(KeyStage.KeyStage3, StringConstants.Science),
            new KeyStageSubject(KeyStage.KeyStage4, StringConstants.Humanities)
        };

        var expectedMapping = new List<(int, int)>()
        {
            { (KeyStages.Id.One, Subjects.Id.KeyStage1English) },
            { (KeyStages.Id.Two, Subjects.Id.KeyStage2Maths) },
            { (KeyStages.Id.Three, Subjects.Id.KeyStage3Science) },
            { (KeyStages.Id.Four, Subjects.Id.KeyStage4Humanities) },
        };

        // Act
        var result = keyStageSubjects.GetIdsForKeyStageSubjects();

        // Assert
        result.Should().BeEquivalentTo(expectedMapping);
    }

    [Fact]
    public void GetIdsForKeyStageSubjects_ReturnsEmptyList_WhenInputIsEmpty()
    {
        // Arrange
        var keyStageSubjects = new KeyStageSubject[] { };

        // Act
        var result = keyStageSubjects.GetIdsForKeyStageSubjects();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetIdsForKeyStageSubjects_ReturnsEmptyList_WhenInputHasUnknownKeyStage()
    {
        // Arrange
        var keyStageSubjects = new KeyStageSubject[]
        {
            new KeyStageSubject (KeyStage.Unspecified,StringConstants.English )
        };

        // Act
        var result = keyStageSubjects.GetIdsForKeyStageSubjects();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetIdsForKeyStageSubjects_ReturnsEmptyList_WhenInputHasUnknownSubject()
    {
        // Arrange
        var keyStageSubjects = new KeyStageSubject[]
        {
            new KeyStageSubject (KeyStage.KeyStage1,"French" )
        };

        // Act
        var result = keyStageSubjects.GetIdsForKeyStageSubjects();

        // Assert
        Assert.Empty(result);
    }

}

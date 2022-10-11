using Application.Mapping;
using Domain.Constants;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Mappings;
public class SchoolDatumTests
{
    [Theory]
    [InlineData(26)]
    [InlineData(37)]
    public void Invalid_overseas_schools(int establishmentType)
    {
        SchoolDatum data = new();
        data.EstablishmentType = establishmentType;
        data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        data.IsValidForService().Should().BeFalse();
    }

    [Theory]
    [InlineData((int)EstablishmentTypeGroups.Colleges)]
    [InlineData((int)EstablishmentTypeGroups.Universities)]
    [InlineData((int)EstablishmentTypeGroups.IndependentSchools)]
    [InlineData((int)EstablishmentTypeGroups.LocalAuthorityMaintainedSchools)]
    [InlineData((int)EstablishmentTypeGroups.SpecialSchools)]
    [InlineData((int)EstablishmentTypeGroups.OtherTypes)]
    [InlineData((int)EstablishmentTypeGroups.Academies)]
    [InlineData((int)EstablishmentTypeGroups.FreeSchools)]
    public void Valid_uk_schools(int establishmentType)
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
        data.EstablishmentTypeGroup = establishmentType;
        data.IsValidForService().Should().BeTrue();
    }

    [Fact]
    public void Invalid_uk_schools()
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.WelshSchools;
        data.IsValidForService().Should().BeFalse();
    }

    [Theory]
    [InlineData((int)EstablishmentsStatus.Closed)]
    [InlineData((int)EstablishmentsStatus.ProposedToOpen)]
    [InlineData((int)EstablishmentsStatus.NotApplicable)]
    public void Invalid_closed_schools(int status)
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = status;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        data.IsValidForService().Should().BeFalse();
    }

    [Theory]
    [InlineData((int)EstablishmentsStatus.Open)]
    [InlineData((int)EstablishmentsStatus.OpenButProposedToClose)]
    public void Valid_open_schools(int status)
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = status;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        data.IsValidForService().Should().BeTrue();
    }
}


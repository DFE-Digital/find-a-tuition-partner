using System;
using Application.Mapping;
using Domain.Constants;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Mappings;
public class SchoolDatumTests
{
    [Theory]
    [InlineData(EstablishmentTypes.Id.BritishSchoolsOverseas)]
    [InlineData(EstablishmentTypes.Id.ServiceChildrensEducation)]
    public void Invalid_overseas_schools(int establishmentType)
    {
        SchoolDatum data = new();
        data.EstablishmentType = establishmentType;
        data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        data.EstablishmentNumber = 111;
        data.IsValidForService().Should().BeFalse();
    }

    [Fact]
    public void Valid_uk_schools()
    {
        foreach (int establishmentTypeGroup in Enum.GetValues(typeof(EstablishmentTypeGroups)))
        {
            if (establishmentTypeGroup != (int)EstablishmentTypeGroups.WelshSchools)
            {
                SchoolDatum data = new();
                data.EstablishmentType = 0;
                data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
                data.EstablishmentTypeGroup = establishmentTypeGroup;
                data.EstablishmentNumber = 111;
                data.IsValidForService().Should().BeTrue();
            }
        }
    }

    [Fact]
    public void Invalid_uk_schools()
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = (int)EstablishmentsStatus.Open;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.WelshSchools;
        data.EstablishmentNumber = 111;
        data.IsValidForService().Should().BeFalse();
    }

    [Theory]
    [InlineData((int)EstablishmentsStatus.Closed)]
    [InlineData((int)EstablishmentsStatus.ProposedToOpen)]
    public void Invalid_closed_schools(int status)
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = status;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        data.EstablishmentNumber = 111;
        data.IsValidForService().Should().BeFalse();
    }

    [Theory]
    [InlineData((int)EstablishmentsStatus.Open)]
    [InlineData((int)EstablishmentsStatus.OpenButProposedToClose)]
    public void Invalid_missing_establishment_number(int status)
    {
        SchoolDatum data = new();
        data.EstablishmentType = 0;
        data.EstablishmentStatus = status;
        data.EstablishmentTypeGroup = (int)EstablishmentTypeGroups.IndependentSchools;
        //data.EstablishmentNumber = 111;
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
        data.EstablishmentNumber = 111;
        data.IsValidForService().Should().BeTrue();
    }
}


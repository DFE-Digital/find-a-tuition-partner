using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using Domain.Enums;
using FluentValidation.TestHelper;
using Tests.TestData;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class CheckYourAnswersTests
{
    private readonly SliceFixture _fixture;

    public CheckYourAnswersTests(SliceFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Has_valid_data(CheckYourAnswersModel model)
    {
        var result = new CheckYourAnswersModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InValidTestData))]
    public void Has_invalid_data(CheckYourAnswersModel model)
    {
        var result = new CheckYourAnswersModelValidator().TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionType = TuitionType.Online,
                Email = "test@test.com",
                TutoringLogistics = "Test content",
                ConfirmTermsAndConditions = true
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionType = TuitionType.Any,
                Email = "test@test.com",
                TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
                ConfirmTermsAndConditions = true
            }
        };
    }

    public static IEnumerable<object[]> InValidTestData()
    {
        yield return new object[]
        {
            new CheckYourAnswersModel {
                //Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionType = TuitionType.Any,
                Email = "test@test.com",
                TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                //KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                //{
                //    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                //},
                //HasKeyStageSubjects = true,
                TuitionType = TuitionType.Any,
                Email = "test@test.com",
                TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                //TuitionType = TuitionType.Any,
                Email = "test@test.com",
                TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionType = TuitionType.Any,
                //Email = "test@test.com",
                TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionType = TuitionType.Any,
                Email = "test@test.com",
                //TutoringLogistics = "Test content",
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information"
            }
        };
    }
}
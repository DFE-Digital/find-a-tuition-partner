using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Validators.Enquiry.Build;
using Domain.Constants;
using Domain.Enums;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Notify.Exceptions;
using Tests.TestData;
using UI.Pages.Enquiry.Build;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class CheckYourAnswersTests
{
    private readonly SliceFixture _fixture;
    private readonly CheckYourAnswersModelValidator _validator;

    public CheckYourAnswersTests(SliceFixture fixture)
    {
        _fixture = fixture;
        _validator = new CheckYourAnswersModelValidator();

        _ = _fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a", "Alpha")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));

        _ = _fixture.AddSchool(A.School);
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Has_valid_data_no_validation_errors(CheckYourAnswersModel model)
    {
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InValidTestData))]
    public void Has_invalid_data_has_validation_errors(CheckYourAnswersModel model)
    {
        var result = _validator.TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = new CheckYourAnswersModel
        {
            TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
            {
                NumberOfPupils = new string('*', IntegerConstants.EnquiryShortQuestionsMaxCharacterSize + 1),
                StartDate = new string('*', IntegerConstants.EnquiryShortQuestionsMaxCharacterSize + 1),
                TuitionDuration = new string('*', IntegerConstants.EnquiryShortQuestionsMaxCharacterSize + 1),
                TimeOfDay = new string('*', IntegerConstants.EnquiryShortQuestionsMaxCharacterSize + 1)
            },
            SENDRequirements = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            AdditionalInformation = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            ConfirmTermsAndConditions = true
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsDetailsModel.NumberOfPupils);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsDetailsModel.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsDetailsModel.TuitionDuration);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsDetailsModel.TimeOfDay);
        result.ShouldHaveValidationErrorFor(x => x.SENDRequirements);
        result.ShouldHaveValidationErrorFor(x => x.AdditionalInformation);
    }

    [Fact]
    public async Task With_a_valid_data_post_moves_to_next_page()
    {
        // Arrange
        var model = new CheckYourAnswersModel
        {
            Postcode = District.Dacorum.SamplePostcode,
            KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.English } }
                },
            Subjects = new string[] { "KeyStage1-English" },
            HasKeyStageSubjects = true,
            TuitionSetting = TuitionSetting.NoPreference,
            SchoolId = 1,
            SchoolUrn = 123,
            Email = "test@test.com",
            TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
            {
                NumberOfPupils = "Test content",
                StartDate = "Test content",
                TuitionDuration = "Test content",
                TimeOfDay = "Test content"
            },
            ConfirmTermsAndConditions = true
        };

        //Act
        var result = await _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            page.Data = model;
            return page.OnPostAsync();
        });

        //Assert
        result.Should().NotBeNull();
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(SubmittedConfirmation));
    }

    [Fact]
    public async Task With_invalid_data_post_throws_exception()
    {
        // Arrange
        var model = new CheckYourAnswersModel
        {
            Postcode = District.Dacorum.SamplePostcode,
            KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.English } }
                },
            Subjects = new string[] { "KeyStage1-English" },
            HasKeyStageSubjects = true,
            TuitionSetting = TuitionSetting.NoPreference,
            SchoolId = 1,
            Email = "test@test.com",
            TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
            {
                NumberOfPupils = "Test content",
                StartDate = "Test content",
                //TuitionDuration = "Test content",
                TimeOfDay = "Test content"
            },
            ConfirmTermsAndConditions = true
        };

        //Act
        Task act() => _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            page.Data = model;
            return page.OnPostAsync();
        });

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        exception.Message.Should().Be("The AddEnquiryCommand Data.TutoringLogisticsDetailsModel.TuitionDuration is null or empty");
    }


    [Fact]
    public async Task With_invalid_email_post_throws_400_exception()
    {
        // Arrange
        var model = new CheckYourAnswersModel
        {
            Postcode = District.Dacorum.SamplePostcode,
            KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.English } }
                },
            Subjects = new string[] { "KeyStage1-English" },
            HasKeyStageSubjects = true,
            TuitionSetting = TuitionSetting.NoPreference,
            SchoolId = 1,
            Email = "400error@test",
            TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
            {
                NumberOfPupils = "Test content",
                StartDate = "Test content",
                TuitionDuration = "Test content",
                TimeOfDay = "Test content"
            },
            ConfirmTermsAndConditions = true
        };

        //Act
        var result = await _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            page.Data = model;
            return page.OnPostAsync();
        });

        //Assert
        result.Should().NotBeNull();
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(EnquirerEmail));
    }

    [Fact]
    public async Task With_invalid_notification_post_throws_500_exception()
    {
        // Arrange
        var model = new CheckYourAnswersModel
        {
            Postcode = District.Dacorum.SamplePostcode,
            KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.English } }
                },
            Subjects = new string[] { "KeyStage1-English" },
            HasKeyStageSubjects = true,
            TuitionSetting = TuitionSetting.NoPreference,
            SchoolId = 1,
            Email = "500error@test",
            TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
            {
                NumberOfPupils = "Test content",
                StartDate = "Test content",
                TuitionDuration = "Test content",
                TimeOfDay = "Test content"
            },
            ConfirmTermsAndConditions = true
        };

        //Act
        Task act() => _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            page.Data = model;
            return page.OnPostAsync();
        });

        //Assert
        var exception = await Assert.ThrowsAsync<NotifyClientException>(act);
        exception.Message.Should().Be("Status code 500. Error: Some serious issue");
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
                TuitionSetting = TuitionSetting.Online,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
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
                TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
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
                Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
                ConfirmTermsAndConditions = false
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                //Postcode = District.EastRidingOfYorkshire.SamplePostcode,
                KeyStageSubjects = new Dictionary<KeyStage, List<Subject>>()
                {
                    {KeyStage.KeyStage1, new List<Subject>() { Subject.Maths } }
                },
                HasKeyStageSubjects = true,
                TuitionSetting = TuitionSetting.NoPreference,
                //SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
                ConfirmTermsAndConditions = true
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
                TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                //TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                //Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                TuitionSetting = TuitionSetting.NoPreference,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    //NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                TuitionSetting = TuitionSetting.NoPreference,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    //StartDate = "Test content",
                    TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                TuitionSetting = TuitionSetting.NoPreference,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    //TuitionDuration = "Test content",
                    TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
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
                TuitionSetting = TuitionSetting.NoPreference,
                SchoolId = 1,
                Email = "test@test.com",
                TutoringLogisticsDetailsModel = new Application.Common.Models.Enquiry.TutoringLogisticsDetailsModel()
                {
                    NumberOfPupils = "Test content",
                    StartDate = "Test content",
                    TuitionDuration = "Test content",
                    //TimeOfDay = "Test content"
                },
                SENDRequirements = "some SEND reqs",
                AdditionalInformation = "some Additional Information",
                ConfirmTermsAndConditions = true
            }
        };
    }

    private static PageContext GetPageContext()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        return new PageContext(actionContext);
    }
}
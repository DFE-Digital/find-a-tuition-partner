using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Application.Validators.Enquiry.Respond;
using Domain;
using Domain.Constants;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Tests.TestData;
using UI.Pages.Enquiry.Respond;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Tests.Enquiry.Respond;

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
    public void Has_valid_data_no_validation_errors(CheckYourAnswersModel model)
    {
        var result = new CheckYourAnswersModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InValidTestData))]
    public void Has_invalid_data_has_validation_errors(CheckYourAnswersModel model)
    {
        var result = new CheckYourAnswersModelValidator().TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = new CheckYourAnswersModel
        {
            KeyStageAndSubjectsText = new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1),
            TuitionSettingText = new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1),
            TutoringLogisticsText = new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1),
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1),
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1)
        };

        var result = new CheckYourAnswersModelValidator().TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.KeyStageAndSubjectsText);
        result.ShouldHaveValidationErrorFor(x => x.TuitionSettingText);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsText);
        result.ShouldHaveValidationErrorFor(x => x.SENDRequirementsText);
        result.ShouldHaveValidationErrorFor(x => x.AdditionalInformationText);
    }

    [Fact]
    public async Task With_a_invalid_token_get_goes_to_404()
    {
        //Arrange
        var model = new CheckYourAnswersModel
        {
            KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
            TuitionSettingText = "TuitionSettingText",
            TutoringLogisticsText = "TutoringLogisticsText",
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = "SENDRequirementsText",
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = "AdditionalInformationText"
        };

        //Act
        var result = await _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnGetAsync(model);
        });

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task With_a_invalid_token_post_goes_to_404()
    {
        //Act
        var result = await _fixture.GetPage<CheckYourAnswers>().Execute(page =>
        {
            return page.OnPostAsync();
        });

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task With_a_valid_data_post_moves_to_next_page()
    {
        // Arrange
        _ = _fixture.ResetDatabase();

        _ = _fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(13)
            .WithName("b", "Beta")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));


        _ = _fixture.InsertAsync(
            new EmailLog()
            {
                Id = 13,
                FinishProcessingDate = DateTime.UtcNow.AddDays(2),
                EmailAddress = "email@test.com",
                EmailTemplateShortName = "abc",
                ClientReferenceNumber = "abc-RF13",
                EmailStatusId = 1
            },
            new MagicLink()
            {
                Id = 13,
                Token = "EnquirerToken"
            },
            new Domain.Enquiry()
            {
                Id = 13,
                TutoringLogistics = "TutoringLogistics",
                SENDRequirements = "SENDRequirements",
                AdditionalInformation = "AdditionalInformation",
                Email = "test@test.com",
                SupportReferenceNumber = "RF13",
                PostCode = District.Dacorum.SamplePostcode,
                LocalAuthorityDistrict = District.Dacorum.LocalAuthorityName,
                MagicLinkId = 13,
                CreatedAt = DateTime.UtcNow,
                EnquirerEnquirySubmittedEmailLogId = 13
            },
            new EmailLog()
            {
                Id = 130,
                FinishProcessingDate = DateTime.UtcNow.AddDays(2),
                EmailAddress = "email@test.com",
                EmailTemplateShortName = "abc",
                ClientReferenceNumber = "abc-RF13-tp",
                EmailStatusId = 1
            },
            new MagicLink()
            {
                Id = 130,
                Token = "TPToken"
            },
            new TuitionPartnerEnquiry()
            {
                Id = 13,
                EnquiryId = 13,
                TuitionPartnerId = 13,
                MagicLinkId = 130,
                ResponseCloseDate = DateTime.UtcNow.AddDays(7),
                TuitionPartnerEnquirySubmittedEmailLogId = 130
            }
        );

        var model = new CheckYourAnswersModel
        {
            KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
            TuitionSettingText = "TuitionSettingText",
            TutoringLogisticsText = "TutoringLogisticsText",
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = "SENDRequirementsText",
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = "AdditionalInformationText",
            LocalAuthorityDistrict = "LocalAuthorityDistrict",
            EnquiryKeyStageSubjects = new List<string>() { "EnquiryKeyStageSubjects" },
            EnquiryTuitionSetting = "EnquiryTuitionSetting",
            EnquiryTutoringLogisticsDisplayModel = new Application.Common.Models.Enquiry.TutoringLogisticsDisplayModel()
            {
                TutoringLogistics = "EnquiryTutoringLogistics"
            },
            TuitionPartnerSeoUrl = "b",
            SupportReferenceNumber = "RF13",
            Token = "TPToken"
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
        var redirect = result.Should().BeOfType<RedirectResult>().Which;
        redirect.Url.Should().Contain("confirmation");
    }

    [Fact]
    public async Task With_invalid_data_post_throws_exception()
    {
        // Arrange
        _ = _fixture.ResetDatabase();

        _ = _fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(14)
            .WithName("c", "Charlie")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));


        _ = _fixture.InsertAsync(
            new EmailLog()
            {
                Id = 14,
                FinishProcessingDate = DateTime.UtcNow.AddDays(2),
                EmailAddress = "email@test.com",
                EmailTemplateShortName = "abc",
                ClientReferenceNumber = "abc-RF14",
                EmailStatusId = 1
            },
            new MagicLink()
            {
                Id = 14,
                Token = "EnquirerToken"
            },
            new Domain.Enquiry()
            {
                Id = 14,
                TutoringLogistics = "TutoringLogistics",
                SENDRequirements = "SENDRequirements",
                AdditionalInformation = "AdditionalInformation",
                Email = "test@test.com",
                SupportReferenceNumber = "RF14",
                PostCode = District.Dacorum.SamplePostcode,
                LocalAuthorityDistrict = District.Dacorum.LocalAuthorityName,
                MagicLinkId = 14,
                CreatedAt = DateTime.UtcNow,
                EnquirerEnquirySubmittedEmailLogId = 14
            },
            new EmailLog()
            {
                Id = 140,
                FinishProcessingDate = DateTime.UtcNow.AddDays(2),
                EmailAddress = "email@test.com",
                EmailTemplateShortName = "abc",
                ClientReferenceNumber = "abc-RF14-tp",
                EmailStatusId = 1
            },
            new MagicLink()
            {
                Id = 140,
                Token = "TPToken"
            },
            new TuitionPartnerEnquiry()
            {
                Id = 14,
                EnquiryId = 14,
                TuitionPartnerId = 14,
                MagicLinkId = 140,
                ResponseCloseDate = DateTime.UtcNow.AddDays(7),
                TuitionPartnerEnquirySubmittedEmailLogId = 140
            }
        );

        var model = new CheckYourAnswersModel
        {
            //KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
            TuitionSettingText = "TuitionSettingText",
            TutoringLogisticsText = "TutoringLogisticsText",
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = "SENDRequirementsText",
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = "AdditionalInformationText",
            LocalAuthorityDistrict = "LocalAuthorityDistrict",
            EnquiryKeyStageSubjects = new List<string>() { "EnquiryKeyStageSubjects" },
            EnquiryTuitionSetting = "EnquiryTuitionSetting",
            EnquiryTutoringLogisticsDisplayModel = new Application.Common.Models.Enquiry.TutoringLogisticsDisplayModel()
            {
                TutoringLogistics = "EnquiryTutoringLogistics"
            },
            TuitionPartnerSeoUrl = "c",
            SupportReferenceNumber = "RF14",
            Token = "TPToken"
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
        exception.Message.Should().Be("The AddEnquiryResponseCommand Data.KeyStageAndSubjectsText is null or empty");
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                //EnquirySENDRequirements = "EnquirySENDRequirements",
                //SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                //EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                //AdditionalInformationText = "AdditionalInformationText"
            }
        };
    }


    public static IEnumerable<object[]> InValidTestData()
    {
        yield return new object[]
        {
            new CheckYourAnswersModel {
                //KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                //TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                //TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                //SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                AdditionalInformationText = "AdditionalInformationText"
            }
        };

        yield return new object[]
        {
            new CheckYourAnswersModel {
                KeyStageAndSubjectsText = "KeyStageAndSubjectsText",
                TuitionSettingText = "TuitionSettingText",
                TutoringLogisticsText = "TutoringLogisticsText",
                EnquirySENDRequirements = "EnquirySENDRequirements",
                SENDRequirementsText = "SENDRequirementsText",
                EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
                //AdditionalInformationText = "AdditionalInformationText"
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
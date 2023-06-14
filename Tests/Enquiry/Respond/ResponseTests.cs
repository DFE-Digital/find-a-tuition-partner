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
public class ResponseTests
{
    private readonly SliceFixture _fixture;

    public ResponseTests(SliceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Has_less_than_max_data()
    {
        var model = new ViewAndCaptureEnquiryResponseModel
        {
            KeyStageAndSubjectsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1),
            TuitionSettingText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1),
            TutoringLogisticsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1),
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1),
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1)
        };

        var result = new ViewAndCaptureEnquiryResponseModelValidator().TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_equal_to_max_data()
    {
        var model = new ViewAndCaptureEnquiryResponseModel
        {
            KeyStageAndSubjectsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize),
            TuitionSettingText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize),
            TutoringLogisticsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize),
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize),
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize)
        };

        var result = new ViewAndCaptureEnquiryResponseModelValidator().TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = new ViewAndCaptureEnquiryResponseModel
        {
            KeyStageAndSubjectsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            TuitionSettingText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            TutoringLogisticsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            EnquirySENDRequirements = "EnquirySENDRequirements",
            SENDRequirementsText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1),
            EnquiryAdditionalInformation = "EnquiryAdditionalInformation",
            AdditionalInformationText = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1)
        };

        var result = new ViewAndCaptureEnquiryResponseModelValidator().TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.KeyStageAndSubjectsText);
        result.ShouldHaveValidationErrorFor(x => x.TuitionSettingText);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogisticsText);
        result.ShouldHaveValidationErrorFor(x => x.SENDRequirementsText);
        result.ShouldHaveValidationErrorFor(x => x.AdditionalInformationText);
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Has_valid_data_no_validation_errors(ViewAndCaptureEnquiryResponseModel model)
    {
        var result = new ViewAndCaptureEnquiryResponseModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InValidTestData))]
    public void Has_invalid_data_has_validation_errors(ViewAndCaptureEnquiryResponseModel model)
    {
        var result = new ViewAndCaptureEnquiryResponseModelValidator().TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async Task With_a_invalid_token_get_goes_to_404()
    {
        //Act
        var result = await _fixture.GetPage<EditResponse>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnGetAsync();
        });

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task With_a_invalid_token_post_goes_to_404()
    {
        //Act
        var result = await _fixture.GetPage<EditResponse>().Execute(page =>
        {
            return page.OnPostAsync();
        });

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        // Arrange
        _ = _fixture.ResetDatabase();

        _ = _fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(15)
            .WithName("d", "Delta")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));


        _ = _fixture.InsertAsync(
            new MagicLink()
            {
                Id = 15,
                Token = "EnquirerToken"
            },
            new Domain.Enquiry()
            {
                Id = 15,
                TutoringLogistics = "TutoringLogistics",
                SENDRequirements = "SENDRequirements",
                AdditionalInformation = "AdditionalInformation",
                Email = "test@test.com",
                SupportReferenceNumber = "RF15",
                PostCode = District.Dacorum.SamplePostcode,
                LocalAuthorityDistrict = District.Dacorum.LocalAuthorityName,
                MagicLinkId = 15,
                CreatedAt = DateTime.UtcNow
            },
            new MagicLink()
            {
                Id = 150,
                Token = "TPToken"
            },
            new TuitionPartnerEnquiry()
            {
                Id = 15,
                EnquiryId = 15,
                TuitionPartnerId = 15,
                MagicLinkId = 150,
                ResponseCloseDate = DateTime.UtcNow.AddDays(7)
            }
        );

        var model = new ViewAndCaptureEnquiryResponseModel
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
            TuitionPartnerSeoUrl = "d",
            SupportReferenceNumber = "RF15",
            Token = "TPToken"
        };

        //Act
        var result = await _fixture.GetPage<EditResponse>().Execute(page =>
        {
            page.Data = model;
            return page.OnPostAsync();
        });

        //Assert
        result.Should().NotBeNull();
        var redirect = result.Should().BeOfType<RedirectResult>().Which;
        redirect.Url.Should().Contain("check-your-answers");
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return new object[]
        {
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
            new ViewAndCaptureEnquiryResponseModel {
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
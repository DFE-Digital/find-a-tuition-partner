using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        //TODO - Postcode needs to be validated, but not shown on check your answers page, so what to do if invalid?
        RuleFor(m => m.Postcode)
            .NotEmpty()
            .WithMessage("Enter a postcode");

        RuleFor(m => m.HasKeyStageSubjects)
            .NotEqual(false)
            .WithMessage("Select at least one key stage and related subject");

        RuleFor(m => m.TuitionType)
            .NotEmpty()
            .WithMessage("Select a tuition type option");

        RuleFor(request => request.Email).NotEmpty().WithMessage("Enter an email address")
            .Matches(StringConstants.EmailRegExp).WithMessage("You must enter an email address in the correct format");

        RuleFor(request => request.TutoringLogistics)
             .NotEmpty()
             .WithMessage("Enter the type of tuition plan that you need")
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"The type of tuition plan must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.SENDRequirements)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"SEND requirements must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.AdditionalInformation)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Any other considerations for tuition partners to consider must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(m => m.ConfirmTermsAndConditions)
            .NotEqual(false)
            .WithMessage("Select to confirm that you have not included any information that would allow anyone to identify pupils, such as names or specific characteristics");
    }
}
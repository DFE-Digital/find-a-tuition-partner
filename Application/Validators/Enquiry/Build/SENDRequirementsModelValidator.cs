﻿using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class SENDRequirementsModelValidator : AbstractValidator<SENDRequirementsModel>
{
    public SENDRequirementsModelValidator()
    {
        RuleFor(request => request.SENDRequirements)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"SEND requirements must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");
    }
}
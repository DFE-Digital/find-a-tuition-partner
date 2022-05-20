using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UI.Filters;

public class FluentValidationExceptionAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            foreach (var validationExceptionError in validationException.Errors)
            {
                context.ModelState.AddModelError(validationExceptionError.PropertyName, validationExceptionError.ErrorMessage);
            }

            var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

            context.Result = new BadRequestObjectResult(validationProblemDetails);
        }
    }
}
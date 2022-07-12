using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UI.TagHelpers;

[HtmlTargetElement("div", Attributes = "asp-validation-group-for")]
public class ValidationRowHelper : TagHelper
{
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    [HtmlAttributeName("asp-validation-group-for")]
    public string PropertyName { get; set; } = null!;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (PropertyIsInvalid())
            output.Attributes.Add("class", "govuk-form-group--error");

        return Task.CompletedTask;
    }

    private bool PropertyIsInvalid()
    {
        return 
            ViewContext.ModelState.TryGetValue(PropertyName, out var modelState)
            && modelState.ValidationState == ModelValidationState.Invalid;
    }
}
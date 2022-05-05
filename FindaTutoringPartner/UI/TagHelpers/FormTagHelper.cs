using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UI.TagHelpers;

[HtmlTargetElement("form")]
public class FormTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);

        output.Attributes.Add("novalidate", string.Empty);
    }
}
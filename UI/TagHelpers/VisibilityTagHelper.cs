using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UI.TagHelpers;

[HtmlTargetElement(Attributes = "show-if")]
public class ShowTagHelper : TagHelper
{
    public bool? ShowIf { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!(ShowIf ?? false)) output.SuppressOutput();
    }
}

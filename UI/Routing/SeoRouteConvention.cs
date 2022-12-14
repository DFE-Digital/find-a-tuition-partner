using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace UI.Routing;

public class SeoRouteConvention : IPageRouteModelConvention, IOutboundParameterTransformer
{
    public void Apply(PageRouteModel model)
    {
        foreach (var selector in model.Selectors.ToList())
        {
            if (selector.AttributeRouteModel != null)
            {
                selector.AttributeRouteModel.Template = selector.AttributeRouteModel.Template.ToSeoUrl();
            }
        }
    }

    public string? TransformOutbound(object? value)
    {
        return value?.ToString().ToSeoUrl();
    }
}
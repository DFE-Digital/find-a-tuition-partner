using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace UI.Routing;

public class SeparatedQueryStringValueProvider : QueryStringValueProvider
{
    private readonly string _separator;

    public SeparatedQueryStringValueProvider(IQueryCollection values, string separator)
        : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
    {
        _separator = separator;
    }

    public override ValueProviderResult GetValue(string key)
    {
        var result = base.GetValue(key);

        if (result.Values.Any(x => x.Contains(_separator, StringComparison.OrdinalIgnoreCase)))
        {
            var splitValues = new StringValues(
                result.Values
                    .SelectMany(x => x.Split(_separator))
                    .ToArray());
            return new ValueProviderResult(splitValues, result.Culture);
        }

        return result;
    }
}

public class SeparatedQueryStringValueProviderFactory : IValueProviderFactory
{
    private readonly string _separator;

    public SeparatedQueryStringValueProviderFactory(string separator)
        => _separator = separator;

    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        context.ValueProviders.Insert(
            0,
            new SeparatedQueryStringValueProvider(
                context.ActionContext.HttpContext.Request.Query,
                _separator));
        return Task.CompletedTask;
    }
}

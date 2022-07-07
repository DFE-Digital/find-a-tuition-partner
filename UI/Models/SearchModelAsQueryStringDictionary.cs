using System.Linq.Expressions;
using UI.Pages.FindATuitionPartner;

namespace UI.Models;

public static class SearchModelAsQueryStringDictionary
{
    public static Dictionary<string, string> ToRouteData(this SearchModel model)
    {
        var dictionary = new Dictionary<string, string>();

        model.Add(x => x.Postcode, dictionary);
        model.Add(x => x.TuitionType, dictionary);
        model.AddAll(x => x.KeyStages, dictionary);
        model.AddAll(x => x.Subjects, dictionary);

        return dictionary;
    }

    private static void AddAll<T>(
        this SearchModel model,
        Expression<Func<SearchModel, IEnumerable<T>?>> expression,
        Dictionary<string, string> dictionary)
    {
        model.Add(expression, (prop, name) => BuildQueryString(prop, name), dictionary);
    }

    private static void Add<T>(
        this SearchModel model,
        Expression<Func<SearchModel, T?>> expression,
        Dictionary<string, string> dictionary)
    {
        model.Add(expression, (prop, _) => prop?.ToString() ?? "", dictionary);
    }

    private static void Add<T>(
        this SearchModel model,
        Expression<Func<SearchModel, T?>> expression,
        Func<T, string, string> buildQueryText,
        Dictionary<string, string> dictionary)
    {
        var name = expression.MemberName();
        var property = expression.Compile()(model);

        if (property is null) return;

        var qs = buildQueryText(property, name);

        dictionary.Add(name, qs);
    }

    private static string MemberName<T>(this Expression<Func<SearchModel, T?>> expression)
        => expression.Body switch
        {
            MemberExpression x when x.Member != null => x.Member.Name,
            _ => throw new ArgumentException("Only member expressions are supported", nameof(expression)),
        };

    private static string BuildQueryString<T>(IEnumerable<T> data, string name)
    {
        var first = data.Take(1).Select(x => x.ToString());
        var rest = data.Skip(1).Select(x => $"{name}={x}");
        return string.Join("&", first.Union(rest));
    }
}

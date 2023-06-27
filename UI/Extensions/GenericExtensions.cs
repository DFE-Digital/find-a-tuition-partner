using System.Linq.Expressions;

namespace UI.Extensions
{
    public static class GenericExtensions
    {
        public static void AddAllToDictionary<T1, T2>(
            this T2 model,
            Expression<Func<T2, IEnumerable<T1>?>> expression,
            Dictionary<string, string> dictionary,
            bool flattenCollection)
        {
            if (flattenCollection)
            {
                model.AddToDictionary(expression, BuildQueryString, dictionary);
            }
            else
            {
                var name = expression.MemberName();
                var collection = expression.Compile()(model);

                if (collection is null) return;

                int i = 0;
                foreach (var item in collection)
                {
                    if (item != null)
                    {
                        var itemString = item.ToString();

                        if (!string.IsNullOrWhiteSpace(itemString))
                        {
                            dictionary.Add($"{name}[{i}]", itemString);
                            i++;
                        }
                    }
                }
            }
        }

        public static void AddToDictionary<T1, T2>(
            this T2 model,
            Expression<Func<T2, T1?>> expression,
            Dictionary<string, string> dictionary)
        {
            model.AddToDictionary(expression, (prop, _) => prop?.ToString() ?? "", dictionary);
        }

        public static void AddToDictionary<T1, T2>(
            this T2 model,
            Expression<Func<T2, T1?>> expression,
            Func<T1, string, string> buildQueryText,
            Dictionary<string, string> dictionary)
        {
            var name = expression.MemberName();
            var property = expression.Compile()(model);

            if (property is null) return;

            var qs = buildQueryText(property, name);

            if (!string.IsNullOrEmpty(qs))
            {
                dictionary.Add(name, qs);
            }
        }

        public static string MemberName<T1, T2>(this Expression<Func<T2, T1?>> expression)
            => expression.Body switch
            {
                MemberExpression x when true => x.Member.Name,
                _ => throw new ArgumentException("Only member expressions are supported", nameof(expression)),
            };

        public static string BuildQueryString<T>(IEnumerable<T> data, string name)
        {
            var enumerable = data as T[] ?? data.ToArray();
            var first = enumerable.Take(1).Select(x => x?.ToString());
            var rest = enumerable.Skip(1).Select(x => $"{name}={x}");
            return string.Join("&", first.Union(rest));
        }
    }
}

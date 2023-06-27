using Application.Common.Models;

namespace UI.Extensions
{
    public static class SearchModelExtensions
    {
        public static string ToQueryString(this SearchModel? model)
        {
            var routes = model?.ToRouteData(true) ?? new();
            return string.Join("&", routes.Select(x => $"{x.Key}={x.Value}"));
        }

        public static string ToQueryString(this SearchModel? model, Dictionary<string, string> itemsToInclude)
        {
            var routes = model?.ToRouteData(itemsToInclude, true) ?? new();
            return string.Join("&", routes.Select(x => $"{x.Key}={x.Value}"));
        }

        public static Dictionary<string, string> ToRouteData(this SearchModel model)
        {
            return model.ToRouteData(false);
        }

        public static Dictionary<string, string> ToRouteData(this SearchModel model, Dictionary<string, string> itemsToInclude, bool flattenCollection = false)
        {
            var dictionary = model.ToRouteData(flattenCollection);

            foreach (var itemToInclude in itemsToInclude)
            {
                dictionary.Add(itemToInclude.Key, itemToInclude.Value);
            }

            return dictionary;
        }

        private static Dictionary<string, string> ToRouteData(this SearchModel model, bool flattenCollection)
        {
            var dictionary = new Dictionary<string, string>();

            model.AddToDictionary(x => x.From, dictionary);
            model.AddToDictionary(x => x.Name, dictionary);
            model.AddToDictionary(x => x.Postcode, dictionary);
            model.AddToDictionary(x => x.TuitionSetting, dictionary);
            model.AddAllToDictionary(x => x.KeyStages, dictionary, flattenCollection);
            model.AddAllToDictionary(x => x.Subjects, dictionary, flattenCollection);
            model.AddToDictionary(x => x.CompareListOrderBy, dictionary);
            model.AddToDictionary(x => x.CompareListOrderByDirection, dictionary);
            model.AddToDictionary(x => x.CompareListTuitionSetting, dictionary);
            model.AddToDictionary(x => x.CompareListGroupSize, dictionary);
            model.AddToDictionary(x => x.CompareListShowWithVAT, dictionary);

            return dictionary;
        }
    }
}

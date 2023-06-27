using Application.Common.Models.Enquiry.Manage;

namespace UI.Extensions
{
    public static class EnquirerResponseResultsModelExtensions
    {
        public static string ToQueryString(this EnquirerResponseResultsModel? model)
        {
            var routes = model?.ToRouteData() ?? new();
            return string.Join("&", routes.Select(x => $"{x.Key}={x.Value}"));
        }

        public static string ToQueryString(this EnquirerResponseResultsModel? model, Dictionary<string, string> itemsToInclude)
        {
            var routes = model?.ToRouteData(itemsToInclude) ?? new();
            return string.Join("&", routes.Select(x => $"{x.Key}={x.Value}"));
        }

        public static Dictionary<string, string> ToRouteData(this EnquirerResponseResultsModel model)
        {
            var dictionary = new Dictionary<string, string>();

            model.AddToDictionary(x => x.EnquiryResponseResultsOrderBy, dictionary);
            model.AddToDictionary(x => x.EnquiryResponseResultsDirection, dictionary);

            return dictionary;
        }

        public static Dictionary<string, string> ToRouteData(this EnquirerResponseResultsModel model, Dictionary<string, string> itemsToInclude)
        {
            var dictionary = model.ToRouteData();

            foreach (var itemToInclude in itemsToInclude)
            {
                dictionary.Add(itemToInclude.Key, itemToInclude.Value);
            }

            return dictionary;
        }
    }
}

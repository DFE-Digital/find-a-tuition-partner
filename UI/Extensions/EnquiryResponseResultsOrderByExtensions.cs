using Application.Common.Models.Enquiry.Manage;

namespace UI.Extensions
{
    public static class EnquiryResponseResultsOrderByExtensions
    {
        public static string GetAriaSort(this EnquiryResponseResultsOrderBy matchedOrderBy, EnquirerResponseResultsModel data)
        {
            string result;
            if (data.EnquiryResponseResultsOrderBy != matchedOrderBy)
            {
                result = "none";
            }
            else
            {
                result = data.EnquiryResponseResultsDirection == OrderByDirection.Ascending ? "ascending" : "descending";
            }
            return result;
        }

        public static string GetSortQueryString(this EnquiryResponseResultsOrderBy matchedOrderBy, EnquirerResponseResultsModel data)
        {
            return (data with { EnquiryResponseResultsOrderBy = matchedOrderBy, EnquiryResponseResultsDirection = (data.EnquiryResponseResultsOrderBy == matchedOrderBy && data.EnquiryResponseResultsDirection == OrderByDirection.Ascending) ? OrderByDirection.Descending : OrderByDirection.Ascending }).ToQueryString();
        }
    }
}
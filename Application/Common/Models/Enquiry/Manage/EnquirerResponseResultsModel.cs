using Domain.Enums;

namespace Application.Common.Models.Enquiry.Manage
{
    public record EnquirerResponseResultsModel
    {
        public EnquirerResponseResultsModel()
        {
        }

        public EnquirerResponseResultsModel(EnquirerResponseResultsModel model)
        {
            EnquiryResponseResultsOrderBy = model.EnquiryResponseResultsOrderBy;
            EnquiryResponseResultsDirection = model.EnquiryResponseResultsDirection;
        }

        public EnquiryResponseResultsOrderBy EnquiryResponseResultsOrderBy { get; set; } = EnquiryResponseResultsOrderBy.Date;

        public OrderByDirection EnquiryResponseResultsDirection { get; set; } = OrderByDirection.Descending;
    }
}
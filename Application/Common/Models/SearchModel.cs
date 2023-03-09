using Domain.Enums;
using KeyStage = Domain.Enums.KeyStage;
using TuitionType = Domain.Enums.TuitionType;

namespace Application.Common.Models
{
    public record SearchModel
    {
        public SearchModel()
        {

        }

        public SearchModel(SearchModel model)
        {
            From = model.From;
            Name = model.Name;
            Postcode = model.Postcode;
            Subjects = model.Subjects;
            TuitionType = model.TuitionType;
            KeyStages = model.KeyStages;
            CompareListOrderBy = model.CompareListOrderBy;
            CompareListOrderByDirection = model.CompareListOrderByDirection;
            CompareListTuitionType = model.CompareListTuitionType;
            CompareListGroupSize = model.CompareListGroupSize;
            CompareListShowWithVAT = model.CompareListShowWithVAT;
            SupportReferenceNumber = model.SupportReferenceNumber;
        }

        public ReferrerList? From { get; set; }

        public string? Name { get; set; }

        public string? Postcode { get; set; }

        public string[]? Subjects { get; set; }

        public TuitionType? TuitionType { get; set; }

        public KeyStage[]? KeyStages { get; set; }

        public TuitionPartnerOrderBy? CompareListOrderBy { get; set; }

        public OrderByDirection? CompareListOrderByDirection { get; set; }

        public TuitionType? CompareListTuitionType { get; set; }

        public GroupSize? CompareListGroupSize { get; set; }

        public bool? CompareListShowWithVAT { get; set; }

        public string? SupportReferenceNumber { get; set; }
    }
}
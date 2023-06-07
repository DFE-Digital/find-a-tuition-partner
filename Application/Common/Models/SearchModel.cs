using Domain.Enums;
using KeyStage = Domain.Enums.KeyStage;
using TuitionSetting = Domain.Enums.TuitionSetting;

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
            TuitionSetting = model.TuitionSetting;
            KeyStages = model.KeyStages;
            CompareListOrderBy = model.CompareListOrderBy;
            CompareListOrderByDirection = model.CompareListOrderByDirection;
            CompareListTuitionSetting = model.CompareListTuitionSetting;
            CompareListGroupSize = model.CompareListGroupSize;
            CompareListShowWithVAT = model.CompareListShowWithVAT;
        }

        public ReferrerList? From { get; set; }

        public string? Name { get; set; }

        public string? Postcode { get; set; }

        public string[]? Subjects { get; set; }

        public TuitionSetting? TuitionSetting { get; set; }

        public KeyStage[]? KeyStages { get; set; }

        public TuitionPartnerOrderBy? CompareListOrderBy { get; set; }

        public OrderByDirection? CompareListOrderByDirection { get; set; }

        public TuitionSetting? CompareListTuitionSetting { get; set; }

        public GroupSize? CompareListGroupSize { get; set; }

        public bool? CompareListShowWithVAT { get; set; }
    }
}
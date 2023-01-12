using Domain.Enums;
using UI.Enums;

namespace UI.Models
{
    public record SearchModel
    {
        public SearchModel()
        {

        }

        public SearchModel(SearchModel model) : base()
        {
            From = model.From;
            Name = model.Name;
            Postcode = model.Postcode;
            Subjects = model.Subjects;
            TuitionType = model.TuitionType;
            OrganisationGroupingType = model.OrganisationGroupingType;
            KeyStages = model.KeyStages;
            ShortlistOrderBy = model.ShortlistOrderBy;
            ShortlistOrderByDirection = model.ShortlistOrderByDirection;
        }

        public ReferrerList? From { get; set; }

        public string? Name { get; set; }

        public string? Postcode { get; set; }

        public string[]? Subjects { get; set; }

        public TuitionType? TuitionType { get; set; }

        public OrganisationGroupingType? OrganisationGroupingType { get; set; }

        public KeyStage[]? KeyStages { get; set; }

        public TuitionPartnerOrderBy? ShortlistOrderBy { get; set; }

        public OrderByDirection? ShortlistOrderByDirection { get; set; }
    }
}
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
            ShortlistOrderBy = model.ShortlistOrderBy;
            ShortlistOrderByDirection = model.ShortlistOrderByDirection;
            ShortlistTuitionType = model.ShortlistTuitionType;
            ShortlistGroupSize = model.ShortlistGroupSize;
        }

        public ReferrerList? From { get; set; }

        public string? Name { get; set; }

        public string? Postcode { get; set; }

        public string[]? Subjects { get; set; }

        public TuitionType? TuitionType { get; set; }

        public KeyStage[]? KeyStages { get; set; }

        public TuitionPartnerOrderBy? ShortlistOrderBy { get; set; }

        public OrderByDirection? ShortlistOrderByDirection { get; set; }

        public TuitionType? ShortlistTuitionType { get; set; }

        public GroupSize? ShortlistGroupSize { get; set; }
    }
}
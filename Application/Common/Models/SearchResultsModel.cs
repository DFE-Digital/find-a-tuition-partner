using Domain.Enums;
using Domain.Search;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Application.Common.Models
{
    public record SearchResultsModel : SearchModel
    {
        public SearchResultsModel()
        {
        }

        public SearchResultsModel(SearchModel query) : base(query)
        {
        }

        public Dictionary<KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
        public IEnumerable<TuitionSetting> AllTuitionSettings { get; set; } = new List<TuitionSetting>();

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
        public TuitionSetting? PreviousTuitionSetting { get; set; }
    }
}

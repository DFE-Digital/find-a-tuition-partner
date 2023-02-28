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
        public IEnumerable<TuitionType> AllTuitionTypes { get; set; } = new List<TuitionType>();

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
        public TuitionType? PreviousTuitionType { get; set; }
    }
}

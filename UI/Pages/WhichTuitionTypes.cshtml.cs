using Application.Common.Interfaces;
using Application.Common.Models;
using UI.Pages.Enquiry.Build;
using TuitionType = Domain.Enums.TuitionType;

namespace UI.Pages;

public class WhichTuitionTypes : PageModel
{
    private readonly ISessionService _sessionService;

    public WhichTuitionTypes(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Command Data { get; set; } = new();

    public async Task<IActionResult> OnGet(GetWhichTuitionTypeQuery query)
    {
        if (query.From == ReferrerList.CheckYourAnswers &&
            !await _sessionService.SessionDataExistsAsync()) return RedirectToPage("/Session/Timeout");

        Data = new Command(query)
        {
            AllTuitionTypes = EnumExtensions.GetAllEnums<TuitionType>(),
        };

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(Command data)
    {
        var redirectPage = nameof(SearchResults);

        if (data.From == ReferrerList.CheckYourAnswers)
        {
            if (!await _sessionService.SessionDataExistsAsync())
                return RedirectToPage("/Session/Timeout");

            redirectPage = $"Enquiry/Build/{nameof(CheckYourAnswers)}";
        }

        if (!ModelState.IsValid)
        {
            Data = data with
            {
                AllTuitionTypes = EnumExtensions.GetAllEnums<TuitionType>(),
            };
            return Page();
        }

        return RedirectToPage(redirectPage, new SearchModel(data));
    }

    public record Command : SearchModel, IRequest<SearchModel>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }

        public List<TuitionType> AllTuitionTypes { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.TuitionType)
                .NotEmpty()
                .WithMessage("Select a type of tuition");
        }
    }
}
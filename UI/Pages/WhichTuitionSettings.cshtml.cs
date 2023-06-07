using Application.Common.Interfaces;
using Application.Common.Models;
using UI.Pages.Enquiry.Build;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace UI.Pages;

public class WhichTuitionSettings : PageModel
{
    private readonly ISessionService _sessionService;

    public WhichTuitionSettings(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Command Data { get; set; } = new();

    public async Task<IActionResult> OnGet(GetWhichTuitionSettingQuery query)
    {
        if (query.From == ReferrerList.CheckYourAnswers &&
            !await _sessionService.SessionDataExistsAsync()) return RedirectToPage("/Session/Timeout");

        Data = new Command(query)
        {
            AllTuitionSettings = EnumExtensions.GetAllEnums<TuitionSetting>(),
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
                AllTuitionSettings = EnumExtensions.GetAllEnums<TuitionSetting>(),
            };
            return Page();
        }

        return RedirectToPage(redirectPage, new SearchModel(data));
    }

    public record Command : SearchModel, IRequest<SearchModel>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }

        public List<TuitionSetting> AllTuitionSettings { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.TuitionSetting)
                .NotEmpty()
                .WithMessage("Select a tuition setting");
        }
    }
}
using Application;
using Application.Constants;
using Application.Extensions;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Models;

namespace UI.Pages;

public partial class Index : PageModel
{
    private readonly ILogger<Index> _logger;
    private readonly IMediator _mediator;

    public Index(ILogger<Index> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [BindProperty(SupportsGet = true)]
    public Command Data { get; set; } = new Command();

    public record Command : SearchModel, IRequest<Domain.IResult> { }

    public IActionResult OnGet()
    {
        // The model is being validated on get due to [BindProperty(SupportsGet = true)].
        // This is not appropriate for this page either when first arriving on this page or using a back link.
        // Therefore clear any model state validation errors
        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(Command data)
    {
        if (!ModelState.IsValid) return Page();

        var validation = await _mediator.Send(data);

        if (validation.IsSuccess)
        {
            return RedirectToPage(nameof(WhichKeyStages), data);
        }

        if (validation is ErrorResult error)
            ModelState.AddModelError("Data.Postcode", error.ToString());

        Data = data;
        return Page();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a valid postcode");
        }
    }

    public class Handler : IRequestHandler<Command, Domain.IResult>
    {
        private readonly ILocationFilterService _locationService;

        public Handler(ILocationFilterService location) => _locationService = location;

        public async Task<Domain.IResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var location = await _locationService.GetLocationFilterParametersAsync(request.Postcode!);
            return location.TryValidate();
        }
    }
}
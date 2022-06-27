using Application.Exceptions;
using Application;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Extensions;

namespace UI.Pages.Find;

public partial class Location : PageModel
{
    private readonly IMediator mediator;
    public Location(IMediator mediator) => this.mediator = mediator;

    [BindProperty(SupportsGet = true)]
    public Command Data { get; set; } = new Command();

    public record Command : SearchModel, IRequest<Command> { }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            return RedirectToPage("Subjects", await mediator.Send(Data));
        }
        catch (LocationNotFoundException)
        {
            ModelState.AddModelError("Data.Postcode", "Enter a valid postcode");
        }
        catch (LocationNotAvailableException)
        {
            ModelState.AddModelError("Data.Postcode", "This service covers England only");
        }
        catch (LocationNotMappedException)
        {
            ModelState.AddModelError("Data.Postcode", "Could not identify Local Authority for the supplied postcode");
        }

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
                .Matches(@"[a-zA-Z]{1,2}([0-9]{1,2}|[0-9][a-zA-Z])\s*[0-9][a-zA-Z]{2}")
                .WithMessage("Enter a valid postcode");
        }
    }

    public class Handler : IRequestHandler<Command, Command>
    {
        private readonly ILocationFilterService locationService;

        public Handler(ILocationFilterService location) => locationService = location;

        public async Task<Command> Handle(Command request, CancellationToken cancellationToken)
        {
            var location = await locationService.GetLocationFilterParametersAsync(request.Postcode!);
            location.Validate();
            return request;
        }
    }
}
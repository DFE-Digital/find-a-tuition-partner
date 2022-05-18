using Application;
using Domain.Search;
using FluentValidation;
using Mapster;
using MediatR;
using UI.Models;

namespace UI.Handlers.SearchTutoringPartners;

public class School
{
    public class Query : IRequest<Command>
    {
        public SearchAction SearchAction { get; set; } = SearchAction.Parameters;
        public string Postcode { get; set; } = string.Empty;
    }

    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly IAddressLookup _addressLookup;
        private readonly ILocationFilterService _locationFilterService;

        public QueryHandler(IAddressLookup addressLookup, ILocationFilterService locationFilterService)
        {
            _addressLookup = addressLookup;
            _locationFilterService = locationFilterService;
        }

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<Command>();

            return await HydrateCommandHandler.Hydrate(command, _addressLookup, _locationFilterService);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(_ => _.Postcode)
                .NotEmpty()
                .WithMessage("Enter a postcode")
                .When(_ => _.IsSearchParameters);

            RuleFor(_ => _.SelectedIndex)
                .NotEmpty()
                .WithMessage("Select an address")
                .When(_ => _.IsSearchResults);
        }
    }

    public class Command : IRequest<Command>
    {
        public SearchAction SearchAction { get; set; }
        public bool IsSearchParameters => SearchAction == SearchAction.Parameters;
        public bool IsSearchResults => SearchAction == SearchAction.Results;

        public string Postcode { get; set; } = string.Empty;
        public AddressSearchResultsPage? Addresses { get; set; }
        public int? SelectedIndex { get; set; }
        public bool IsComplete => SelectedIndex.HasValue;
    }

    public class CommandHandler : IRequestHandler<Command, Command>
    {
        public async Task<Command> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.IsSearchParameters)
            {
                request.SearchAction = SearchAction.Results;
            }

            return request;
        }
    }

    public class HydrateCommand : IRequest<Command>
    {
        public Command Command { get; set; }

        public HydrateCommand(Command command)
        {
            Command = command;
        }
    }

    public class HydrateCommandHandler : IRequestHandler<HydrateCommand, Command>
    {
        private readonly IAddressLookup _addressLookup;
        private readonly ILocationFilterService _locationFilterService;

        public HydrateCommandHandler(IAddressLookup addressLookup, ILocationFilterService locationFilterService)
        {
            _addressLookup = addressLookup;
            _locationFilterService = locationFilterService;
        }

        public async Task<Command> Handle(HydrateCommand request, CancellationToken cancellationToken)
        {
            var command = request.Command;

            return await Hydrate(command, _addressLookup, _locationFilterService);
        }

        public static async Task<Command> Hydrate(Command command, IAddressLookup addressLookup, ILocationFilterService locationFilterService)
        {
            if (command.IsSearchResults)
            {
                command.Addresses = await addressLookup.LookupAddressAsync(command.Postcode);
                var parameters = await locationFilterService.GetLocationFilterParametersAsync(command.Postcode);
            }

            return command;
        }
    }
}
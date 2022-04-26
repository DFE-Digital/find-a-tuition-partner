using System.ComponentModel.DataAnnotations;
using Application;
using Domain.Search;
using Mapster;
using MediatR;
using UI.Models;

namespace UI.Handlers.SearchTutoringPartners;

public class School
{
    public class Query : IRequest<Command>
    {
        public SearchAction Action { get; set; } = SearchAction.Parameters;
        public string Postcode { get; set; } = string.Empty;
    }

    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly IAddressLookup _addressLookup;

        public QueryHandler(IAddressLookup addressLookup)
        {
            _addressLookup = addressLookup;
        }

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<Command>();

            return await HydrateCommandHandler.Hydrate(command, _addressLookup);
        }
    }

    public class Command
    {
        public SearchAction Action { get; set; }
        public bool IsSearchParameters => Action == SearchAction.Parameters;
        public bool IsSearchResults => Action == SearchAction.Results;

        [Required(ErrorMessage = "Enter a postcode")]
        public string Postcode { get; set; } = string.Empty;
        
        public AddressSearchResultsPage? Results { get; set; }
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

        public HydrateCommandHandler(IAddressLookup addressLookup)
        {
            _addressLookup = addressLookup;
        }

        public async Task<Command> Handle(HydrateCommand request, CancellationToken cancellationToken)
        {
            var command = request.Command;

            return await Hydrate(command, _addressLookup);
        }

        public static async Task<Command> Hydrate(Command command, IAddressLookup addressLookup)
        {
            if (command.IsSearchResults)
            {
                command.Results = await addressLookup.LookupAddressAsync(command.Postcode);
            }

            return command;
        }
    }
}
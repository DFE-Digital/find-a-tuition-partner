using System.ComponentModel.DataAnnotations;
using Application;
using Domain;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UI.Handlers.SearchTutoringPartners;

public class Subjects
{
    public class Query : IRequest<Command>
    {

    }

    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly INtpDbContext _dbContext;

        public QueryHandler(INtpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<Command>();

            return await HydrateCommandHandler.Hydrate(command, _dbContext);
        }
    }

    public class Command : IRequest<Command>
    {
        [Required(ErrorMessage = "Select the subject or subjects")]
        public ICollection<int>? SubjectIds { get; set; }
        public IEnumerable<Subject>? Subjects { get; set; }
    }

    public class CommandHandler : IRequestHandler<Command, Command>
    {
        public async Task<Command> Handle(Command request, CancellationToken cancellationToken)
        {
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
        private readonly INtpDbContext _dbContext;

        public HydrateCommandHandler(INtpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Command> Handle(HydrateCommand request, CancellationToken cancellationToken)
        {
            var command = request.Command;

            return await Hydrate(command, _dbContext);
        }

        public static async Task<Command> Hydrate(Command command, INtpDbContext dbContext)
        {
            command.Subjects = await dbContext.Subjects.OrderBy(e => e.Id).ToArrayAsync();

            return command;
        }
    }
}
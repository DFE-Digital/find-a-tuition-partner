﻿using Application.Common.Interfaces;
using Application.Common.Models.Admin;

namespace Application.Commands.Admin;

public record ProcessEmailsCommand : IRequest<ProcessedEmailsModel>;

public class ProcessEmailsCommandHandler : IRequestHandler<ProcessEmailsCommand, ProcessedEmailsModel>
{
    private readonly IProcessEmailsService _processEmailsService;

    public ProcessEmailsCommandHandler(IProcessEmailsService processEmailsService)
    {
        _processEmailsService = processEmailsService;
    }

    public async Task<ProcessedEmailsModel> Handle(ProcessEmailsCommand request, CancellationToken cancellationToken)
    {
        return await _processEmailsService.ProcessAllEmailsAsync();
    }
}

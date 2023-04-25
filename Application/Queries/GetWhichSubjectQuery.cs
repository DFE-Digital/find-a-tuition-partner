using Application.Common.Models;
using Application.Extensions;
using Domain.Enums;
using KeyStageSubjectDictionary = System.Collections.Generic.Dictionary<Domain.Enums.KeyStage, Application.Common.Models.Selectable<string>[]>;

namespace Application.Queries;

public record GetWhichSubjectQuery : SearchModel, IRequest<KeyStageSubjectDictionary>
{
    public GetWhichSubjectQuery() { }
    public GetWhichSubjectQuery(SearchModel query) : base(query) { }
}

public class GetWhichSubjectQueryHandler : IRequestHandler<GetWhichSubjectQuery, KeyStageSubjectDictionary>
{
    public Dictionary<KeyStage, string[]> KeyStageSubjects = new()
    {
        { KeyStage.KeyStage1, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName() } },
        { KeyStage.KeyStage2, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName() } },
        { KeyStage.KeyStage3, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName(), Subject.Humanities.DisplayName(), Subject.ModernForeignLanguages.DisplayName() } },
        { KeyStage.KeyStage4, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName(), Subject.Humanities.DisplayName(), Subject.ModernForeignLanguages.DisplayName() } },
    };

    public Task<KeyStageSubjectDictionary> Handle(GetWhichSubjectQuery request, CancellationToken cancellationToken)
    {
        request.KeyStages ??= new[]
        {
            KeyStage.KeyStage1,
            KeyStage.KeyStage2,
            KeyStage.KeyStage3,
            KeyStage.KeyStage4,
        };
        request.Subjects ??= Array.Empty<string>();

        var selectable = KeyStageSubjects
            .Where(x => request.KeyStages.Contains(x.Key))
            .ToDictionary(
                x => x.Key,
                x => x.Value.Select(subject => new Selectable<string>
                {
                    Name = subject,
                    Selected = request.Subjects.ParseKeyStageSubjects().Any(s => s.KeyStage == x.Key && s.Subject == subject),
                }).ToArray()
            );

        return Task.FromResult(selectable);
    }
}
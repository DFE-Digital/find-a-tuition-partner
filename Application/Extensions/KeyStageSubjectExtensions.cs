using Application.Common.Models;

namespace Application.Extensions
{
    public static class KeyStageSubjectExtensions
    {
        public static KeyStageSubject[] ParseKeyStageSubjects(this string[] keyStageSubjects)
            => keyStageSubjects.Select(KeyStageSubject.TryParse)
            .OfType<KeyStageSubject>()
            .ToArray();
    }
}

namespace UI.Extensions
{
    public static class KeyStageSubjectExtensions
    {
        public static KeyStageSubject[] ParseKeyStageSubjects(this string[] keyStageSubjects)
            => keyStageSubjects.Select(KeyStageSubject.TryParse)
            .OfType<KeyStageSubject>()
            .ToArray();
    }
}
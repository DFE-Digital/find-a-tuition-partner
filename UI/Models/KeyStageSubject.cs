using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UI.Models
{
    public record KeyStageSubject(KeyStage KeyStage, string Subject)
    {
        public static KeyStageSubject Parse(string value)
        {
            var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);

            if (!re.Success)
                throw new ArgumentException("Subject must be of the form KS1-English");

            if (!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
                throw new ArgumentException($"'{re.Groups[0].Value}' is not a valid key stage");

            return new KeyStageSubject(ks, re.Groups[2].Value);
        }

        public static KeyStageSubject? TryParse(string value) =>
            TryParse(value, out var parsed) ? parsed : null;

        public static bool TryParse(string value, [MaybeNullWhen(false)] out KeyStageSubject parsed)
        {
            if (value == null)
            {
                parsed = default;
                return false;
            }

            var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);

            if (!re.Success)
            {  // Subject must be of the form KS1-English
                parsed = default;
                return false;
            }

            if (!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
            { // Value is not a valid key stage
                parsed = default;
                return false;
            }

            parsed = new KeyStageSubject(ks, re.Groups[2].Value);
            return true;
        }
    }
}

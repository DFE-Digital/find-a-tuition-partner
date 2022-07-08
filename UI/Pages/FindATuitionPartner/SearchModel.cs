using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UI.Pages.FindATuitionPartner;

public record SearchModel
{
    public SearchModel(SearchModel model) : base()
    {
        Postcode = model.Postcode;
        Subjects = model.Subjects;
        TuitionType = model.TuitionType;
        KeyStages = model.KeyStages;
    }

    public string? Postcode { get; set; }

    public string[]? Subjects { get; set; }
        
    public TuitionType? TuitionType { get; set; }
    
    public KeyStage[]? KeyStages { get; set; }
}

public static class ModelExtensions
{
    public static KeyStageSubject[] ParseKeyStageSubjects(this string[] keyStageSubjects)
        => keyStageSubjects.Select(KeyStageSubject.Parse).ToArray();
}

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

    public static bool TryParse(string value, [MaybeNullWhen(false)] out KeyStageSubject parsed)
    {
        if(value == null)
        {
            parsed = default;
            return false;
        }

        var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);

        if (!re.Success)
        {
            parsed = default;
            return false;
        }

        if (!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
        {
            parsed = default;
            return false;
        }

        parsed = new KeyStageSubject(ks, re.Groups[2].Value);
        return true;
    }
}

public enum KeyStage
{
    Unspecified = 0,

    [Description("Key stage 1")]
    KeyStage1 = 1,
    
    [Description("Key stage 2")]
    KeyStage2,
    
    [Description("Key stage 3")]
    KeyStage3,
    
    [Description("Key stage 4")]
    KeyStage4,
}

public enum Subject
{
    Unspecified = 0,

    English,
    Maths,
    Science,

    Humanities,
    ModernForeignLanguages,
}

public enum TuitionType
{
    [Description("Any")]
    Any = 0,

    [Description("Online")]
    Online = 1,

    [Description("In School")]
    InSchool,
}
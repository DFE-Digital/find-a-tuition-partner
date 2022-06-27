namespace UI.Pages.FindATuitionPartner;

public record SearchModel
{
    public SearchModel(SearchModel model) : base()
    {
        Postcode = model.Postcode;
        Subjects = model.Subjects;
        TuitionType = model.TuitionType;
    }

    public string? Postcode { get; set; }

    public IEnumerable<string> Subjects { get; set; } = new List<string>();
    public TuitionType TuitionType { get; set; }
    public KeyStage[] KeyStages { get; set; } = Array.Empty<KeyStage>();
}

public enum TuitionType
{
    Any = 0,
    InPerson = 1,
    Online = 2,
}

public enum KeyStage
{
    Unspecified = 0,
    KeyStage1 = 1,
    KeyStage2,
    KeyStage3,
    KeyStage4,
}
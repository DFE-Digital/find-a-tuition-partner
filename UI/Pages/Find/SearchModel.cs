namespace UI.Pages.Find;

public record SearchModel
{
    public SearchModel(SearchModel model) : base()
    {
        Postcode = model.Postcode;
        Subjects = model.Subjects;
    }

    public string? Postcode { get; set; }

    public IEnumerable<string> Subjects { get; set; } = new List<string>();
}
namespace UI.Pages.Shared;

public record OptionsSelectModel(
    string Name,
    string DisplayName,
    IEnumerable<(string Name, string Value, string DisplayName, bool Selected)> Items,
    bool? ClosedOnLoad = null)
{
    public string ClosedData => !ClosedOnLoad ?? Items.Any(x => x.Selected) ? "" : @"data-closed-on-load=true";
}

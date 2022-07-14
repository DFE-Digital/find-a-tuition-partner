namespace UI.Pages.Shared;

public record OptionsSelectModel(string Name, string DisplayName, IEnumerable<(string Name, string Value, string DisplayName, bool Selected)> Items)
{
    public string ClosedData => Items.Any(x => x.Selected) ? "" : @"data-closed-on-load=true";
}

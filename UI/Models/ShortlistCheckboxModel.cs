namespace UI.Models;

public class ShortlistCheckboxModel
{
    public string? Id { get; set; }
    public string? SeoUrl { get; set; }
    public string? LabelValue { get; set; }
    public string? CheckboxName { get; set; }
    public bool IsShortlisted { get; set; }

    public ShortlistCheckboxModel()
    {

    }
    public ShortlistCheckboxModel(string id, string seoUrl, string labelValue,
        string checkboxName, bool isShortlisted)
    {
        Id = id;
        SeoUrl = seoUrl;
        LabelValue = labelValue;
        CheckboxName = checkboxName;
        IsShortlisted = isShortlisted;
    }
}
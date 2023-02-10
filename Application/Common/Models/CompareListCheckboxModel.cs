namespace Application.Common.Models;

public class CompareListCheckboxModel
{
    public string? Id { get; set; }
    public string? SeoUrl { get; set; }
    public string? LabelValue { get; set; }
    public string? CheckboxName { get; set; }
    public bool IsCompareListed { get; set; }

    public CompareListCheckboxModel()
    {

    }
    public CompareListCheckboxModel(string id, string seoUrl, string labelValue,
        string checkboxName, bool isCompareListed)
    {
        Id = id;
        SeoUrl = seoUrl;
        LabelValue = labelValue;
        CheckboxName = checkboxName;
        IsCompareListed = isCompareListed;
    }
}
namespace UI.Models;

public class SelectableTuitionPartnerModel
{
    public string SeoUrl { get; set; } = string.Empty;
    public bool IsSelected { get; set; }

    //Without this constructor .net core razor pages engine would scream at you
    public SelectableTuitionPartnerModel() { }

    public SelectableTuitionPartnerModel(string seoUrl, bool isSelected = false)
    {
        SeoUrl = seoUrl;
        IsSelected = isSelected;
    }
}
using UI.Models;

namespace UI.Extensions;

public class SelectableTuitionPartnerModelComparer : IEqualityComparer<SelectableTuitionPartnerModel>
{
    public bool Equals(SelectableTuitionPartnerModel? x, SelectableTuitionPartnerModel? y)
    {
        if (x != null && y != null) return x.SeoUrl == y.SeoUrl;

        return false;
    }

    public int GetHashCode(SelectableTuitionPartnerModel obj)
        => obj.SeoUrl != null ? obj.SeoUrl.GetHashCode() : obj.GetHashCode();
}
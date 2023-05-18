namespace Domain.Search;

public class TuitionPartnersDataFilter
{
    public int? GroupSize { get; set; }
    public int? TuitionSettingId { get; set; }
    public IEnumerable<int>? SubjectIds { get; set; }
    public bool? ShowWithVAT { get; set; }
}
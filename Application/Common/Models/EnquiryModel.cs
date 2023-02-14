namespace Application.Common.Models;

public class EnquiryModel
{
    public int EnquiryId { get; set; }
    public string? EnquiryText { get; set; }
    public string? Email { get; set; }

    public string? BaseServiceUrl { get; set; }

    public List<string>? SelectedTuitionPartners { get; set; }
}
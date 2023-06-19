namespace Application.Common.Models.Enquiry;

public record TutoringLogisticsDetailsModel
{
    public string? NumberOfPupils { get; set; }
    public string? StartDate { get; set; }
    public string? TuitionDuration { get; set; }
    public string? TimeOfDay { get; set; }
}
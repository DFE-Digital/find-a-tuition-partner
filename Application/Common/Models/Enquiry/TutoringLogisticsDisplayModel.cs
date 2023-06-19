namespace Application.Common.Models.Enquiry;

public record TutoringLogisticsDisplayModel
{
    public string TutoringLogistics { get; set; } = string.Empty;
    public TutoringLogisticsDetailsModel? TutoringLogisticsDetailsModel { get; set; }
}
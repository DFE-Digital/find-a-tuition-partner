namespace Application.Common.Models.Enquiry.Build;

public record ConfirmSchoolModel : SchoolPostcodeModel
{
    public int? SelectedSchoolId { get; set; }
}
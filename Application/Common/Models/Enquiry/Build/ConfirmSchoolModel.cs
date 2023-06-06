using Domain;

namespace Application.Common.Models.Enquiry.Build;

public record ConfirmSchoolModel : SchoolPostcodeModel
{
    public List<School> Schools { get; set; } = new List<School>();
    public bool HasSingleSchool { get; set; } = false;
    public bool? ConfirmedIsSchool { get; set; } = null;
}
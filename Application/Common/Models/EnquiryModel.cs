namespace Application.Common.Models;

public class EnquiryModel
{
    public EnquiryModel()
    {
            
    }
    public string? EnquiryText { get; set; }
    public string? Email { get; set; }
    
    public List<string>? SelectedTuitionPartners { get; set; }
}
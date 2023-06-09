using System.ComponentModel;

namespace Domain.Enums
{
    public enum EnquiryResponseStatus
    {
        [Description("Interested")]
        Interested = 1,

        [Description("Undecided")]
        Undecided = 2,

        [Description("Unread")]
        Unread = 3,

        [Description("Not Set")]
        NotSet = 4,

        [Description("Rejected")]
        Rejected = 5
    }
}

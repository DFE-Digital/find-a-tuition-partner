using System.ComponentModel;

namespace Domain.Enums
{
    public enum EnquiryResponseStatus
    {
        [Description("INTERESTED")]
        Interested = 1,

        [Description("UNDECIDED")]
        Undecided = 2,

        [Description("UNREAD")]
        Unread = 3,

        [Description("NOT SET")]
        NotSet = 4,

        [Description("NOT INTERESTED")]
        NotInterested = 5
    }
}

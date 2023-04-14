using System.ComponentModel;

namespace Domain.Enums
{
    public enum EmailStatus
    {
        [Description("to-be-processed")]
        ToBeProcessed = 1,

        [Description("waiting-to-be-triggered")]
        WaitingToBeTriggered = 2,

        [Description("delayed-email")]
        DelayedEmail = 3,

        [Description("been-processed")]
        BeenProcessed = 4,

        [Description("created")]
        NotifyCreated = 5,

        [Description("sending")]
        NotifySending = 6,

        [Description("delivered")]
        NotifyDelivered = 7,

        [Description("permanent-failure")]
        NotifyPermanentFailure = 8,

        [Description("temporary-failure")]
        NotifyTemporaryFailure = 9,

        [Description("technical-failure")]
        NotifyTechnicalFailure = 10,

        [Description("processing-failure")]
        ProcessingFailure = 11,
    }
}

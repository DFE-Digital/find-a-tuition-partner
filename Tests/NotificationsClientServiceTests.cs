using Application.Common.DTO;
using Domain.Enums;
using Infrastructure.Configuration;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace Tests;

using static Logging<NotificationsClientService>;

public class NotificationsClientServiceTests
{
    private NotificationsClientService? _notificationsClientService;
    private readonly Mock<IOptions<GovUkNotifyOptions>> _configMock;
    private readonly Mock<ILogger<NotificationsClientService>> _loggerMock;
    private readonly Mock<IAsyncNotificationClient> _notificationClientMock;

    public NotificationsClientServiceTests()
    {
        _configMock = new Mock<IOptions<GovUkNotifyOptions>>();
        _loggerMock = new Mock<ILogger<NotificationsClientService>>();
        _notificationClientMock = new Mock<IAsyncNotificationClient>();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldCallSendEmailAsync_WhenRecipientsSupplied()
    {
        // Arrange
        var personalisation = new Dictionary<string, dynamic>();
        var notificationsRecipients = new List<NotificationsRecipientDto> { new()
        { Email = "test@example.com",
            Personalisation = personalisation} };
        var emailTemplateType = EmailTemplateType.Enquiry;
        var emailTemplateId = "template-id";

        _configMock.Setup(x => x.Value)
            .Returns(new GovUkNotifyOptions { EnquiryTemplateId = emailTemplateId });
        _notificationClientMock.Setup(x =>
                x.SendEmailAsync(It.IsAny<string>(), emailTemplateId,
                    notificationsRecipients.First().Personalisation, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new EmailNotificationResponse
            { id = "id", reference = "reference", uri = "uri", content = new EmailResponseContent() });

        _notificationsClientService =
            new NotificationsClientService(_configMock.Object, _loggerMock.Object, _notificationClientMock.Object);

        // Act
        await _notificationsClientService.SendEmailAsync(notificationsRecipients, emailTemplateType);

        // Assert
        _notificationClientMock.Verify(x =>
                x.SendEmailAsync(It.IsAny<string>(), emailTemplateId,
                    notificationsRecipients.First().Personalisation, It.IsAny<string>(), It.IsAny<string>()),
            Times.Exactly(1));
    }

    [Fact]
    public async Task SendEmailAsync_ShouldLogError_WhenNoRecipientsSupplied()
    {
        // Arrange
        var personalisation = new Dictionary<string, dynamic>();
        var notificationsRecipients = new List<NotificationsRecipientDto> { new()
        { Email = "",
            Personalisation = personalisation} };
        var emailTemplateType = EmailTemplateType.Enquiry;

        _configMock.Setup(x => x.Value)
            .Returns(new GovUkNotifyOptions { });


        _notificationsClientService =
          new NotificationsClientService(_configMock.Object, _loggerMock.Object, _notificationClientMock.Object);

        // Act
        await _notificationsClientService.SendEmailAsync(notificationsRecipients, emailTemplateType);

        // Assert
        VerifyLogging(_loggerMock, LogLevel.Error, $"No email address was supplied for the recipient: {notificationsRecipients.First()}.",
            Times.Once());
    }

    [Fact]
    public async Task Test_SendEmailAsync_Should_Send_Emails()
    {
        // Arrange
        var mockNotificationClient = new Mock<IAsyncNotificationClient>();
        var mockOptions = new Mock<IOptions<GovUkNotifyOptions>>();
        var personalisation = new Dictionary<string, dynamic> { { "key1", "value1" } };
        var notificationsRecipients = new List<NotificationsRecipientDto>
        {
            new() { Email = "test@example.com", Personalisation = personalisation },
            new() { Email = "test2@example.com", Personalisation = personalisation }
        };
        var emailTemplateType = EmailTemplateType.Enquiry;

        var enquiryTemplateId = "enquiry-template-id";

        var govUkNotifyOptions = new GovUkNotifyOptions
        {
            EnquiryTemplateId = enquiryTemplateId
        };
        var emailResponse = new EmailNotificationResponse
        {
            id = Guid.NewGuid().ToString(),
            reference = "reference",
            uri = "uri",
            content = new EmailResponseContent()
        };

        mockOptions.Setup(o => o.Value).Returns(govUkNotifyOptions);
        mockNotificationClient.Setup(nc =>
                nc.SendEmailAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Dictionary<string,
                        dynamic>>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(emailResponse);

        _notificationsClientService = new NotificationsClientService(mockOptions.Object,
            _loggerMock.Object, mockNotificationClient.Object);

        // Act
        await _notificationsClientService.SendEmailAsync(notificationsRecipients, emailTemplateType);

        // Assert
        mockNotificationClient.Verify(nc =>
            nc.SendEmailAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        foreach (var recipient in notificationsRecipients)
        {
            VerifyLogging(_loggerMock, LogLevel.Information, $"Preparing to send to {recipient.Email}", Times.Exactly(1));

            VerifyLogging(_loggerMock, LogLevel.Information, $"Email successfully sent to: {recipient.Email}", Times.Exactly(1));
        }

        VerifyLogging(_loggerMock, LogLevel.Information, $"Result: {emailResponse.id} {emailResponse.reference} {emailResponse.uri}",
            Times.Exactly(2));

        VerifyLogging(_loggerMock, LogLevel.Information, $"Result: {emailResponse.content}",
            Times.Exactly(2));
    }
}

using Application.Common.DTO;
using Domain.Enums;
using Infrastructure.Configuration;
using Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace Tests;

using static Logging<NotificationsClientService>;

public class NotificationsClientServiceTests
{
    private NotificationsClientService? _notificationsClientService;
    private readonly Mock<IOptions<GovUkNotifyOptions>> _notifyConfigMock;
    private readonly Mock<IOptions<EmailSettings>> _emailSettingsConfigMock;
    private readonly Mock<ILogger<NotificationsClientService>> _loggerMock;
    private readonly Mock<IAsyncNotificationClient> _notificationClientMock;
    private readonly Mock<IHostEnvironment> _hostEnvironment;

    public NotificationsClientServiceTests()
    {
        _notifyConfigMock = new Mock<IOptions<GovUkNotifyOptions>>();
        _emailSettingsConfigMock = new Mock<IOptions<EmailSettings>>();
        _loggerMock = new Mock<ILogger<NotificationsClientService>>();
        _notificationClientMock = new Mock<IAsyncNotificationClient>();
        _hostEnvironment = new Mock<IHostEnvironment>();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldCallSendEmailAsync_WhenRecipientsSupplied()
    {
        // Arrange
        var personalisation = new Dictionary<string, dynamic>();
        var notifyEmails = new List<NotifyEmailDto> { new()
        { Email = "test@example.com",
            Personalisation = personalisation,
            EmailTemplateType = EmailTemplateType.EnquirySubmittedConfirmationToEnquirer} };

        var emailTemplateId = "template-id";

        _notifyConfigMock.Setup(x => x.Value)
            .Returns(new GovUkNotifyOptions { TemplateIdEnquirySubmittedConfirmationToEnquirer = emailTemplateId });

        _emailSettingsConfigMock.Setup(x => x.Value)
            .Returns(new EmailSettings());

        _notificationClientMock.Setup(x =>
                x.SendEmailAsync(It.IsAny<string>(), emailTemplateId,
                    notifyEmails.First().Personalisation, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new EmailNotificationResponse
            { id = "id", reference = "reference", uri = "uri", content = new EmailResponseContent(), template = new Notify.Models.Template() });

        _notificationsClientService =
            new NotificationsClientService(_notifyConfigMock.Object, _emailSettingsConfigMock.Object,
                _loggerMock.Object, _notificationClientMock.Object, _hostEnvironment.Object);

        // Act
        await _notificationsClientService.SendEmailAsync(notifyEmails);

        // Assert
        _notificationClientMock.Verify(x =>
                x.SendEmailAsync(It.IsAny<string>(), emailTemplateId,
                    notifyEmails.First().Personalisation, It.IsAny<string>(), It.IsAny<string>()),
            Times.Exactly(1));
    }

    [Fact]
    public async Task Test_SendEmailAsync_Should_Send_Emails()
    {
        // Arrange
        var mockNotificationClient = new Mock<IAsyncNotificationClient>();
        var mockOptions = new Mock<IOptions<GovUkNotifyOptions>>();
        var personalisation = new Dictionary<string, dynamic> { { "key1", "value1" } };
        var emailTemplateType = EmailTemplateType.EnquirySubmittedConfirmationToEnquirer;
        var notifyEmails = new List<NotifyEmailDto>
        {
            new() { Email = "test@example.com", Personalisation = personalisation, ClientReference = "1example-ref", EmailTemplateType = emailTemplateType },
            new() { Email = "test2@example.com", Personalisation = personalisation, ClientReference = "example-ref-2", EmailTemplateType = emailTemplateType }
        };

        var enquiryTemplateId = "enquiry-template-id";

        var govUkNotifyOptions = new GovUkNotifyOptions
        {
            TemplateIdEnquirySubmittedConfirmationToEnquirer = enquiryTemplateId
        };
        var emailResponse = new EmailNotificationResponse
        {
            id = Guid.NewGuid().ToString(),
            reference = "reference",
            uri = "uri",
            content = new EmailResponseContent(),
            template = new Notify.Models.Template()
        };

        mockOptions.Setup(o => o.Value).Returns(govUkNotifyOptions);

        _emailSettingsConfigMock.Setup(x => x.Value)
            .Returns(new EmailSettings());

        mockNotificationClient.Setup(nc =>
                nc.SendEmailAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Dictionary<string,
                        dynamic>>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(emailResponse);

        _notificationsClientService = new NotificationsClientService(mockOptions.Object, _emailSettingsConfigMock.Object,
            _loggerMock.Object, mockNotificationClient.Object, _hostEnvironment.Object);

        // Act
        await _notificationsClientService.SendEmailAsync(notifyEmails);

        // Assert
        mockNotificationClient.Verify(nc =>
            nc.SendEmailAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        foreach (var recipient in notifyEmails)
        {
            VerifyLogging(_loggerMock, LogLevel.Information, $"Preparing to send, Notify client ref: {recipient.ClientReference}", Times.Exactly(1));

            VerifyLogging(_loggerMock, LogLevel.Information, $"Email successfully sent, Notify client ref: {recipient.ClientReference}.  Result details: Id: {emailResponse.id}; Ref: {emailResponse.reference}; URI: {emailResponse.uri}; Content: {emailResponse.content}", Times.Exactly(1));
        }
    }
}

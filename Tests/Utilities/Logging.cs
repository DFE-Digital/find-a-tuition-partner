namespace Tests.Utilities;

public abstract class Logging<T>
{
    public static void VerifyLogging(Mock<ILogger<T>> loggerMock, LogLevel inputLogLevel, string message, Times numberOfTimes)
    {
        loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == inputLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString()!.StartsWith(message) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            numberOfTimes);
    }
}
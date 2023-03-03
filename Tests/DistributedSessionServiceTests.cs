using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UI.Services;

namespace Tests;

public class DistributedSessionServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly DistributedSessionService _sessionService;
    private readonly Mock<ISession> _sessionMock;

    public DistributedSessionServiceTests()
    {
        _sessionMock = new Mock<ISession>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _sessionService = new DistributedSessionService(_mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task AddOrUpdateDataAsync_Should_Update_Data_If_Already_Exists()
    {
        // Arrange
        var sessionIdKey = "test-session-id";
        var existingData = new Dictionary<string, string> { { "foo", "bar" } };
        var updatedData = new Dictionary<string, string> { { "foo", "baz" }, { "qux", "quux" } };
        var storedValue = JsonConvert.SerializeObject(existingData);

        var testSessionData = Encoding.UTF8.GetBytes(storedValue);
        _sessionMock.SetupGet(x => x.Id).Returns(sessionIdKey);
        _sessionMock.SetupGet(x => x.IsAvailable).Returns(true);
        _sessionMock
            .Setup(x => x.LoadAsync(default))
            .Returns(Task.CompletedTask);
        _sessionMock
            .Setup(x => x.TryGetValue(sessionIdKey, out testSessionData))
            .Returns(true);
        _sessionMock
            .Setup(x => x.Set(sessionIdKey, It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) =>
            {
                storedValue = Encoding.UTF8.GetString(value);
            });
        _sessionMock
            .Setup(x => x.CommitAsync(default))
            .Returns(Task.CompletedTask);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext
            .Setup(x => x.Session)
            .Returns(_sessionMock.Object);

        _mockHttpContextAccessor
            .Setup(x => x.HttpContext)
            .Returns(mockHttpContext.Object);

        var sessionService = new DistributedSessionService(_mockHttpContextAccessor.Object);

        // Act
        await sessionService.AddOrUpdateDataAsync(updatedData);

        // Assert
        var expectedStoredValue = JsonConvert.SerializeObject(updatedData);
        Assert.Equal(expectedStoredValue, storedValue);
    }


    [Fact]
    public async Task RetrieveDataAsync_ShouldReturnDeserializedData_WhenDataExists()
    {
        // Arrange
        var sessionIdKey = "testKey";
        var testData = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
        var testDataString = JsonConvert.SerializeObject(testData);
        var testSessionData = Encoding.UTF8.GetBytes(testDataString);
        _sessionMock.SetupGet(x => x.Id).Returns(sessionIdKey);
        _sessionMock.SetupGet(x => x.IsAvailable).Returns(true);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext!.Session).Returns(_sessionMock.Object);
        _sessionMock.Setup(x => x.LoadAsync(default)).Returns(Task.CompletedTask);
        _sessionMock.Setup(x => x.TryGetValue(sessionIdKey, out testSessionData)).Returns(true);

        // Act
        var result = await _sessionService.RetrieveDataAsync();

        // Assert
        result.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task DeleteDataAsync_Should_Remove_Data_If_StoredValue_Exists()
    {
        // Arrange
        var sessionIdKey = "test-session-id";
        var storedValue = JsonConvert.SerializeObject(new Dictionary<string, string>());
        var testSessionData = Encoding.UTF8.GetBytes(storedValue);
        _sessionMock.SetupGet(x => x.Id).Returns(sessionIdKey);
        _sessionMock.SetupGet(x => x.IsAvailable).Returns(true);
        _sessionMock
            .Setup(x => x.LoadAsync(default))
            .Returns(Task.CompletedTask);

        _sessionMock.Setup(x => x.TryGetValue(sessionIdKey, out testSessionData)).Returns(true);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext
            .SetupGet(x => x.Session)
            .Returns(_sessionMock.Object);

        _mockHttpContextAccessor
            .SetupGet(x => x.HttpContext)
            .Returns(mockHttpContext.Object);

        var sessionService = new DistributedSessionService(_mockHttpContextAccessor.Object);

        // Act
        await sessionService.DeleteDataAsync();

        // Assert
        mockHttpContext.Verify(x => x.Session.Remove(sessionIdKey), Times.Once);
    }

}

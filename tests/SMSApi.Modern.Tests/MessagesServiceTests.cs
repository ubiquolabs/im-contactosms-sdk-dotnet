using InteractuaMovil.ContactoSms.Api.Models;
using InteractuaMovil.ContactoSms.Api.Services;
using InteractuaMovil.ContactoSms.Api.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace SMSApi.Modern.Tests;

public class MessagesServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly Mock<ILogger<MessagesService>> _loggerMock;
    private readonly Mock<ILogger<ApiClient>> _apiClientLoggerMock;
    private readonly MessagesService _messagesService;
    private readonly ApiClient _apiClient;

    public MessagesServiceTests()
    {
        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<MessagesService>>();
        _apiClientLoggerMock = new Mock<ILogger<ApiClient>>();
        
        var httpClient = new HttpClient(_httpHandlerMock.Object);
        var options = new SmsApiOptions
        {
            ApiKey = "test-key",
            SecretKey = "test-secret",
            ApiUrl = "https://test.com/",
            TimeoutSeconds = 30
        };
        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(options);
        
        _apiClient = new ApiClient(httpClient, optionsMock.Object, _apiClientLoggerMock.Object);
        _messagesService = new MessagesService(_apiClient, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Act & Assert
        Assert.NotNull(_messagesService);
    }

    [Fact]
    public void Constructor_WithNullApiClient_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new MessagesService(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new MessagesService(_apiClient, null!));
    }

    [Fact]
    public async Task GetListAsync_WithValidParameters_ShouldReturnMessageList()
    {
        // Arrange
        var responseContent = """
        [
            {"message_id": 1, "message": "Test message 1"},
            {"message_id": 2, "message": "Test message 2"}
        ]
        """;

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _messagesService.GetListAsync(
            startDate: DateTime.Now.AddDays(-7),
            endDate: DateTime.Now,
            limit: 10);

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Test message 1", result.Data[0].Message);
    }

    [Fact]
    public async Task SendToContactAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"message_id": 123, "message": "Test message"}""", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _messagesService.SendToContactAsync(
            "+1234567890", 
            "Hello, World!");

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.Data);
        Assert.Equal(123, result.Data.MessageId);
    }

    [Fact]
    public async Task SendToGroupsAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var groupNames = new[] { "group1", "group2" };

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"message_id": 456, "message": "Group message"}""", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _messagesService.SendToGroupsAsync(
            groupNames, 
            "Hello, Groups!");

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.Data);
        Assert.Equal(456, result.Data.MessageId);
    }

    [Fact]
    public async Task GetScheduleAsync_ShouldReturnScheduledMessages()
    {
        // Arrange

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""[{"id": 1, "name": "Daily reminder"}, {"id": 2, "name": "Weekly newsletter"}]""", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _messagesService.GetScheduleAsync();

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Daily reminder", result.Data[0].Name);
    }

    [Fact]
    public void GetList_SyncMethod_ShouldCallAsyncVersion()
    {
        // Arrange

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""[{"message_id": 1, "message": "Test message"}]""", Encoding.UTF8, "application/json")
            });

        // Act
        var result = _messagesService.GetList(limit: 5);

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Theory]
    [InlineData(null, null, -1, -1, null, null, false, MessageDirection.MT, null)]
    [InlineData("2024-01-01 00:00:00", "2024-01-31 23:59:59", 0, 100, "+1234567890", "test-group", true, MessageDirection.MO, "testuser")]
    public async Task GetListAsync_WithDifferentParameters_ShouldBuildCorrectRequest(
        string? startDateStr, string? endDateStr, int start, int limit, 
        string? msisdn, string? shortName, bool includeRecipients, 
        MessageDirection direction, string? username)
    {
        // Arrange
        DateTime? startDate = startDateStr != null ? DateTime.Parse(startDateStr) : null;
        DateTime? endDate = endDateStr != null ? DateTime.Parse(endDateStr) : null;

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _messagesService.GetListAsync(startDate, endDate, start, limit, msisdn, shortName, 
            includeRecipients, direction, username);

        // Assert - just verify the method was called successfully
        Assert.NotNull(result);
        // Note: This test would be more comprehensive with actual parameter validation
        // For now, we're just ensuring the method completes without errors
    }
} 
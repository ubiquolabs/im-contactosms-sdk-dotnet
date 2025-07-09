using System.Net;
using System.Net.Http;
using System.Text;
using InteractuaMovil.ContactoSms.Api.Configuration;
using InteractuaMovil.ContactoSms.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace SMSApi.Modern.Tests;

public class ApiClientTests
{
    private readonly Mock<ILogger<ApiClient>> _loggerMock;
    private readonly SmsApiOptions _options;
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly HttpClient _httpClient;

    public ApiClientTests()
    {
        _loggerMock = new Mock<ILogger<ApiClient>>();
        _options = new SmsApiOptions
        {
            ApiKey = "test-api-key",
            SecretKey = "test-secret-key",
            ApiUrl = "https://api.test.com/",
            TimeoutSeconds = 30,
            EnableLogging = true
        };

        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);

        // Act
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(apiClient);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new ApiClient(null!, optionsMock.Object, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new ApiClient(_httpClient, null!, _loggerMock.Object));
    }

    [Fact]
    public async Task RequestAsync_WithSuccessfulResponse_ShouldReturnSuccessApiResponse()
    {
        // Arrange
        var responseContent = """{"result": "success"}""";
        var expectedResponse = new { result = "success" };

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await apiClient.RequestAsync<dynamic>("test-endpoint", HttpMethod.Get);

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(HttpStatusCode.OK, result.HttpCode);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task RequestAsync_WithErrorResponse_ShouldReturnErrorApiResponse()
    {
        // Arrange
        var errorContent = """{"code": 400, "error": "Bad Request"}""";

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(errorContent, Encoding.UTF8, "application/json")
            });

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await apiClient.RequestAsync<dynamic>("test-endpoint", HttpMethod.Get);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpCode);
        Assert.Equal(400, result.ErrorCode);
        Assert.Equal("Bad Request", result.ErrorDescription);
    }

    [Fact]
    public async Task RequestAsync_WithHttpRequestException_ShouldReturnErrorApiResponse()
    {
        // Arrange
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await apiClient.RequestAsync<dynamic>("test-endpoint", HttpMethod.Get);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal(-1, result.ErrorCode);
        Assert.Contains("Network error", result.ErrorDescription);
    }

    [Fact]
    public async Task RequestAsync_ShouldAddCorrectHeaders()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        await apiClient.RequestAsync<dynamic>("test-endpoint", HttpMethod.Get);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.StartsWith("IM test-api-key:", capturedRequest.Headers.Authorization.ToString());
        Assert.NotNull(capturedRequest.Headers.Date);
        Assert.Contains("X-IM-ORIGIN", capturedRequest.Headers.Select(h => h.Key));
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    public async Task RequestAsync_WithDifferentHttpMethods_ShouldWork(string methodName)
    {
        // Arrange
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        var method = new HttpMethod(methodName);
        var result = await apiClient.RequestAsync<dynamic>("test-endpoint", method);

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public void Request_SyncMethod_ShouldCallAsyncAndReturnResult()
    {
        // Arrange
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });

        var optionsMock = new Mock<IOptions<SmsApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        var apiClient = new ApiClient(_httpClient, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = apiClient.Request<dynamic>("test-endpoint", HttpMethod.Get);

        // Assert
        Assert.True(result.IsOk);
    }
} 
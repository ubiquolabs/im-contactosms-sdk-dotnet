using InteractuaMovil.ContactoSms.Api.Configuration;
using InteractuaMovil.ContactoSms.Api.Extensions;
using Microsoft.Extensions.Options;
using Xunit;

namespace SMSApi.Modern.Tests;

public class ConfigurationTests
{
    [Fact]
    public void SmsApiOptions_WithValidConfiguration_ShouldBeValid()
    {
        // Arrange
        var options = new SmsApiOptions
        {
            ApiKey = "test-api-key",
            SecretKey = "test-secret-key",
            ApiUrl = "https://api.example.com/",
            TimeoutSeconds = 30
        };

        // Act & Assert
        Assert.NotEmpty(options.ApiKey);
        Assert.NotEmpty(options.SecretKey);
        Assert.NotEmpty(options.ApiUrl);
        Assert.True(options.TimeoutSeconds > 0);
    }

    [Fact]
    public void SmsApiOptionsValidator_WithValidOptions_ShouldReturnSuccess()
    {
        // Arrange
        var validator = new SmsApiOptionsValidator();
        var options = new SmsApiOptions
        {
            ApiKey = "test-api-key",
            SecretKey = "test-secret-key",
            ApiUrl = "https://api.example.com/",
            TimeoutSeconds = 30
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Theory]
    [InlineData("", "secret", "https://api.com/", 30)]
    [InlineData("key", "", "https://api.com/", 30)]
    [InlineData("key", "secret", "", 30)]
    [InlineData("key", "secret", "invalid-url", 30)]
    [InlineData("key", "secret", "https://api.com/", 0)]
    [InlineData("key", "secret", "https://api.com/", -1)]
    public void SmsApiOptionsValidator_WithInvalidOptions_ShouldReturnFailure(
        string apiKey, string secretKey, string apiUrl, int timeoutSeconds)
    {
        // Arrange
        var validator = new SmsApiOptionsValidator();
        var options = new SmsApiOptions
        {
            ApiKey = apiKey,
            SecretKey = secretKey,
            ApiUrl = apiUrl,
            TimeoutSeconds = timeoutSeconds
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.NotEmpty(result.Failures);
    }

    [Fact]
    public void ProxyOptions_WithValidConfiguration_ShouldBeValid()
    {
        // Arrange
        var proxyOptions = new ProxyOptions
        {
            Address = "http://proxy.example.com:8080",
            Username = "user",
            Password = "pass"
        };

        // Act & Assert
        Assert.NotEmpty(proxyOptions.Address);
        Assert.NotEmpty(proxyOptions.Username!);
        Assert.NotEmpty(proxyOptions.Password!);
    }
} 
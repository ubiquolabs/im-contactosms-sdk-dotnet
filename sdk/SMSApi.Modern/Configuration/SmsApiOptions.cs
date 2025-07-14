using System.ComponentModel.DataAnnotations;

namespace InteractuaMovil.ContactoSms.Api.Configuration;

/// <summary>
/// Configuration options for the SMS API client
/// </summary>
public class SmsApiOptions
{
    public const string SectionName = "SmsApi";

    /// <summary>
    /// API Key for authentication
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Secret Key for HMAC signature generation
    /// </summary>
    [Required]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for the API endpoints
    /// </summary>
    [Required]
    [Url]
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Proxy configuration (optional)
    /// </summary>
    public ProxyOptions? Proxy { get; set; }

    /// <summary>
    /// HTTP client timeout in seconds (increased for message queries)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Specific timeout for message list queries (can be longer)
    /// </summary>
    public int MessageQueryTimeoutSeconds { get; set; } = 90;

    /// <summary>
    /// Enable request/response logging for debugging
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Maximum retry attempts for failed requests
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;
}

/// <summary>
/// Proxy configuration options
/// </summary>
public class ProxyOptions
{
    /// <summary>
    /// Proxy server address
    /// </summary>
    [Required]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Proxy username (optional)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Proxy password (optional)
    /// </summary>
    public string? Password { get; set; }
} 
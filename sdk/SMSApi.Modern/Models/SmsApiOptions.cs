namespace InteractuaMovil.ContactoSms.Api.Models;

/// <summary>
/// Configuration options for the SMS API
/// </summary>
public class SmsApiOptions
{
    /// <summary>
    /// API key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Secret key for HMAC authentication
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Base API URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Request timeout in seconds (increased for message queries)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60; // Increased from 30 to 60 seconds

    /// <summary>
    /// Specific timeout for message list queries (can be longer)
    /// </summary>
    public int MessageQueryTimeoutSeconds { get; set; } = 90; // 90 seconds for heavy queries

    /// <summary>
    /// Enable detailed logging
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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmsApiOptions = InteractuaMovil.ContactoSms.Api.Configuration.SmsApiOptions;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Modern HTTP client for SMS API communication
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly SmsApiOptions _options;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient, IOptions<SmsApiOptions> options, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure JSON serialization options with UTF-8 support (like Python's ensure_ascii=False)
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Preserve UTF-8 characters
        };

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_options.ApiUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("X-IM-ORIGIN", "IM_SDK_DOTNET_MODERN");
    }

    /// <summary>
    /// Make an asynchronous API request
    /// </summary>
    public async Task<ApiResponse<T>> RequestAsync<T>(string endpoint, HttpMethod method, 
        Dictionary<string, string>? urlParams = null, object? bodyData = null, 
        bool addParamsToQueryString = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await CreateRequestAsync(endpoint, method, urlParams, bodyData, addParamsToQueryString);
            
            if (_options.EnableLogging)
            {
                _logger.LogDebug("Making {Method} request to {Endpoint}", method, endpoint);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Endpoint}", endpoint);
            return ApiResponse<T>.Error(-1, ex.Message, HttpStatusCode.ServiceUnavailable);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Request timeout for {Endpoint}", endpoint);
            return ApiResponse<T>.Error(-1, "Request timeout", HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error for {Endpoint}", endpoint);
            return ApiResponse<T>.Error(-1, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Make a synchronous API request (wrapper for backward compatibility)
    /// </summary>
    public ApiResponse<T> Request<T>(string endpoint, HttpMethod method, 
        Dictionary<string, string>? urlParams = null, object? bodyData = null, 
        bool addParamsToQueryString = false)
    {
        return RequestAsync<T>(endpoint, method, urlParams, bodyData, addParamsToQueryString)
            .GetAwaiter().GetResult();
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(string endpoint, HttpMethod method,
        Dictionary<string, string>? urlParams, object? bodyData, bool addParamsToQueryString)
    {
        var request = new HttpRequestMessage(method, endpoint);
        
        // Serialize body data
        string bodyJson = string.Empty;
        if (bodyData != null)
        {
            bodyJson = JsonSerializer.Serialize(bodyData, _jsonOptions);
            request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        }

        // Build query string
        string queryString = string.Empty;
        if (urlParams?.Count > 0)
        {
            queryString = BuildQueryString(urlParams);
            if (addParamsToQueryString)
            {
                var baseUri = _httpClient.BaseAddress!;
                var fullPath = baseUri.AbsolutePath.TrimEnd('/') + "/" + endpoint.TrimStart('/');
                
                var uriBuilder = new UriBuilder(baseUri)
                {
                    Path = fullPath,
                    Query = queryString
                };
                request.RequestUri = uriBuilder.Uri;
            }
        }

        // Add authentication headers
        await AddAuthenticationHeadersAsync(request, method, queryString, bodyJson);

        return request;
    }

    private Task AddAuthenticationHeadersAsync(HttpRequestMessage request, HttpMethod method, string queryString, string bodyData)
    {
        var date = DateTime.UtcNow.ToString("r");
        
        // Use SAME canonical string structure as Java (always works)
        // Java: apiKey + httpDate + filters + jsonText
        var canonical = $"{_options.ApiKey}{date}{queryString}{bodyData}";

        // Generate HMAC signature
        var signature = GenerateHmacSignature(canonical);
        var authHeader = $"IM {_options.ApiKey}:{signature}";

        // Add headers
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        request.Headers.Date = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
        
        // Add missing X-IM-ORIGIN header that JavaScript sends
        request.Headers.Add("X-IM-ORIGIN", "IM_SDK_DOTNET_MODERN");
        
        if (_options.EnableLogging)
        {
            _logger.LogDebug("Added authentication: {AuthHeader} | Canonical: {Canonical}", authHeader, canonical);
        }

        return Task.CompletedTask;
    }

    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = new ApiResponse<T>
        {
            Response = content,
            HttpCode = response.StatusCode,
            HttpDescription = response.ReasonPhrase ?? string.Empty
        };

        if (response.IsSuccessStatusCode)
        {
            HandleSuccessResponse(content, ref apiResponse);
        }
        else
        {
            // Handle error responses
            apiResponse.ErrorCode = (int)response.StatusCode;
            apiResponse.ErrorDescription = response.ReasonPhrase ?? "Unknown error";

            // Try to parse error details
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
                    if (errorResponse != null)
                    {
                        apiResponse.ErrorCode = errorResponse.Code;
                        apiResponse.ErrorDescription = errorResponse.Error;
                    }
                }
            }
            catch (JsonException)
            {
                // Use default error handling if JSON parsing fails
                _logger.LogWarning("Could not parse error response JSON: {Content}", content);
            }

            _logger.LogWarning("API error: {StatusCode} - {Error}", response.StatusCode, apiResponse.ErrorDescription);
            
            // DEBUG: Log full response details for 500 errors
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Console.WriteLine("=== 500 ERROR DETAILS ===");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Reason: {response.ReasonPhrase}");
                Console.WriteLine($"Content: {content}");
                Console.WriteLine($"Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join("; ", h.Value)}"))}");
                Console.WriteLine("========================");
            }
        }

        return apiResponse;
    }

    private void HandleSuccessResponse<T>(string content, ref ApiResponse<T> apiResponse)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.ValueKind != JsonValueKind.Null && dataElement.ValueKind != JsonValueKind.Undefined)
                {
                    apiResponse.Data = JsonSerializer.Deserialize<T>(dataElement.GetRawText(), _jsonOptions);

                    if (apiResponse.Data is List<ShortlinkResponse> shortlinkList)
                    {
                        foreach (var shortlink in shortlinkList)
                        {
                            if (string.IsNullOrWhiteSpace(shortlink.UrlId))
                            {
                                shortlink.UrlId = shortlink.Id;
                            }
                        }
                    }
                    else if (apiResponse.Data is ShortlinkResponse singleShortlink && string.IsNullOrWhiteSpace(singleShortlink.UrlId))
                    {
                        singleShortlink.UrlId = singleShortlink.Id;
                    }
                }

                if (root.TryGetProperty("success", out var successElement) && successElement.ValueKind == JsonValueKind.False && apiResponse.ErrorCode == 0)
                {
                    apiResponse.ErrorCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                apiResponse.Data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }

            if (_options.EnableLogging)
            {
                _logger.LogDebug("Successful response: {StatusCode}", apiResponse.HttpCode);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response: {Content}", content);
            apiResponse.ErrorCode = (int)HttpStatusCode.InternalServerError;
            apiResponse.ErrorDescription = "Failed to parse response JSON";
        }
    }

    private string GenerateHmacSignature(string data)
    {
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_options.SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    private static string BuildQueryString(Dictionary<string, string>? parameters)
    {
        if (parameters == null || parameters.Count == 0) return string.Empty;

        // Sort parameters alphabetically like Java SDK
        var pairs = parameters
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{UrlEncodeCustomStyle(kvp.Key)}={UrlEncodeCustomStyle(kvp.Value)}");
        
        return string.Join("&", pairs);
    }

    /// <summary>
    /// Custom URL encoding to match JavaScript's implementation (spaces as '+')
    /// </summary>
    private static string UrlEncodeCustomStyle(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        // Use .NET's standard URL encoding, which converts spaces to %20 and colons to %3A
        var encoded = Uri.EscapeDataString(value);
        
        // Replace %20 with '+' to match JavaScript's final encoding style for this API
        return encoded.Replace("%20", "+");
    }
} 
using System.Net;
using System.Text.Json.Serialization;

namespace InteractuaMovil.ContactoSms.Api.Models;

/// <summary>
/// Generic API response wrapper
/// </summary>
/// <typeparam name="T">The type of data returned in the response</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Raw response content from the API
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// Deserialized response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public HttpStatusCode HttpCode { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// HTTP status description
    /// </summary>
    public string HttpDescription { get; set; } = string.Empty;

    /// <summary>
    /// Error code (0 for success)
    /// </summary>
    public int ErrorCode { get; set; } = 0;

    /// <summary>
    /// Error description (empty for success)
    /// </summary>
    public string ErrorDescription { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the API call was successful
    /// </summary>
    public bool IsOk => HttpCode == HttpStatusCode.OK && ErrorCode == 0;

    /// <summary>
    /// Create a successful response
    /// </summary>
    public static ApiResponse<T> Success(T data, string rawResponse = "")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Response = rawResponse,
            HttpCode = HttpStatusCode.OK,
            HttpDescription = "OK",
            ErrorCode = 0,
            ErrorDescription = string.Empty
        };
    }

    /// <summary>
    /// Create an error response
    /// </summary>
    public static ApiResponse<T> Error(int errorCode, string errorDescription, HttpStatusCode httpCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Response = string.Empty,
            HttpCode = httpCode,
            HttpDescription = httpCode.ToString(),
            ErrorCode = errorCode,
            ErrorDescription = errorDescription
        };
    }
}

/// <summary>
/// Error response from the API
/// </summary>
public class ErrorResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
} 
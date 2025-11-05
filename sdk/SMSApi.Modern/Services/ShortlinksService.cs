using System.Net.Http;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Implementation of shortlink operations
/// </summary>
public class ShortlinksService : IShortlinks
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<ShortlinksService> _logger;

    public ShortlinksService(ApiClient apiClient, ILogger<ShortlinksService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Synchronous Methods

    public ApiResponse<List<ShortlinkResponse>> GetList(string? startDate = null, string? endDate = null, 
        int limit = -1, int offset = -1, string? id = null)
    {
        return GetListAsync(startDate, endDate, limit, offset, id).GetAwaiter().GetResult();
    }

    public ApiResponse<ShortlinkResponse> GetById(string id)
    {
        return GetByIdAsync(id).GetAwaiter().GetResult();
    }

    public ApiResponse<ShortlinkResponse> Create(string longUrl, string? name = null, ShortlinkStatus status = ShortlinkStatus.ACTIVE)
    {
        return CreateAsync(longUrl, name, status).GetAwaiter().GetResult();
    }

    public ApiResponse<ShortlinkResponse> UpdateStatus(string id, ShortlinkStatus status)
    {
        return UpdateStatusAsync(id, status).GetAwaiter().GetResult();
    }

    #endregion

    #region Asynchronous Methods

    public async Task<ApiResponse<List<ShortlinkResponse>>> GetListAsync(string? startDate = null, string? endDate = null, 
        int limit = -1, int offset = -1, string? id = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting shortlinks list with filters");

        var parameters = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(id))
        {
            parameters.Add("id", id);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(startDate))
                parameters.Add("start_date", startDate);

            if (!string.IsNullOrWhiteSpace(endDate))
                parameters.Add("end_date", endDate);

            if (limit > 0)
                parameters.Add("limit", limit.ToString());

            if (offset >= 0)
                parameters.Add("offset", offset.ToString());
        }

        var response = await _apiClient.RequestAsync<List<ShortlinkResponse>>("short_link/", HttpMethod.Get, 
            parameters, null, true, cancellationToken);

        if (response.IsOk && response.Data != null)
        {
            return response;
        }

        if (response.HttpCode == System.Net.HttpStatusCode.NotFound || response.ErrorCode == 404)
        {
            _logger.LogWarning("No shortlinks found");
            return ApiResponse<List<ShortlinkResponse>>.Success(new List<ShortlinkResponse>(), response.Response);
        }

        return response;
    }

    public async Task<ApiResponse<ShortlinkResponse>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Shortlink ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting shortlink by ID: {Id}", id);

        var parameters = new Dictionary<string, string>
        {
            ["id"] = id
        };

        var response = await _apiClient.RequestAsync<ShortlinkResponse>($"short_link/", HttpMethod.Get, 
            parameters, null, true, cancellationToken);

        if (response.IsOk && response.Data != null)
        {
            return response;
        }

        if (response.HttpCode == System.Net.HttpStatusCode.NotFound || response.ErrorCode == 404)
        {
            _logger.LogWarning("Shortlink not found: {Id}", id);
            return ApiResponse<ShortlinkResponse>.Error(404, "Shortlink not found", System.Net.HttpStatusCode.NotFound);
        }

        return response;
    }

    public async Task<ApiResponse<ShortlinkResponse>> CreateAsync(string longUrl, string? name = null, 
        ShortlinkStatus status = ShortlinkStatus.ACTIVE, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
            throw new ArgumentException("Long URL cannot be null or empty", nameof(longUrl));

        _logger.LogDebug("Creating shortlink: {LongUrl}, {Name}, {Status}", longUrl, name, status);

        var request = new CreateShortlinkRequest
        {
            LongUrl = longUrl,
            Name = name,
            Status = status
        };

        return await _apiClient.RequestAsync<ShortlinkResponse>("short_link", HttpMethod.Post, 
            null, request, false, cancellationToken);
    }

    public async Task<ApiResponse<ShortlinkResponse>> UpdateStatusAsync(string id, ShortlinkStatus status, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Shortlink ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Updating shortlink status: {Id}, {Status}", id, status);

        var request = new UpdateShortlinkStatusRequest
        {
            Status = status
        };

        var parameters = new Dictionary<string, string>
        {
            ["id"] = id
        };

        return await _apiClient.RequestAsync<ShortlinkResponse>($"short_link/{id}/status", HttpMethod.Put, 
            parameters, request, false, cancellationToken);
    }

    #endregion
}


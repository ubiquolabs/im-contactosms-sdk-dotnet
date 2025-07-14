using System.Net;
using System.Net.Http;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Implementation of tag operations
/// </summary>
public class TagsService : ITags
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<TagsService> _logger;

    public TagsService(ApiClient apiClient, ILogger<TagsService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Synchronous Methods

    public ApiResponse<List<TagResponse>> GetList()
    {
        return GetListAsync().GetAwaiter().GetResult();
    }

    public ApiResponse<TagResponse> Get(string tagName)
    {
        return GetAsync(tagName).GetAwaiter().GetResult();
    }

    public ApiResponse<TagResponse> Delete(string tagName)
    {
        return DeleteAsync(tagName).GetAwaiter().GetResult();
    }

    public ApiResponse<List<ContactResponse>> GetContactList(string tagName, int start = -1, int limit = -1)
    {
        return GetContactListAsync(tagName, start, limit).GetAwaiter().GetResult();
    }

    #endregion

    #region Asynchronous Methods

    public async Task<ApiResponse<List<TagResponse>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting tags list");

        return await _apiClient.RequestAsync<List<TagResponse>>("tags", HttpMethod.Get, 
            null, null, true, cancellationToken);
    }

    public async Task<ApiResponse<TagResponse>> GetAsync(string tagName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        _logger.LogDebug("Getting tag: {TagName}", tagName);

        var parameters = new Dictionary<string, string>
        {
            ["tag_name"] = tagName
        };

        return await _apiClient.RequestAsync<TagResponse>($"tags/{tagName}", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    public async Task<ApiResponse<TagResponse>> DeleteAsync(string tagName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        _logger.LogDebug("Deleting tag: {TagName}", tagName);

        var parameters = new Dictionary<string, string>
        {
            ["tag_name"] = tagName
        };

        return await _apiClient.RequestAsync<TagResponse>($"tags/{tagName}", HttpMethod.Delete, 
            parameters, null, true, cancellationToken);
    }

    public async Task<ApiResponse<List<ContactResponse>>> GetContactListAsync(string tagName, int start = -1, int limit = -1, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        _logger.LogDebug("Getting contacts for tag: {TagName}", tagName);

        var parameters = new Dictionary<string, string>
        {
            ["tag_name"] = tagName
        };

        if (start >= 0)
            parameters.Add("start", start.ToString());
        if (limit > 0)
            parameters.Add("limit", limit.ToString());

        return await _apiClient.RequestAsync<List<ContactResponse>>($"tags/{tagName}/contacts", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    #endregion
} 
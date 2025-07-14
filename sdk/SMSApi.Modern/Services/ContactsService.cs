using System.Net.Http;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Implementation of contact operations
/// </summary>
public class ContactsService : IContacts
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<ContactsService> _logger;

    public ContactsService(ApiClient apiClient, ILogger<ContactsService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Synchronous Methods

    public ApiResponse<List<ContactResponse>> GetList(List<ContactStatus>? contactStatuses = null, string? query = null, 
        int start = -1, int limit = -1, bool shortResults = false)
    {
        return GetListAsync(contactStatuses, query, start, limit, shortResults).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> GetByMsisdn(string msisdn)
    {
        return GetByMsisdnAsync(msisdn).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Add(string countryCode, string msisdn, string? firstName = null, string? lastName = null)
    {
        return AddAsync(countryCode, msisdn, firstName, lastName).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Update(string countryCode, string msisdn, string? firstName = null, string? lastName = null)
    {
        return UpdateAsync(countryCode, msisdn, firstName, lastName).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Delete(string msisdn)
    {
        return DeleteAsync(msisdn).GetAwaiter().GetResult();
    }

    public ApiResponse<List<TagResponse>> GetTagList(string msisdn)
    {
        return GetTagListAsync(msisdn).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> AddTag(string msisdn, string tagName)
    {
        return AddTagAsync(msisdn, tagName).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> RemoveTag(string msisdn, string tagName)
    {
        return RemoveTagAsync(msisdn, tagName).GetAwaiter().GetResult();
    }

    #endregion

    #region Asynchronous Methods

    public async Task<ApiResponse<List<ContactResponse>>> GetListAsync(List<ContactStatus>? contactStatuses = null, 
        string? query = null, int start = -1, int limit = -1, bool shortResults = false, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting contacts list with filters");

        var parameters = new Dictionary<string, string>();

        if (contactStatuses != null && contactStatuses.Count > 0)
            parameters.Add("status", string.Join(",", contactStatuses.Select(s => s.ToString())));

        if (!string.IsNullOrWhiteSpace(query))
            parameters.Add("query", query);

        if (start >= 0)
            parameters.Add("start", start.ToString());

        if (limit > 0)
            parameters.Add("limit", limit.ToString());

        if (shortResults)
            parameters.Add("short_results", "true");

        return await _apiClient.RequestAsync<List<ContactResponse>>("contacts", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> GetByMsisdnAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));

        _logger.LogDebug("Getting contact by MSISDN: {Msisdn}", msisdn);

        var parameters = new Dictionary<string, string>
        {
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}", HttpMethod.Get, 
            parameters, null, false, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> AddAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be null or empty", nameof(countryCode));
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));

        _logger.LogDebug("Adding contact: {CountryCode}, {Msisdn}, {FirstName}, {LastName}", 
            countryCode, msisdn, firstName, lastName);

        var contact = new ContactRequest
        {
            CountryCode = countryCode,
            Msisdn = msisdn,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            AddedFrom = AddedFrom.API
        };

        var parameters = new Dictionary<string, string>
        {
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}", HttpMethod.Post, 
            parameters, contact, false, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> UpdateAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be null or empty", nameof(countryCode));
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));

        _logger.LogDebug("Updating contact: {CountryCode}, {Msisdn}, {FirstName}, {LastName}", 
            countryCode, msisdn, firstName, lastName);

        var contact = new ContactRequest
        {
            CountryCode = countryCode,
            Msisdn = msisdn,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty
        };

        var parameters = new Dictionary<string, string>
        {
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}", HttpMethod.Put, 
            parameters, contact, false, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> DeleteAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));

        _logger.LogDebug("Deleting contact: {Msisdn}", msisdn);

        var parameters = new Dictionary<string, string>
        {
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}", HttpMethod.Delete, 
            parameters, null, false, cancellationToken);
    }

    public async Task<ApiResponse<List<TagResponse>>> GetTagListAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));

        _logger.LogDebug("Getting tags for contact: {Msisdn}", msisdn);

        var parameters = new Dictionary<string, string>
        {
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<List<TagResponse>>($"contacts/{msisdn}/tags", HttpMethod.Get, 
            parameters, null, false, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> AddTagAsync(string msisdn, string tagName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        _logger.LogDebug("Adding tag {TagName} to contact {Msisdn}", tagName, msisdn);

        var parameters = new Dictionary<string, string>
        {
            ["tag_name"] = tagName,
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}/tags/{tagName}", HttpMethod.Post, 
            parameters, null, false, cancellationToken);
    }

    public async Task<ApiResponse<ContactResponse>> RemoveTagAsync(string msisdn, string tagName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(msisdn))
            throw new ArgumentException("MSISDN cannot be null or empty", nameof(msisdn));
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        _logger.LogDebug("Removing tag {TagName} from contact {Msisdn}", tagName, msisdn);

        var parameters = new Dictionary<string, string>
        {
            ["tag_name"] = tagName,
            ["msisdn"] = msisdn
        };

        return await _apiClient.RequestAsync<ContactResponse>($"contacts/{msisdn}/tags/{tagName}", HttpMethod.Delete, 
            parameters, null, false, cancellationToken);
    }

    #endregion
} 
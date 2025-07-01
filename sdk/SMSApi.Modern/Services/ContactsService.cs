using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Modern implementation of contact operations
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

    #region Synchronous Methods (Backward Compatibility)

    public ApiResponse<List<ContactResponse>> GetList(List<ContactStatus>? contactStatuses = null, 
        string? query = null, int start = -1, int limit = -1, bool shortResults = false)
    {
        return GetListAsync(contactStatuses, query, start, limit, shortResults).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> GetByMsisdn(string msisdn)
    {
        return GetByMsisdnAsync(msisdn).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Update(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null)
    {
        return UpdateAsync(countryCode, msisdn, firstName, lastName).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Add(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null)
    {
        return AddAsync(countryCode, msisdn, firstName, lastName).GetAwaiter().GetResult();
    }

    public ApiResponse<ContactResponse> Delete(string msisdn)
    {
        return DeleteAsync(msisdn).GetAwaiter().GetResult();
    }

    public ApiResponse<List<GroupResponse>> GetGroupList(string msisdn)
    {
        return GetGroupListAsync(msisdn).GetAwaiter().GetResult();
    }

    #endregion

    #region Asynchronous Methods (Recommended)

    public Task<ApiResponse<List<ContactResponse>>> GetListAsync(List<ContactStatus>? contactStatuses = null, 
        string? query = null, int start = -1, int limit = -1, bool shortResults = false, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement contact list functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    public Task<ApiResponse<ContactResponse>> GetByMsisdnAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        // TODO: Implement get contact by MSISDN functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    public Task<ApiResponse<ContactResponse>> UpdateAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implement update contact functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    public Task<ApiResponse<ContactResponse>> AddAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implement add contact functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    public Task<ApiResponse<ContactResponse>> DeleteAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        // TODO: Implement delete contact functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    public Task<ApiResponse<List<GroupResponse>>> GetGroupListAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        // TODO: Implement get contact groups functionality
        throw new NotImplementedException("Contact operations will be implemented in the next phase");
    }

    #endregion
} 
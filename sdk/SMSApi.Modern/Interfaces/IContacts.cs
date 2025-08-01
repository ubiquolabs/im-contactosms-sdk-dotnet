using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for contact management operations
/// </summary>
public interface IContacts
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<List<ContactResponse>> GetList(List<ContactStatus>? contactStatuses = null, 
        string? query = null, int start = -1, int limit = -1, bool shortResults = false);
    ApiResponse<ContactResponse> GetByMsisdn(string msisdn);
    ApiResponse<ContactResponse> Update(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null);
    ApiResponse<ContactResponse> Add(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null);
    ApiResponse<ContactResponse> Delete(string msisdn);
    ApiResponse<List<TagResponse>> GetTagList(string msisdn);
    ApiResponse<ContactResponse> AddTag(string msisdn, string tagName);
    ApiResponse<ContactResponse> RemoveTag(string msisdn, string tagName);

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<List<ContactResponse>>> GetListAsync(List<ContactStatus>? contactStatuses = null, 
        string? query = null, int start = -1, int limit = -1, bool shortResults = false, 
        CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> GetByMsisdnAsync(string msisdn, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> UpdateAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> AddAsync(string countryCode, string msisdn, 
        string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> DeleteAsync(string msisdn, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<TagResponse>>> GetTagListAsync(string msisdn, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> AddTagAsync(string msisdn, string tagName, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContactResponse>> RemoveTagAsync(string msisdn, string tagName, CancellationToken cancellationToken = default);
} 
using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for group management operations
/// </summary>
public interface IGroups
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<List<GroupResponse>> GetList();
    ApiResponse<GroupResponse> Get(string shortName);
    ApiResponse<ActionMessageResponse> Update(string shortName, string name, string description, string? newShortName = null);
    ApiResponse<ActionMessageResponse> Add(string shortName, string name, string description);
    ApiResponse<ActionMessageResponse> Delete(string shortName);
    ApiResponse<List<ContactResponse>> GetContactList(string shortName);
    ApiResponse<ActionMessageResponse> AddContact(string shortName, string msisdn);
    ApiResponse<ActionMessageResponse> RemoveContact(string shortName, string msisdn);

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<List<GroupResponse>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<GroupResponse>> GetAsync(string shortName, CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> UpdateAsync(string shortName, string name, string description, 
        string? newShortName = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> AddAsync(string shortName, string name, string description, 
        CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> DeleteAsync(string shortName, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ContactResponse>>> GetContactListAsync(string shortName, CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> AddContactAsync(string shortName, string msisdn, 
        CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> RemoveContactAsync(string shortName, string msisdn, 
        CancellationToken cancellationToken = default);
} 
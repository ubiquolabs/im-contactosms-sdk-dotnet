using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for tag management operations
/// </summary>
public interface ITags
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<List<TagResponse>> GetList();
    ApiResponse<TagResponse> Get(string tagName);
    ApiResponse<TagResponse> Delete(string tagName);
    ApiResponse<List<ContactResponse>> GetContactList(string tagName, int start = -1, int limit = -1);

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<List<TagResponse>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<TagResponse>> GetAsync(string tagName, CancellationToken cancellationToken = default);
    Task<ApiResponse<TagResponse>> DeleteAsync(string tagName, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ContactResponse>>> GetContactListAsync(string tagName, int start = -1, int limit = -1, 
        CancellationToken cancellationToken = default);
} 
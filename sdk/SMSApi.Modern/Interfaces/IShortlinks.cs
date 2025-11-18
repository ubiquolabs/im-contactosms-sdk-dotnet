using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for shortlink management operations
/// </summary>
public interface IShortlinks
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<List<ShortlinkResponse>> GetList(string? startDate = null, string? endDate = null, 
        int limit = -1, int offset = -1, string? id = null);
    ApiResponse<ShortlinkResponse> GetById(string id);
    ApiResponse<ShortlinkResponse> Create(string longUrl, string? name = null, ShortlinkStatus status = ShortlinkStatus.ACTIVE, string? alias = null);
    ApiResponse<ShortlinkResponse> UpdateStatus(string id, ShortlinkStatus status);

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<List<ShortlinkResponse>>> GetListAsync(string? startDate = null, string? endDate = null, 
        int limit = -1, int offset = -1, string? id = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<ShortlinkResponse>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ShortlinkResponse>> CreateAsync(string longUrl, string? name = null,
        ShortlinkStatus status = ShortlinkStatus.ACTIVE,
        string? alias = null,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<ShortlinkResponse>> UpdateStatusAsync(string id, ShortlinkStatus status, CancellationToken cancellationToken = default);
}


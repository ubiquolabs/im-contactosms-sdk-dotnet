using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for account operations
/// </summary>
public interface IAccounts
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<AccountStatusResponse> GetStatus();

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<AccountStatusResponse>> GetStatusAsync(CancellationToken cancellationToken = default);
} 
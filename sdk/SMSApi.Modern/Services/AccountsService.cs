using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Modern implementation of account operations
/// </summary>
public class AccountsService : IAccounts
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<AccountsService> _logger;

    public AccountsService(ApiClient apiClient, ILogger<AccountsService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Synchronous Methods (Backward Compatibility)

    public ApiResponse<AccountStatusResponse> GetStatus()
    {
        return GetStatusAsync().GetAwaiter().GetResult();
    }

    #endregion

    #region Asynchronous Methods (Recommended)

    public Task<ApiResponse<AccountStatusResponse>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement account status functionality
        throw new NotImplementedException("Account operations will be implemented in the next phase");
    }

    #endregion
} 
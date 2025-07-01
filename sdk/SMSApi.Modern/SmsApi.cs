using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Services;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api;

/// <summary>
/// Modern SMS API client with dependency injection support
/// </summary>
public class SmsApi : ISmsApi
{
    /// <summary>
    /// Contact management operations
    /// </summary>
    public IContacts Contacts { get; }

    /// <summary>
    /// Group management operations
    /// </summary>
    public IGroups Groups { get; }

    /// <summary>
    /// Message operations
    /// </summary>
    public IMessages Messages { get; }

    /// <summary>
    /// Account operations
    /// </summary>
    public IAccounts Account { get; }

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    public SmsApi(IContacts contacts, IGroups groups, IMessages messages, IAccounts accounts)
    {
        Contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
        Groups = groups ?? throw new ArgumentNullException(nameof(groups));
        Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        Account = accounts ?? throw new ArgumentNullException(nameof(accounts));
    }

    /// <summary>
    /// Legacy constructor for backward compatibility (not recommended for new code)
    /// </summary>
    [Obsolete("Use dependency injection instead. This constructor is provided for backward compatibility only.")]
    public SmsApi(string apiKey, string secretKey, string apiUrl)
    {
        throw new NotSupportedException(
            "Legacy constructor is not supported in the modern version. " +
            "Please use dependency injection or the SmsApiBuilder for configuration.");
    }

    /// <summary>
    /// Legacy constructor with proxy support (not recommended for new code)
    /// </summary>
    [Obsolete("Use dependency injection instead. This constructor is provided for backward compatibility only.")]
    public SmsApi(string apiKey, string secretKey, string apiUrl, string proxyAddress, string userName, string password)
    {
        throw new NotSupportedException(
            "Legacy constructor is not supported in the modern version. " +
            "Please use dependency injection or the SmsApiBuilder for configuration.");
    }
} 
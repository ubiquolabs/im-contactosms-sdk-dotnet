namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Main interface for the SMS API client
/// </summary>
public interface ISmsApi
{
    /// <summary>
    /// Contact management operations
    /// </summary>
    IContacts Contacts { get; }

    /// <summary>
    /// Group management operations
    /// </summary>
    IGroups Groups { get; }

    /// <summary>
    /// Message operations
    /// </summary>
    IMessages Messages { get; }

    /// <summary>
    /// Account operations
    /// </summary>
    IAccounts Account { get; }

    /// <summary>
    /// Tag management operations
    /// </summary>
    ITags Tags { get; }
} 
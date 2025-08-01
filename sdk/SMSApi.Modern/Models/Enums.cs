namespace InteractuaMovil.ContactoSms.Api.Models;

/// <summary>
/// Message direction enumeration
/// </summary>
public enum MessageDirection
{
    /// <summary>
    /// Mobile Originated (incoming)
    /// </summary>
    MO,
    
    /// <summary>
    /// Mobile Terminated (outgoing)
    /// </summary>
    MT
}

/// <summary>
/// Message status enumeration
/// </summary>
public enum MessageStatus
{
    PENDING,
    PROCESSING,
    READY,
    SENT,
    UNREAD,
    READ,
    REPLIED,
    FORWARDED,
    ERROR
}

/// <summary>
/// Message sent from enumeration
/// </summary>
public enum MessageSentFrom
{
    WEB,
    API_HTTP,
    API_REST,
    SMS,
    SYSTEM,
    SCHEDULER,
    ADDON // ✅ Missing value found in API response
}

/// <summary>
/// Repeat interval for scheduled messages
/// </summary>
public enum RepeatInterval
{
    MONTHLY,
    WEEKLY,
    DAILY,
    HOURLY,
    ONCE
}

/// <summary>
/// Contact status enumeration
/// </summary>
public enum ContactStatus
{
    PENDING,
    CONFIRMED,
    CANCELLED,
    BLOCKED,
    SUSCRIBED,     // ✅ API real value (sin 'B')
    SUBSCRIBED,    // Alternative spelling
    INVITED
}

/// <summary>
/// Source where contact was added from
/// </summary>
public enum AddedFrom
{
    WEB,
    API,
    SMS,
    IMPORT,
    MANUAL,
    FILE_UPLOAD    // ✅ Real API value
} 
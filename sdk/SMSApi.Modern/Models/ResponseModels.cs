using System.Text.Json.Serialization;

namespace InteractuaMovil.ContactoSms.Api.Models;

/// <summary>
/// Contact response model
/// </summary>
public class ContactResponse
{
    [JsonPropertyName("msisdn")]
    public string Msisdn { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContactStatus Status { get; set; }

    [JsonPropertyName("added_from")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AddedFrom AddedFrom { get; set; }

    [JsonPropertyName("custom_field_1")]
    public string CustomField1 { get; set; } = string.Empty;

    [JsonPropertyName("custom_field_2")]
    public string CustomField2 { get; set; } = string.Empty;

    [JsonPropertyName("custom_field_3")]
    public string CustomField3 { get; set; } = string.Empty;

    [JsonPropertyName("custom_field_4")]
    public string CustomField4 { get; set; } = string.Empty;

    [JsonPropertyName("custom_field_5")]
    public string CustomField5 { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Group response model
/// </summary>
public class GroupResponse
{
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("members")]
    public GroupMembers Members { get; set; } = new();
}

/// <summary>
/// Group members summary
/// </summary>
public class GroupMembers
{
    [JsonPropertyName("total")]
    public string Total { get; set; } = string.Empty;

    [JsonPropertyName("pending")]
    public string Pending { get; set; } = string.Empty;

    [JsonPropertyName("confirmed")]
    public string Confirmed { get; set; } = string.Empty;

    [JsonPropertyName("cancelled")]
    public string Cancelled { get; set; } = string.Empty;
}

/// <summary>
/// Message response model
/// </summary>
public class MessageResponse
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }

    [JsonPropertyName("short_code")]
    public string ShortCode { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public int MessageTypeId { get; set; }

    [JsonPropertyName("direction")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageDirection MessageDirection { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageStatus MessageStatus { get; set; }

    [JsonPropertyName("sent_from")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageSentFrom SentFrom { get; set; }

    [JsonPropertyName("id")]
    public string ClientMessageId { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("sent_count")]
    public int SentCount { get; set; }

    [JsonPropertyName("error_count")]
    public int ErrorCount { get; set; }

    [JsonPropertyName("total_recipients")]
    public int TotalRecipients { get; set; }

    [JsonPropertyName("msisdn")]
    public string Msisdn { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("is_billable")]
    public bool Billable { get; set; }

    [JsonPropertyName("is_scheduled")]
    public bool Scheduled { get; set; }

    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("total_monitors")]
    public int TotalMonitors { get; set; }

    [JsonPropertyName("groups")]
    public List<string> Groups { get; set; } = new();

    [JsonPropertyName("recipients")]
    public List<RecipientResponse> Recipients { get; set; } = new();
}

/// <summary>
/// Recipient response model
/// </summary>
public class RecipientResponse
{
    [JsonPropertyName("msisdn")]
    public string Msisdn { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageStatus MessageStatus { get; set; }
}

/// <summary>
/// Account status response model
/// </summary>
public class AccountStatusResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sms_short_name")]
    public string SmsShortName { get; set; } = string.Empty;

    [JsonPropertyName("sms_subscription_type")]
    public string SmsSubscriptionType { get; set; } = string.Empty;

    [JsonPropertyName("sms_optin_keyword")]
    public string SmsOptinKeyword { get; set; } = string.Empty;

    [JsonPropertyName("messages_limit")]
    public int MessagesLimit { get; set; }

    [JsonPropertyName("messages_sent")]
    public int MessagesSent { get; set; }
}

/// <summary>
/// Action message response model
/// </summary>
public class ActionMessageResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
}

/// <summary>
/// Scheduled message response model
/// </summary>
public class ScheduleMessageResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("date_expires")]
    public DateTime DateExpires { get; set; }

    [JsonPropertyName("groups")]
    public List<MessageGroup> Groups { get; set; } = new();
}

/// <summary>
/// Message group model
/// </summary>
public class MessageGroup
{
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; } = string.Empty;
}

/// <summary>
/// Inbox message response model
/// </summary>
public class InboxMessageResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("msisdn")]
    public string Msisdn { get; set; } = string.Empty;

    [JsonPropertyName("datetime")]
    public string DateTime { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;

    [JsonPropertyName("short_number")]
    public string ShortNumber { get; set; } = string.Empty;

    [JsonPropertyName("created_on")]
    public string CreatedOn { get; set; } = string.Empty;

    [JsonPropertyName("is_deleted")]
    public int IsDeleted { get; set; }
} 
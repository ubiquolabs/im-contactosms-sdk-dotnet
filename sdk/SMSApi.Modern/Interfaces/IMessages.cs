using InteractuaMovil.ContactoSms.Api.Models;

namespace InteractuaMovil.ContactoSms.Api.Interfaces;

/// <summary>
/// Interface for SMS message operations
/// </summary>
public interface IMessages
{
    // Synchronous methods (for backward compatibility)
    ApiResponse<List<MessageResponse>> GetList(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, string? shortName = null, 
        bool includeRecipients = false, MessageDirection direction = MessageDirection.MT, string? username = null);

    ApiResponse<MessageResponse> SendToGroups(string[] shortNames, string message, string? id = null);
    ApiResponse<MessageResponse> SendToContact(string msisdn, string message, string? id = null);
    ApiResponse<List<ScheduleMessageResponse>> GetSchedule();
    ApiResponse<ActionMessageResponse> RemoveSchedule(string messageId);
    ApiResponse<ActionMessageResponse> AddSchedule(DateTime startDate, DateTime endDate, string name, 
        string message, string time, string frequency, string[] groups);
    ApiResponse<List<InboxMessageResponse>> GetInbox(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, int status = -1);

    // New synchronous methods for enhanced functionality
    ApiResponse<MessageResponse> SendToTags(string[] tags, string message, string? id = null);
    ApiResponse<List<MessageResponse>> GetListWithDeliveryStatus(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, MessageDirection direction = MessageDirection.MT, 
        bool deliveryStatusEnabled = true);
    ApiResponse<MessageResponse> SendToMultipleContacts(string[] msisdns, string message, string? id = null);
    ApiResponse<MessageResponse> GetMessageStatus(string messageId);

    // Asynchronous methods (recommended for new code)
    Task<ApiResponse<List<MessageResponse>>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, string? shortName = null, 
        bool includeRecipients = false, MessageDirection direction = MessageDirection.MT, string? username = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<MessageResponse>> SendToGroupsAsync(string[] shortNames, string message, string? id = null,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<MessageResponse>> SendToContactAsync(string msisdn, string message, string? id = null,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ScheduleMessageResponse>>> GetScheduleAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> RemoveScheduleAsync(string messageId, CancellationToken cancellationToken = default);
    Task<ApiResponse<ActionMessageResponse>> AddScheduleAsync(DateTime startDate, DateTime endDate, string name, 
        string message, string time, string frequency, string[] groups, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<InboxMessageResponse>>> GetInboxAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, int status = -1, CancellationToken cancellationToken = default);

    // New asynchronous methods for enhanced functionality
    Task<ApiResponse<MessageResponse>> SendToTagsAsync(string[] tags, string message, string? id = null,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MessageResponse>>> GetListWithDeliveryStatusAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, MessageDirection direction = MessageDirection.MT, 
        bool deliveryStatusEnabled = true, CancellationToken cancellationToken = default);
    Task<ApiResponse<MessageResponse>> SendToMultipleContactsAsync(string[] msisdns, string message, string? id = null,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<MessageResponse>> GetMessageStatusAsync(string messageId, CancellationToken cancellationToken = default);
} 
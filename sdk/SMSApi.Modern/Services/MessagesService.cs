using System.Net;
using System.Net.Http;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Logging;

namespace InteractuaMovil.ContactoSms.Api.Services;

/// <summary>
/// Modern implementation of message operations
/// </summary>
public class MessagesService : IMessages
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<MessagesService> _logger;

    public MessagesService(ApiClient apiClient, ILogger<MessagesService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Synchronous Methods (Backward Compatibility)

    public ApiResponse<List<MessageResponse>> GetList(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, string? shortName = null, 
        bool includeRecipients = false, MessageDirection direction = MessageDirection.MT, string? username = null)
    {
        return GetListAsync(startDate, endDate, start, limit, msisdn, shortName, 
            includeRecipients, direction, username).GetAwaiter().GetResult();
    }

    public ApiResponse<MessageResponse> SendToGroups(string[] shortNames, string message, string? id = null)
    {
        return SendToGroupsAsync(shortNames, message, id).GetAwaiter().GetResult();
    }

    public ApiResponse<MessageResponse> SendToContact(string msisdn, string message, string? id = null)
    {
        return SendToContactAsync(msisdn, message, id).GetAwaiter().GetResult();
    }

    public ApiResponse<List<ScheduleMessageResponse>> GetSchedule()
    {
        return GetScheduleAsync().GetAwaiter().GetResult();
    }

    public ApiResponse<ActionMessageResponse> RemoveSchedule(string messageId)
    {
        return RemoveScheduleAsync(messageId).GetAwaiter().GetResult();
    }

    public ApiResponse<ActionMessageResponse> AddSchedule(DateTime startDate, DateTime endDate, string name, 
        string message, string time, string frequency, string[] groups)
    {
        return AddScheduleAsync(startDate, endDate, name, message, time, frequency, groups).GetAwaiter().GetResult();
    }

    public ApiResponse<List<InboxMessageResponse>> GetInbox(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, int status = -1)
    {
        return GetInboxAsync(startDate, endDate, start, limit, msisdn, status).GetAwaiter().GetResult();
    }

    #endregion

    #region New Methods for Tags Support

    /// <summary>
    /// Send message to tags (synchronous)
    /// </summary>
    public ApiResponse<MessageResponse> SendToTags(string[] tags, string message, string? id = null)
    {
        return SendToTagsAsync(tags, message, id).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Send message to tags (asynchronous)
    /// </summary>
    public async Task<ApiResponse<MessageResponse>> SendToTagsAsync(string[] tags, string message, string? id = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, object>
        {
            ["tags"] = tags,
            ["message"] = message
        };

        if (!string.IsNullOrEmpty(id))
            payload["id"] = id;

        _logger.LogDebug("Sending message to {TagCount} tags", tags.Length);

        return await _apiClient.RequestAsync<MessageResponse>("messages/send", HttpMethod.Post, 
            null, payload, false, cancellationToken);
    }

    /// <summary>
    /// Get messages with delivery status support (synchronous)
    /// </summary>
    public ApiResponse<List<MessageResponse>> GetListWithDeliveryStatus(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, MessageDirection direction = MessageDirection.MT, 
        bool deliveryStatusEnabled = true)
    {
        return GetListWithDeliveryStatusAsync(startDate, endDate, start, limit, msisdn, direction, deliveryStatusEnabled).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get messages with delivery status support (asynchronous)
    /// </summary>
    public async Task<ApiResponse<List<MessageResponse>>> GetListWithDeliveryStatusAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, MessageDirection direction = MessageDirection.MT, 
        bool deliveryStatusEnabled = true, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();

        // ✅ Use exact date format like JavaScript: "2025-07-01 00:00:00" (consistent with GetListAsync)
        if (startDate.HasValue)
            parameters.Add("start_date", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss")); // "2025-07-01 00:00:00"
        if (endDate.HasValue)
            parameters.Add("end_date", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss")); // JavaScript format
        if (start != -1)
            parameters.Add("start", start.ToString());
        if (limit != -1)
            parameters.Add("limit", limit.ToString());
        if (!string.IsNullOrEmpty(msisdn))
            parameters.Add("msisdn", msisdn);
        
        parameters.Add("direction", direction.ToString().ToUpper());
        
        // Add delivery status parameter - this is the key fix!
        if (deliveryStatusEnabled)
            parameters.Add("delivery_status_enable", "true");

        _logger.LogDebug("Getting message list with {ParameterCount} parameters", parameters.Count);
        
        // Use the correct base endpoint
        return await _apiClient.RequestAsync<List<MessageResponse>>("messages", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    #endregion

    #region Asynchronous Methods (Recommended)

    public async Task<ApiResponse<List<MessageResponse>>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, string? shortName = null, 
        bool includeRecipients = false, MessageDirection direction = MessageDirection.MT, string? username = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();

        // ✅ Use exact date format like JavaScript: "2025-07-01 00:00:00"
        if (startDate.HasValue)
            parameters.Add("start_date", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss")); // "2025-07-01 00:00:00"
        if (endDate.HasValue)
            parameters.Add("end_date", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss")); // JavaScript format
        if (start != -1)
            parameters.Add("start", start.ToString());
        if (limit != -1)
            parameters.Add("limit", limit.ToString());
        if (!string.IsNullOrEmpty(msisdn))
            parameters.Add("msisdn", msisdn);
        if (!string.IsNullOrEmpty(shortName))
            parameters.Add("short_name", shortName);
        if (!string.IsNullOrEmpty(username))
            parameters.Add("user", username);
        
        // ✅ Add direction parameter (required by API)
        parameters.Add("direction", direction.ToString().ToUpper());

        // ✅ Re-enable delivery status as requested
        parameters.Add("delivery_status_enable", "true");

        _logger.LogDebug("Getting message list with {ParameterCount} parameters (Java-compatible)", parameters.Count);
        
        return await _apiClient.RequestAsync<List<MessageResponse>>("messages", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    public async Task<ApiResponse<MessageResponse>> SendToGroupsAsync(string[] shortNames, string message, string? id = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, object>
        {
            ["groups"] = shortNames,
            ["message"] = message
        };

        if (!string.IsNullOrEmpty(id))
            payload["id"] = id;

        _logger.LogDebug("Sending message to {GroupCount} groups", shortNames.Length);

        return await _apiClient.RequestAsync<MessageResponse>("messages/send", HttpMethod.Post, 
            null, payload, false, cancellationToken);
    }

    public async Task<ApiResponse<MessageResponse>> SendToContactAsync(string msisdn, string message, string? id = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, object>
        {
            ["msisdn"] = msisdn,
            ["message"] = message
        };

        if (!string.IsNullOrEmpty(id))
            payload["id"] = id;

        _logger.LogDebug("Sending message to contact {Msisdn}", msisdn);

        return await _apiClient.RequestAsync<MessageResponse>("messages/send_to_contact", HttpMethod.Post, 
            null, payload, false, cancellationToken);
    }

    /// <summary>
    /// Send message to multiple contacts (asynchronous)
    /// </summary>
    public async Task<ApiResponse<MessageResponse>> SendToMultipleContactsAsync(string[] msisdns, string message, string? id = null,
        CancellationToken cancellationToken = default)
    {
        // Send to each contact individually and collect results
        var results = new List<object>();
        int successCount = 0;
        int failedCount = 0;

        foreach (var msisdn in msisdns)
        {
            try
            {
                var individualId = !string.IsNullOrEmpty(id) ? $"{id}-{msisdn}" : null;
                var response = await SendToContactAsync(msisdn, message, individualId, cancellationToken);
                
                if (response.IsOk)
                {
                    successCount++;
                }
                else
                {
                    failedCount++;
                }

                results.Add(new { msisdn, success = response.IsOk, error = response.ErrorDescription });
            }
            catch (Exception ex)
            {
                failedCount++;
                results.Add(new { msisdn, success = false, error = ex.Message });
                _logger.LogError(ex, "Error sending message to {Msisdn}", msisdn);
            }
        }

        _logger.LogDebug("Sent messages to {TotalCount} contacts - Success: {SuccessCount}, Failed: {FailedCount}", 
                        msisdns.Length, successCount, failedCount);

        // Return a summary response
        var summaryResponse = new ApiResponse<MessageResponse>
        {
            Data = new MessageResponse
            {
                SentCount = successCount,
                Message = $"Processed {msisdns.Length} contacts - Success: {successCount}, Failed: {failedCount}"
            },
            HttpCode = failedCount == 0 ? HttpStatusCode.OK : HttpStatusCode.PartialContent
        };

        return summaryResponse;
    }

    /// <summary>
    /// Send message to multiple contacts (synchronous)
    /// </summary>
    public ApiResponse<MessageResponse> SendToMultipleContacts(string[] msisdns, string message, string? id = null)
    {
        return SendToMultipleContactsAsync(msisdns, message, id).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get message status by ID (asynchronous)
    /// </summary>
    public async Task<ApiResponse<MessageResponse>> GetMessageStatusAsync(string messageId, CancellationToken cancellationToken = default)
    {
        // Try to find the message in recent messages
        var today = DateTime.Today;
        var response = await GetListAsync(
            startDate: today,
            endDate: today.AddDays(1),
            limit: 100,
            cancellationToken: cancellationToken
        );

        if (response.IsOk && response.Data?.Count > 0)
        {
            var message = response.Data.FirstOrDefault(m => m.MessageId == messageId);
            if (message != null)
            {
                return new ApiResponse<MessageResponse>
                {
                    Data = message,
                    HttpCode = HttpStatusCode.OK
                };
            }
        }

        return new ApiResponse<MessageResponse>
        {
            ErrorCode = 404,
            ErrorDescription = "Message not found",
            HttpCode = HttpStatusCode.NotFound
        };
    }

    /// <summary>
    /// Get message status by ID (synchronous)
    /// </summary>
    public ApiResponse<MessageResponse> GetMessageStatus(string messageId)
    {
        return GetMessageStatusAsync(messageId).GetAwaiter().GetResult();
    }

    public async Task<ApiResponse<List<ScheduleMessageResponse>>> GetScheduleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting scheduled messages");
        
        return await _apiClient.RequestAsync<List<ScheduleMessageResponse>>("messages/scheduled", HttpMethod.Get, 
            null, null, false, cancellationToken);
    }

    public async Task<ApiResponse<ActionMessageResponse>> RemoveScheduleAsync(string messageId, CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, object>
        {
            ["message_id"] = messageId
        };

        _logger.LogDebug("Removing scheduled message {MessageId}", messageId);

        return await _apiClient.RequestAsync<ActionMessageResponse>("messages/scheduled", HttpMethod.Delete, 
            null, payload, false, cancellationToken);
    }

    public async Task<ApiResponse<ActionMessageResponse>> AddScheduleAsync(DateTime startDate, DateTime endDate, string name, 
        string message, string time, string frequency, string[] groups, CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, object>
        {
            ["start_date"] = startDate.ToString("yyyy-MM-dd"),
            ["end_date"] = endDate.ToString("yyyy-MM-dd"),
            ["name"] = name,
            ["message"] = message,
            ["time"] = time,
            ["frequency"] = frequency,
            ["groups"] = groups
        };

        _logger.LogDebug("Adding scheduled message '{Name}' for {GroupCount} groups", name, groups.Length);

        return await _apiClient.RequestAsync<ActionMessageResponse>("messages/scheduled", HttpMethod.Post, 
            null, payload, false, cancellationToken);
    }

    public async Task<ApiResponse<List<InboxMessageResponse>>> GetInboxAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, int status = -1, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();

        if (startDate.HasValue)
            parameters.Add("start_date", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        if (endDate.HasValue)
            parameters.Add("end_date", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        if (start != -1)
            parameters.Add("start", start.ToString());
        if (limit != -1)
            parameters.Add("limit", limit.ToString());
        if (!string.IsNullOrEmpty(msisdn))
            parameters.Add("msisdn", msisdn);
        if (status != -1)
            parameters.Add("status", status.ToString());

        _logger.LogDebug("Getting inbox messages with {ParameterCount} parameters", parameters.Count);

        return await _apiClient.RequestAsync<List<InboxMessageResponse>>("messages/inbox", HttpMethod.Get, 
            parameters, null, true, cancellationToken);
    }

    #endregion
} 
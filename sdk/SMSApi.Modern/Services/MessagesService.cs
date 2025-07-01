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

    #region Asynchronous Methods (Recommended)

    public async Task<ApiResponse<List<MessageResponse>>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null, 
        int start = -1, int limit = -1, string? msisdn = null, string? shortName = null, 
        bool includeRecipients = false, MessageDirection direction = MessageDirection.MT, string? username = null,
        CancellationToken cancellationToken = default)
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
        if (!string.IsNullOrEmpty(shortName))
            parameters.Add("short_name", shortName);
        if (!string.IsNullOrEmpty(username))
            parameters.Add("user", username);
        
        parameters.Add("direction", direction.ToString().ToUpper());
        parameters.Add("include_recipients", includeRecipients.ToString().ToLower());

        _logger.LogDebug("Getting message list with {ParameterCount} parameters", parameters.Count);
        
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
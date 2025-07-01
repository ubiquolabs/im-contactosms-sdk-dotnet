# 🚀 Modern SMS API SDK for .NET

A modern, async-first .NET SDK for the InteractuaMovil ContactoSMS API, featuring dependency injection, comprehensive logging, and multi-framework support.

## ✨ Features

- **🔄 Async/Await Support** - Full async implementation with sync compatibility
- **💉 Dependency Injection** - Built-in DI container integration
- **🎯 Multi-targeting** - Supports .NET 8.0 and .NET Framework 4.8
- **🛡️ Type Safety** - Strong typing with nullable reference types
- **📝 Comprehensive Logging** - Structured logging with Microsoft.Extensions.Logging
- **⚙️ Configuration** - Type-safe configuration with validation
- **🧪 Testable** - Built with testing in mind

## 📦 Installation

### Using Package Manager Console
```powershell
Install-Package InteractuaMovil.ContactoSms.Api.Modern
```

### Using .NET CLI
```bash
dotnet add package InteractuaMovil.ContactoSms.Api.Modern
```

## 🚀 Quick Start

### 1. Configuration

#### appsettings.json
```json
{
  "SmsApi": {
    "ApiKey": "your-api-key",
    "SecretKey": "your-secret-key",
    "ApiUrl": "https://your-api-url.com/api/",
    "TimeoutSeconds": 30,
    "EnableLogging": true,
    "Proxy": {
      "Address": "http://proxy.example.com:8080",
      "Username": "proxy-user",
      "Password": "proxy-password"
    }
  }
}
```

### 2. Dependency Injection Setup

#### ASP.NET Core / Generic Host
```csharp
using InteractuaMovil.ContactoSms.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add SMS API services
builder.Services.AddSmsApi(builder.Configuration);

var app = builder.Build();
```

#### Manual Configuration
```csharp
services.AddSmsApi(options =>
{
    options.ApiKey = "your-api-key";
    options.SecretKey = "your-secret-key";
    options.ApiUrl = "https://your-api-url.com/api/";
    options.TimeoutSeconds = 30;
    options.EnableLogging = true;
});
```

### 3. Usage

#### With Dependency Injection (Recommended)
```csharp
public class MessageController : ControllerBase
{
    private readonly ISmsApi _smsApi;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ISmsApi smsApi, ILogger<MessageController> logger)
    {
        _smsApi = smsApi;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            // Send message asynchronously
            var result = await _smsApi.Messages.SendToContactAsync(
                request.PhoneNumber, 
                request.Message,
                request.MessageId);

            if (result.IsOk)
            {
                _logger.LogInformation("Message sent successfully: {MessageId}", result.Data?.MessageId);
                return Ok(result.Data);
            }
            else
            {
                _logger.LogWarning("Failed to send message: {Error}", result.ErrorDescription);
                return BadRequest(result.ErrorDescription);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "Internal server error");
        }
    }
}
```

## 📚 API Reference

### Messages

#### Send Message to Contact
```csharp
// Async (Recommended)
var result = await smsApi.Messages.SendToContactAsync("+1234567890", "Hello World!");

// Sync (Backward Compatibility)
var result = smsApi.Messages.SendToContact("+1234567890", "Hello World!");
```

#### Send Message to Groups
```csharp
var groups = new[] { "group1", "group2" };
var result = await smsApi.Messages.SendToGroupsAsync(groups, "Hello Groups!");
```

#### Get Message List
```csharp
var messages = await smsApi.Messages.GetListAsync(
    startDate: DateTime.Now.AddDays(-7),
    endDate: DateTime.Now,
    limit: 100,
    includeRecipients: true);

if (messages.IsOk)
{
    foreach (var message in messages.Data)
    {
        Console.WriteLine($"Message {message.MessageId}: {message.Message}");
    }
}
```

#### Scheduled Messages
```csharp
// Get scheduled messages
var scheduled = await smsApi.Messages.GetScheduleAsync();

// Add scheduled message
var result = await smsApi.Messages.AddScheduleAsync(
    startDate: DateTime.Now.AddDays(1),
    endDate: DateTime.Now.AddDays(30),
    name: "Daily Reminder",
    message: "Don't forget your appointment!",
    time: "09:00",
    frequency: "DAILY",
    groups: new[] { "customers" });
```

### Error Handling

```csharp
var result = await smsApi.Messages.SendToContactAsync("+1234567890", "Test");

if (result.IsOk)
{
    // Success
    Console.WriteLine($"Message sent with ID: {result.Data?.MessageId}");
}
else
{
    // Error
    Console.WriteLine($"Error {result.ErrorCode}: {result.ErrorDescription}");
    
    // Check HTTP status
    if (result.HttpCode == HttpStatusCode.Unauthorized)
    {
        Console.WriteLine("Check your API credentials");
    }
}
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test tests/SMSApi.Modern.Tests/
```

### Run Example Application
```bash
cd examples/ModernSmsApiExample
dotnet run
```

**Note**: Update `appsettings.json` with your actual API credentials before running.

## 🔄 Migration from Legacy SDK

### Old Way (.NET Framework)
```csharp
var api = new SmsApi("api-key", "secret-key", "https://api.url/");
var result = api.Messages.SendToContact("+1234567890", "Hello");
```

### New Way (Modern)
```csharp
// With DI
public class MyService
{
    private readonly ISmsApi _smsApi;
    
    public MyService(ISmsApi smsApi)
    {
        _smsApi = smsApi;
    }
    
    public async Task SendMessageAsync()
    {
        var result = await _smsApi.Messages.SendToContactAsync("+1234567890", "Hello");
    }
}
```

## 📊 Performance & Benefits

| Feature | Legacy SDK | Modern SDK |
|---------|------------|------------|
| **Framework** | .NET 2.0/3.5/4.0 | .NET 8.0 + .NET 4.8 |
| **HTTP Client** | HttpWebRequest | HttpClient |
| **Async Support** | ❌ | ✅ Full async/await |
| **Dependency Injection** | ❌ | ✅ Built-in |
| **Configuration** | Manual strings | ✅ Type-safe IOptions |
| **Logging** | Console.WriteLine | ✅ Structured logging |
| **JSON Serialization** | Newtonsoft.Json 4.5 | ✅ System.Text.Json 8.0 |
| **Connection Pooling** | ❌ | ✅ HttpClient pooling |
| **Cancellation** | ❌ | ✅ CancellationToken |

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [Legacy SDK Documentation](../sdk/SMSApi/)
- [API Documentation](https://docs.example.com)
- [Support](mailto:support@example.com)

---

**⚠️ Security Note**: Never commit API keys to version control. Use environment variables, user secrets, or secure configuration management.



# .NET SMS API SDK

A modern, feature-rich .NET SDK for interacting with the SMS API service. This SDK provides easy-to-use methods for managing contacts, sending messages, handling tags, and creating shortlinks with enhanced functionality and improved error handling.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C# 12](https://img.shields.io/badge/C%23-12.0-green?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Production Ready](https://img.shields.io/badge/Status-Production%20Ready-brightgreen)](https://github.com)

## Features

- **Production Ready** - Fully tested with real API
- **Async/Await First** - Fully asynchronous implementation
- **Dependency Injection** - Microsoft.Extensions.DI integration
- **Type Safety** - C# 12, nullable reference types
- **Structured Logging** - Microsoft.Extensions.Logging support
- **Configuration Management** - IOptions pattern + User Secrets
- **API Compatibility** - Fully compatible with backend API
- **UTF-8 Full Support** - Perfect handling of special characters
- **Cross-SDK Compatibility** - Consistent behavior with other platform SDKs
- **Fully Tested** - Unit and integration tests

## Implemented Features

### Messages
- Send to individual contact
- Send to tags (groups)
- Query messages by date
- Delivery status
- Scheduled messages

### Contacts
- Create/update contacts
- Get individual contact
- Tag management per contact
- Status: SUSCRIBED, SUBSCRIBED, INVITED

### Tags
- List all tags
- Get contacts by tag
- Delete tags

### Accounts & Reports
- Account information
- Balance and limits
- Usage statistics

### Shortlinks
- Create shortlinks
- List shortlinks (with date filters)
- Get shortlink by ID
- Update shortlink status (ACTIVE/INACTIVE)

## Requirements

- .NET 8.0 or higher
- Visual Studio 2022 17.8+ or VS Code
- C# 12 language features
- Valid API credentials

## Installation

### From Source Code (Recommended)
```bash
cd sdk/SMSApi.Modern
dotnet build
dotnet pack
```

### Project Reference
```xml
<ProjectReference Include="path/to/SMSApi.Modern/SMSApi.Modern.csproj" />
```

## Configuration

### Option A: User Secrets (Recommended for development)
```bash
cd examples/QuickTest
dotnet user-secrets set "SmsApi:ApiKey" "your-api-key-here"
dotnet user-secrets set "SmsApi:SecretKey" "your-secret-key-here"  
dotnet user-secrets set "SmsApi:ApiUrl" "https://your-api-url.com/api/rest/"
```

### Option B: Edit appsettings.json
```json
{
  "SmsApi": {
    "ApiKey": "your-api-key-here",
    "SecretKey": "your-secret-key-here", 
    "ApiUrl": "https://your-api-url.com/api/rest/",
    "TimeoutSeconds": 30,
    "EnableLogging": true
  },
  "TestData": {
    "TestPhoneNumber": "50212345678",
    "TestMessage": "Hello from .NET SDK!",
    "TestTagName": "TestTag",
    "TestContactFirstName": "John",
    "TestContactLastName": "Doe"
  }
}
```

## Quick Start

### Basic Usage

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InteractuaMovil.ContactoSms.Api.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSmsApi(context.Configuration);
    })
    .Build();

var smsApi = host.Services.GetRequiredService<ISmsApi>();

// Test connection
var account = await smsApi.Account.GetStatusAsync();
Console.WriteLine(account.IsOk ? "Connected!" : "Connection failed");
```

## Usage

### Basic Configuration
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InteractuaMovil.ContactoSms.Api.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Registrar SMS API
        services.AddSmsApi(context.Configuration);
        
        // Tus servicios
        services.AddTransient<NotificationService>();
    })
    .Build();
```

### Envío de Mensajes
```csharp
public class NotificationService
{
    private readonly ISmsApi _smsApi;
    
    public NotificationService(ISmsApi smsApi)
    {
        _smsApi = smsApi;
    }

    // Enviar a contacto individual
    public async Task<bool> SendWelcomeAsync(string phone, string name)
    {
        var result = await _smsApi.Messages.SendToContactAsync(
            msisdn: phone,
            message: $"¡Bienvenido {name}!",
            messageId: $"welcome-{DateTime.Now:yyyyMMddHHmmss}"
        );
        
        return result.IsOk;
    }

    // Send to tags (groups)
    public async Task<bool> SendPromotionAsync(string tagName, string promo)
    {
        var result = await _smsApi.Messages.SendToTagsAsync(
            message: $"Special promotion: {promo}",
            tags: new[] { tagName },
            messageId: $"promo-{DateTime.Now:yyyyMMddHHmmss}"
        );
        
        return result.IsOk;
    }
}
```

### Contact Management
```csharp
// Add contact
var contact = await _smsApi.Contacts.AddAsync(
    countryCode: "502",
    msisdn: "12345678",
    firstName: "John",
    lastName: "Doe"
);

// Get contact
var existing = await _smsApi.Contacts.GetByMsisdnAsync("50212345678");

// Add tag to contact
await _smsApi.Contacts.AddTagAsync("50212345678", "VIP");
```

### Tag Management
```csharp
// List all tags
var tags = await _smsApi.Tags.GetListAsync();

// Get contacts by tag
var contacts = await _smsApi.Tags.GetContactListAsync("VIP");

// Delete tag
await _smsApi.Tags.DeleteAsync("OldTag");
```

### Message Query
```csharp
// Today's messages
var today = DateTime.Today;
var messages = await _smsApi.Messages.GetListAsync(
    startDate: today,
    endDate: today.AddDays(1),
    direction: MessageDirection.MT,
    limit: 50
);
```

### Shortlink Management
```csharp
// Create shortlink
var shortlink = await _smsApi.Shortlinks.CreateAsync(
    longUrl: "https://www.example.com/very-long-url-with-parameters",
    name: "Example Shortlink",
    status: ShortlinkStatus.ACTIVE
);

// List shortlinks (no parameters)
var shortlinks = await _smsApi.Shortlinks.GetListAsync();

// List shortlinks with limit
var shortlinksWithLimit = await _smsApi.Shortlinks.GetListAsync(limit: 10);

// List shortlinks by date range
var shortlinksByDate = await _smsApi.Shortlinks.GetListAsync(
    startDate: "2025-01-01",
    endDate: "2025-01-31",
    limit: 20
);

// List shortlinks by date with limit and offset
var shortlinksByDateWithOffset = await _smsApi.Shortlinks.GetListAsync(
    startDate: "2025-01-01",
    endDate: "2025-12-31",
    limit: 20,
    offset: -6
);

// List shortlink by ID (using GetListAsync)
var shortlinkListById = await _smsApi.Shortlinks.GetListAsync(id: "123ABC");

// Get shortlink by ID (specific method)
var shortlinkById = await _smsApi.Shortlinks.GetByIdAsync("123ABC");

// Update shortlink status
await _smsApi.Shortlinks.UpdateStatusAsync("123ABC", ShortlinkStatus.INACTIVE);
```

## Available Examples

### QuickTest - Complete Test Suite
```bash
cd examples/QuickTest
dotnet run
```
**Purpose**: Tests all main functionalities quickly.

### ApiExample.Modern - Detailed Example
```bash
cd examples/ApiExample.Modern  
dotnet run
```
**Purpose**: Complete example with error handling and logging.

### SendSmsExample.Modern - Simple Example
```bash
cd examples/SendSmsExample.Modern
dotnet run
```
**Purpose**: Minimal example for sending SMS.

## Rate Limits

The API has rate limits to ensure fair usage:

- **Shortlinks**: Maximum of 10 shortlinks created per minute per account (default)
- When you exceed the limit, you'll receive a 403 error with code `42900`
- **For inquiries or requests to increase the limit**: Please contact Technical Support directly through their support channels

Example error response:
```json
{
  "code": 42900,
  "error": "Ha excedido el límite de solicitudes. Intente nuevamente más tarde"
}
```

## API Response Examples

### Create Shortlink - Success
```json
{
  "success": true,
  "message": "Shortlink created successfully",
  "account_id": 12345,
  "url_id": "123ABC",
  "short_url": "https://shorturl-pais.com/123ABC",
  "long_url": "https://www.example.com/very-long-url-with-parameters"
}
```

### List Shortlinks - Success
```json
{
  "success": true,
  "message": "Shortlinks retrieved successfully",
  "data": [
    {
      "_id": "123ABC",
      "account_uid": "abcde12345678kklm",
      "name": "Enlace corto de prueba",
      "status": "INACTIVE",
      "base_url": "https://shorturl-pais.com/",
      "short_url": "https://shorturl-pais.com/123ABC",
      "long_url": "https://www.example.com/long-url-here",
      "visits": 0,
      "unique_visits": 0,
      "preview_visits": 0,
      "created_by": "SHORTLINK_API",
      "reference_type": "SHORT_LINK",
      "expiration": false,
      "expiration_date": null,
      "created_on": 1735689600000
    }
  ],
  "account_id": 12345
}
```

### Get Shortlink by ID - Success
```json
{
  "success": true,
  "message": "Shortlink found",
  "account_id": 12345,
  "url_id": "123ABC",
  "short_url": "https://shorturl-pais.com/123ABC",
  "long_url": "https://www.example.com/long-url-with-parameters",
  "name": "Example Shortlink",
  "status": "ACTIVE",
  "visits": 0,
  "unique_visits": 0,
  "preview_visits": 0,
  "created_by": "SHORTLINK_API",
  "created_on": 1735689600000
}
```

### Get Shortlink by ID - Not Found
```json
{
  "success": false,
  "message": "Shortlink not found"
}
```

## Testing

```bash
# Build everything
dotnet build

# Run unit tests
cd tests/SMSApi.Modern.Tests
dotnet test

# Integration test with real API
cd examples/QuickTest
dotnet run
```

## Troubleshooting

### Error: "Failed to parse response JSON"
**Resolved** - Enums updated to match real API:
- `ContactStatus.SUSCRIBED` (without 'B')
- `AddedFrom.FILE_UPLOAD`

### Error: "The JSON value could not be converted"
**Resolved** - Full compatibility with API responses.

### Error: "Not authorized" for some operations
**Expected** - Some operations require specific permissions in your API account.

### Error: "Message blocked because it is duplicate"  
**Expected** - API anti-spam protection. Change the message or wait a few minutes.

## Compatibility Status

| Feature | Status | Notes |
|---------|--------|-------|
| Send messages | Supported | Contact + Tags |
| Contact management | Supported | Full CRUD |
| Tag management | Supported | List, query, delete |
| Message query | Supported | By date, filters |
| Shortlink management | Supported | Full CRUD, date filters |
| Authentication | Supported | API Key + Secret |
| Logging | Supported | Structured logging |
| Async/Await | Supported | Fully asynchronous |
| DI Container | Supported | Microsoft.Extensions.DI |

## Resources

- **Usage examples** - Check the `examples/` folder
- **Tests** - Run `dotnet test` to see examples
- **Configuration** - See `appsettings.json` in examples

---

**Security Note**: Never commit API keys or secrets to version control. Always use user secrets, environment variables, or secure vaults for sensitive credentials.

**Ready to integrate SMS into your .NET application?** This SDK provides everything you need for reliable, scalable, and maintainable SMS functionality.



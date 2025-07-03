# 🚀 Modern SMS API SDK for .NET

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C# 12](https://img.shields.io/badge/C%23-12.0-green?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A **modern**, **async-first** .NET SDK for SMS API services, designed with .NET 8 best practices, dependency injection, and comprehensive logging support.

## ✨ Features

- **Async/Await First** - Full async implementation
- **Dependency Injection** - Microsoft.Extensions.DI integration
- **Type Safety** - C# 12, nullable reference types
- **Structured Logging** - Microsoft.Extensions.Logging
- **Configuration Management** - IOptions pattern
- **Security First** - HMAC authentication
- **Fully Tested** - Unit tests for all core features

## 📦 Installation

### NuGet Package Manager
```powershell
Install-Package SMSApi.Modern
```

### .NET CLI
```bash
dotnet add package SMSApi.Modern
```

### PackageReference
```xml
<PackageReference Include="SMSApi.Modern" Version="1.0.0" />
```

## 🚀 Quick Start Guide

### 1. Configuration

#### appsettings.json (Recommended)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "SMSApi.Modern": "Debug"
    }
  },
  "SmsApi": {
    "ApiKey": "your-api-key-here",
    "SecretKey": "your-secret-key-here",
    "ApiUrl": "https://your-api-url.com/api/",
    "TimeoutSeconds": 30,
    "EnableLogging": true,
    "Proxy": {
      "Address": "http://proxy.example.com:8080",
      "Username": "proxy-user",
      "Password": "proxy-password"
    }
  },
  "TestData": {
    "TestPhoneNumber": "+1234567890",
    "TestMessage": "Hello from Modern SMS .NET API SDK!"
  }
}
```

### 2. Dependency Injection Setup

#### Console Application with Generic Host
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SMSApi.Modern.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add SMS API with configuration
        services.AddSmsApi(context.Configuration);
        
        // Add your application services
        services.AddTransient<MyMessageService>();
    })
    .Build();

// Get and use the service
var messageService = host.Services.GetRequiredService<MyMessageService>();
await messageService.SendWelcomeMessageAsync();
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

### 3. Basic Usage Example

```csharp
public class NotificationService
{
    private readonly ISmsApi _smsApi;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ISmsApi smsApi, ILogger<NotificationService> logger)
    {
        _smsApi = smsApi;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeMessageAsync(string phoneNumber, string customerName)
    {
        try
        {
            var message = $"Welcome {customerName}! Thank you for joining our service.";
            var messageId = $"welcome-{DateTime.Now:yyyyMMdd-HHmmss}";

            _logger.LogInformation("Sending welcome message to {PhoneNumber}", phoneNumber);

            var result = await _smsApi.Messages.SendToContactAsync(
                msisdn: phoneNumber,
                message: message,
                messageId: messageId);

            if (result.IsOk)
            {
                _logger.LogInformation("Welcome message sent successfully. MessageId: {MessageId}", 
                    result.Data?.MessageId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to send welcome message: {Error} (Code: {ErrorCode})", 
                    result.ErrorDescription, result.ErrorCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending welcome message to {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}
```

## 📚 API Reference

(Describe aquí los métodos principales del SDK moderno, como en la sección anterior, pero solo para la versión moderna)

## 🏃‍♂️ Cómo Ejecutar y Modificar

### Estructura del Proyecto
```
.NET/
├── sdk/SMSApi.Modern/          # 🎯 SDK Principal (aquí haces cambios)
├── examples/                   # 🔧 Ejemplos de uso
│   ├── ApiExample.Modern/      # Ejemplo completo
│   └── SendSmsExample.Modern/  # Ejemplo simple
├── tests/SMSApi.Modern.Tests/  # 🧪 Tests unitarios
```

### Ejecución Rápida

```bash
# Restaurar dependencias
 dotnet restore
# Compilar
 dotnet build
# Ejecutar ejemplo completo
 cd examples/ApiExample.Modern
 dotnet run
# Ejecutar ejemplo simple
 cd ../SendSmsExample.Modern
 dotnet run
```

## 🧪 Testing

```bash
# Run all tests
 dotnet test
```

## 📋 Requirements

- **.NET 8.0** (recomendado)
- **Visual Studio 2022** 17.8+ o **VS Code**
- **C# 12** language features

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 🔗 Resources

- 📖 **[API Documentation](https://docs.your-api.com)** - Complete API reference
- 🐛 **[Issue Tracker](https://github.com/your-org/sms-api-dotnet/issues)** - Report bugs or request features
- 💬 **[Discussions](https://github.com/your-org/sms-api-dotnet/discussions)** - Community support

---

**⚠️ Security Note**: Never commit API keys or secrets to version control. Always use secure configuration management (user secrets, environment variables, or secure vaults) for sensitive credentials.

**🎯 Ready to modernize your SMS integration?** This SDK provides everything you need for reliable, scalable, and maintainable SMS functionality in your .NET applications.



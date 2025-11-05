using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Simple Example: Send SMS with Modern SDK
Console.WriteLine("Sending SMS with Modern SMS API SDK");
Console.WriteLine("=====================================");

// Configuración desde appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

// Configurar DI
var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => services.AddSmsApi(configuration))
    .Build();

// Obtener el servicio SMS
var smsApi = host.Services.GetRequiredService<ISmsApi>();

try
{
    // CHANGE THESE VALUES IF YOU WANT:
    string phoneNumber = "50212345678";  // Your test number
    string message = $"Modern .NET SMS SDK working perfectly! {DateTime.Now:HH:mm:ss}";

    Console.WriteLine($"Sending message to: {phoneNumber}");
    Console.WriteLine($"Message: {message}");
    Console.WriteLine("Sending...\n");

    // ASYNC METHOD (Recommended)
    var result = await smsApi.Messages.SendToContactAsync(phoneNumber, message);

    if (result.IsOk)
    {
        Console.WriteLine("MESSAGE SENT SUCCESSFULLY!");
        Console.WriteLine($"Message ID: {result.Data?.MessageId}");
        Console.WriteLine($"Destination: {result.Data?.Msisdn}");
        Console.WriteLine($"Message sent: {result.Data?.Message}");
        Console.WriteLine($"HTTP Status: {result.HttpCode}");
        Console.WriteLine($"Sent at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }
    else
    {
        Console.WriteLine("ERROR SENDING MESSAGE:");
        Console.WriteLine($"Error code: {result.ErrorCode}");
        Console.WriteLine($"Description: {result.ErrorDescription}");
        Console.WriteLine($"HTTP Status: {result.HttpCode}");
    }

    // Also test sync method for compatibility
    Console.WriteLine("\nTesting synchronous method...");
    var syncResult = smsApi.Messages.SendToContact(phoneNumber, $"Sync method also works! {DateTime.Now:HH:mm:ss}");
    
    if (syncResult.IsOk)
    {
        Console.WriteLine("Synchronous method also worked!");
        Console.WriteLine($"Sync Message ID: {syncResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"Sync method failed: {syncResult.ErrorDescription}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"EXCEPTION: {ex.Message}");
    Console.WriteLine($"Details: {ex.StackTrace}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey(); 
using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ModernSmsApiExample;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Modern SMS API SDK Example");
        Console.WriteLine("============================");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>() // For secure API keys in development
            .AddEnvironmentVariables()
            .Build();

        // Build host with dependency injection
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register our SMS API services
                services.AddSmsApi(configuration);
                
                // Register our example service
                services.AddScoped<SmsApiExampleService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

        try
        {
            // Get our example service from DI container
            var exampleService = host.Services.GetRequiredService<SmsApiExampleService>();
            
            // Run examples
            await exampleService.RunAllExamplesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

/// <summary>
/// Service that demonstrates various SMS API usage patterns
/// </summary>
public class SmsApiExampleService
{
    private readonly ISmsApi _smsApi;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsApiExampleService> _logger;

    public SmsApiExampleService(ISmsApi smsApi, IConfiguration configuration, ILogger<SmsApiExampleService> logger)
    {
        _smsApi = smsApi;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunAllExamplesAsync()
    {
        Console.WriteLine("\n📋 Running SMS API Examples...\n");

        // Check configuration
        if (!ValidateConfiguration())
        {
            Console.WriteLine("⚠️  Please update appsettings.json with valid API credentials");
            return;
        }

        // Test SMS sending first
        await TestSendSmsAsync();
        
        // Run async examples
        await TestAsyncMethodsAsync();
        
        // Run sync examples (for backward compatibility)
        TestSyncMethods();
        
        // Test error handling
        await TestErrorHandlingAsync();
    }

    private bool ValidateConfiguration()
    {
        var apiKey = _configuration["SmsApi:ApiKey"];
        var secretKey = _configuration["SmsApi:SecretKey"];
        var apiUrl = _configuration["SmsApi:ApiUrl"];

        if (string.IsNullOrEmpty(apiKey) || apiKey == "your-api-key-here" ||
            string.IsNullOrEmpty(secretKey) || secretKey == "your-secret-key-here" ||
            string.IsNullOrEmpty(apiUrl) || apiUrl == "https://your-api-url.com/api/")
        {
            return false;
        }

        Console.WriteLine("✅ Configuration validated");
        return true;
    }

    private async Task TestAsyncMethodsAsync()
    {
        Console.WriteLine("🔄 Testing Async Methods");
        Console.WriteLine("-----------------------");

        try
        {
            // Test getting message list (async)
            _logger.LogInformation("Testing GetListAsync...");
            var messageList = await _smsApi.Messages.GetListAsync(
                startDate: DateTime.Now.AddDays(-7),
                endDate: DateTime.Now,
                limit: 10
            );

            if (messageList.IsOk)
            {
                Console.WriteLine($"✅ GetListAsync: Found {messageList.Data?.Count ?? 0} messages");
                
                // Display first few messages
                if (messageList.Data?.Count > 0)
                {
                    foreach (var msg in messageList.Data.Take(3))
                    {
                        Console.WriteLine($"   📨 Message {msg.MessageId}: {msg.Message?[..Math.Min(50, msg.Message.Length)]}...");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ GetListAsync failed: {messageList.ErrorDescription}");
            }

            // Test scheduled messages
            _logger.LogInformation("Testing GetScheduleAsync...");
            var scheduledMessages = await _smsApi.Messages.GetScheduleAsync();
            
            if (scheduledMessages.IsOk)
            {
                Console.WriteLine($"✅ GetScheduleAsync: Found {scheduledMessages.Data?.Count ?? 0} scheduled messages");
            }
            else
            {
                Console.WriteLine($"❌ GetScheduleAsync failed: {scheduledMessages.ErrorDescription}");
            }

            // Test inbox messages
            _logger.LogInformation("Testing GetInboxAsync...");
            var inboxMessages = await _smsApi.Messages.GetInboxAsync(limit: 5);
            
            if (inboxMessages.IsOk)
            {
                Console.WriteLine($"✅ GetInboxAsync: Found {inboxMessages.Data?.Count ?? 0} inbox messages");
            }
            else
            {
                Console.WriteLine($"❌ GetInboxAsync failed: {inboxMessages.ErrorDescription}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Async test failed: {ex.Message}");
            _logger.LogError(ex, "Error in async tests");
        }

        Console.WriteLine();
    }

    private void TestSyncMethods()
    {
        Console.WriteLine("🔄 Testing Sync Methods (Backward Compatibility)");
        Console.WriteLine("-----------------------------------------------");

        try
        {
            // Test sync version for backward compatibility
            _logger.LogInformation("Testing GetList (sync)...");
            var messageList = _smsApi.Messages.GetList(limit: 5);

            if (messageList.IsOk)
            {
                Console.WriteLine($"✅ GetList (sync): Found {messageList.Data?.Count ?? 0} messages");
            }
            else
            {
                Console.WriteLine($"❌ GetList (sync) failed: {messageList.ErrorDescription}");
            }

            // Test scheduled messages sync
            var scheduledMessages = _smsApi.Messages.GetSchedule();
            if (scheduledMessages.IsOk)
            {
                Console.WriteLine($"✅ GetSchedule (sync): Found {scheduledMessages.Data?.Count ?? 0} scheduled messages");
            }
            else
            {
                Console.WriteLine($"❌ GetSchedule (sync) failed: {scheduledMessages.ErrorDescription}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Sync test failed: {ex.Message}");
            _logger.LogError(ex, "Error in sync tests");
        }

        Console.WriteLine();
    }

    private async Task TestErrorHandlingAsync()
    {
        Console.WriteLine("🔄 Testing Error Handling");
        Console.WriteLine("-------------------------");

        try
        {
            // Test with invalid parameters to see error handling
            var invalidResult = await _smsApi.Messages.GetListAsync(
                startDate: DateTime.Now.AddDays(1), // Future date should cause error
                endDate: DateTime.Now.AddDays(-1)   // End before start
            );

            if (!invalidResult.IsOk)
            {
                Console.WriteLine($"✅ Error handling works: {invalidResult.ErrorDescription}");
            }
            else
            {
                Console.WriteLine("⚠️  Expected error but got success - API might be very permissive");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✅ Exception handling works: {ex.Message}");
        }

        Console.WriteLine();
    }

    private async Task TestSendSmsAsync()
    {
        Console.WriteLine("📱 Testing SMS Sending");
        Console.WriteLine("======================");

        try
        {
            // Get phone number from configuration
            var testPhoneNumber = _configuration["TestData:TestPhoneNumber"];
            var testMessage = _configuration["TestData:TestMessage"] ?? "Hello from Modern SMS API SDK!";

            if (string.IsNullOrEmpty(testPhoneNumber) || testPhoneNumber == "PUT_YOUR_TEST_PHONE_NUMBER_HERE")
            {
                Console.WriteLine("⚠️  Please configure TestData:TestPhoneNumber in appsettings.json");
                Console.WriteLine("   Example: \"TestPhoneNumber\": \"50245858369\"");
                Console.WriteLine();
                return;
            }

            Console.WriteLine($"📤 Sending SMS to: {testPhoneNumber}");
            Console.WriteLine($"💬 Message: {testMessage}");
            Console.WriteLine("⏳ Sending...\n");

            // 🚀 ASYNC SMS SENDING
            _logger.LogInformation("Testing SendToContactAsync...");
            var result = await _smsApi.Messages.SendToContactAsync(
                msisdn: testPhoneNumber,
                message: testMessage,
                messageId: $"test-{DateTime.Now:yyyyMMdd-HHmmss}"
            );

            if (result.IsOk)
            {
                Console.WriteLine("✅ ¡SMS ENVIADO EXITOSAMENTE!");
                Console.WriteLine($"📧 Message ID: {result.Data?.MessageId}");
                Console.WriteLine($"📱 Destination: {result.Data?.Msisdn}");
                Console.WriteLine($"💬 Message: {result.Data?.Message}");
                Console.WriteLine($"📊 HTTP Status: {result.HttpCode}");
                Console.WriteLine($"🕐 Sent at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                Console.WriteLine("❌ ERROR SENDING SMS:");
                Console.WriteLine($"🔴 Error Code: {result.ErrorCode}");
                Console.WriteLine($"📝 Description: {result.ErrorDescription}");
                Console.WriteLine($"🌐 HTTP Status: {result.HttpCode}");
            }

            // Test sync version too
            Console.WriteLine("\n🔄 Testing sync version...");
            var syncResult = _smsApi.Messages.SendToContact(
                testPhoneNumber, 
                "Sync SMS test from Modern SDK"
            );

            if (syncResult.IsOk)
            {
                Console.WriteLine("✅ Sync SMS also sent successfully!");
                Console.WriteLine($"📧 Sync Message ID: {syncResult.Data?.MessageId}");
            }
            else
            {
                Console.WriteLine($"❌ Sync SMS failed: {syncResult.ErrorDescription}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SMS sending failed: {ex.Message}");
            _logger.LogError(ex, "Error sending SMS");
        }

        Console.WriteLine();
    }
} 
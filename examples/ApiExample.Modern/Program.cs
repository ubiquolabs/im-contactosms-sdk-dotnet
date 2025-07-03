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

    // Test data - update these values for testing
    private readonly string _testGroupName = "TestGroup";
    private readonly string _testPhoneNumber = "+50212345678";
    private readonly string _testFirstName = "John";
    private readonly string _testLastName = "Doe";

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
            Console.WriteLine("   Update the TestData section with your test values");
            return;
        }

        // Test SMS sending first
        await TestSendSmsAsync();
        
        // Test Groups functionality
        await TestGroupsAsync();
        
        // Test Contacts functionality
        await TestContactsAsync();
        
        // Test Messages functionality
        await TestMessagesAsync();
        
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

    #region Groups Examples

    private async Task TestGroupsAsync()
    {
        Console.WriteLine("\n👥 Testing Groups Functionality");
        Console.WriteLine("==============================");

        try
        {
            // Get group list
            await GetGroupListAsync();
            
            // Add new group
            await AddGroupAsync();
            
            // Get specific group
            await GetGroupAsync();
            
            // Add contact to group
            await AddContactToGroupAsync();
            
            // Get contacts in group
            await GetContactListByGroupAsync();
            
            // Remove contact from group
            await RemoveContactFromGroupAsync();
            
            // Update group
            await UpdateGroupAsync();
            
            // Delete group (commented out to avoid deleting test data)
            // await DeleteGroupAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Groups test failed: {ex.Message}");
            _logger.LogError(ex, "Error in groups tests");
        }
    }

    private async Task GetGroupListAsync()
    {
        Console.WriteLine("\n📋 Getting Group List...");
        
        var groups = await _smsApi.Groups.GetListAsync();
        
        if (groups.IsOk)
        {
            Console.WriteLine($"✅ Found {groups.Data?.Count ?? 0} groups:");
            if (groups.Data?.Count > 0)
            {
                foreach (var group in groups.Data)
                {
                    Console.WriteLine($"   📁 {group.Name} (Short: {group.ShortName})");
                }
            }
        }
        else
        {
            Console.WriteLine($"❌ Error: {groups.ErrorDescription}");
        }
    }

    private async Task AddGroupAsync()
    {
        Console.WriteLine($"\n➕ Adding Group: {_testGroupName}...");
        
        var result = await _smsApi.Groups.AddAsync(_testGroupName, "Test Group", "Group for testing purposes");
        
        if (result.IsOk)
        {
            Console.WriteLine($"✅ Group '{_testGroupName}' added successfully");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.ErrorDescription}");
        }
    }

    private async Task GetGroupAsync()
    {
        Console.WriteLine($"\n🔍 Getting Group: {_testGroupName}...");
        
        var group = await _smsApi.Groups.GetAsync(_testGroupName);
        
        if (group.IsOk)
        {
            Console.WriteLine($"✅ Group found:");
            Console.WriteLine($"   Name: {group.Data?.Name}");
            Console.WriteLine($"   Short Name: {group.Data?.ShortName}");
            Console.WriteLine($"   Description: {group.Data?.Description}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {group.ErrorDescription}");
        }
    }

    private async Task AddContactToGroupAsync()
    {
        Console.WriteLine($"\n➕ Adding Contact {_testPhoneNumber} to Group {_testGroupName}...");
        
        var result = await _smsApi.Groups.AddContactAsync(_testGroupName, _testPhoneNumber);
        
        if (result.IsOk)
        {
            Console.WriteLine($"✅ Contact added to group successfully");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.ErrorDescription}");
        }
    }

    private async Task GetContactListByGroupAsync()
    {
        Console.WriteLine($"\n📋 Getting Contacts in Group: {_testGroupName}...");
        
        var contacts = await _smsApi.Groups.GetContactListAsync(_testGroupName);
        
        if (contacts.IsOk)
        {
            Console.WriteLine($"✅ Found {contacts.Data?.Count ?? 0} contacts in group:");
            if (contacts.Data?.Count > 0)
            {
                foreach (var contact in contacts.Data)
                {
                    Console.WriteLine($"   👤 {contact.FirstName} {contact.LastName} - {contact.PhoneNumber}");
                }
            }
        }
        else
        {
            Console.WriteLine($"❌ Error: {contacts.ErrorDescription}");
        }
    }

    private async Task RemoveContactFromGroupAsync()
    {
        Console.WriteLine($"\n➖ Removing Contact {_testPhoneNumber} from Group {_testGroupName}...");
        
        var result = await _smsApi.Groups.RemoveContactAsync(_testGroupName, _testPhoneNumber);
        
        if (result.IsOk)
        {
            Console.WriteLine($"✅ Contact removed from group successfully");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.ErrorDescription}");
        }
    }

    private async Task UpdateGroupAsync()
    {
        Console.WriteLine($"\n✏️  Updating Group: {_testGroupName}...");
        
        var result = await _smsApi.Groups.UpdateAsync(_testGroupName, "Updated Test Group", "Updated group description");
        
        if (result.IsOk)
        {
            Console.WriteLine($"✅ Group updated successfully");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.ErrorDescription}");
        }
    }

    private async Task DeleteGroupAsync()
    {
        Console.WriteLine($"\n🗑️  Deleting Group: {_testGroupName}...");
        
        var result = await _smsApi.Groups.DeleteAsync(_testGroupName);
        
        if (result.IsOk)
        {
            Console.WriteLine($"✅ Group deleted successfully");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.ErrorDescription}");
        }
    }

    #endregion

    #region Contacts Examples

    private async Task TestContactsAsync()
    {
        Console.WriteLine("\n👤 Testing Contacts Functionality");
        Console.WriteLine("================================");

        try
        {
            // Create new contact
            await CreateNewContactAsync();
            
            // Get contact by phone number
            await GetContactByPhoneNumberAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Contacts test failed: {ex.Message}");
            _logger.LogError(ex, "Error in contacts tests");
        }
    }

    private async Task CreateNewContactAsync()
    {
        Console.WriteLine($"\n➕ Creating Contact: {_testFirstName} {_testLastName} ({_testPhoneNumber})...");
        
        var contact = await _smsApi.Contacts.AddAsync("502", _testPhoneNumber, _testFirstName, _testLastName);
        
        if (contact.IsOk)
        {
            Console.WriteLine($"✅ Contact created successfully:");
            Console.WriteLine($"   Phone: {contact.Data?.PhoneNumber}");
            Console.WriteLine($"   Name: {contact.Data?.FirstName} {contact.Data?.LastName}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {contact.ErrorDescription}");
        }
    }

    private async Task GetContactByPhoneNumberAsync()
    {
        Console.WriteLine($"\n🔍 Getting Contact: {_testPhoneNumber}...");
        
        var contact = await _smsApi.Contacts.GetByPhoneNumberAsync(_testPhoneNumber);
        
        if (contact.IsOk)
        {
            Console.WriteLine($"✅ Contact found:");
            Console.WriteLine($"   Phone: {contact.Data?.PhoneNumber}");
            Console.WriteLine($"   Name: {contact.Data?.FirstName} {contact.Data?.LastName}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {contact.ErrorDescription}");
        }
    }

    #endregion

    #region Messages Examples

    private async Task TestMessagesAsync()
    {
        Console.WriteLine("\n📨 Testing Messages Functionality");
        Console.WriteLine("=================================");

        try
        {
            // Send message to contact
            await SendMessageToContactAsync();
            
            // Send message to group
            await SendMessageToGroupAsync();
            
            // Get message log
            await GetMessageLogAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Messages test failed: {ex.Message}");
            _logger.LogError(ex, "Error in messages tests");
        }
    }

    private async Task SendMessageToContactAsync()
    {
        var messageId = Guid.NewGuid().ToString("N")[..8];
        var message = "Hello from Modern SMS API SDK!";
        
        Console.WriteLine($"\n📤 Sending Message to Contact: {_testPhoneNumber}...");
        Console.WriteLine($"   Message: {message}");
        Console.WriteLine($"   ID: {messageId}");
        
        var response = await _smsApi.Messages.SendToContactAsync(_testPhoneNumber, message, messageId);
        
        if (response.IsOk)
        {
            Console.WriteLine($"✅ Message sent successfully:");
            Console.WriteLine($"   Sent Count: {response.Data?.SentCount}");
            Console.WriteLine($"   Message: {response.Data?.Message}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {response.ErrorDescription}");
        }
    }

    private async Task SendMessageToGroupAsync()
    {
        var messageId = Guid.NewGuid().ToString("N")[..8];
        var message = "Hello Group from Modern SMS API SDK!";
        
        Console.WriteLine($"\n📤 Sending Message to Group: {_testGroupName}...");
        Console.WriteLine($"   Message: {message}");
        Console.WriteLine($"   ID: {messageId}");
        
        var response = await _smsApi.Messages.SendToGroupsAsync(new[] { _testGroupName }, message, messageId);
        
        if (response.IsOk)
        {
            Console.WriteLine($"✅ Message sent successfully:");
            Console.WriteLine($"   Sent Count: {response.Data?.SentCount}");
            Console.WriteLine($"   Message: {response.Data?.Message}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {response.ErrorDescription}");
        }
    }

    private async Task GetMessageLogAsync()
    {
        Console.WriteLine($"\n📋 Getting Message Log (last 5 days)...");
        
        var response = await _smsApi.Messages.GetListAsync(
            startDate: DateTime.Today.AddDays(-5),
            endDate: DateTime.Today,
            direction: MessageDirection.MT,
            limit: 10
        );
        
        if (response.IsOk)
        {
            Console.WriteLine($"✅ Found {response.Data?.Count ?? 0} messages:");
            if (response.Data?.Count > 0)
            {
                foreach (var msg in response.Data.Take(3))
                {
                    Console.WriteLine($"   📨 {msg.MessageId}: {msg.Message?[..Math.Min(50, msg.Message.Length)]}...");
                }
            }
        }
        else
        {
            Console.WriteLine($"❌ Error: {response.ErrorDescription}");
        }
    }

    #endregion

    #region SMS Sending Test

    private async Task TestSendSmsAsync()
    {
        Console.WriteLine("\n📱 Testing SMS Sending");
        Console.WriteLine("======================");

        try
        {
            var testPhone = _configuration["TestData:TestPhoneNumber"] ?? _testPhoneNumber;
            var testMessage = _configuration["TestData:TestMessage"] ?? "Hello from Modern SMS API SDK!";
            var messageId = Guid.NewGuid().ToString("N")[..8];

            Console.WriteLine($"📤 Sending test SMS to: {testPhone}");
            Console.WriteLine($"   Message: {testMessage}");
            Console.WriteLine($"   ID: {messageId}");

            var response = await _smsApi.Messages.SendToContactAsync(testPhone, testMessage, messageId);

            if (response.IsOk)
            {
                Console.WriteLine($"✅ SMS sent successfully!");
                Console.WriteLine($"   Sent Count: {response.Data?.SentCount}");
                Console.WriteLine($"   Message ID: {response.Data?.MessageId}");
            }
            else
            {
                Console.WriteLine($"❌ SMS sending failed: {response.ErrorDescription}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SMS test failed: {ex.Message}");
            _logger.LogError(ex, "Error in SMS test");
        }
    }

    #endregion

    #region Error Handling

    private async Task TestErrorHandlingAsync()
    {
        Console.WriteLine("\n⚠️  Testing Error Handling");
        Console.WriteLine("=========================");

        try
        {
            // Test with invalid phone number
            Console.WriteLine("\n🔍 Testing with invalid phone number...");
            var invalidResponse = await _smsApi.Messages.SendToContactAsync("invalid-phone", "Test message", "test-id");
            
            if (!invalidResponse.IsOk)
            {
                Console.WriteLine($"✅ Error handling works: {invalidResponse.ErrorDescription}");
            }

            // Test with invalid group
            Console.WriteLine("\n🔍 Testing with invalid group...");
            var invalidGroupResponse = await _smsApi.Groups.GetAsync("non-existent-group");
            
            if (!invalidGroupResponse.IsOk)
            {
                Console.WriteLine($"✅ Error handling works: {invalidGroupResponse.ErrorDescription}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error handling test failed: {ex.Message}");
            _logger.LogError(ex, "Error in error handling test");
        }
    }

    #endregion
} 
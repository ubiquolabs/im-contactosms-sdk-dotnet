using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("SMS API .NET SDK - Quick Test");
Console.WriteLine("=================================\n");

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>() // For secure API keys
    .AddEnvironmentVariables()
    .Build();

// Build host with dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSmsApi(configuration);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

try
{
    var smsApi = host.Services.GetRequiredService<ISmsApi>();
    
    // Validate configuration
    if (!ValidateConfiguration(configuration))
    {
        Console.WriteLine("Invalid configuration. Please update appsettings.json");
        Console.WriteLine("   or use: dotnet user-secrets set \"SmsApi:ApiKey\" \"your-api-key\"");
        return;
    }

    // Get test data from configuration with UTF-8 examples
    var testPhone = configuration["TestData:TestPhoneNumber"] ?? "50212345678";
    var testMessage = configuration["TestData:TestMessage"] ?? "¡Hola desde .NET SDK! ¿Te llegó el mensaje?";
    var testTagName = configuration["TestData:TestTagName"] ?? "TestTag";
    var testFirstName = configuration["TestData:TestContactFirstName"] ?? "Juan";
    var testLastName = configuration["TestData:TestContactLastName"] ?? "Pérez";

    Console.WriteLine("Test Data:");
    Console.WriteLine($"   Phone: {testPhone}");
    Console.WriteLine($"   Message: {testMessage}");
    Console.WriteLine($"   Tag: {testTagName}\n");

    // === 1. TEST MESSAGES ===
    Console.WriteLine("1. TESTING MESSAGE SENDING");
    Console.WriteLine("─────────────────────────────────");
    
    // Send to contact
    Console.Write("   Sending to contact... ");
    var messageResult = await smsApi.Messages.SendToContactAsync(testPhone, testMessage);
    if (messageResult.IsOk)
    {
        Console.WriteLine("Sent!");
        Console.WriteLine($"      ID: {messageResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"Error: {messageResult.ErrorDescription}");
    }

    // Send to tags
    Console.Write("   Sending to tags... ");
    var tagsResult = await smsApi.Messages.SendToTagsAsync(new[] { testTagName }, testMessage);
    if (tagsResult.IsOk)
    {
        Console.WriteLine("Sent!");
        Console.WriteLine($"      ID: {tagsResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"Error: {tagsResult.ErrorDescription}");
    }

    // === 2. TEST CONTACTS ===
    Console.WriteLine("\n2. TESTING CONTACT MANAGEMENT");
    Console.WriteLine("──────────────────────────────────");
    
    // Add contact
    Console.Write("   Adding contact... ");
    var contactResult = await smsApi.Contacts.AddAsync("502", testPhone, testFirstName, testLastName);
    if (contactResult.IsOk)
    {
        Console.WriteLine("Added!");
        Console.WriteLine($"      Name: {contactResult.Data?.FirstName} {contactResult.Data?.LastName}");
    }
    else
    {
        Console.WriteLine($"Error: {contactResult.ErrorDescription}");
    }

    // Get contact
    Console.Write("   Getting contact... ");
    var getContactResult = await smsApi.Contacts.GetByMsisdnAsync(testPhone);
    if (getContactResult.IsOk)
    {
        Console.WriteLine("Found!");
        Console.WriteLine($"      Status: {getContactResult.Data?.Status}");
    }
    else
    {
        Console.WriteLine($"Error: {getContactResult.ErrorDescription}");
    }

    // Add tag to contact
    Console.Write("   Adding tag to contact... ");
    var addTagResult = await smsApi.Contacts.AddTagAsync(testPhone, testTagName);
    if (addTagResult.IsOk)
    {
        Console.WriteLine("Tag added!");
    }
    else
    {
        Console.WriteLine($"Error: {addTagResult.ErrorDescription}");
    }

    // === 3. TEST TAGS ===
    Console.WriteLine("\n3. TESTING TAG MANAGEMENT");
    Console.WriteLine("───────────────────────────────");
    
    // Get tags list
    Console.Write("   Listing tags... ");
    var tagsListResult = await smsApi.Tags.GetListAsync();
    if (tagsListResult.IsOk)
    {
        Console.WriteLine($"Found {tagsListResult.Data?.Count ?? 0} tags");
        if (tagsListResult.Data?.Count > 0)
        {
            foreach (var tag in tagsListResult.Data.Take(3))
            {
                Console.WriteLine($"      - {tag.TagName}");
            }
        }
    }
    else
    {
        Console.WriteLine($"Error: {tagsListResult.ErrorDescription}");
    }

    // Get contacts in tag
    Console.Write($"   Contacts in tag '{testTagName}'... ");
    var tagContactsResult = await smsApi.Tags.GetContactListAsync(testTagName);
    if (tagContactsResult.IsOk)
    {
        Console.WriteLine($"{tagContactsResult.Data?.Count ?? 0} contacts found");
    }
    else
    {
        Console.WriteLine($"Error: {tagContactsResult.ErrorDescription}");
    }

    // === 4. TEST MESSAGE RETRIEVAL ===
    Console.WriteLine("\n4. TESTING MESSAGE QUERY");
    Console.WriteLine("──────────────────────────────────");
    
    Console.Write("   Querying messages... ");
    // Use exact same dates as JavaScript that works
    var startDate = new DateTime(2025, 7, 1);   // "2025-07-01"
    var endDate = new DateTime(2025, 7, 4);     // "2025-07-04" 
    var messagesResult = await smsApi.Messages.GetListAsync(
        startDate: startDate, 
        endDate: endDate, 
        limit: 10);  // Mismo límite que JavaScript
    
    if (messagesResult.IsOk)
    {
        Console.WriteLine($"{messagesResult.Data?.Count ?? 0} messages found");
        if (messagesResult.Data?.Count > 0)
        {
            foreach (var msg in messagesResult.Data)
            {
                Console.WriteLine("   ---------------------------------");
                Console.WriteLine($"     ID: {msg.MessageId}");
                Console.WriteLine($"     Status: {msg.MessageStatus}");
                Console.WriteLine($"     To: {msg.Msisdn}");
                Console.WriteLine($"     From: {msg.SentFrom}");
                Console.WriteLine($"     Message: {msg.Message}");
                Console.WriteLine($"     Created: {msg.CreatedOn}");
                Console.WriteLine("   ---------------------------------");
            }
        }
    }
    else
    {
        Console.WriteLine($"Error: {messagesResult.ErrorDescription}");
    }

    // === 5. UTF-8 COMPATIBILITY TEST ===
    Console.WriteLine("\n5. TESTING UTF-8 COMPATIBILITY");
    Console.WriteLine("──────────────────────────────────");
    
    Console.Write("   Sending message with extended characters... ");
    var utf8Message = "Acentos: áéíóú ÁÉÍÓÚ ñÑ.";
    var utf8Result = await smsApi.Messages.SendToContactAsync(testPhone, utf8Message, "utf8-test");
    if (utf8Result.IsOk)
    {
        Console.WriteLine("Sent!");
        Console.WriteLine($"      UTF-8 Message: {utf8Message}");
        Console.WriteLine($"      ID: {utf8Result.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"Error: {utf8Result.ErrorDescription}");
    }

    // === 6. TEST SHORTLINKS ===
    Console.WriteLine("\n6. TESTING SHORTLINK MANAGEMENT");
    Console.WriteLine("──────────────────────────────────");
    
    // Create shortlink
    Console.Write("   Creating shortlink... ");
    var createShortlinkResult = await smsApi.Shortlinks.CreateAsync(
        longUrl: "https://www.example.com/very-long-url-with-parameters",
        name: "Test Shortlink",
        status: ShortlinkStatus.ACTIVE
    );
    if (createShortlinkResult.IsOk && createShortlinkResult.Data != null)
    {
        Console.WriteLine("Created!");
        Console.WriteLine($"      ID: {createShortlinkResult.Data.Id}");
        Console.WriteLine($"      Short URL: {createShortlinkResult.Data.ShortUrl}");
        Console.WriteLine($"      Long URL: {createShortlinkResult.Data.LongUrl}");
        
        var shortlinkId = createShortlinkResult.Data.Id;
        
        // Get by ID
        Console.Write($"   Getting shortlink by ID... ");
        var getShortlinkResult = await smsApi.Shortlinks.GetByIdAsync(shortlinkId);
        if (getShortlinkResult.IsOk)
        {
            Console.WriteLine("Found!");
            Console.WriteLine($"      Status: {getShortlinkResult.Data?.Status}");
        }
        else
        {
            Console.WriteLine($"Error: {getShortlinkResult.ErrorDescription}");
        }
        
        // Update status
        Console.Write($"   Updating status to INACTIVE... ");
        var updateStatusResult = await smsApi.Shortlinks.UpdateStatusAsync(shortlinkId, ShortlinkStatus.INACTIVE);
        if (updateStatusResult.IsOk)
        {
            Console.WriteLine("Updated!");
        }
        else
        {
            Console.WriteLine($"Error: {updateStatusResult.ErrorDescription}");
        }
    }
    else
    {
        Console.WriteLine($"Error: {createShortlinkResult.ErrorDescription}");
    }
    
    // List shortlinks
    Console.Write("   Listing shortlinks... ");
    var listShortlinksResult = await smsApi.Shortlinks.GetListAsync(limit: 10);
    if (listShortlinksResult.IsOk)
    {
        Console.WriteLine($"{listShortlinksResult.Data?.Count ?? 0} shortlinks found");
        if (listShortlinksResult.Data?.Count > 0)
        {
            foreach (var sl in listShortlinksResult.Data.Take(3))
            {
                Console.WriteLine($"      - {sl.Id}: {sl.ShortUrl}");
            }
        }
    }
    else
    {
        Console.WriteLine($"Error: {listShortlinksResult.ErrorDescription}");
    }

    Console.WriteLine("\nTESTS COMPLETED!");
    Console.WriteLine("========================");
    Console.WriteLine("If you see messages sent, the SDK is working correctly!");
    Console.WriteLine("UTF-8 characters (¡¿áéíóú) should be sent perfectly");
    Console.WriteLine("If there are errors, check the API configuration in appsettings.json");
}
catch (Exception ex)
{
    Console.WriteLine($"\nGENERAL ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Detail: {ex.InnerException.Message}");
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

static bool ValidateConfiguration(IConfiguration configuration)
{
    var apiKey = configuration["SmsApi:ApiKey"];
    var secretKey = configuration["SmsApi:SecretKey"];
    var apiUrl = configuration["SmsApi:ApiUrl"];

    return !string.IsNullOrEmpty(apiKey) && apiKey != "your-api-key-here" &&
           !string.IsNullOrEmpty(secretKey) && secretKey != "your-secret-key-here" &&
           !string.IsNullOrEmpty(apiUrl) && apiUrl != "https://your-api-url.com/api/";
} 
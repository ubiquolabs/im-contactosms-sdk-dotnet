using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

var argsList = args.ToList();
var command = argsList.Count > 0 ? argsList[0] : null;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var testPhoneNumber = configuration["TestData:TestPhoneNumber"] ?? "50212345678";
var testMessageText = configuration["TestData:TestMessage"] ?? "Hola desde .NET SDK";
var testTagName = configuration["TestData:TestTagName"] ?? "TestTag";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSmsApi(configuration);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Warning);
    })
    .Build();

var smsApi = host.Services.GetRequiredService<ISmsApi>();

if (!ValidateConfiguration(configuration))
{
    Console.WriteLine("Invalid configuration. Please update appsettings.json or use:");
    Console.WriteLine("  dotnet user-secrets set \"SmsApi:ApiKey\" \"your-api-key\"");
    Console.WriteLine("  dotnet user-secrets set \"SmsApi:SecretKey\" \"your-secret-key\"");
    Console.WriteLine("  dotnet user-secrets set \"SmsApi:ApiUrl\" \"your-api-url\"");
    return;
}

Console.WriteLine("CONTACTO SMS SDK CLI");
Console.WriteLine("====================\n");

switch (command?.ToLower())
{
    case "accounts":
        await TestAccountStatus(smsApi);
        break;

    case "contacts":
        var contactLimit = argsList.Count > 1 ? int.Parse(argsList[1]) : 20;
        var query = argsList.Count > 2 ? argsList[2] : string.Empty;
        await TestListContacts(smsApi, contactLimit, query);
        break;

    case "contact":
        if (argsList.Count < 2)
        {
            Console.WriteLine("MSISDN is required");
            PrintUsage();
            return;
        }
        await TestGetContact(smsApi, argsList[1]);
        break;

    case "messages":
        var messageStart = argsList.Count > 1 ? argsList[1] : null;
        var messageEnd = argsList.Count > 2 ? argsList[2] : null;
        var messageLimit = argsList.Count > 3 ? int.Parse(argsList[3]) : 50;
        await TestListMessages(smsApi, messageStart, messageEnd, messageLimit);
        break;

    case "message":
        if (argsList.Count < 2)
        {
            Console.WriteLine("Message ID is required");
            PrintUsage();
            return;
        }
        await TestGetMessage(smsApi, argsList[1]);
        break;

    case "tags":
        await TestListTags(smsApi);
        break;

    case "tag":
        if (argsList.Count < 2)
        {
            Console.WriteLine("Tag name is required");
            PrintUsage();
            return;
        }
        await TestGetTagContacts(smsApi, argsList[1]);
        break;

    case "send":
        if (argsList.Count < 3)
        {
            Console.WriteLine("MSISDN and message are required");
            PrintUsage();
            return;
        }
        await TestSendMessageToContact(smsApi, argsList[1], string.Join(' ', argsList.Skip(2)));
        break;

    case "send-tag":
        if (argsList.Count < 3)
        {
            Console.WriteLine("Tag and message are required");
            PrintUsage();
            return;
        }
        await TestSendMessageToTag(smsApi, argsList[1], string.Join(' ', argsList.Skip(2)));
        break;

    case "create":
        var longUrl = argsList.Count > 1 ? argsList[1] : GetRandomLongUrl();
        var name = argsList.Count > 2 ? argsList[2] : GetRandomName();
        var alias = argsList.Count > 3 ? argsList[3] : null;
        var status = argsList.Count > 4 ? Enum.Parse<ShortlinkStatus>(argsList[4].ToUpper()) : ShortlinkStatus.ACTIVE;
        await TestCreateShortlink(smsApi, longUrl, name, alias, status);
        break;

    case "list":
        await TestListShortlinks(smsApi);
        break;

    case "date":
        var startDate = argsList.Count > 1 ? argsList[1] : null;
        var endDate = argsList.Count > 2 ? argsList[2] : null;
        var limit = argsList.Count > 3 ? int.Parse(argsList[3]) : 10;
        var offset = argsList.Count > 4 ? int.Parse(argsList[4]) : -6;
        await TestListShortlinksByDate(smsApi, startDate, endDate, limit, offset);
        break;

    case "id":
        if (argsList.Count < 2)
        {
            Console.WriteLine("Shortlink ID is required");
            PrintUsage();
            return;
        }
        await TestGetShortlinkById(smsApi, argsList[1]);
        break;

    case "update":
        if (argsList.Count < 3)
        {
            Console.WriteLine("ID and status are required");
            PrintUsage();
            return;
        }
        var updateStatus = Enum.Parse<ShortlinkStatus>(argsList[2].ToUpper());
        if (updateStatus == ShortlinkStatus.ACTIVE)
        {
            Console.WriteLine("Shortlinks cannot be reactivated; only INACTIVE is supported.");
            return;
        }
        await TestUpdateShortlinkStatus(smsApi, argsList[1], updateStatus);
        break;

    case "status":
        await TestStatusValidation(smsApi);
        break;

    case "multiple":
        var count = argsList.Count > 1 ? int.Parse(argsList[1]) : 5;
        await TestMultipleShortlinks(smsApi, count);
        break;

    case "--help":
    case "-h":
        PrintUsage();
        break;

    default:
        PrintUsage();
        await TestSendMessageToContact(smsApi, testPhoneNumber, testMessageText);
        if (!string.IsNullOrWhiteSpace(testTagName))
        {
            await TestSendMessageToTag(smsApi, testTagName, testMessageText);
        }
        await TestAccountStatus(smsApi);
        await TestListContacts(smsApi, 20, string.Empty);
        await TestListMessages(smsApi, null, null, 20);
        await TestListTags(smsApi);
        await TestListShortlinks(smsApi);
        await TestCreateShortlink(smsApi);
        break;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run                                  - Default flow (send message + full tour)");
    Console.WriteLine("  dotnet run -- accounts                        - Show account status/balance");
    Console.WriteLine("  dotnet run -- contacts [limit] [query]        - List contacts");
    Console.WriteLine("  dotnet run -- contact <msisdn>                - Get contact details");
    Console.WriteLine("  dotnet run -- messages [start] [end] [limit]  - List messages (defaults: last 7 days)");
    Console.WriteLine("  dotnet run -- message <messageId>             - Get message by ID");
    Console.WriteLine("  dotnet run -- tags                            - List tags");
    Console.WriteLine("  dotnet run -- tag <tagName>                   - Get contacts by tag");
    Console.WriteLine("  dotnet run -- send <msisdn> <message>         - Send SMS to contact");
    Console.WriteLine("  dotnet run -- send-tag <tag> <message>        - Send SMS to tag");
    Console.WriteLine("  dotnet run -- create [url] [name] [alias] [status]     - Create shortlink (status defaults to ACTIVE)");
    Console.WriteLine("  dotnet run -- list                             - List all shortlinks");
    Console.WriteLine("  dotnet run -- date [start] [end] [limit] [offset] - List shortlinks by date");
    Console.WriteLine("  dotnet run -- id <id>                          - Get shortlink by ID");
    Console.WriteLine("  dotnet run -- update <id> <status>             - Update shortlink status (only INACTIVE)");
    Console.WriteLine("  dotnet run -- status                           - Test shortlink status validation");
    Console.WriteLine("  dotnet run -- multiple [count]                 - Stress shortlink creation");
    Console.WriteLine("\nExamples:");
    Console.WriteLine("  dotnet run -- accounts");
    Console.WriteLine("  dotnet run -- contacts 20 Julio");
    Console.WriteLine("  dotnet run -- messages 2025-01-01 2025-01-07 50");
    Console.WriteLine("  dotnet run -- send 50212345678 \"Hola desde .NET\"");
    Console.WriteLine("  dotnet run -- create https://midominio.com \"Demo .NET\" promoAlias ACTIVE");
    Console.WriteLine("  dotnet run -- date 2025-01-01 2025-12-31 20 -6");
    Console.WriteLine("  dotnet run -- update 123ABC INACTIVE");
    Console.WriteLine();
}

static bool ValidateConfiguration(IConfiguration configuration)
{
    var apiKey = configuration["SmsApi:ApiKey"];
    var secretKey = configuration["SmsApi:SecretKey"];
    var apiUrl = configuration["SmsApi:ApiUrl"];

    return !string.IsNullOrEmpty(apiKey) && apiKey != "your-api-key-here" &&
           !string.IsNullOrEmpty(secretKey) && secretKey != "your-secret-key-here" &&
           !string.IsNullOrEmpty(apiUrl) && apiUrl != "https://your-api-url.com/api/rest/";
}

static string GetRandomLongUrl()
{
    var urls = new[]
    {
        "https://www.ejemplo.com/mi-pagina-muy-larga-con-parametros",
        "https://www.google.com/search?q=test+shortlink+api+local+development",
        "https://www.youtube.com/watch?v=dQw4w9WgXcQ&list=PLrAXtmRdnEQy6nuLMOVdJmC2_8q1uJ",
        "https://www.amazon.com/producto-super-largo-con-muchos-parametros-y-filtros",
        "https://www.facebook.com/groups/comunidad-desarrolladores-guatemala/posts/123456789",
        "https://www.github.com/usuario/repositorio-muy-largo/commits/abc123def456ghi789",
        "https://www.stackoverflow.com/questions/123456/como-crear-shortlinks-con-api-rest",
        "https://www.linkedin.com/in/perfil-profesional-desarrollador-guatemala",
        "https://www.twitter.com/usuario/status/1234567890123456789",
        "https://www.instagram.com/p/ABC123DEF456GHI789JKL/"
    };

    return urls[Random.Shared.Next(urls.Length)];
}

static string GetRandomName()
{
    var names = new[]
    {
        "Test Shortlink desde REST API",
        "Enlace corto de prueba",
        "Shortlink para testing",
        "Link de desarrollo local",
        "Prueba API Shortlink",
        "Test desde .NET SDK",
        "Shortlink automático",
        "Enlace de prueba local",
        "Test API REST",
        "Shortlink generado automáticamente"
    };

    return names[Random.Shared.Next(names.Length)];
}

static string GetRandomAlias()
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    return new string(Enumerable.Range(0, 8).Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
}

static async Task TestAccountStatus(ISmsApi smsApi)
{
    Console.WriteLine("TEST ACCOUNT STATUS");
    Console.WriteLine("===================\n");

    var result = await smsApi.Account.GetStatusAsync();
    if (result.IsOk && result.Data != null)
    {
        var account = result.Data;
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Name: {account.Name}");
        Console.WriteLine($"Short Name: {account.SmsShortName}");
        Console.WriteLine($"Subscription: {account.SmsSubscriptionType}");
        Console.WriteLine($"Opt-in Keyword: {account.SmsOptinKeyword}");
        Console.WriteLine($"Messages Limit: {account.MessagesLimit}");
        Console.WriteLine($"Messages Sent: {account.MessagesSent}");
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestListContacts(ISmsApi smsApi, int limit, string query)
{
    Console.WriteLine("TEST LIST CONTACTS");
    Console.WriteLine("==================\n");

    var result = await smsApi.Contacts.GetListAsync(
        contactStatuses: null,
        query: string.IsNullOrWhiteSpace(query) ? null : query,
        start: 0,
        limit: limit,
        shortResults: false);

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine($"Contacts found: {result.Data.Count}");
        foreach (var contact in result.Data.Take(10))
        {
            Console.WriteLine($"{contact.FullName} ({contact.Msisdn}) - Status: {contact.Status}");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestGetContact(ISmsApi smsApi, string msisdn)
{
    Console.WriteLine("TEST GET CONTACT");
    Console.WriteLine("================\n");

    var result = await smsApi.Contacts.GetByMsisdnAsync(msisdn);
    if (result.IsOk && result.Data != null)
    {
        var contact = result.Data;
        Console.WriteLine($"MSISDN: {contact.Msisdn}");
        Console.WriteLine($"Name: {contact.FullName}");
        Console.WriteLine($"Status: {contact.Status}");
        Console.WriteLine($"Email: {contact.Email}");
        Console.WriteLine($"Tags: {(contact.Tags.Count == 0 ? "(none)" : string.Join(", ", contact.Tags))}");
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestListMessages(ISmsApi smsApi, string? startDate, string? endDate, int limit)
{
    Console.WriteLine("TEST LIST MESSAGES");
    Console.WriteLine("==================\n");

    DateTime start = DateTime.UtcNow.AddDays(-7);
    DateTime end = DateTime.UtcNow;
    if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedStart))
    {
        start = parsedStart;
    }
    if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedEnd))
    {
        end = parsedEnd;
    }

    var result = await smsApi.Messages.GetListWithDeliveryStatusAsync(
        startDate: start,
        endDate: end,
        start: 0,
        limit: limit,
        msisdn: null,
        direction: MessageDirection.MT,
        deliveryStatusEnabled: true);

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine($"Messages found: {result.Data.Count}");
        foreach (var message in result.Data.Take(10))
        {
            Console.WriteLine($"Message {message.MessageId} - {message.MessageDirection} - Status: {message.MessageStatus} - {message.Message}");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestGetMessage(ISmsApi smsApi, string messageId)
{
    Console.WriteLine("TEST GET MESSAGE STATUS");
    Console.WriteLine("=======================\n");

    var result = await smsApi.Messages.GetMessageStatusAsync(messageId);
    if (result.IsOk && result.Data != null)
    {
        var message = result.Data;
        Console.WriteLine($"Message ID: {message.MessageId}");
        Console.WriteLine($"Status: {message.MessageStatus}");
        Console.WriteLine($"Direction: {message.MessageDirection}");
        Console.WriteLine($"MSISDN: {message.Msisdn}");
        Console.WriteLine($"Message: {message.Message}");
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestListTags(ISmsApi smsApi)
{
    Console.WriteLine("TEST LIST TAGS");
    Console.WriteLine("==============\n");

    var result = await smsApi.Tags.GetListAsync();
    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine($"Tags found: {result.Data.Count}");
        foreach (var tag in result.Data.Take(10))
        {
            Console.WriteLine($"{tag.TagName} - {tag.Description} (contacts: {tag.TotalContacts})");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestGetTagContacts(ISmsApi smsApi, string tagName)
{
    Console.WriteLine("TEST TAG CONTACTS");
    Console.WriteLine("=================\n");

    var result = await smsApi.Tags.GetContactListAsync(tagName, start: 0, limit: 20);
    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine($"Contacts under tag \"{tagName}\": {result.Data.Count}");
        foreach (var contact in result.Data.Take(10))
        {
            Console.WriteLine($"{contact.FullName} ({contact.Msisdn}) - Status: {contact.Status}");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestSendMessageToContact(ISmsApi smsApi, string msisdn, string message)
{
    Console.WriteLine("TEST SEND MESSAGE TO CONTACT");
    Console.WriteLine("============================\n");

    var result = await smsApi.Messages.SendToContactAsync(msisdn, message, Guid.NewGuid().ToString());
    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Message ID: {result.Data.MessageId}");
        Console.WriteLine($"Status: {result.Data.MessageStatus}");
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static async Task TestSendMessageToTag(ISmsApi smsApi, string tag, string message)
{
    Console.WriteLine("TEST SEND MESSAGE TO TAG");
    Console.WriteLine("=======================\n");

    var result = await smsApi.Messages.SendToTagsAsync(new[] { tag }, message, Guid.NewGuid().ToString());
    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Message ID: {result.Data.MessageId}");
        Console.WriteLine($"Status: {result.Data.MessageStatus}");
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
    }
    Console.WriteLine();
}

static string? NormalizeAlias(string? alias)
{
    if (alias == null)
    {
        return null;
    }

    var trimmed = alias.Trim();
    if (trimmed.Length == 0)
    {
        throw new ArgumentException("Alias cannot be empty");
    }

    if (trimmed.Length > 30)
    {
        throw new ArgumentException("Alias must be 30 characters or fewer");
    }

    if (trimmed.Any(char.IsWhiteSpace))
    {
        throw new ArgumentException("Alias cannot contain whitespace");
    }

    return trimmed;
}

static ShortlinkStatus GetRandomStatus()
{
    return Random.Shared.Next(2) == 0 ? ShortlinkStatus.ACTIVE : ShortlinkStatus.INACTIVE;
}

static async Task TestCreateShortlink(ISmsApi smsApi, string? longUrl = null, string? name = null, string? alias = null, ShortlinkStatus? status = null)
{
    Console.WriteLine("TEST CREATE SHORTLINK");
    Console.WriteLine("=====================\n");

    longUrl ??= GetRandomLongUrl();
    name ??= GetRandomName();
    status ??= ShortlinkStatus.ACTIVE;
    alias = NormalizeAlias(alias) ?? (Random.Shared.NextDouble() < 0.6 ? GetRandomAlias() : null);

    Console.WriteLine($"URL: {longUrl}");
    Console.WriteLine($"Name: {name}");
    Console.WriteLine($"Alias: {alias ?? "(auto)"}");
    Console.WriteLine($"Status: {status}");
    Console.WriteLine();
    var result = await smsApi.Shortlinks.CreateAsync(longUrl, name, status.Value, alias);

    if (result.IsOk && result.Data != null)
    {
        var shortlink = result.Data;
        Console.WriteLine("SUCCESS! Shortlink created:");
        Console.WriteLine($"  ID: {shortlink.UrlId ?? shortlink.Id}");
        Console.WriteLine($"  Name: {shortlink.Name}");
        Console.WriteLine($"  Alias: {shortlink.Alias ?? "(auto)"}");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
        Console.WriteLine();
    }
}

static async Task TestListShortlinks(ISmsApi smsApi)
{
    Console.WriteLine("TEST LIST SHORTLINKS");
    Console.WriteLine("====================\n");

    var result = await smsApi.Shortlinks.GetListAsync();

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Shortlinks found: {result.Data.Count}");
        Console.WriteLine();

        if (result.Data.Count > 0)
        {
            Console.WriteLine("SHORTLINKS LIST:");
            Console.WriteLine("================\n");

            for (int index = 0; index < result.Data.Count; index++)
            {
                var shortlink = result.Data[index];
                Console.WriteLine($"  {index + 1}. {shortlink.Name} - {shortlink.ShortUrl} - {shortlink.Status}");
                Console.WriteLine($"     Alias: {shortlink.Alias ?? "(auto)"}");
                Console.WriteLine($"  ID: {shortlink.Id}");
                Console.WriteLine($"  Short URL: {shortlink.ShortUrl}");
                Console.WriteLine($"  Long URL: {shortlink.LongUrl}");
                Console.WriteLine($"  Status: {shortlink.Status}");
                Console.WriteLine($"  Created By: {shortlink.CreatedBy}");
                Console.WriteLine($"  Created On: {shortlink.CreatedOn}");
                Console.WriteLine($"  Visits: {shortlink.Visits}");
                Console.WriteLine($"  Unique Visits: {shortlink.UniqueVisits}");
                Console.WriteLine($"  Preview Visits: {shortlink.PreviewVisits}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No shortlinks found for this account");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
        Console.WriteLine();
    }
}

static async Task TestListShortlinksByDate(ISmsApi smsApi, string? startDate, string? endDate, int limit, int offset)
{
    Console.WriteLine("TEST LIST SHORTLINKS BY DATE");
    Console.WriteLine("=============================\n");

    Console.WriteLine($"Start Date: {startDate ?? "Not specified"}");
    Console.WriteLine($"End Date: {endDate ?? "Not specified"}");
    Console.WriteLine($"Limit: {limit}");
    Console.WriteLine($"Offset: {offset} hours");
    Console.WriteLine();

    var result = await smsApi.Shortlinks.GetListAsync(startDate, endDate, limit, offset);

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Shortlinks found: {result.Data.Count}");
        Console.WriteLine();

        if (result.Data.Count > 0)
        {
            Console.WriteLine($"SHORTLINKS FOUND ({result.Data.Count}):");
            Console.WriteLine("=====================================\n");

            foreach (var shortlink in result.Data)
            {
                Console.WriteLine($"{shortlink.Name}");
                Console.WriteLine($"  ID: {shortlink.Id}");
                Console.WriteLine($"  Short URL: {shortlink.ShortUrl}");
                Console.WriteLine($"  Long URL: {shortlink.LongUrl}");
                Console.WriteLine($"  Status: {shortlink.Status}");
                Console.WriteLine($"  Created: {shortlink.CreatedOn} (Local time UTC{(offset >= 0 ? "+" : "")}{offset})");
                Console.WriteLine($"  Created By: {shortlink.CreatedBy}");
                Console.WriteLine($"  Statistics:");
                Console.WriteLine($"    Total Visits: {shortlink.Visits}");
                Console.WriteLine($"    Unique Visits: {shortlink.UniqueVisits}");
                Console.WriteLine($"    Preview Visits: {shortlink.PreviewVisits}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No shortlinks found with the specified criteria");
        }
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
        Console.WriteLine();
    }
}

static async Task TestGetShortlinkById(ISmsApi smsApi, string id)
{
    Console.WriteLine("TEST GET SHORTLINK BY ID");
    Console.WriteLine("=========================\n");

    Console.WriteLine($"Shortlink ID: {id}");
    Console.WriteLine();

    var result = await smsApi.Shortlinks.GetByIdAsync(id);

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine();

        Console.WriteLine("SHORTLINK DETAILS:");
        Console.WriteLine("==================\n");
        Console.WriteLine($"ID: {result.Data.Id}");
        Console.WriteLine($"Name: {result.Data.Name}");
        Console.WriteLine($"Short URL: {result.Data.ShortUrl}");
        Console.WriteLine($"Long URL: {result.Data.LongUrl}");
        Console.WriteLine($"Status: {result.Data.Status}");
        Console.WriteLine($"Created By: {result.Data.CreatedBy}");
        Console.WriteLine($"Created On: {result.Data.CreatedOn}");

        Console.WriteLine("\nVISIT STATISTICS:");
        Console.WriteLine("==================");
        Console.WriteLine($"Total Visits: {result.Data.Visits}");
        Console.WriteLine($"Unique Visits: {result.Data.UniqueVisits}");
        Console.WriteLine($"Preview Visits: {result.Data.PreviewVisits}");
        Console.WriteLine();

        Console.WriteLine("DEBUG - FULL RESPONSE:");
        Console.WriteLine("======================");
        Console.WriteLine(JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
        Console.WriteLine();
    }
}

static async Task TestUpdateShortlinkStatus(ISmsApi smsApi, string id, ShortlinkStatus status)
{
    Console.WriteLine("TEST UPDATE SHORTLINK STATUS");
    Console.WriteLine("=============================\n");

    Console.WriteLine($"Shortlink ID: {id}");
    Console.WriteLine($"New Status: {status}");
    Console.WriteLine();

    var result = await smsApi.Shortlinks.UpdateStatusAsync(id, status);

    if (result.IsOk && result.Data != null)
    {
        Console.WriteLine("SUCCESS!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine();

        Console.WriteLine("UPDATED SHORTLINK DETAILS:");
        Console.WriteLine("==========================\n");
        Console.WriteLine($"ID: {result.Data.Id}");
        Console.WriteLine($"Name: {result.Data.Name}");
        Console.WriteLine($"Short URL: {result.Data.ShortUrl}");
        Console.WriteLine($"Long URL: {result.Data.LongUrl}");
        Console.WriteLine($"Status: {result.Data.Status} <- UPDATED");
        Console.WriteLine($"Created By: {result.Data.CreatedBy}");
        Console.WriteLine($"Created On: {result.Data.CreatedOn}");

        Console.WriteLine("\nVISIT STATISTICS:");
        Console.WriteLine("==================");
        Console.WriteLine($"Total Visits: {result.Data.Visits}");
        Console.WriteLine($"Unique Visits: {result.Data.UniqueVisits}");
        Console.WriteLine($"Preview Visits: {result.Data.PreviewVisits}");
        Console.WriteLine();

        Console.WriteLine("DEBUG - FULL RESPONSE:");
        Console.WriteLine("======================");
        Console.WriteLine(JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("ERROR!");
        Console.WriteLine($"Status: {result.HttpCode}");
        Console.WriteLine($"Error: {result.ErrorDescription}");
        Console.WriteLine();
    }
}

static async Task TestStatusValidation(ISmsApi smsApi)
{
    Console.WriteLine("TEST STATUS VALIDATION");
    Console.WriteLine("=======================\n");

    var longUrl = GetRandomLongUrl();
    var name = "Test Status Validation";

    var invalidStatuses = new[] { "PENDING", "DRAFT", "DELETED", "SUSPENDED", "active", "inactive", "Active", "Inactive" };

    foreach (var invalidStatus in invalidStatuses)
    {
        Console.WriteLine($"Testing invalid status: \"{invalidStatus}\"");
        try
        {
            var status = Enum.Parse<ShortlinkStatus>(invalidStatus, ignoreCase: true);
            await TestCreateShortlink(smsApi, longUrl, $"{name} - {invalidStatus}", null, status);
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"  Status \"{invalidStatus}\" is not a valid enum value");
        }
        await Task.Delay(1000);
    }

    Console.WriteLine("\nTesting valid status: ACTIVE");
    await TestCreateShortlink(smsApi, longUrl, $"{name} - ACTIVE", null, ShortlinkStatus.ACTIVE);

    Console.WriteLine("\nTesting valid status: INACTIVE");
    await TestCreateShortlink(smsApi, longUrl, $"{name} - INACTIVE", null, ShortlinkStatus.INACTIVE);

    Console.WriteLine("\nTesting without status (should default to ACTIVE)");
    await TestCreateShortlink(smsApi, longUrl, $"{name} - No Status", null);
}

static async Task TestMultipleShortlinks(ISmsApi smsApi, int count)
{
    Console.WriteLine($"TEST MULTIPLE SHORTLINKS ({count} requests)");
    Console.WriteLine("==========================================\n");

    int success = 0;
    int error = 0;

    for (int i = 1; i <= count; i++)
    {
        Console.WriteLine($"Request #{i}/{count}");
        Console.WriteLine(new string('-', 30));
        Console.WriteLine();

        var longUrl = GetRandomLongUrl();
        var name = GetRandomName();
        var status = GetRandomStatus();

        var result = await smsApi.Shortlinks.CreateAsync(longUrl, name, status, null);

        if (result.IsOk)
        {
            Console.WriteLine("SUCCESS!");
            Console.WriteLine($"  ID: {result.Data?.Id}");
            Console.WriteLine($"  Short URL: {result.Data?.ShortUrl}");
            success++;
        }
        else
        {
            Console.WriteLine("ERROR!");
            Console.WriteLine($"  {result.ErrorDescription}");
            error++;
        }

        Console.WriteLine();

        if (i < count)
        {
            Console.WriteLine("Waiting 2 seconds...");
            await Task.Delay(2000);
        }
    }

    Console.WriteLine("\nFINAL SUMMARY");
    Console.WriteLine("==============");
    Console.WriteLine($"Total: {count}");
    Console.WriteLine($"Success: {success}");
    Console.WriteLine($"Errors: {error}");
    Console.WriteLine($"Success Rate: {((double)success / count * 100):F1}%");
    Console.WriteLine();
}


using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("🚀 SMS API .NET SDK - Quick Test");
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
        Console.WriteLine("❌ Configuración inválida. Por favor actualiza appsettings.json");
        Console.WriteLine("   o usa: dotnet user-secrets set \"SmsApi:ApiKey\" \"tu-api-key\"");
        return;
    }

    // Get test data from configuration with UTF-8 examples
    var testPhone = configuration["TestData:TestPhoneNumber"] ?? "50212345678";
    var testMessage = configuration["TestData:TestMessage"] ?? "¡Hola desde .NET SDK! ¿Te llegó el mensaje?";
    var testTagName = configuration["TestData:TestTagName"] ?? "TestTag";
    var testFirstName = configuration["TestData:TestContactFirstName"] ?? "Juan";
    var testLastName = configuration["TestData:TestContactLastName"] ?? "Pérez";

    Console.WriteLine("📱 Datos de prueba:");
    Console.WriteLine($"   Teléfono: {testPhone}");
    Console.WriteLine($"   Mensaje: {testMessage}");
    Console.WriteLine($"   Tag: {testTagName}\n");

    // === 1. TEST MESSAGES ===
    Console.WriteLine("💬 1. PROBANDO ENVÍO DE MENSAJES");
    Console.WriteLine("─────────────────────────────────");
    
    // Send to contact
    Console.Write("   📞 Enviando a contacto... ");
    var messageResult = await smsApi.Messages.SendToContactAsync(testPhone, testMessage);
    if (messageResult.IsOk)
    {
        Console.WriteLine("✅ Enviado!");
        Console.WriteLine($"      ID: {messageResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"❌ Error: {messageResult.ErrorDescription}");
    }

    // Send to tags
    Console.Write("   🏷️  Enviando por tags... ");
    var tagsResult = await smsApi.Messages.SendToTagsAsync(new[] { testTagName }, testMessage);
    if (tagsResult.IsOk)
    {
        Console.WriteLine("✅ Enviado!");
        Console.WriteLine($"      ID: {tagsResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"❌ Error: {tagsResult.ErrorDescription}");
    }

    // === 2. TEST CONTACTS ===
    Console.WriteLine("\n👥 2. PROBANDO GESTIÓN DE CONTACTOS");
    Console.WriteLine("──────────────────────────────────");
    
    // Add contact
    Console.Write("   ➕ Agregando contacto... ");
    var contactResult = await smsApi.Contacts.AddAsync("502", testPhone, testFirstName, testLastName);
    if (contactResult.IsOk)
    {
        Console.WriteLine("✅ Agregado!");
        Console.WriteLine($"      Nombre: {contactResult.Data?.FirstName} {contactResult.Data?.LastName}");
    }
    else
    {
        Console.WriteLine($"❌ Error: {contactResult.ErrorDescription}");
    }

    // Get contact
    Console.Write("   🔍 Consultando contacto... ");
    var getContactResult = await smsApi.Contacts.GetByMsisdnAsync(testPhone);
    if (getContactResult.IsOk)
    {
        Console.WriteLine("✅ Encontrado!");
        Console.WriteLine($"      Estado: {getContactResult.Data?.Status}");
    }
    else
    {
        Console.WriteLine($"❌ Error: {getContactResult.ErrorDescription}");
    }

    // Add tag to contact
    Console.Write("   🏷️  Agregando tag al contacto... ");
    var addTagResult = await smsApi.Contacts.AddTagAsync(testPhone, testTagName);
    if (addTagResult.IsOk)
    {
        Console.WriteLine("✅ Tag agregado!");
    }
    else
    {
        Console.WriteLine($"❌ Error: {addTagResult.ErrorDescription}");
    }

    // === 3. TEST TAGS ===
    Console.WriteLine("\n🏷️  3. PROBANDO GESTIÓN DE TAGS");
    Console.WriteLine("───────────────────────────────");
    
    // Get tags list
    Console.Write("   📋 Listando tags... ");
    var tagsListResult = await smsApi.Tags.GetListAsync();
    if (tagsListResult.IsOk)
    {
        Console.WriteLine($"✅ Encontrados {tagsListResult.Data?.Count ?? 0} tags");
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
        Console.WriteLine($"❌ Error: {tagsListResult.ErrorDescription}");
    }

    // Get contacts in tag
    Console.Write($"   👥 Contactos en tag '{testTagName}'... ");
    var tagContactsResult = await smsApi.Tags.GetContactListAsync(testTagName);
    if (tagContactsResult.IsOk)
    {
        Console.WriteLine($"✅ {tagContactsResult.Data?.Count ?? 0} contactos encontrados");
    }
    else
    {
        Console.WriteLine($"❌ Error: {tagContactsResult.ErrorDescription}");
    }

    // === 4. TEST MESSAGE RETRIEVAL ===
    Console.WriteLine("\n📊 4. PROBANDO CONSULTA DE MENSAJES");
    Console.WriteLine("──────────────────────────────────");
    
    Console.Write("   📜 Consultando mensajes (JavaScript dates)... ");
    // ✅ Usar exactamente las mismas fechas que JavaScript que funciona
    var startDate = new DateTime(2025, 7, 1);   // "2025-07-01"
    var endDate = new DateTime(2025, 7, 4);     // "2025-07-04" 
    var messagesResult = await smsApi.Messages.GetListAsync(
        startDate: startDate, 
        endDate: endDate, 
        limit: 10);  // Mismo límite que JavaScript
    
    if (messagesResult.IsOk)
    {
        Console.WriteLine($"✅ {messagesResult.Data?.Count ?? 0} mensajes encontrados");
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
        Console.WriteLine($"❌ Error: {messagesResult.ErrorDescription}");
    }

    // === 5. UTF-8 COMPATIBILITY TEST ===
    Console.WriteLine("\n🌍 5. PROBANDO COMPATIBILIDAD UTF-8");
    Console.WriteLine("──────────────────────────────────");
    
    Console.Write("   🔤 Enviando mensaje con caracteres extendidos... ");
    var utf8Message = "Acentos: áéíóú ÁÉÍÓÚ ñÑ.";
    var utf8Result = await smsApi.Messages.SendToContactAsync(testPhone, utf8Message, "utf8-test");
    if (utf8Result.IsOk)
    {
        Console.WriteLine("✅ Enviado!");
        Console.WriteLine($"      Mensaje UTF-8: {utf8Message}");
        Console.WriteLine($"      ID: {utf8Result.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"❌ Error: {utf8Result.ErrorDescription}");
    }

    Console.WriteLine("\n🎉 PRUEBAS COMPLETADAS!");
    Console.WriteLine("========================");
    Console.WriteLine("✅ Si ves mensajes enviados, ¡el SDK funciona correctamente!");
    Console.WriteLine("✅ Los caracteres UTF-8 (¡¿áéíóú) deberían enviarse perfectamente");
    Console.WriteLine("❌ Si hay errores, revisa la configuración de API en appsettings.json");
}
catch (Exception ex)
{
    Console.WriteLine($"\n💥 ERROR GENERAL: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Detalle: {ex.InnerException.Message}");
    }
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
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
using InteractuaMovil.ContactoSms.Api.Extensions;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 🚀 Ejemplo Simple: Enviar SMS con el Modern SDK
Console.WriteLine("📱 Enviando SMS con Modern SMS API SDK");
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
    // 📞 CAMBIAR ESTOS VALORES SI QUIERES:
    string phoneNumber = "+1234567890";  // Tu número de prueba
    string message = $"✅ ¡Modern .NET SMS SDK funcionando perfectamente! {DateTime.Now:HH:mm:ss} 🚀";

    Console.WriteLine($"📤 Enviando mensaje a: {phoneNumber}");
    Console.WriteLine($"💬 Mensaje: {message}");
    Console.WriteLine("⏳ Enviando...\n");

    // 🚀 MÉTODO ASYNC (Recomendado)
    var result = await smsApi.Messages.SendToContactAsync(phoneNumber, message);

    if (result.IsOk)
    {
        Console.WriteLine("✅ ¡MENSAJE ENVIADO EXITOSAMENTE!");
        Console.WriteLine($"📧 ID del Mensaje: {result.Data?.MessageId}");
        Console.WriteLine($"📱 Número de destino: {result.Data?.Msisdn}");
        Console.WriteLine($"💬 Mensaje enviado: {result.Data?.Message}");
        Console.WriteLine($"📊 Estado HTTP: {result.HttpCode}");
        Console.WriteLine($"🕐 Enviado a las: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }
    else
    {
        Console.WriteLine("❌ ERROR AL ENVIAR MENSAJE:");
        Console.WriteLine($"🔴 Código de error: {result.ErrorCode}");
        Console.WriteLine($"📝 Descripción: {result.ErrorDescription}");
        Console.WriteLine($"🌐 Estado HTTP: {result.HttpCode}");
    }

    // También probemos el método sync para compatibilidad
    Console.WriteLine("\n🔄 Probando método sincrónico...");
    var syncResult = smsApi.Messages.SendToContact(phoneNumber, $"✅ Método SYNC también funciona! {DateTime.Now:HH:mm:ss}");
    
    if (syncResult.IsOk)
    {
        Console.WriteLine("✅ ¡Método sincrónico también funcionó!");
        Console.WriteLine($"📧 ID del Mensaje Sync: {syncResult.Data?.MessageId}");
    }
    else
    {
        Console.WriteLine($"❌ Método sync falló: {syncResult.ErrorDescription}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"💥 EXCEPCIÓN: {ex.Message}");
    Console.WriteLine($"🔍 Detalles: {ex.StackTrace}");
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey(); 
# 🚀 Modern SMS API SDK for .NET

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C# 12](https://img.shields.io/badge/C%23-12.0-green?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Production Ready](https://img.shields.io/badge/Status-Production%20Ready-brightgreen)](https://github.com)

Un SDK **moderno**, **completamente funcional** y **probado en producción** para servicios de SMS API, diseñado con las mejores prácticas de .NET 8, inyección de dependencias y compatibilidad total con la API real.

## ✨ Características

- **🎯 Producción Ready** - Completamente probado con API real
- **⚡ Async/Await First** - Implementación completamente asíncrona
- **🏗️ Dependency Injection** - Integración con Microsoft.Extensions.DI
- **🔒 Type Safety** - C# 12, nullable reference types
- **📝 Structured Logging** - Microsoft.Extensions.Logging
- **⚙️ Configuration Management** - Patrón IOptions + User Secrets
- **🛡️ API Compatibility** - Totalmente compatible con backend real
- **🧪 Fully Tested** - Pruebas unitarias y de integración

## 🚀 Funcionalidades Implementadas

### 📱 Mensajes
- ✅ Envío a contacto individual
- ✅ Envío por tags (grupos)
- ✅ Consulta de mensajes por fecha
- ✅ Estado de entrega
- ✅ Mensajes programados

### 👥 Contactos
- ✅ Agregar/actualizar contactos
- ✅ Consultar contacto individual
- ✅ Gestión de tags por contacto
- ✅ Estados: SUSCRIBED, SUBSCRIBED, INVITED

### 🏷️ Tags
- ✅ Listar todos los tags
- ✅ Consultar contactos por tag
- ✅ Eliminación de tags

### 📊 Cuentas & Reportes
- ✅ Información de cuenta
- ✅ Balance y límites
- ✅ Estadísticas de uso

## 📦 Instalación

### Desde el código fuente (Recomendado)
```bash
cd .netLATEST/sdk/SMSApi.Modern
dotnet build
dotnet pack
```

### Referencia de proyecto
```xml
<ProjectReference Include="path/to/SMSApi.Modern/SMSApi.Modern.csproj" />
```

## ⚡ INICIO RÁPIDO (5 minutos)

### 📁 Estructura del Proyecto
```
.netLATEST/
├── 🎯 sdk/SMSApi.Modern/           # SDK Principal
├── 🔧 examples/                    # Ejemplos de uso
│   ├── QuickTest/                  # ⚡ Prueba rápida (EMPEZAR AQUÍ)
│   ├── ApiExample.Modern/          # 🔧 Ejemplo completo  
│   └── SendSmsExample.Modern/      # 📱 Ejemplo simple
└── 🧪 tests/SMSApi.Modern.Tests/   # Tests unitarios
```

### 1️⃣ Configurar Credenciales

**Opción A: User Secrets (Recomendado para desarrollo)**
```bash
cd .netLATEST/examples/QuickTest
dotnet user-secrets set "SmsApi:ApiKey" "tu-api-key-real"
dotnet user-secrets set "SmsApi:SecretKey" "tu-secret-key-real"  
dotnet user-secrets set "SmsApi:ApiUrl" "https://tu-api-url.com/api/rest/"
```

**Opción B: Editar appsettings.json**
```json
{
  "SmsApi": {
    "ApiKey": "tu-api-key-aquí",
    "SecretKey": "tu-secret-key-aquí", 
    "ApiUrl": "https://tu-api-url.com/api/rest/",
    "TimeoutSeconds": 30,
    "EnableLogging": true
  }
}
```

### 2️⃣ Personalizar Datos de Prueba

Edita `examples/QuickTest/appsettings.json`:
```json
{
  "TestData": {
    "TestPhoneNumber": "50212345678",        // 📱 Tu número de prueba real
    "TestMessage": "¡Hola desde .NET SDK!",
    "TestTagName": "TestTag",                // 🏷️ Tag existente en tu cuenta
    "TestContactFirstName": "Juan",
    "TestContactLastName": "Pérez"
  }
}
```

### 3️⃣ Ejecutar Prueba Completa

```bash
cd .netLATEST/examples/QuickTest
dotnet run
```

**🎯 Resultado esperado:**
```
🚀 SMS API .NET SDK - Quick Test
=================================

📱 Datos de prueba:
   Teléfono: 50212345678
   Mensaje: ¡Hola desde .NET SDK!
   Tag: TestTag

💬 1. PROBANDO ENVÍO DE MENSAJES
─────────────────────────────────
   🏷️  Enviando por tags... ✅ Enviado!

👥 2. PROBANDO GESTIÓN DE CONTACTOS  
──────────────────────────────────
   ➕ Agregando contacto... ✅ Agregado!
   🔍 Consultando contacto... ✅ Encontrado!

🏷️  3. PROBANDO GESTIÓN DE TAGS
───────────────────────────────
   📋 Listando tags... ✅ Encontrados X tags
   👥 Contactos en tag... ✅ X contactos encontrados

🎉 PRUEBAS COMPLETADAS!
```

## 📚 Uso del SDK

### Configuración Básica
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

    // Enviar por tags (grupos)
    public async Task<bool> SendPromotionAsync(string tagName, string promo)
    {
        var result = await _smsApi.Messages.SendToTagsAsync(
            message: $"🎉 Promoción especial: {promo}",
            tags: new[] { tagName },
            messageId: $"promo-{DateTime.Now:yyyyMMddHHmmss}"
        );
        
        return result.IsOk;
    }
}
```

### Gestión de Contactos
```csharp
// Agregar contacto
var contact = await _smsApi.Contacts.AddContactAsync(
    msisdn: "50212345678",
    firstName: "Juan",
    lastName: "Pérez"
);

// Consultar contacto
var existing = await _smsApi.Contacts.GetContactAsync("50212345678");

// Agregar tag a contacto
await _smsApi.Contacts.AddTagToContactAsync("50212345678", "VIP");
```

### Gestión de Tags
```csharp
// Listar todos los tags
var tags = await _smsApi.Tags.GetTagsAsync();

// Contactos en un tag específico
var contacts = await _smsApi.Tags.GetContactsByTagAsync("VIP");

// Eliminar tag
await _smsApi.Tags.DeleteTagAsync("OldTag");
```

### Consulta de Mensajes
```csharp
// Mensajes de hoy
var today = DateTime.Today;
var messages = await _smsApi.Messages.GetMessagesAsync(
    startDate: today,
    endDate: today.AddDays(1),
    direction: MessageDirection.MT,
    limit: 50
);
```

## 🔧 Ejemplos Disponibles

### QuickTest - Prueba Completa ⚡
```bash
cd examples/QuickTest
dotnet run
```
**Propósito**: Prueba todas las funcionalidades principales en 30 segundos.

### ApiExample.Modern - Ejemplo Detallado 🔧
```bash
cd examples/ApiExample.Modern  
dotnet run
```
**Propósito**: Ejemplo completo con manejo de errores y logging.

### SendSmsExample.Modern - Envío Simple 📱
```bash
cd examples/SendSmsExample.Modern
dotnet run
```
**Propósito**: Ejemplo mínimo para envío de SMS.

## 🧪 Testing

```bash
# Compilar todo
cd .netLATEST
dotnet build

# Ejecutar tests unitarios
cd tests/SMSApi.Modern.Tests
dotnet test

# Test de integración con API real
cd examples/QuickTest
dotnet run
```

## 🐛 Solución de Problemas

### Error: "Failed to parse response JSON"
✅ **Solucionado** - Enums actualizados para coincidir con API real:
- `ContactStatus.SUSCRIBED` (sin 'B')
- `AddedFrom.FILE_UPLOAD`

### Error: "The JSON value could not be converted"
✅ **Solucionado** - Compatibilidad total con respuestas de API.

### Error: "No autorizado" en algunas operaciones
⚠️ **Esperado** - Algunas operaciones requieren permisos específicos en tu cuenta API.

### Error: "El mensaje ha sido bloqueado debido a que esta duplicado"  
⚠️ **Esperado** - Protección anti-spam de la API. Cambia el mensaje o espera unos minutos.

## 📋 Requisitos

- **.NET 8.0** (recomendado)
- **Visual Studio 2022** 17.8+ o **VS Code**
- **C# 12** language features
- **Credenciales de API válidas**

## ✅ Estado de Compatibilidad

| Funcionalidad | Estado | Notas |
|---------------|--------|-------|
| Envío mensajes | ✅ | Contacto + Tags |
| Gestión contactos | ✅ | CRUD completo |
| Gestión tags | ✅ | Listar, consultar, eliminar |
| Consulta mensajes | ✅ | Por fecha, filtros |
| Autenticación | ✅ | API Key + Secret |
| Logging | ✅ | Structured logging |
| Async/Await | ✅ | Completamente asíncrono |
| DI Container | ✅ | Microsoft.Extensions.DI |

## 🔗 Recursos

- 📖 **Ejemplos de uso** - Revisa la carpeta `examples/`
- 🧪 **Tests** - Ejecuta `dotnet test` para ver ejemplos
- 🔧 **Configuración** - Ver `appsettings.json` en ejemplos

---

**⚠️ Nota de Seguridad**: Nunca commits API keys o secrets al control de versiones. Siempre usa user secrets, variables de entorno o vaults seguros para credenciales sensibles.

**🎯 ¿Listo para integrar SMS en tu aplicación .NET?** Este SDK te proporciona todo lo que necesitas para funcionalidad SMS confiable, escalable y mantenible.



# 🚀 Quick Test - SMS API .NET SDK

Prueba rápida y completa de todas las funcionalidades principales del SMS API SDK.

## ⚡ Uso Rápido

### 1. Configurar credenciales

**Opción A: appsettings.json**
```bash
# Edita appsettings.json y reemplaza:
# "your-api-key-here" con tu API key real
# "your-secret-key-here" con tu secret key real
# "https://your-api-url.com/api/" con tu URL real
```

**Opción B: User Secrets (Recomendado para desarrollo)**
```bash
cd .netLATEST/examples/QuickTest
dotnet user-secrets set "SmsApi:ApiKey" "tu-api-key-real"
dotnet user-secrets set "SmsApi:SecretKey" "tu-secret-key-real"
dotnet user-secrets set "SmsApi:ApiUrl" "https://tu-api-url.com/api/"
```

### 2. Personalizar datos de prueba

Edita en `appsettings.json`:
```json
"TestData": {
  "TestPhoneNumber": "50212345678",     // 📱 Tu número de prueba
  "TestMessage": "¡Hola desde SDK! 🚀",  // 💬 Mensaje de prueba
  "TestTagName": "TestTag",              // 🏷️ Tag existente
  "TestContactFirstName": "Juan",        // 👤 Nombre
  "TestContactLastName": "Pérez"         // 👤 Apellido
}
```

### 3. Ejecutar

```bash
cd .netLATEST/examples/QuickTest
dotnet run
```

## 🧪 Qué prueba

✅ **Envío de Mensajes**
- A contacto individual
- Por tags

✅ **Gestión de Contactos**  
- Agregar contacto
- Consultar contacto
- Agregar tag a contacto

✅ **Gestión de Tags**
- Listar tags
- Consultar contactos por tag

✅ **Consulta de Mensajes**
- Retrieve mensajes del día

## 📋 Salida Esperada

```
🚀 SMS API .NET SDK - Quick Test
=================================

📱 Datos de prueba:
   Teléfono: +50212345678
   Mensaje: ¡Hola desde SMS API .NET SDK! 🚀
   Tag: TestTag

💬 1. PROBANDO ENVÍO DE MENSAJES
─────────────────────────────────
   📞 Enviando a contacto... ✅ Enviado!
      ID: msg_12345
   🏷️  Enviando por tags... ✅ Enviado!
      ID: msg_12346

👥 2. PROBANDO GESTIÓN DE CONTACTOS
──────────────────────────────────
   ➕ Agregando contacto... ✅ Agregado!
      Nombre: Juan Pérez
   🔍 Consultando contacto... ✅ Encontrado!
      Estado: Active
   🏷️  Agregando tag al contacto... ✅ Tag agregado!

🏷️  3. PROBANDO GESTIÓN DE TAGS
───────────────────────────────
   📋 Listando tags... ✅ Encontrados 5 tags
      - TestTag
      - Clientes
      - VIP
   👥 Contactos en tag 'TestTag'... ✅ 1 contactos encontrados

📊 4. PROBANDO CONSULTA DE MENSAJES
──────────────────────────────────
   📜 Consultando mensajes de hoy... ✅ 3 mensajes encontrados
      - ¡Hola desde SMS API .NET SDK...
      - Mensaje de prueba...

🎉 PRUEBAS COMPLETADAS!
========================
✅ Si ves mensajes enviados, ¡el SDK funciona correctamente!
```

## 🔧 Troubleshooting

**❌ "Configuración inválida"**
- Verifica que las credenciales sean correctas
- Usa `dotnet user-secrets` para credenciales seguras

**❌ "Error al enviar mensaje"**  
- Verifica que el número de teléfono sea válido
- Asegúrate de tener créditos/balance en la cuenta

**❌ "Tag no encontrado"**
- Cambia `TestTagName` por un tag que exista en tu cuenta
- O crea el tag primero en tu plataforma SMS 
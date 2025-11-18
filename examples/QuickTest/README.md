# Quick Test - SMS API .NET SDK

Quick and complete test of all main SMS API SDK functionalities.

## Quick Start

### 1. Configure credentials

**Option A: appsettings.json**
```bash
# Edita appsettings.json y reemplaza:
# "your-api-key-here" con tu API key real
# "your-secret-key-here" con tu secret key real
# "https://your-api-url.com/api/" con tu URL real
```

**Option B: User Secrets (Recommended for development)**
```bash
cd .netLATEST/examples/QuickTest
dotnet user-secrets set "SmsApi:ApiKey" "tu-api-key-real"
dotnet user-secrets set "SmsApi:SecretKey" "tu-secret-key-real"
dotnet user-secrets set "SmsApi:ApiUrl" "https://tu-api-url.com/api/"
```

### 2. Customize test data

Edit `appsettings.json`:
```json
"TestData": {
  "TestPhoneNumber": "50212345678",
  "TestMessage": "Hello from SDK!",
  "TestTagName": "TestTag",
  "TestContactFirstName": "John",
  "TestContactLastName": "Doe"
}
```

### 3. Run

```bash
cd .netLATEST/examples/QuickTest
dotnet run
```

## What It Tests

**Message Sending**
- To individual contact
- By tags

**Contact Management**  
- Add contact
- Get contact
- Add tag to contact

**Tag Management**
- List tags
- Get contacts by tag

**Message Query**
- Retrieve messages from today

## Expected Output

```
SMS API .NET SDK - Quick Test
=================================

Test Data:
   Phone: +50212345678
   Message: Hello from SMS API .NET SDK!
   Tag: TestTag

1. TESTING MESSAGE SENDING
─────────────────────────────────
   Sending to contact... Sent!
      ID: msg_12345
   Sending to tags... Sent!
      ID: msg_12346

2. TESTING CONTACT MANAGEMENT
──────────────────────────────────
   Adding contact... Added!
      Name: John Doe
   Getting contact... Found!
      Status: Active
   Adding tag to contact... Tag added!

3. TESTING TAG MANAGEMENT
───────────────────────────────
   Listing tags... Found 5 tags
      - TestTag
      - Clientes
      - VIP
   Contacts in tag 'TestTag'... 1 contacts found

4. TESTING MESSAGE QUERY
──────────────────────────────────
   Querying messages... 3 messages found
      - Hello from SMS API .NET SDK...
      - Test message...

TESTS COMPLETED!
========================
If you see messages sent, the SDK is working correctly!
```

## Troubleshooting

**"Invalid configuration"**
- Verify credentials are correct
- Use `dotnet user-secrets` for secure credentials

**"Error sending message"**  
- Verify phone number is valid
- Ensure you have credits/balance in your account

**"Tag not found"**
- Change `TestTagName` to a tag that exists in your account
- Or create the tag first in your SMS platform 
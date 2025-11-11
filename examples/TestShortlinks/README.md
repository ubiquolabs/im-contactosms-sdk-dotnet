# Test Shortlinks

Test suite for shortlinks functionality in the .NET SDK.

## Configuration

Configure your API credentials using User Secrets (recommended):

```bash
dotnet user-secrets set "SmsApi:ApiKey" "your-api-key-here"
dotnet user-secrets set "SmsApi:SecretKey" "your-secret-key-here"
dotnet user-secrets set "SmsApi:ApiUrl" "https://your-api-url.com/api/rest/"
```

Or edit `appsettings.json` directly.

## Usage

### List all shortlinks
```bash
dotnet run -- list
```

### Create a shortlink
```bash
dotnet run -- create
dotnet run -- create "https://www.example.com" "My Shortlink" ACTIVE
```

### List shortlinks by date range
```bash
dotnet run -- date 2025-01-01 2025-12-31 20 -6
```

### Get shortlink by ID
```bash
dotnet run -- id 123ABC
```

### Update shortlink status
```bash
dotnet run -- update 123ABC ACTIVE
dotnet run -- update 123ABC INACTIVE
```

### Test status validation
```bash
dotnet run -- status
```

### Test multiple shortlinks
```bash
dotnet run -- multiple 10
```

### Show help
```bash
dotnet run -- --help
```

## Commands Summary

- `create [url] [name] [status]` - Create shortlink
- `list` - List all shortlinks
- `date [start] [end] [limit] [offset]` - List by date range
- `id <id>` - Get shortlink by ID
- `update <id> <status>` - Update shortlink status
- `status` - Test status validation
- `multiple [count]` - Test multiple shortlinks
- `--help` - Show usage information


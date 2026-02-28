# Fitliyo Elasticsearch Generic Loglama Sistemi

Bu doküman, Fitliyo projesinde Elasticsearch için oluşturulan generic loglama sisteminin kullanımını açıklar.

## Genel Bakış

Generic loglama sistemi, uygulamanın her yerinden kolayca **Info**, **Warning** ve **Error** seviyelerinde loglama yapabilmenizi sağlar. Tüm loglar hem yerel logger'a hem de Elasticsearch'e gönderilir.

## Özellikler

- ✅ **Tüm Log Seviyeleri**: Trace, Debug, Info, Warning, Error, Critical
- ✅ **Çoklu Veri Tipi Desteği**: Dictionary, Model, JSON String, Text
- ✅ **Detaylı Exception Loglama**:
  - Tam exception hierarchy (inner exceptions)
  - Stack trace analizi ve method bilgileri
  - Exception data ve properties
  - Aggregate exception desteği
- ✅ **HTTP Context Bilgileri**:
  - Request path, method, headers
  - Client IP adresi (proxy desteği)
  - User agent, referer bilgileri
  - Query parameters (hassas bilgiler filtrelenir)
- ✅ **Performans loglama** (işlem süresi ölçümü)
- ✅ **Kullanıcı aktivitesi loglama**
- ✅ **Otomatik kullanıcı ve kiracı bilgisi** ekleme
- ✅ **Correlation ID** desteği
- ✅ **Elasticsearch entegrasyonu**
- ✅ **Asenkron loglama**
- ✅ **Performans ölçümü** için disposable pattern
- ✅ **Generic Type Support** (Model loglama)
- ✅ **JSON Auto-parsing** (Geçersiz JSON'lar için fallback)
- ✅ **Güvenlik Filtreleme**: Hassas header ve parameter'lar otomatik filtrelenir

## Kurulum ve Konfigürasyon

### 1. Elasticsearch Konfigürasyonu

`appsettings.json` dosyasında Elasticsearch ayarlarını yapılandırın:

```json
{
  "Elasticsearch": {
    "ConnectionString": "http://localhost:9200",
    "IndexPrefix": "Fitliyo-logs",
    "Username": "",
    "Password": "",
    "EnableSecurity": false,
    "EnableSsl": false,
    "BatchPostingLimit": 50,
    "Period": "00:00:02",
    "QueueSizeLimit": 100000,
    "AutoRegisterTemplate": true,
    "NumberOfShards": 1,
    "NumberOfReplicas": 0,
    "IndexRetentionDays": "30.00:00:00",
    "MinimumLogLevel": "Information",
    "EnableStructuredLogging": true,
    "EnablePerformanceLogging": true,
    "EnableRequestLogging": true,
    "ExcludedPaths": [
      "/health",
      "/metrics",
      "/favicon.ico",
      "/_framework/"
    ],
    "LogLevelOverrides": {
      "Microsoft": "Warning",
      "System": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### 2. Modül Bağımlılığı

Loglama yapmak istediğiniz modülde `FitliyoLoggingModule` bağımlılığını ekleyin:

```csharp
[DependsOn(typeof(FitliyoLoggingModule))]
public class YourModule : AbpModule
{
    // ...
}
```

## Kullanım Örnekleri

### 1. Temel Loglama - Farklı Veri Tipleri

```csharp
public class UserAppService : ApplicationService
{
    private readonly IGenericLogService _logService;

    public UserAppService(IGenericLogService logService)
    {
        _logService = logService;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        // 1. Dictionary ile loglama
        await _logService.LogInfoAsync("Yeni kullanıcı oluşturma işlemi başladı",
            new Dictionary<string, object>
            {
                ["UserName"] = input.UserName,
                ["Email"] = input.Email
            });

        // 2. Model objesi ile loglama
        await _logService.LogInfoModelAsync("Kullanıcı input modeli", input);

        // 3. JSON string ile loglama
        var jsonData = """{"operation": "create", "timestamp": "2024-01-15T10:30:00Z"}""";
        await _logService.LogInfoJsonAsync("İşlem detayları", jsonData);

        // 4. Text ile loglama
        await _logService.LogInfoAsync("Basit metin loglama", "Bu bir text verisidir");

        try
        {
            var user = await CreateUserAsync(input);

            // Başarılı işlem - Model ile loglama
            await _logService.LogInfoModelAsync("Kullanıcı başarıyla oluşturuldu", user);

            return user;
        }
        catch (Exception ex)
        {
            // Hata loglama - Exception + Model
            await _logService.LogErrorModelAsync("Kullanıcı oluşturulurken hata oluştu", ex, input);
            throw;
        }
    }
}
```

### 2. Tüm Log Seviyeleri

```csharp
public async Task ProcessDataAsync()
{
    // Trace - En detaylı loglama
    await _logService.LogTraceAsync("İşlem başlıyor", "Detaylı trace bilgisi");

    // Debug - Geliştirme amaçlı
    await _logService.LogDebugAsync("Debug bilgisi", new { Step = 1, Data = "test" });

    // Info - Genel bilgi
    await _logService.LogInfoAsync("İşlem devam ediyor", "Normal akış");

    // Warning - Uyarı
    var invalidRecords = GetInvalidRecords();
    if (invalidRecords.Any())
    {
        await _logService.LogWarningModelAsync("Geçersiz kayıtlar bulundu", invalidRecords);
    }

    // Error - Hata
    try
    {
        await RiskyOperation();
    }
    catch (Exception ex)
    {
        await _logService.LogErrorAsync("İşlem hatası", ex, "Ek hata bilgisi");
    }

    // Critical - Kritik hata
    if (SystemFailure())
    {
        await _logService.LogCriticalAsync("Sistem kritik hatası",
            new SystemException("Kritik hata"),
            new { SystemComponent = "Database", Impact = "High" });
    }
}
```

### 3. Performans Loglama

#### Using Bloğu ile Otomatik Ölçüm

```csharp
public async Task<List<UserDto>> GetUsersAsync()
{
    using var performanceLogger = _logService.StartPerformanceLogging("GetUsers",
        new Dictionary<string, object>
        {
            ["Operation"] = "DatabaseQuery"
        });

    // İşlem burada yapılır
    var users = await _userRepository.GetListAsync();
    return ObjectMapper.Map<List<User>, List<UserDto>>(users);

    // using bloğu bittiğinde otomatik olarak performans logu oluşturulur
}
```

#### Manuel Performans Loglama

```csharp
public async Task<ReportDto> GenerateReportAsync()
{
    var startTime = DateTime.Now;

    try
    {
        var report = await CreateReportAsync();

        var duration = DateTime.Now - startTime;
        await _logService.LogPerformanceAsync("GenerateReport", duration,
            new Dictionary<string, object>
            {
                ["ReportType"] = "Monthly",
                ["RecordCount"] = report.RecordCount
            });

        return report;
    }
    catch (Exception ex)
    {
        var duration = DateTime.Now - startTime;
        await _logService.LogErrorAsync("Rapor oluşturulurken hata oluştu", ex,
            new Dictionary<string, object>
            {
                ["Duration"] = duration.TotalMilliseconds,
                ["ReportType"] = "Monthly"
            });
        throw;
    }
}
```

### 4. Kullanıcı Aktivitesi Loglama

```csharp
public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
{
    var user = await _userRepository.GetAsync(id);

    // Güncelleme öncesi değerleri kaydet
    var oldValues = new Dictionary<string, object>
    {
        ["OldName"] = user.Name,
        ["OldEmail"] = user.Email
    };

    ObjectMapper.Map(input, user);
    await _userRepository.UpdateAsync(user);

    // Kullanıcı aktivitesi loglama
    await _logService.LogUserActivityAsync("Update", "User", id.ToString(),
        new Dictionary<string, object>
        {
            ["NewName"] = user.Name,
            ["NewEmail"] = user.Email,
            ["Changes"] = oldValues
        });

    return ObjectMapper.Map<User, UserDto>(user);
}
```

### 5. Detaylı Exception Handling ve Error Loglama

```csharp
public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto input)
{
    try
    {
        var user = await _userRepository.GetAsync(id);

        // İşlem öncesi loglama
        await _logService.LogInfoAsync("Kullanıcı güncelleme işlemi başladı",
            new Dictionary<string, object>
            {
                ["UserId"] = id,
                ["UpdateData"] = input
            });

        // Güncelleme işlemi
        ObjectMapper.Map(input, user);
        await _userRepository.UpdateAsync(user);

        await _logService.LogInfoAsync("Kullanıcı başarıyla güncellendi", user);

        return ObjectMapper.Map<User, UserDto>(user);
    }
    catch (EntityNotFoundException ex)
    {
        // Özel exception türü için loglama - Detaylı bilgiler otomatik eklenir:
        // - Exception hierarchy (inner exceptions)
        // - Stack trace ve method bilgileri
        // - HTTP context bilgileri (request path, IP, user agent)
        await _logService.LogWarningAsync("Kullanıcı bulunamadı", ex,
            new Dictionary<string, object>
            {
                ["UserId"] = id,
                ["RequestedOperation"] = "Update"
            });
        throw;
    }
    catch (AggregateException ex)
    {
        // Aggregate exception - Tüm inner exception'lar detaylı loglanır
        await _logService.LogErrorAsync("Çoklu hata oluştu", ex,
            new Dictionary<string, object>
            {
                ["UserId"] = id,
                ["Input"] = input,
                ["InnerExceptionCount"] = ex.InnerExceptions.Count
            });
        throw;
    }
    catch (Exception ex)
    {
        // Genel exception loglama - Otomatik olarak eklenen bilgiler:
        // - Exception.Data dictionary
        // - HResult ve HelpLink
        // - Tam stack trace analizi
        // - Method ve class bilgileri
        // - HTTP request detayları
        await _logService.LogErrorAsync("Kullanıcı güncellenirken beklenmeyen hata oluştu", ex,
            new Dictionary<string, object>
            {
                ["UserId"] = id,
                ["Input"] = input,
                ["ErrorType"] = ex.GetType().Name
            });
        throw;
    }
}

// Karmaşık exception senaryosu
public async Task ProcessComplexOperationAsync()
{
    try
    {
        await SomeComplexOperation();
    }
    catch (Exception ex)
    {
        // Bu loglama otomatik olarak şunları içerir:
        // 1. Exception bilgileri:
        //    - Type: "System.InvalidOperationException"
        //    - Message: "İşlem geçersiz durumda"
        //    - StackTrace: Tam stack trace
        //    - HResult: -2146233079
        //    - Data: Exception.Data dictionary
        //
        // 2. Method bilgileri (stack trace'den çıkarılır):
        //    - ClassName: "Fitliyo.Application.Users.UserAppService"
        //    - MethodName: "ProcessComplexOperationAsync"
        //    - FullSignature: "ProcessComplexOperationAsync()"
        //    - FileInfo: "UserAppService.cs:line 123"
        //
        // 3. HTTP Context bilgileri:
        //    - RequestPath: "/api/app/user/process"
        //    - RequestMethod: "POST"
        //    - UserAgent: "Mozilla/5.0..."
        //    - IpAddress: "192.168.1.100"
        //    - Headers: Filtrelenmiş header'lar
        //    - QueryParameters: Filtrelenmiş parametreler
        //
        // 4. Inner Exception'lar (recursive):
        //    - Her inner exception için aynı detaylar
        //    - Tam exception hierarchy

        await _logService.LogErrorAsync("Karmaşık işlem hatası", ex);
        throw;
    }
}
```

#### Elasticsearch'te Error Log Yapısı

Detaylı error logları Elasticsearch'te şu yapıda saklanır:

```json
{
  "timestamp": "2024-01-15T10:30:00.000Z",
  "level": "Error",
  "message": "Kullanıcı güncellenirken beklenmeyen hata oluştu",
  "userId": "12345678-1234-1234-1234-123456789012",
  "tenantId": "87654321-4321-4321-4321-210987654321",
  "correlationId": "abc123-def456-ghi789",
  "data": {
    "userId": "12345678-1234-1234-1234-123456789012",
    "input": { "name": "John Doe", "email": "john@example.com" },
    "errorType": "InvalidOperationException"
  },
  "exception": {
    "type": "System.InvalidOperationException",
    "message": "İşlem geçersiz durumda",
    "stackTrace": "   at Fitliyo.Application.Users.UserAppService.UpdateUserAsync...",
    "source": "Fitliyo.Application",
    "helpLink": null,
    "hResult": -2146233079,
    "data": {
      "CustomProperty": "CustomValue"
    },
    "methodInfo": {
      "className": "Fitliyo.Application.Users.UserAppService",
      "methodName": "UpdateUserAsync",
      "fullSignature": "UpdateUserAsync(Guid id, UpdateUserDto input)",
      "fileInfo": "UserAppService.cs:line 123"
    },
    "innerExceptions": [
      {
        "type": "System.Data.SqlClient.SqlException",
        "message": "Veritabanı bağlantı hatası",
        "stackTrace": "   at System.Data.SqlClient...",
        "source": "System.Data.SqlClient"
      }
    ]
  },
  "httpContext": {
    "requestPath": "/api/app/user/12345678-1234-1234-1234-123456789012",
    "requestMethod": "PUT",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "ipAddress": "192.168.1.100",
    "referer": "https://app.Fitliyo.com/users",
    "statusCode": 500,
    "headers": {
      "Accept": "application/json",
      "Content-Type": "application/json",
      "Accept-Language": "tr-TR,tr;q=0.9,en;q=0.8"
    },
    "queryParameters": {
      "includeDetails": "true"
    }
  },
  "systemInfo": {
    "application": "Fitliyo",
    "environment": "Production",
    "machineName": "WEB-SERVER-01",
    "processId": 1234,
    "threadId": 5
  }
}
```

### 6. Farklı Veri Tipleri ile Loglama

```csharp
public async Task DataTypeExamplesAsync()
{
    // 1. Dictionary ile loglama
    var dictData = new Dictionary<string, object>
    {
        ["Key1"] = "Value1",
        ["Key2"] = 42,
        ["Key3"] = DateTime.Now
    };
    await _logService.LogInfoAsync("Dictionary verisi", dictData);

    // 2. Model objesi ile loglama
    var userModel = new UserDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" };
    await _logService.LogInfoModelAsync("Kullanıcı modeli", userModel);

    // 3. JSON string ile loglama
    var jsonString = """
    {
        "orderId": "12345",
        "amount": 99.99,
        "currency": "USD",
        "items": [
            {"name": "Product 1", "quantity": 2},
            {"name": "Product 2", "quantity": 1}
        ]
    }
    """;
    await _logService.LogInfoJsonAsync("Sipariş detayları", jsonString);

    // 4. Geçersiz JSON ile loglama (otomatik fallback)
    var invalidJson = "{ invalid json }";
    await _logService.LogWarningJsonAsync("Geçersiz JSON", invalidJson);

    // 5. Text verisi ile loglama
    var textData = "Bu bir metin verisidir. Özel karakterler: çğıöşü";
    await _logService.LogInfoAsync("Metin verisi", textData);

    // 6. Karmaşık nesne ile loglama
    var complexObject = new
    {
        User = userModel,
        Settings = dictData,
        Metadata = new { Version = "1.0", Environment = "Production" }
    };
    await _logService.LogInfoAsync("Karmaşık nesne", complexObject);

    // 7. Exception ile farklı veri tipleri
    try
    {
        throw new InvalidOperationException("Test hatası");
    }
    catch (Exception ex)
    {
        // Exception + Model
        await _logService.LogErrorModelAsync("Model ile hata", ex, userModel);

        // Exception + JSON
        await _logService.LogErrorJsonAsync("JSON ile hata", ex, jsonString);

        // Exception + Text
        await _logService.LogErrorAsync("Text ile hata", ex, "Ek hata bilgisi");
    }
}
```

## Yardımcı Sınıflar

### LoggingHelper Kullanımı

```csharp
// Exception'dan otomatik özellik çıkarma
try
{
    // Risky operation
}
catch (Exception ex)
{
    var exceptionProperties = LoggingHelper.ExtractExceptionProperties(ex);
    await _logService.LogErrorAsync("İşlem hatası", ex, exceptionProperties);
}

// Performans kategorisi belirleme
var duration = TimeSpan.FromMilliseconds(1500);
var category = LoggingHelper.GetPerformanceCategory(duration); // "Slow"

// Kullanıcı aktivitesi özellikleri oluşturma
var activityProps = LoggingHelper.CreateUserActivityProperties(
    "Login", "User", userId, ipAddress, userAgent);

// Sistem bilgileri ekleme
var properties = LoggingHelper.AddSystemProperties(new Dictionary<string, object>
{
    ["CustomProperty"] = "CustomValue"
});
```

## Extension Metotları

Extension metotları ile daha kısa kullanım:

```csharp
// Performans logger başlatma
using var perfLogger = _logService.StartPerformanceLogging("DatabaseOperation");

// Diğer extension metotları da mevcuttur
```

## Elasticsearch Index Yapısı

Loglar aşağıdaki yapıda Elasticsearch'e kaydedilir:

```json
{
  "timestamp": "2024-01-15T10:30:00.000Z",
  "level": "Information",
  "message": "Kullanıcı başarıyla oluşturuldu",
  "userId": "12345",
  "tenantId": "67890",
  "correlationId": "abc-def-ghi",
  "properties": {
    "UserName": "john.doe",
    "Email": "john@example.com",
    "Application": "Fitliyo",
    "Environment": "Production",
    "MachineName": "WEB-SERVER-01",
    "ProcessId": 1234
  },
  "exception": {
    "type": "ArgumentException",
    "message": "Invalid argument",
    "stackTrace": "...",
    "source": "MyApplication",
    "innerException": "..."
  }
}
```

## Index Adlandırma

Elasticsearch index'leri günlük olarak oluşturulur:
- Format: `{IndexPrefix}-{yyyy.MM.dd}`
- Örnek: `Fitliyo-logs-2024.01.15`

## Performans Önerileri

1. **Batch Loglama**: Elasticsearch ayarlarında `BatchPostingLimit` ve `Period` değerlerini optimize edin
2. **Minimum Log Level**: Gereksiz logları engellemek için `MinimumLogLevel` ayarını kullanın
3. **Async Kullanım**: Tüm loglama metotları async'dir, await kullanmayı unutmayın
4. **Exception Handling**: Loglama hatalarının uygulamayı etkilememesi için internal exception handling mevcuttur

## Monitoring ve Alerting

Elasticsearch'te aşağıdaki sorgularla monitoring yapabilirsiniz:

```json
// Son 1 saatteki error logları
{
  "query": {
    "bool": {
      "must": [
        {"term": {"level": "Error"}},
        {"range": {"timestamp": {"gte": "now-1h"}}}
      ]
    }
  }
}

// Yavaş işlemler (>5 saniye)
{
  "query": {
    "bool": {
      "must": [
        {"term": {"properties.LogType": "Performance"}},
        {"range": {"properties.Duration": {"gte": 5000}}}
      ]
    }
  }
}
```

## Troubleshooting

### Elasticsearch'e Log Gönderilmiyor

1. Elasticsearch bağlantı ayarlarını kontrol edin
2. `EnableStructuredLogging` ayarının `true` olduğundan emin olun
3. Elasticsearch servisinin çalıştığını doğrulayın
4. Network bağlantısını test edin

### Performans Sorunları

1. `BatchPostingLimit` değerini artırın
2. `Period` değerini azaltın
3. `QueueSizeLimit` değerini kontrol edin
4. Elasticsearch cluster performansını kontrol edin

### Log Seviyeleri Çalışmıyor

1. `MinimumLogLevel` ayarını kontrol edin
2. `LogLevelOverrides` ayarlarını gözden geçirin
3. Uygulama seviyesindeki log konfigürasyonunu kontrol edin

Bu dokümantasyon, Fitliyo projesinde Elasticsearch generic loglama sisteminin tam kullanımını kapsar. Herhangi bir sorun yaşarsanız, loglama modülü geliştiricileri ile iletişime geçin.

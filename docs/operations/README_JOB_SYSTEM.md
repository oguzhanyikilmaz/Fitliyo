# 🚀 Fitliyo Generic Background Job Sistemi

## 📋 İçindekiler
- [Genel Bakış](#genel-bakış)
- [Klasik Sistem vs Yeni Sistem](#klasik-sistem-vs-yeni-sistem)
- [Nasıl Kullanılır](#nasıl-kullanılır)
- [Örnekler](#örnekler)
- [Job Monitoring](#job-monitoring)
- [Best Practices](#best-practices)

## 🎯 Genel Bakış

Fitliyo projesinde, **FileAppService.cs pattern'ından** esinlenerek generic ve yeniden kullanılabilir bir background job sistemi oluşturulmuştur. Bu sistem:

- ✅ **Tenant ve Principal Management** otomatik
- ✅ **Authenticated Principal** - Job'lar authenticated context'te çalışır
- ✅ **Generic interfaces** ile type-safe çalışma
- ✅ **Fluent API** ile kolay konfigürasyon
- ✅ **Comprehensive logging** ve error handling
- ✅ **Hangfire Dashboard** entegrasyonu
- ✅ **Legacy sistem ile uyumluluk**

## 🔄 Klasik Sistem vs Yeni Sistem

### ❌ Eski Sistem (GenesisHR Pattern)
```csharp
// Karmaşık, tekrar eden kod
public class ReportApprovalJob : HangfireJobBase<ReportApprovalInput>
{
    public override void ExecuteJob(PerformContext aContext, ReportApprovalInput aParams)
    {
        // Manuel tenant switching
        // Manuel principal management
        // Tekrar eden kod...
    }
}
```

### ✅ Yeni Sistem (Fitliyo Generic Pattern)
```csharp
// Temiz, generic, yeniden kullanılabilir
public class ReportJobHandler : BaseJobHandler<ReportJobInput>
{
    protected override async Task ExecuteInternalAsync(ReportJobInput input)
    {
        // Tenant/Principal management otomatik!
        // Sadece iş mantığına odaklan
        await _reportAppService.ApprovalForJobAsync();
    }
}
```

## 🛠 Nasıl Kullanılır

### 1️⃣ Job Input Tanımla
```csharp
public class MyJobInput : JobInput
{
    public string MyParameter { get; set; }
    public DateTime ProcessDate { get; set; }
}
```

### 2️⃣ Job Handler Yaz
```csharp
public class MyJobHandler : BaseJobHandler<MyJobInput>
{
    public MyJobHandler(
        ILogger<MyJobHandler> logger,
        ICurrentTenant currentTenant,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IUnitOfWorkManager unitOfWorkManager,
        IdentityUserManager userManager,
        IMyAppService myAppService) // İhtiyacın olan servisleri inject et
        : base(logger, currentTenant, currentPrincipalAccessor, unitOfWorkManager, userManager)
    {
        _myAppService = myAppService;
    }

    protected override async Task ExecuteInternalAsync(MyJobInput input)
    {
        // İş mantığın burada - tenant/principal management otomatik!
        await _myAppService.DoSomethingAsync(input.MyParameter);
    }
}
```

### 3️⃣ Job Enqueuer Yaz
```csharp
public class MyJobEnqueuer : BaseJobEnqueuer<MyJobInput, MyJobHandler>
{
    public MyJobEnqueuer(
        IBackgroundJobClient backgroundJobClient,
        ILogger<MyJobEnqueuer> logger)
        : base(backgroundJobClient, logger, "my-queue")
    {
    }

    public void CreateDailyJob()
    {
        var input = new MyJobInput
        {
            MyParameter = "daily-value",
            ProcessDate = DateTime.Today
        }.WithTenant(tenantId);

        CreateRecurringJob(
            "daily-my-job",
            input,
            CronExpressions.DailyAt2AM);
    }
}
```

### 4️⃣ DI'da Register Et
```csharp
// FitliyoHangfireModule.cs içinde
context.Services.AddJob<MyJobInput, MyJobHandler, MyJobEnqueuer>();
```

## 🎯 Örnekler

### Manuel Job Çalıştırma
```csharp
public class MyAppService
{
    private readonly MyJobEnqueuer _jobEnqueuer;

    public async Task TriggerManualProcessAsync(string parameter)
    {
        var input = new MyJobInput
        {
            MyParameter = parameter,
            ProcessDate = DateTime.Now
        }
        .WithTenant(CurrentTenant.Id)
        .WithUser(CurrentUser.GetId(), CurrentUser.UserName)
        .WithTimeout(60); // 60 dakika

        var jobId = _jobEnqueuer.EnqueueJob(input);
        Logger.LogInformation("Job started: {JobId}", jobId);
    }
}
```

### Scheduled Job
```csharp
var input = new MyJobInput { MyParameter = "test" }
    .WithTenant(tenantId);

var jobId = _jobEnqueuer.ScheduleJob(input, TimeSpan.FromHours(2));
```

### Recurring Job
```csharp
_jobEnqueuer.CreateRecurringJob(
    "weekly-report",
    input,
    CronExpressions.WeeklyMondayAt2AM);
```

### Fluent Configuration
```csharp
var input = new ReportJobInput
{
    OperationType = ReportJobOperationTypes.Sync,
    BatchSize = 100
}
.WithTenant(tenantId)
.WithUser(userId, "admin")
.WithSubTenant(subTenantId)
.WithMetadata(JsonSerializer.Serialize(additionalData))
.WithTimeout(120); // 2 saat
```

## 📊 Job Monitoring

### Hangfire Dashboard
```
https://localhost:44332/hangfire
```

Dashboard'ta görebilirsiniz:
- ✅ **Recurring Jobs:** Zamanlı job'lar
- ✅ **Queued:** Beklemede olan job'lar
- ✅ **Processing:** Çalışan job'lar
- ✅ **Succeeded/Failed:** Başarılı/başarısız job'lar
- ✅ **Retry:** Otomatik yeniden deneme

### Job Durumları
```csharp
// Job kontrolü
var isRunning = _jobEnqueuer.IsJobRunning(jobId);
var result = _jobEnqueuer.GetJobResult(jobId);

// Job management
_jobEnqueuer.DeleteJob(jobId);      // Job'ı sil
_jobEnqueuer.RetryJob(jobId);       // Yeniden dene
_jobEnqueuer.RemoveRecurringJob("job-id"); // Recurring job'ı kaldır
```

## 🔧 Cron Expressions Helper

```csharp
using Fitliyo.Hangfire.Extensions;

// Hazır cron expression'lar
CronExpressions.EveryMinute        // "* * * * *"
CronExpressions.Every5Minutes     // "*/5 * * * *"
CronExpressions.Hourly            // "0 * * * *"
CronExpressions.DailyAt2AM         // "0 2 * * *"
CronExpressions.DailyAt3AM         // "0 3 * * *"
CronExpressions.WeeklyMondayAt2AM  // "0 2 * * 1"
CronExpressions.MonthlyFirst       // "0 2 1 * *"
CronExpressions.MonthlyLast        // "0 2 L * *"

// Custom cron
CronExpressions.Custom(30, 14, "15", "*", "*"); // Her ayın 15'i saat 14:30
```

## 🎯 Best Practices

### ✅ DO's
- Job input'ları immutable yapın
- Heavy işlemler için chunk/batch kullanın
- Job süresini reasonable tutun (max 30 dakika)
- Error handling ve logging ekleyin
- Tenant context'e dikkat edin
- CancellationToken kullanın uzun işlemler için

### ❌ DON'Ts
- Job içinde UI thread'e access etmeyin
- Job içinde infinite loop yapmayın
- Çok fazla memory kullanmayın
- Database transaction'ları çok uzun tutmayın
- Exception'ları yutmayın

### Job Input Best Practices
```csharp
public class GoodJobInput : JobInput
{
    public int BatchSize { get; set; } = 100;
    public DateTime? ProcessDate { get; set; }
    public string OperationType { get; set; } = "default";

    // Helper method
    public GoodJobInput ForDate(DateTime date)
    {
        ProcessDate = date;
        return this;
    }
}
```

### Error Handling Pattern
```csharp
protected override async Task ExecuteInternalAsync(MyJobInput input)
{
    try
    {
        Logger.LogInformation("Processing {BatchSize} items", input.BatchSize);

        var items = await GetItemsToProcessAsync(input.BatchSize);
        var processedCount = 0;

        foreach (var item in items)
        {
            try
            {
                await ProcessItemAsync(item);
                processedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing item {ItemId}", item.Id);
                // Continue with next item
            }
        }

        Logger.LogInformation("Processed {ProcessedCount}/{TotalCount} items",
            processedCount, items.Count);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Critical error in job execution");
        throw; // Hangfire will retry
    }
}
```

## 🏗 Sistem Mimarisi

```
┌─────────────────────────────────────────────────┐
│                  Hangfire Dashboard             │
│              (Job Monitoring)                   │
└─────────────────────────────────────────────────┘
                         │
┌─────────────────────────────────────────────────┐
│                Job Factory                      │
│              (Type-Safe Creation)               │
└─────────────────────────────────────────────────┘
                         │
┌─────────────────────────────────────────────────┐
│              BaseJobEnqueuer<T>                 │
│         (Queue Management & Scheduling)        │
└─────────────────────────────────────────────────┘
                         │
┌─────────────────────────────────────────────────┐
│              BaseJobHandler<T>                  │
│        (Tenant/Principal Management)            │
│         - UnitOfWork Management                 │
│         - CurrentTenant.Change()                │
│         - CurrentPrincipalAccessor.Change()     │
│         - Authenticated Context Creation   🆕   │
│         - Admin User Context Creation           │
└─────────────────────────────────────────────────┘
                         │
┌─────────────────────────────────────────────────┐
│              Your Business Logic                │
│            (Domain Services, etc.)              │
└─────────────────────────────────────────────────┘
```

## 🔒 Authentication Context (v3.8)

Job'lar artık **authenticated principal** ile çalışır:

```csharp
// BaseJobHandler otomatik olarak authenticated principal oluşturur
var claims = new List<Claim>
{
    new Claim(AbpClaimTypes.UserId, jobUser.Id.ToString()),
    new Claim(AbpClaimTypes.UserName, jobUser.UserName ?? ""),
    // Roller eklenir...
};

// ⚠️ ÖNEMLİ: Authentication type ile oluşturulmalı
return new ClaimsPrincipal(new ClaimsIdentity(claims, "Job"));
```

**Neden Önemli:**
- Job'larda authorization kontrolü çalışır
- Permission check'ler doğru çalışır
- Danışman modu job'larda desteklenir
- Self-access bypass job'larda çalışır

**Detaylar:** [`docs/CHANGELOG.md`](../CHANGELOG.md) - v3.8

---

Bu sistem ile tüm job'larınızı standardize edebilir, maintenance yükünü azaltabilir ve güvenli şekilde multi-tenant environment'da çalıştırabilirsiniz! 🎉

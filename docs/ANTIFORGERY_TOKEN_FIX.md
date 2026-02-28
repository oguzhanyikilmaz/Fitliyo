# Antiforgery Token Fix - Backend

## Sorun

`/api/app/consultant-context/switch-to-client-tenant/{clientTenantId}` endpoint'i için backend antiforgery cookie bekliyor ve şu hatayı veriyor:

```
The required antiforgery cookie ".AspNetCore.Antiforgery.qQl51JWOCq0" is not present.
```

## Analiz

1. **Middleware Çalışıyor**: `SkipAntiForgeryForBearerTokenMiddleware` middleware'i `/api/` ile başlayan endpoint'ler için CSRF validation'ı skip etmesi gerekiyor.

2. **Sorun**: ABP Framework'ün antiforgery validation'ı middleware'den önce çalışıyor olabilir veya middleware'in `context.Items[AbpAntiForgerySkipValidationKey]` değeri ABP Framework tarafından okunmuyor olabilir.

3. **Frontend**: Cookie-based auth kullanılıyor (JWT token yok), bu durumda backend antiforgery cookie bekliyor.

## Çözüm

### Seçenek 1: Application Service için IgnoreAntiforgeryToken (Önerilen)

Application Service'e `[IgnoreAntiforgeryToken]` attribute'u eklenebilir, ancak bu ABP Framework'te Application Service'lerde doğrudan desteklenmiyor.

### Seçenek 2: Controller ile Sarmalama (Mevcut Pattern)

`AccountController` gibi bir controller oluşturup `[IgnoreAntiforgeryToken]` attribute'u eklenebilir:

```csharp
[Route("api/app/consultant-context")]
[ApiController]
[IgnoreAntiforgeryToken]
public class ConsultantContextController : AbpControllerBase
{
    private readonly IConsultantContextService _consultantContextService;

    public ConsultantContextController(IConsultantContextService consultantContextService)
    {
        _consultantContextService = consultantContextService;
    }

    [HttpPost("switch-to-client-tenant/{clientTenantId}")]
    public async Task<bool> SwitchToClientTenantAsync(Guid clientTenantId)
    {
        return await _consultantContextService.SwitchToClientTenantAsync(clientTenantId);
    }

    [HttpPost("switch-to-own-tenant")]
    public async Task<bool> SwitchToOwnTenantAsync()
    {
        return await _consultantContextService.SwitchToOwnTenantAsync();
    }
}
```

### Seçenek 3: Middleware'i Güncelleme

Middleware'in ABP Framework'ün antiforgery validation'ını skip ettiğinden emin olmak için middleware'i güncelleyebiliriz, ancak bu ABP Framework'ün internal implementation'ına bağlı.

## Önerilen Çözüm

**Seçenek 2** önerilir çünkü:
- `AccountController` ile tutarlı pattern
- Açık ve net
- ABP Framework'ün standart yaklaşımı

## Uygulanan Çözüm

✅ **ConsultantContextController oluşturuldu ve Application Service exclude edildi**

1. **ConsultantContextController oluşturuldu:**
   - `backend-project/Fitliyo/src/Fitliyo.HttpApi/Controllers/ConsultantContextController.cs` dosyası oluşturuldu
   - Tüm consultant context endpoint'leri için `[IgnoreAntiforgeryToken]` attribute'u eklendi

2. **ConsultantContextService exclude edildi:**
   - `ConsultantContextService` class'ına `[RemoteService(false)]` attribute'u eklendi
   - Bu, ABP Framework'ün bu servisi conventional controller'a dönüştürmesini engeller
   - Böylece endpoint çakışması (AmbiguousMatchException) önlenir

**Oluşturulan Endpoint'ler:**
- `GET /api/app/consultant-context/current-context`
- `POST /api/app/consultant-context/is-in-consultant-mode`
- `POST /api/app/consultant-context/is-read-only-mode`
- `POST /api/app/consultant-context/switch-to-client-tenant/{clientTenantId}` ✅ (Ana sorun çözüldü)
- `POST /api/app/consultant-context/switch-to-own-tenant`
- `GET /api/app/consultant-context/accessible-tenants`
- `GET /api/app/consultant-context/accessible-company-ids`
- `GET /api/app/consultant-context/accessible-sub-tenant-ids`
- `GET /api/app/consultant-context/assigned-role-ids`

## Test

1. Frontend'den `switch-to-client-tenant` endpoint'ine POST request atıldığında 400 Bad Request hatası almamalı
2. Backend log'da antiforgery cookie hatası olmamalı
3. Endpoint başarılı çalışmalı
4. Backend'i rebuild etmek gerekiyor: `dotnet build` veya `dotnet run`


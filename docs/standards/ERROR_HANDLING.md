# Hata Yönetimi Standardı (Backend)

**Kapsam:** API hata yanıtları, exception sınıfları, HTTP status eşlemesi, loglama seviyesi ve güvenlik (prod’da detay gizleme).
**Doküman Versiyonu:** v1.0
**Son Güncelleme:** 2026-01-03
**Sahip:** Backend Team
**Geçerlilik:** Fitliyo >= 2.0

## 1) Standart Hata Yanıtı (ABP)

ABP, API/AJAX isteklerinde hataları standart bir JSON formatında döndürür (**RemoteServiceErrorResponse**).

### 1.1 Client’a Gönderilecek Detay Seviyesi

ABP ayarı:

- `AbpExceptionHandlingOptions.SendExceptionsDetailsToClients`
- `AbpExceptionHandlingOptions.SendStackTraceToClients`

> **Kural:** Production ortamında **detay ve stack trace kapalı** olmalıdır. Development’ta debug için açılabilir.
> (ABP referansı: `AbpExceptionHandlingOptions` konfigürasyonu)

## 2) Exception Türleri ve Kullanım Kuralları

### 2.1 BusinessException (Tercih Edilen)

- İş kuralı ihlallerinde kullanılır.
- Error code taşıyabilir (`IHasErrorCode`).
- Default log seviyesi genellikle **Warning**’dir.

**Kural:**
- İş kuralı ihlali = `BusinessException` (veya `UserFriendlyException` değil)
- Exception içine **domain error code** eklenir

### 2.2 UserFriendlyException

- Kullanıcıya direkt mesaj göstermek için.
- Sadece gerçekten “kullanıcıya aynen göster” gereken, teknik olmayan durumlarda.

**Kural:**
- Domain/business rule için öncelik `BusinessException`
- “UI metni” gibi davranan mesajları domain’e yayma (localization key kullan)

### 2.3 Validation Hataları

- DTO validation (DataAnnotations / FluentValidation) hataları **400** ile döner.
- UI için “alan bazlı” hata listesi sunulmalıdır.

## 3) HTTP Status Kod Eşlemesi (Önerilen)

> ABP standart mapping üzerine ek olarak, özel error code’lar için `AbpExceptionHttpStatusCodeOptions` kullanılabilir.

Önerilen eşleştirme:

- **400 BadRequest**: Validation / yanlış format / eksik parametre
- **401 Unauthorized**: Kimlik doğrulama yok/expired
- **403 Forbidden**: Yetki yok (permission / scope / role)
- **404 NotFound**: Kayıt yok
- **409 Conflict**: Çakışma (ör. benzersiz kural ihlali, tarih çakışması vb.)
- **500 InternalServerError**: Beklenmeyen hata

## 4) Error Code Standardı

Tüm business hataları için error code formatı:

```
Fitliyo:<Module>:<NNNNN>
```

Örnek:

- `Fitliyo:UserLeave:00001` → İzin çakışması
- `Fitliyo:Authorization:00001` → Organizasyon erişim reddi

**Kural:**
- Yeni bir iş kuralı hatası ekleniyorsa:
  - `Fitliyo.Domain.Shared` altında error code tanımlanır
  - İlgili doküman(lar)a “Error Scenarios” bölümü eklenir

## 5) Loglama Kuralları

### 5.1 Log Seviyesi

- BusinessException: **Warning**
- Yetkisiz erişim denemeleri: **Warning** (gerekirse audit)
- Beklenmeyen exception: **Error**

### 5.2 Hassas Veri

**Asla loglanmayacaklar:**
- Password, refresh token, access token
- Kimlik doğrulama cookie’leri
- Kişisel hassas veriler (KVKK)

## 6) Kural: Her Yeni Geliştirmede Doküman Güncelle

Bu standart, [`docs/standards/DOCUMENTATION_WORKFLOW.md`](../standards/DOCUMENTATION_WORKFLOW.md) ile birlikte zorunludur.



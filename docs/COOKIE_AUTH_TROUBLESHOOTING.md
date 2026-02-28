# Cookie Authentication Troubleshooting

## Sorun: Frontend cookie almıyor

### Adım 1: Backend'i Rebuild ve Restart

Backend kodunda yapılan değişikliklerin etkili olması için:

```bash
cd backend-project/Fitliyo
dotnet build
# Backend'i restart edin (dotnet run veya servisi yeniden başlatın)
```

### Adım 2: Browser DevTools Kontrolü

1. **Network Tab** → `/api/account/login-with-tenant` isteğini bulun
2. **Response Headers** → `Set-Cookie` header'ı var mı?
   - Varsa: Cookie backend tarafından gönderiliyor, frontend tarafında sorun var
   - Yoksa: Backend cookie göndermiyor, backend ayarlarını kontrol edin

3. **Application Tab** → Cookies → `http://localhost:3000`
   - `.AspNetCore.Identity.Application` cookie'si var mı?
   - Domain: `localhost` veya `.localhost` olmalı
   - Path: `/` olmalı
   - SameSite: `Lax` (development) olmalı
   - Secure: `false` (development, HTTP) veya `true` (production, HTTPS) olmalı

### Adım 3: Backend Log Kontrolü

Backend console/log'larında şu hataları kontrol edin:
- Cookie set edilirken hata var mı?
- CORS hatası var mı?
- Authentication hatası var mı?

### Adım 4: Yapılandırma Kontrolü

**Dosya:** `src/Fitliyo.Web/FitliyoWebModule.cs` (Satır 457-472)

```csharp
if (hostingEnvironment.IsDevelopment())
{
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ Development: Lax
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ✅ Development: HTTP'de çalışır
}
```

**Kontrol Listesi:**
- ✅ `SameSite = Lax` (development)
- ✅ `SecurePolicy = SameAsRequest` (development, HTTP için)
- ✅ `HttpOnly = true`
- ✅ CORS `AllowCredentials()` aktif (satır 225, 245)

### Adım 5: CORS Kontrolü

**Dosya:** `src/Fitliyo.Web/FitliyoWebModule.cs` (Satır 209-256)

Development modunda:
- ✅ `AllowCredentials()` aktif
- ✅ `SetIsOriginAllowed` localhost'u kabul ediyor (satır 235-242)

### Yaygın Sorunlar

1. **Cookie Domain Sorunu**
   - Cookie domain `localhost` olmalı (domain belirtilmemeli)
   - Bizim kodda domain belirtilmemiş ✅ (default davranış doğru)

2. **SameSite Sorunu**
   - Development'ta `Lax` yeterli ✅
   - Production'da cross-origin için `None` + `Secure` gerekli ✅

3. **CORS + Cookie Sorunu**
   - `AllowCredentials()` aktif mi? ✅
   - Frontend `credentials: 'include'` gönderiyor mu? ✅ (frontend'de var)

4. **Backend Restart Edilmemiş**
   - Kod değişiklikleri için backend restart gerekli ⚠️

### Test Adımları

1. Backend'i rebuild/restart edin
2. Browser'ı tamamen kapatıp açın (cookie cache temizlemek için)
3. Login yapın
4. DevTools → Network → `/api/account/login-with-tenant` → Response Headers → `Set-Cookie` kontrol edin
5. DevTools → Application → Cookies → Cookie var mı kontrol edin


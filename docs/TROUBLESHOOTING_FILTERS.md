# Filtreleme Sorunları Troubleshooting

## Sorun: Personel Kendi Verisini Göremiyor

### Kontrol Listesi

#### 1. Logout/Login Yaptınız mı?
**ÖNEMLİ:** Eski session eski claim'leri ve cache'i taşır!

```bash
# Logout yapın
# Tarayıcı cache temizleyin (veya Incognito)
# Yeniden login yapın
```

#### 2. CurrentUserId Set Ediliyor mu?

**Middleware Log Kontrolü:**
```bash
# Terminal'de ara:
grep "CurrentUserId set edildi" logs/...
```

**Kod Konumu:**
- `SubTenantResolverMiddleware.cs` - Line 257, 387
- Her iki akışta da (OrganizationAccessProvider + Claims fallback) set edilmeli

#### 3. IHasUserId Filter Aktif mi?

**Filter Mantığı:**
```csharp
Veri görünür eğer:
├─ !IsUserDataFilterEnabled      → Admin için false
├─ !IsUserAuthenticated           → Login süreci
├─ HasHoldingAccess               → Admin için true
├─ CurrentUserId == entity.UserId → KENDİ verisi ✅
└─ SubordinateUserIds.Contains()  → AST verisi
```

**SQL Debug:**
```sql
-- Filter'ın ürettiği SQL'i görmek için:
-- Development'ta EF logging aktif olmalı
```

#### 4. OrganizationAccessProvider Çalışıyor mu?

**Log Kontrolü:**
```bash
grep "OrganizationAccessProvider" logs/...
grep "Accessible ID.*claim.*eklendi" logs/...
```

**Cache Kontrolü:**
```bash
# Memory cache TTL: 30s
# İlk request'te DB'ye gidiyor, sonrakiler cache'den
```

#### 5. SubordinateUserIds Hesaplanıyor mu?

**Log Kontrolü:**
```bash
grep "SubordinateUserIds" logs/...
```

**Kod Konumu:**
- `SetupSubordinateUserIdsDirectAsync` - Line 793
- Cache'li çalışır (30s TTL)

### Manuel Test

**Browser Console:**
```javascript
// Claim'leri kontrol et
document.cookie

// CurrentUserId claim'de mi?
// HasHoldingAccess claim'de mi?
// OrganizationRole claim'de mi?
```

### Sorun Devam Ediyorsa

**1. Cache Temizle:**
```bash
# Sistemi tamamen kapat
pkill -9 -f "dotnet.*Fitliyo"

# Redis'i flush et (opsiyonel)
redis-cli FLUSHALL

# Yeniden başlat
cd src/Fitliyo.Web && dotnet run
```

**2. Debug Mode:**
```csharp
// FitliyoDbContext.cs - ApplyUserDataFilterGeneric metoduna log ekle:
_logger.LogInformation(
    "Filter check: IsUserDataFilterEnabled={IsUserDataFilterEnabled}, " +
    "IsUserAuthenticated={IsUserAuthenticated}, HasHoldingAccess={HasHoldingAccess}, " +
    "CurrentUserId={CurrentUserId}, SubordinateCount={SubordinateCount}",
    IsUserDataFilterEnabled, IsUserAuthenticated, HasHoldingAccess,
    CurrentUserId, SubordinateUserIds.Count);
```

**3. Filter Devre Dışı Bırak (Geçici Test):**
```csharp
// FitliyoDbContext.cs:
protected virtual bool IsUserDataFilterEnabled => false; // Test için
```

### Beklenen Davranış

| Kullanıcı | CurrentUserId | SubordinateUserIds | HasHoldingAccess | Sonuç |
|-----------|---------------|-------------------|------------------|-------|
| **Admin** | Set | - | **true** | Tümü görür |
| **Yönetici** | Set | [astlar] | false | Kendi + astlar |
| **Personel** | **Set** | [] veya [kendisi] | false | **Sadece kendisi** |

---

**Son Güncelleme:** 2026-01-20
**Durum:** Troubleshooting aktif

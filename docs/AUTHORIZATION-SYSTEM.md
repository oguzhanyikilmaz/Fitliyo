# Fitliyo Yetkilendirme Sistemi (Marketplace)

**Kapsam:** Rol Bazlı Yetkilendirme + Sahiplik Kontrolü + Guest Erişimi
**Doküman Versiyonu:** v4.0
**Son Güncelleme:** 2026-02-28
**Sahip:** Backend Team

---

## 📋 İçindekiler

1. [Genel Bakış](#1-genel-bakış)
2. [Kullanıcı Rolleri](#2-kullanıcı-rolleri)
3. [Permission Yapısı](#3-permission-yapısı)
4. [Yetkilendirme Akışı](#4-yetkilendirme-akışı)
5. [Sahiplik Kontrolü (Ownership)](#5-sahiplik-kontrolü-ownership)
6. [Guest Erişimi](#6-guest-erişimi)
7. [Modül Bazlı Yetkiler](#7-modül-bazlı-yetkiler)
8. [Kullanım Örnekleri](#8-kullanım-örnekleri)
9. [Best Practices](#9-best-practices)

---

## 1. Genel Bakış

Fitliyo marketplace yetkilendirme sistemi **5 temel rol** ile çalışır. HR sistemlerindeki hiyerarşik organizasyon yapısı yerine, **kullanıcı tipi bazlı** yetkilendirme ve **sahiplik kontrolü** kullanılır.

### 1.1 Temel Prensipler

- **Rol bazlı erişim**: Her kullanıcı bir role sahiptir (SuperAdmin, Admin, Trainer, Student, Guest)
- **Sahiplik kontrolü**: Trainer kendi profil/paket/takvimini yönetir, Student kendi siparişlerini görür
- **Guest erişimi**: Anonim kullanıcılar arama yapabilir ve profil görüntüleyebilir
- **Escrow güvenliği**: Ödeme işlemleri platform tarafından yönetilir, doğrudan erişim yok

---

## 2. Kullanıcı Rolleri

### 2.1 Rol Tanımları

```csharp
public enum UserType
{
    /// <summary>
    /// Eğitmen - Profil, paket, takvim yönetimi
    /// </summary>
    Trainer = 1,

    /// <summary>
    /// Öğrenci - Paket satın alma, seans takibi
    /// </summary>
    Student = 2,

    /// <summary>
    /// Yönetici - Platform yönetimi
    /// </summary>
    Admin = 3
}
```

### 2.2 Rol Detayları

#### SuperAdmin
- **Yetki**: Tam platform erişimi
- **İşlemler**: Tüm CRUD, sistem konfigürasyonu, kullanıcı yönetimi, ödeme yönetimi
- **Kapsam**: Tüm veriler

#### Admin
- **Yetki**: Kullanıcı/içerik yönetimi
- **İşlemler**: Moderasyon, destek talepleri, raporlar, featured listeleme, anlaşmazlık çözümü
- **Kapsam**: Tüm kullanıcı verileri (finansal hassas veriler hariç)

#### Trainer (Eğitmen)
- **Yetki**: Kendi profil ve iş yönetimi
- **İşlemler**: Profil CRUD, paket CRUD, takvim yönetimi, seans yönetimi, mesajlaşma, wallet
- **Kapsam**: Kendi profili, paketleri, siparişleri, wallet'ı
- **Alt roller** (uzmanlık etiketi): PersonalTrainer, Dietitian, BasketballCoach, FootballCoach, TennisCoach, SwimmingCoach, YogaInstructor, Other

#### Student (Öğrenci)
- **Yetki**: Satın alma ve iletişim
- **İşlemler**: Paket satın alma, sipariş takibi, seans takibi, mesajlaşma, yorum yazma
- **Kapsam**: Kendi siparişleri, seansları, mesajları

#### Guest (Misafir)
- **Yetki**: Sadece görüntüleme
- **İşlemler**: Eğitmen arama, profil görüntüleme, paket inceleme, blog okuma
- **Kapsam**: Public veriler

### 2.3 Rol-Yetki Matrisi

| İşlem | SuperAdmin | Admin | Trainer | Student | Guest |
|-------|-----------|-------|---------|---------|-------|
| Eğitmen arama/listeleme | ✅ | ✅ | ✅ | ✅ | ✅ |
| Profil görüntüleme | ✅ | ✅ | ✅ | ✅ | ✅ |
| Paket görüntüleme | ✅ | ✅ | ✅ | ✅ | ✅ |
| Eğitmen profil yönetimi | ✅ | ✅ | 🟡* | ❌ | ❌ |
| Paket CRUD | ✅ | ✅ | 🟡* | ❌ | ❌ |
| Sipariş oluşturma | ✅ | ❌ | ❌ | ✅ | ❌ |
| Sipariş yönetimi | ✅ | ✅ | 🟡* | 🟡* | ❌ |
| Ödeme işlemleri | ✅ | ✅ | ❌ | ✅ | ❌ |
| Wallet yönetimi | ✅ | ✅ | 🟡* | ❌ | ❌ |
| Para çekme talebi | ❌ | ❌ | ✅ | ❌ | ❌ |
| Para çekme onayı | ✅ | ✅ | ❌ | ❌ | ❌ |
| Mesajlaşma | ✅ | ✅ | ✅ | ✅ | ❌ |
| Yorum yazma | ❌ | ❌ | ❌ | ✅ | ❌ |
| Yorum yanıtlama | ❌ | ❌ | 🟡* | ❌ | ❌ |
| Blog yazma | ✅ | ✅ | ✅ | ❌ | ❌ |
| Kullanıcı yönetimi | ✅ | ✅ | ❌ | ❌ | ❌ |
| Raporlar | ✅ | ✅ | ❌ | ❌ | ❌ |
| Sistem konfigürasyonu | ✅ | ❌ | ❌ | ❌ | ❌ |

**🟡* Sahiplik kontrolü:** Sadece kendi verileri üzerinde işlem yapabilir.

---

## 3. Permission Yapısı

### 3.1 Permission Tanımları

```csharp
public static class FitliyoPermissions
{
    public const string GroupName = "Fitliyo";

    public static class Trainers
    {
        public const string Default = GroupName + ".Trainers";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Verify = Default + ".Verify";
    }

    public static class Packages
    {
        public const string Default = GroupName + ".Packages";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
        public const string Create = Default + ".Create";
        public const string Cancel = Default + ".Cancel";
        public const string Refund = Default + ".Refund";
    }

    public static class Payments
    {
        public const string Default = GroupName + ".Payments";
        public const string ApproveWithdrawal = Default + ".ApproveWithdrawal";
    }

    public static class Reviews
    {
        public const string Default = GroupName + ".Reviews";
        public const string Create = Default + ".Create";
        public const string Moderate = Default + ".Moderate";
    }

    public static class Admin
    {
        public const string Dashboard = GroupName + ".Admin.Dashboard";
        public const string UserManagement = GroupName + ".Admin.UserManagement";
        public const string Reports = GroupName + ".Admin.Reports";
        public const string SystemConfig = GroupName + ".Admin.SystemConfig";
    }
}
```

### 3.2 Rol-Permission Eşleştirmesi

| Permission | SuperAdmin | Admin | Trainer | Student |
|-----------|-----------|-------|---------|---------|
| `Fitliyo.Trainers` | ✅ | ✅ | ✅ | ✅ |
| `Fitliyo.Trainers.Create` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Trainers.Edit` | ✅ | ✅ | 🟡 | ❌ |
| `Fitliyo.Trainers.Delete` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Trainers.Verify` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Packages` | ✅ | ✅ | ✅ | ✅ |
| `Fitliyo.Packages.Create` | ✅ | ❌ | ✅ | ❌ |
| `Fitliyo.Packages.Edit` | ✅ | ❌ | 🟡 | ❌ |
| `Fitliyo.Packages.Delete` | ✅ | ✅ | 🟡 | ❌ |
| `Fitliyo.Orders` | ✅ | ✅ | 🟡 | 🟡 |
| `Fitliyo.Orders.Create` | ❌ | ❌ | ❌ | ✅ |
| `Fitliyo.Orders.Cancel` | ✅ | ✅ | 🟡 | 🟡 |
| `Fitliyo.Orders.Refund` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Reviews.Create` | ❌ | ❌ | ❌ | ✅ |
| `Fitliyo.Reviews.Moderate` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Payments.ApproveWithdrawal` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Admin.*` | ✅ | ✅ | ❌ | ❌ |
| `Fitliyo.Admin.SystemConfig` | ✅ | ❌ | ❌ | ❌ |

🟡 = Sahiplik kontrolü ile (sadece kendi verileri)

---

## 4. Yetkilendirme Akışı

```
┌─────────────────────────────────────────────────────────────────────┐
│                    YETKİLENDİRME AKIŞI (Marketplace)                │
│                                                                      │
│  İstek → [Auth Check] → [Rol Kontrolü] → [Sahiplik] → İZİN/RED    │
│             ↓              ↓                ↓                        │
│       "Kim bu?"      "NE yapabilir?"    "KİMİN verisi?"             │
│     (JWT Token)      (Permission)       (Ownership)                  │
│                                                                      │
│  Örnekler:                                                           │
│  - Guest → [AllowAnonymous] → Profil görüntüleme → İZİN             │
│  - Student → [Authorize] → Sipariş oluşturma → İZİN                 │
│  - Trainer → [Authorize] → Kendi paketi düzenleme → Sahiplik → İZİN │
│  - Trainer → [Authorize] → Başkasının paketi → Sahiplik → RED       │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 5. Sahiplik Kontrolü (Ownership)

### 5.1 Trainer Sahipliği

Eğitmenler sadece **kendi** profil, paket, takvim ve wallet verilerine erişebilir:

```csharp
public class ServicePackageAppService : FitliyoAppService
{
    [Authorize(FitliyoPermissions.Packages.Edit)]
    public async Task<ServicePackageDto> UpdateAsync(Guid id, CreateUpdateServicePackageDto input)
    {
        var package = await _repository.GetAsync(id);
        var trainerProfile = await GetCurrentTrainerProfileAsync();

        if (package.TrainerProfileId != trainerProfile.Id)
            throw new AbpAuthorizationException("Bu paketi düzenleme yetkiniz yok.");

        // güncelleme...
    }

    private async Task<TrainerProfile> GetCurrentTrainerProfileAsync()
    {
        var profile = await _trainerProfileRepository.FindAsync(x => x.UserId == CurrentUser.GetId());
        if (profile == null)
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        return profile;
    }
}
```

### 5.2 Student Sahipliği

Öğrenciler sadece **kendi** sipariş, seans ve yorum verilerine erişebilir:

```csharp
[Authorize(FitliyoPermissions.Orders.Default)]
public async Task<OrderDto> GetAsync(Guid id)
{
    var order = await _repository.GetAsync(id);

    if (order.StudentUserId != CurrentUser.GetId())
        throw new AbpAuthorizationException("Bu siparişe erişim yetkiniz yok.");

    return ObjectMapper.Map<Order, OrderDto>(order);
}
```

### 5.3 Admin Override

Admin ve SuperAdmin rolleri sahiplik kontrolünü **bypass** eder:

```csharp
private async Task EnsureOwnershipOrAdminAsync(Guid ownerUserId)
{
    if (CurrentUser.IsInRole("SuperAdmin") || CurrentUser.IsInRole("Admin"))
        return;

    if (ownerUserId != CurrentUser.GetId())
        throw new AbpAuthorizationException("Bu veriye erişim yetkiniz yok.");
}
```

---

## 6. Guest Erişimi

Guest (anonim) kullanıcılar `[AllowAnonymous]` attribute'u ile işaretlenen endpoint'lere erişebilir:

```csharp
[AllowAnonymous]
public async Task<PagedResultDto<TrainerProfileListDto>> SearchTrainersAsync(SearchTrainersInput input)
{
    // Eğitmen arama — anonim erişim
}

[AllowAnonymous]
public async Task<TrainerProfileDetailDto> GetBySlugAsync(string slug)
{
    // Profil detay görüntüleme — anonim erişim
}

[AllowAnonymous]
public async Task<PagedResultDto<ServicePackageDto>> GetPackagesByTrainerAsync(Guid trainerProfileId)
{
    // Eğitmenin paketlerini listeleme — anonim erişim
}
```

### Guest Erişimi Olan Endpoint'ler

| Endpoint | Açıklama |
|----------|----------|
| Eğitmen arama/listeleme | Filtreleme, sıralama ile |
| Eğitmen profil detayı | Slug ile |
| Paket listeleme | Eğitmene göre |
| Kategori listeleme | Tüm kategoriler |
| Blog listeleme/detay | Yayınlanmış makaleler |
| Yorum listeleme | Eğitmene göre |

---

## 7. Modül Bazlı Yetkiler

| Modül | Guest | Student | Trainer | Admin | SuperAdmin |
|-------|-------|---------|---------|-------|-----------|
| TrainerModule (okuma) | ✅ | ✅ | ✅ | ✅ | ✅ |
| TrainerModule (yazma) | ❌ | ❌ | 🟡 | ✅ | ✅ |
| PackageModule (okuma) | ✅ | ✅ | ✅ | ✅ | ✅ |
| PackageModule (yazma) | ❌ | ❌ | 🟡 | ❌ | ✅ |
| OrderModule | ❌ | 🟡 | 🟡 | ✅ | ✅ |
| PaymentModule | ❌ | 🟡 | 🟡 | ✅ | ✅ |
| SubscriptionModule | ❌ | ❌ | 🟡 | ✅ | ✅ |
| MessagingModule | ❌ | ✅ | ✅ | ✅ | ✅ |
| ReviewModule (okuma) | ✅ | ✅ | ✅ | ✅ | ✅ |
| ReviewModule (yazma) | ❌ | ✅ | ❌ | ✅ | ✅ |
| NotificationModule | ❌ | 🟡 | 🟡 | ✅ | ✅ |
| ContentModule (okuma) | ✅ | ✅ | ✅ | ✅ | ✅ |
| ContentModule (yazma) | ❌ | ❌ | ✅ | ✅ | ✅ |
| AdminModule | ❌ | ❌ | ❌ | ✅ | ✅ |

🟡 = Sahiplik kontrolü ile

---

## 8. Kullanım Örnekleri

### 8.1 Trainer AppService

```csharp
[Authorize]
public class TrainerProfileAppService : FitliyoAppService
{
    [AllowAnonymous]
    public async Task<TrainerProfileDetailDto> GetBySlugAsync(string slug)
    {
        // Guest erişimi — herkes görebilir
    }

    [Authorize(FitliyoPermissions.Trainers.Edit)]
    public async Task<TrainerProfileDto> UpdateMyProfileAsync(UpdateTrainerProfileDto input)
    {
        var profile = await GetCurrentTrainerProfileAsync();
        // Sadece kendi profilini güncelleyebilir
    }

    [Authorize(FitliyoPermissions.Trainers.Verify)]
    public async Task VerifyTrainerAsync(Guid trainerProfileId)
    {
        // Sadece Admin/SuperAdmin
    }
}
```

### 8.2 Order AppService

```csharp
[Authorize]
public class OrderAppService : FitliyoAppService
{
    [Authorize(FitliyoPermissions.Orders.Create)]
    public async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        // Sadece Student sipariş oluşturabilir
        if (CurrentUser.UserType != UserType.Student)
            throw new AbpAuthorizationException();
    }

    [Authorize(FitliyoPermissions.Orders.Default)]
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _repository.GetAsync(id);
        // Student kendi siparişini, Trainer kendi siparişlerini, Admin hepsini görebilir
        await EnsureOrderAccessAsync(order);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
}
```

---

## 9. Best Practices

### ✅ Yapılması Gerekenler

1. **Her endpoint'te açık yetkilendirme belirt**
   ```csharp
   [Authorize(FitliyoPermissions.Packages.Create)]  // Spesifik permission
   [AllowAnonymous]                                   // Veya açıkça anonim
   ```

2. **Sahiplik kontrolü yap**
   ```csharp
   if (package.TrainerProfileId != currentTrainerProfile.Id)
       throw new AbpAuthorizationException();
   ```

3. **Guest endpoint'leri minimal veri döndürsün**
   ```csharp
   // Guest'e BankAccountInfo, IBAN gibi hassas veri dönme
   ```

### ❌ Yapılmaması Gerekenler

1. **Class-level policy kullanma**
   ```csharp
   // YANLIŞ
   [Authorize(FitliyoPermissions.Trainers.Default)]
   public class TrainerAppService { }

   // DOĞRU
   [Authorize]
   public class TrainerAppService { }
   ```

2. **Sahiplik kontrolünü atlatma**
   ```csharp
   // YANLIŞ — herkes erişebilir
   public async Task<OrderDto> GetAsync(Guid id)
   {
       return await _repository.GetAsync(id);
   }
   ```

3. **Hardcoded rol kontrolü**
   ```csharp
   // YANLIŞ
   if (CurrentUser.Roles.Contains("Admin")) { }

   // DOĞRU — Permission bazlı kontrol
   await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);
   ```

---

## İlgili Dökümanlar

| Döküman | Açıklama |
|---------|----------|
| [CHANGELOG.md](./CHANGELOG.md) | Tüm değişiklik geçmişi |
| [BUSINESS-RULES.md](./BUSINESS-RULES.md) | İş kuralları (ödeme, komisyon) |
| [standards/ERROR_HANDLING.md](./standards/ERROR_HANDLING.md) | Hata yönetimi standartları |

---

**Son Güncelleme:** 2026-02-28
**Doküman Versiyonu:** v4.0

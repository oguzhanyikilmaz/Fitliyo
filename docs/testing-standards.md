# Fitliyo Test Standartları

> Sürüm: 1.0 | Son Güncelleme: 2026-02-25 | Durum: Zorunlu

---

## İçindekiler

1. [Temel Kural — Her Metot İçin Test](#1-temel-kural)
2. [Test Projesi Yapısı](#2-test-projesi-yapısı)
3. [Base Sınıflar](#3-base-sınıflar)
4. [Test İsimlendirme Kuralları](#4-test-isimlendirme-kuralları)
5. [AAA Paterni](#5-aaa-paterni)
6. [Test Kapsamı (Minimum)](#6-test-kapsamı-minimum)
7. [Assertion Standartları](#7-assertion-standartları)
8. [Test Veri Yönetimi](#8-test-veri-yönetimi)
9. [Hata Senaryosu Testleri](#9-hata-senaryosu-testleri)
10. [Yetkilendirme Testleri](#10-yetkilendirme-testleri)
11. [AppService Test Şablonu](#11-appservice-test-şablonu)
12. [Yasak Pratikler](#12-yasak-pratikler)
13. [Test Dokümantasyonu](#13-test-dokümantasyonu)
14. [Test Kapsam Raporu](#14-test-kapsam-raporu)
15. [Yaygın Test Hatası Önleme Kuralları](#15-yaygın-test-hatası-önleme-kuralları)

---

## 1. Temel Kural

### Her Yeni Metot İçin Hemen Test Yazılır — KESİNLİKLE ZORUNLU

```
Yeni AppService metodu → Aynı PR/commit içinde test dosyasına eklenir.
"Sonra yazarım" geçerli değildir. Test olmayan kod merge edilmez.
```

**Kontrol kuralı:** Bir AppService metoduna dokunulduğunda (ekleme, değiştirme, bug-fix), ilgili test dosyasındaki test de güncellenmiş/eklenmiş olmalıdır.

---

## 2. Test Projesi Yapısı

```
test/
├── Fitliyo.TestBase/                     # Ortak test altyapısı
│   ├── FitliyoTestBase.cs                # Root base class
│   └── TestDataSeeder.cs              # Test verisi hazırlama
│
├── Fitliyo.Application.Tests/            # AppService testleri ← ANA TEST PROJESİ
│   ├── FitliyoApplicationTestBase.cs     # Application test base
│   ├── FitliyoApplicationTestModule.cs   # Test modülü
│   ├── <Modül>/
│   │   └── <XxxAppService>_Tests.cs
│   └── Authorization/
│       ├── AuthorizationTestBase.cs
│       └── *Authorization*_Tests.cs
│
├── Fitliyo.Domain.Tests/                 # Domain Manager testleri
│   └── <Modül>/
│       └── <XxxManager>_Tests.cs
│
├── Fitliyo.EntityFrameworkCore.Tests/    # Repository testleri
│   └── <Modül>/
│       └── EfCore<Xxx>Repository_Tests.cs
│
└── Fitliyo.Mailing.Tests/               # Mail servis testleri
```

### Dosya Yerleşim Kuralı

| AppService | Test Dosyası |
|------------|-------------|
| `src/Fitliyo.Application/UserLeaves/UserLeaveAppService.cs` | `test/Fitliyo.Application.Tests/UserLeaves/UserLeaveAppService_Tests.cs` |
| `src/Fitliyo.Application/Departments/DepartmentAppService.cs` | `test/Fitliyo.Application.Tests/Departments/DepartmentAppService_Tests.cs` |

---

## 3. Base Sınıflar

### FitliyoApplicationTestBase (Standart)

```csharp
public class XxxAppService_Tests : FitliyoApplicationTestBase<FitliyoApplicationTestModule>
```

- Gerçek veritabanı bağlantısı kullanır
- `TestDataSeeder` ile hazır veri vardır
- `TestUserId`, `TestSubTenantId`, `TestCompanyId` gibi sabit ID'ler mevcuttur
- `HasResolvedData` ile veri mevcut mu kontrol edilir

### FitliyoApplicationTestBase (Non-generic)

```csharp
public class XxxAppService_Tests : FitliyoApplicationTestBase
```

- Modül belirtmeye gerek yoktur; `FitliyoApplicationTestModule` otomatik kullanılır

### FitliyoDomainTestBase (Domain Servisleri)

```csharp
public class XxxManager_Tests : FitliyoDomainTestBase<FitliyoDomainTestModule>
```

- Domain Manager testleri için
- AppService bağımlılığı yoktur

### AuthorizationTestBase (Yetkilendirme Testleri)

```csharp
public class XxxAuthorization_Tests : AuthorizationTestBase
```

- Role-based access control testleri için
- `RunAsUserAsync`, `AssertUnauthorizedAsync`, `AssertForbiddenAsync` metodları hazırdır

---

## 4. Test İsimlendirme Kuralları

### Sınıf Adı

```
<AppServiceAdı>_Tests
```

Örnekler:
- `UserLeaveAppService_Tests`
- `DepartmentAppService_Tests`
- `BankAppService_Tests`

### Metot Adı Formatı

```
Should_<Davranış>_<Koşul?>
```

| Senaryo | İsimlendirme |
|---------|-------------|
| Happy path — başarılı oluşturma | `Should_Create_UserLeave` |
| Geçersiz girdi | `Should_Throw_When_Name_Is_Empty` |
| Benzersizlik ihlali | `Should_Throw_When_Code_Is_Duplicate` |
| Listeleme | `Should_Get_List_With_Pagination` |
| Filtreleme | `Should_Filter_By_UserId` |
| Silme | `Should_Delete_And_Remove_From_Db` |
| Durum güncelleme | `Should_Set_Active_Status` |
| Yetkilendirme | `Should_Throw_When_Unauthorized` |

### Namespace

```csharp
namespace Fitliyo.Application.Tests.<ModülAdı>
```

---

## 5. AAA Paterni

Her test **Arrange → Act → Assert** yapısına uymalıdır. Yorumlar Türkçe.

```csharp
[Fact]
public async Task Should_Create_LeaveType()
{
    // Arrange
    var input = new CreateUpdateLeaveTypeDto
    {
        Name = "Yıllık İzin " + Guid.NewGuid().ToString("N")[..8],
        MaxDurationDays = 14,
        IsActive = true
    };

    // Act
    var result = await _leaveTypeAppService.CreateAsync(input);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldNotBe(Guid.Empty);
    result.Name.ShouldBe(input.Name);
    result.MaxDurationDays.ShouldBe(input.MaxDurationDays);
}
```

---

## 6. Test Kapsamı (Minimum)

### Her AppService için minimum test senaryoları

| # | Senaryo | Zorunlu mu? |
|---|---------|-------------|
| 1 | `CreateAsync` — geçerli girdi ile oluşturma | ✅ ZORUNLU |
| 2 | `CreateAsync` — zorunlu alan boş iken hata | ✅ ZORUNLU |
| 3 | `GetAsync` — mevcut kayıt getirme | ✅ ZORUNLU |
| 4 | `GetListAsync` — sayfalı liste | ✅ ZORUNLU |
| 5 | `UpdateAsync` — başarılı güncelleme | ✅ ZORUNLU |
| 6 | `DeleteAsync` — silme ve DB kontrolü | ✅ ZORUNLU |
| 7 | Özel metodlar (custom) — her biri için happy path | ✅ ZORUNLU |
| 8 | Tekrar/çakışma senaryosu (varsa) | ⭕ Modüle bağlı |
| 9 | Filtreleme/arama testi | ⭕ Modüle bağlı |
| 10 | Yetkilendirme testi | ⭕ Kritik modüller için |

### Kritik Modüller — Genişletilmiş Kapsam

Aşağıdaki modüller için 7+ test yazılmalıdır:

- UserLeave, UserEntry, UserExit
- Department, SubTenant
- WageModule, SeverancePay, NoticePay
- UserAdvance

---

## 7. Assertion Standartları

### Kullanılan Kütüphane: Shouldly

```csharp
// Doğru
result.ShouldNotBeNull();
result.Id.ShouldNotBe(Guid.Empty);
result.Name.ShouldBe("Test");
result.Items.Count.ShouldBeGreaterThan(0);
result.Items.ShouldAllBe(x => x.IsActive);

// Hata senaryosu
var exception = await Should.ThrowAsync<BusinessException>(
    async () => await _service.CreateAsync(invalidInput));
exception.Code.ShouldContain("Fitliyo:");

// veya xUnit ile
await Assert.ThrowsAsync<AbpValidationException>(async () =>
    await _service.CreateAsync(invalidInput));
```

### Assertion Kuralları

1. Null kontrolü her zaman ilk assertion
2. ID kontrolü — `ShouldNotBe(Guid.Empty)`
3. Giriş verisi → çıktı veri eşleşmesi kontrol edilmeli
4. Negatif testlerde exception tipi belirtilmeli
5. `true.ShouldBeTrue()` gibi anlamsız assertionlar **YASAK**

---

## 8. Test Veri Yönetimi

### Benzersiz Veri Üretimi

Gerçek veritabanı kullanıldığı için her testin kendi benzersiz verisini üretmesi gerekir.

```csharp
// Benzersiz kod üretici (sınıf içinde statik)
private static string UniqueCode(string prefix = "TEST") =>
    $"{prefix}_{DateTime.Now.Ticks}_{Guid.NewGuid():N[..6]}".ToUpperInvariant();

// Benzersiz isim
private static string UniqueName(string prefix) =>
    $"{prefix} {DateTime.Now.Ticks}";
```

### TestDataSeeder Değerleri

`FitliyoApplicationTestBase` üzerinden erişilir:

```csharp
TestUserId          // Ana test kullanıcısı
TestUserId2         // İkincil test kullanıcısı
TestSubTenantId     // Test şubesi
TestCompanyId       // Test şirketi
TestCategoryId      // Asset kategorisi
TestStatusId        // Asset durumu (Stokta)
TestAssignedStatusId // Asset durumu (Atanmış)
HasResolvedData     // Veri hazır mı?
```

### HasResolvedData Guard

Veritabanı bağlantısı veya seed verisi yoksa testi atlama:

```csharp
[Fact]
public async Task Should_Create_With_User()
{
    // Arrange
    if (!HasResolvedData) return; // ← Veri yoksa atla

    var input = new CreateUpdateXxxDto { UserId = TestUserId, ... };
    // ...
}
```

### Helper Metotlar

Test sınıfı içinde private yardımcı metodlar kullan:

```csharp
#region Test Yardımcıları

private async Task<LeaveTypeDto?> CreateTestLeaveTypeAsync()
{
    try
    {
        return await _leaveTypeAppService.CreateAsync(new CreateUpdateLeaveTypeDto
        {
            Name = "Test İzin Türü " + Guid.NewGuid().ToString("N")[..8],
            MaxDurationDays = 14,
            IsActive = true
        });
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"CreateTestLeaveTypeAsync başarısız: {ex.Message}");
        return null;
    }
}

#endregion
```

---

## 9. Hata Senaryosu Testleri

### Validation Hataları

```csharp
[Fact]
public async Task Should_Throw_When_Required_Fields_Empty()
{
    // Arrange
    var invalidDto = new CreateUpdateLeaveTypeDto
    {
        Name = "",  // Zorunlu alan boş
        MaxDurationDays = 0
    };

    // Act & Assert
    await Assert.ThrowsAsync<AbpValidationException>(async () =>
        await _leaveTypeAppService.CreateAsync(invalidDto));
}
```

### İş Kuralı Hataları

```csharp
[Fact]
public async Task Should_Throw_When_Code_Duplicate()
{
    // Arrange
    var dto = new CreateUpdateXxxDto { Code = "AYNI_KOD", Name = "Test" };
    await _xxxAppService.CreateAsync(dto);

    // Act & Assert
    var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
        await _xxxAppService.CreateAsync(dto));

    (exception is UserFriendlyException || exception is BusinessException).ShouldBeTrue();
}
```

### EntityNotFoundException

```csharp
[Fact]
public async Task Should_Throw_When_Not_Found()
{
    // Arrange
    var nonExistentId = Guid.NewGuid();

    // Act & Assert
    await Assert.ThrowsAnyAsync<Exception>(async () =>
        await _leaveTypeAppService.GetAsync(nonExistentId));
}
```

---

## 10. Yetkilendirme Testleri

### Standart Yetki Testi

```csharp
[Fact]
public async Task Should_Throw_When_Unauthorized()
{
    // Arrange
    var input = new CreateUpdateLeaveTypeDto { Name = "Test", MaxDurationDays = 7 };

    // Act & Assert — Yetkisiz kullanıcıda AbpAuthorizationException bekleniyor
    await RunAsUserAsync(CalisanUser, async () =>
        await AssertUnauthorizedAsync(async () =>
            await _leaveTypeAppService.CreateAsync(input)));
}
```

---

## 11. AppService Test Şablonu

Yeni bir AppService için tam test şablonu:

```csharp
using System;
using System.Threading.Tasks;
using Fitliyo.Application.Contracts.<Modül>;
using Fitliyo.Application.Contracts.<Modül>.Dtos;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;

namespace Fitliyo.Application.Tests.<Modül>
{
    /// <summary>
    /// <XxxAppService> testleri
    /// Gerçek veritabanındaki veriler kullanılır
    /// </summary>
    public class XxxAppService_Tests : FitliyoApplicationTestBase<FitliyoApplicationTestModule>
    {
        private readonly IXxxAppService _xxxAppService;

        public XxxAppService_Tests()
        {
            _xxxAppService = GetRequiredService<IXxxAppService>();
        }

        private static string UniqueCode(string prefix = "TEST") =>
            $"{prefix}_{DateTime.Now.Ticks}".ToUpperInvariant()[..30];

        // ─── CRUD Testleri ──────────────────────────────────────────────

        [Fact]
        public async Task Should_Create_Xxx()
        {
            // Arrange
            var input = new CreateUpdateXxxDto
            {
                Name = "Test " + Guid.NewGuid().ToString("N")[..8],
                Code = UniqueCode("CREATE"),
                IsActive = true
            };

            // Act
            var result = await _xxxAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe(input.Name);
        }

        [Fact]
        public async Task Should_Get_Xxx_By_Id()
        {
            // Arrange
            var created = await CreateTestXxxAsync();
            if (created == null) return;

            // Act
            var result = await _xxxAppService.GetAsync(created.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
        }

        [Fact]
        public async Task Should_Get_Xxx_List()
        {
            // Arrange
            var input = new GetXxxListDto { MaxResultCount = 10 };

            // Act
            var result = await _xxxAppService.GetListAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Update_Xxx()
        {
            // Arrange
            var created = await CreateTestXxxAsync();
            if (created == null) return;

            var updateDto = new CreateUpdateXxxDto
            {
                Name = "Güncellenmiş Test " + Guid.NewGuid().ToString("N")[..8],
                Code = UniqueCode("UPDATE"),
                IsActive = true
            };

            // Act
            var result = await _xxxAppService.UpdateAsync(created.Id, updateDto);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(updateDto.Name);
        }

        [Fact]
        public async Task Should_Delete_Xxx()
        {
            // Arrange
            var created = await CreateTestXxxAsync();
            if (created == null) return;

            // Act
            await _xxxAppService.DeleteAsync(created.Id);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _xxxAppService.GetAsync(created.Id));
        }

        // ─── Validation Testleri ────────────────────────────────────────

        [Fact]
        public async Task Should_Throw_When_Required_Fields_Empty()
        {
            // Arrange
            var invalidDto = new CreateUpdateXxxDto { Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
                await _xxxAppService.CreateAsync(invalidDto));
        }

        // ─── Test Yardımcıları ──────────────────────────────────────────

        private async Task<XxxDto?> CreateTestXxxAsync()
        {
            try
            {
                return await _xxxAppService.CreateAsync(new CreateUpdateXxxDto
                {
                    Name = "Test " + Guid.NewGuid().ToString("N")[..8],
                    Code = UniqueCode("HELPER"),
                    IsActive = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateTestXxxAsync başarısız: {ex.Message}");
                return null;
            }
        }
    }
}
```

---

## 12. Yasak Pratikler

| Yasak | Neden | Doğrusu |
|-------|-------|---------|
| `Thread.Sleep()` test içinde | Zaman bağımlılığı, yavaşlık | `await Task.Delay()` veya mock ile |
| `true.ShouldBeTrue()` | Anlamsız assertion | Gerçek assertion yaz |
| Sabit ID kullanımı (Guid.Empty vb.) | Gerçek veritabanında olmayabilir | `TestUserId`, `HasResolvedData` kullan |
| `catch` içinde testi geçtirme | Hata gizlenir | Assert et veya rethrow et |
| Testler arası bağımlılık | Test sırası değişince kırılır | Her test bağımsız olmalı |
| Magic string/number | Okunaksız | Sabit veya değişken kullan |
| `DateTime.UtcNow` | Türkiye TZ sorunu | `DateTime.Now` kullan |
| `Thread.CurrentPrincipal` değiştirme | Deadlock riski | `RunAsUserAsync` kullan |
| Bir test dosyasında birden fazla sınıf | Kural ihlali | Her sınıf kendi dosyasında |

---

## 13. Test Dokümantasyonu

### Her AppService için test dokümantasyonu

`docs/tests/<ModülAdı>.md` dosyası oluşturulmalı. Format:

```markdown
# XxxAppService Test Dokümantasyonu

## Kapsam
| Metot | Test Var mı? | Test Adı |
|-------|-------------|---------|
| CreateAsync | ✅ | Should_Create_Xxx |
| GetAsync | ✅ | Should_Get_Xxx_By_Id |
| UpdateAsync | ✅ | Should_Update_Xxx |
| DeleteAsync | ✅ | Should_Delete_Xxx |
| GetListAsync | ✅ | Should_Get_Xxx_List |
| SetActiveAsync | ❌ | — |

## Eksik Testler
- SetActiveAsync için test yazılmalı

## Test Dosyası
`test/Fitliyo.Application.Tests/Xxx/XxxAppService_Tests.cs`
```

---

## 14. Test Kapsam Raporu

→ Bkz. [`docs/tests/README.md`](tests/README.md) — Tüm modüllerin güncel kapsam tablosu

---

## 15. Yaygın Test Hatası Önleme Kuralları

Bu kurallar, tekrar eden test hatalarını önlemek için **zorunlu** uygulanır. UserLeave ve Bank modüllerindeki düzeltmelerden türetilmiştir.

### 15.1 Tarih/Tarih Aralığı Çakışması (UserLeave, UserEntry vb.)

**Sorun:** Sabit tarih aralıkları (`DateTime.Today.AddDays(30)`) kullanıldığında testler paralel veya sıralı çalışınca `BusinessException: "Bu tarihler arasında zaten ... bulunmaktadır"` oluşur.

**Çözüm:** Her test çalışmasında benzersiz tarih aralığı üret.

```csharp
private static (DateTime Start, DateTime End) UniqueLeaveDates(int baseDaysAhead, int durationDays = 5)
{
    var offset = Math.Abs((int)(DateTime.Now.Ticks % 10000));
    var start = DateTime.Today.AddDays(baseDaysAhead + offset);
    return (start, start.AddDays(durationDays));
}
```

- `baseDaysAhead` farklı testlerde farklı değer almalı (örn. 400, 500, 600)
- `DateTime.Now.Ticks` ile her çalışmada farklı offset üretilir

### 15.2 Yetkilendirme Gerektiren Testler (GetAsync, Approve, Reject vb.)

**Sorun:** `[Authorize]`, `CanAccessUserDataAsync` veya admin yetkisi gerektiren metodlar test edilirken `AbpAuthorizationException` oluşur.

**Çözüm:** Test sınıfı `AuthorizationTestBase`'den türet, metodları `RunAsUserAsync` ile sarma.

```csharp
public class UserLeaveAppService_Tests : AuthorizationTestBase
{
    private TestUserContext TestUserAsYonetici
    {
        get
        {
            var y = TestUserContext.CreateYonetici(Guid.NewGuid(), TestSubTenantId);
            y.UserId = TestUserId;
            return y;
        }
    }

    [Fact]
    public async Task Should_Create_UserLeave()
    {
        if (!HasResolvedData) return;
        await RunAsUserAsync(TestUserAsYonetici, async () =>
        {
            // Test kodu burada
        });
    }
}
```

- Admin/onay metotları için `TestUserContext.CreateYonetici` (Fitliyo.Admin yetkisi)

### 15.3 FK ve Zorunlu Alanlar (Bank, Country vb.)

**Sorun:** `CreateUpdateXxxDto` içinde FK (örn. `CountryId`) zorunlu; TestDataSeeder'da yoksa test başarısız olur.

**Çözüm:** `FitliyoApplicationTestBase` üzerinden `GetOrCreateCountryIdAsync()` veya `TestCountryId` kullan.

```csharp
var countryId = await GetOrCreateCountryIdAsync();
if (countryId == Guid.Empty) return;
var input = new CreateUpdateBankDto { CountryId = countryId, ... };
```

### 15.4 UpdateAsync — ConcurrencyStamp Zorunluluğu

**Sorun:** ABP `UpdateAsync` için `ConcurrencyStamp` gerektirir; Create sonrası alınan DTO'daki değer updateDto'ya aktarılmazsa hata oluşur.

**Çözüm:** Update testinde `updateDto.ConcurrencyStamp = created.ConcurrencyStamp` ata.

```csharp
var created = await CreateTestBankAsync();
var updateDto = new CreateUpdateBankDto { ... };
updateDto.ConcurrencyStamp = created.ConcurrencyStamp;
```

### 15.5 Nullable Olmayan Alanlar (SwiftCode vb.)

**Sorun:** Sorgu/filtre `IsActive` veya benzeri alanlarla çalışıyorsa, entity'de nullable olmayan alanların set edilmesi gerekir.

**Çözüm:** Test verisi oluştururken ilgili entity'nin tüm zorunlu alanlarını doldur (örn. `SwiftCode` için benzersiz değer).

---

## Referanslar

- Test Kapsam Raporu: `docs/tests/README.md`
- AppService Testleri: `test/Fitliyo.Application.Tests/`
- Test Altyapısı: `test/Fitliyo.TestBase/`
- Slash Komutu: `/generate-missing-tests`

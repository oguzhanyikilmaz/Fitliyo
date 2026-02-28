# Fitliyo - Spor & Sağlık Koçluğu Marketplace
## Cursor IDE için Geliştirici Rehberi

### Hızlı Başlangıç

Bu proje, Cursor IDE ile geliştirme için optimize edilmiştir.

- Çekirdek kural seti: `.cursor/rules/*.mdc`
- Eski/uzun arşiv içerikler temizlendi (aktif rule seti sadece `.cursor/rules/*.mdc`).

#### Proje Kurulumu
```bash
# 1. Dependencies restore
dotnet restore

# 2. Client-side packages
abp install-libs

# 3. Database migration
cd src/Fitliyo.DbMigrator
dotnet run

# 4. Run application
cd ../Fitliyo.Web
dotnet run
```

#### Docker ile Çalıştırma
```bash
docker-compose up -d
docker-compose logs -f
docker-compose down
```

### Proje Yapısı (Cursor için)

```
Fitliyo/
├── .cursor/rules/              # Cursor AI kuralları
├── Fitliyo.slnx                # Solution file
├── docs/                       # Dokümantasyon
│   ├── README.md               # Ana giriş
│   ├── AUTHORIZATION-SYSTEM.md # Yetkilendirme
│   ├── DATABASE-SCHEMA.md      # Veritabanı şeması
│   ├── BUSINESS-RULES.md       # İş kuralları
│   ├── app-services/           # AppService dokümanları
│   └── frontend-changes/       # Frontend değişiklik takibi
├── src/                        # Kaynak kod
│   ├── Fitliyo.Domain/         # Entity'ler, domain servisleri
│   ├── Fitliyo.Application/    # AppService'ler
│   ├── Fitliyo.HttpApi/        # API Controller'lar
│   ├── Fitliyo.Web/            # Swagger host
│   └── ...
└── test/                       # Test projeleri
```

### Cursor ile Geliştirme İpuçları

#### 1. Marketplace Entity Oluşturma
```
"TrainerProfile entity'si oluştur, FullAuditedAggregateRoot kullan"
"ServicePackage entity'si oluştur, TrainerProfileId FK ile"
"Order entity'si oluştur, Student ve Trainer ilişkisi ile"
```

Cursor otomatik olarak şu pattern'i kullanacak:
```csharp
public class ServicePackage : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; set; }
    [Required]
    [StringLength(256)]
    public string Title { get; set; }
    public PackageType PackageType { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    protected ServicePackage() { }
    public ServicePackage(Guid id, Guid trainerProfileId, string title) : base(id)
    {
        TrainerProfileId = trainerProfileId;
        Title = title;
    }
}
```

#### 2. Cache Service Oluşturma
```
"TrainerProfile için cache servisi oluştur"
"ServicePackage cache-first pattern ile"
"Slug bazlı cache key pattern uygula"
```

#### 3. App Service Oluşturma
```
"ServicePackage için app service oluştur, ownership kontrolü ile"
"Order app servisi yap, Student ve Trainer erişim kontrolü ile"
"TrainerProfile app servisi, Guest (AllowAnonymous) endpoint ile"
```

#### 4. Ödeme / Escrow Akışı
```
"Order için ödeme akışı implement et, iyzico webhook ile"
"TrainerWallet escrow → available balance aktarımı yap"
"WithdrawalRequest CRUD ve admin onay akışı"
```

### Yaygın Cursor Komutları

#### Tam Modül Oluşturma
```
"TrainerModule" için tam CRUD modülü oluştur:
- Domain entity (FullAuditedAggregateRoot)
- Validation'lı DTO'lar
- Cache'li app service
- Permission tanımları
- Guest erişimli arama endpoint'i
- EF Core mapping
```

#### Business Logic Ekleme
```
"Sıralama algoritması implement et:
AverageRating×0.35 + ReviewCount(log)×0.20 + ResponseRate×0.15 +
ProfileCompletion×0.10 + RecentActivity×0.10 + SubscriptionTier×0.10"
```

#### İptal/İade Mantığı
```
"Order iptal/iade mantığını implement et:
- 48 saat öncesi: %100 iade
- 24-48 saat: %50 iade
- 24 saat içi: iade yok
- Eğitmen iptal: %100 iade"
```

### Debugging ve Troubleshooting

#### Yaygın Sorunlar
```
"Ödeme webhook'u çalışmıyor, iyzico callback kontrol et"
"Eğitmen profil cache invalidation sorunu var"
"SignalR mesajlaşma bağlantı problemi"
"Order status geçişi hatalı"
```

### Test Yazımı
```
"ServicePackageAppService için CRUD testleri yaz"
"Order oluşturma ve ödeme akışı integration test"
"TrainerProfile arama ve filtreleme testleri"
"Review oluşturma ve puan hesaplama testleri"
```

---

**Son güncelleme**: 2026-02-28
**Fitliyo Version**: v4.0
**Cursor Compatibility**: Optimized

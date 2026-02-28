# Fitliyo Redis Cache Implementation Guide

Bu doküman, Fitliyo projesindeki Redis Cache yapısının implementasyonunu ve yeni entity'ler için cache ekleme kurallarını açıklar.

## Genel Mimari

Fitliyo projesi, performans optimizasyonu için Redis tabanlı cache sistemi kullanır. Cache sistemi şu katmanlardan oluşur:

1. **Base Cache Services** - Temel cache işlemleri
2. **Entity-Specific Cache Services** - Her entity için özel cache servisleri
3. **AppService Integration** - Uygulama servislerinde cache entegrasyonu

## Klasör Yapısı

```
src/Fitliyo.RedisCache/
├── EntityCaches/
│   ├── Books/
│   │   ├── IBookCacheService.cs
│   │   └── BookCacheService.cs
│   ├── Users/
│   │   ├── IUserCacheService.cs
│   │   └── UserCacheService.cs
│   ├── UserContractTypes/
│   │   ├── IUserContractTypeCacheService.cs
│   │   └── UserContractTypeCacheService.cs
│   ├── Departments/
│   ├── Countries/
│   ├── Provinces/
│   ├── Districts/
│   ├── SgkLogins/
│   ├── SubTenants/
│   ├── WorkDays/
│   ├── UserFinanceInfos/
│   ├── ContractTypes/
│   ├── UserAddresses/
│   ├── IEntityCacheService.cs
│   └── BaseEntityCacheService.cs
└── ...
```

## Cache Service Yapısı

### 1. Base Interface (IEntityCacheService)

```csharp
public interface IEntityCacheService<TEntity, TKey> : ITransientDependency where TEntity : class
{
    // Temel CRUD operasyonları
    Task<TEntity?> GetAsync(TKey id, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<Dictionary<TKey, TEntity?>> GetManyAsync(IEnumerable<TKey> ids, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task SetAsync(TKey id, TEntity entity, Guid? tenantId = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task SetManyAsync(Dictionary<TKey, TEntity> entities, Guid? tenantId = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(TKey id, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task RemoveManyAsync(IEnumerable<TKey> ids, Guid? tenantId = null, CancellationToken cancellationToken = default);

    // Liste ve sayım operasyonları
    Task<List<TEntity>?> GetListAsync(string cacheKey, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task SetListAsync(List<TEntity> entities, string cacheKey, Guid? tenantId = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<long?> GetCountAsync(string cacheKey, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task SetCountAsync(long count, string cacheKey, Guid? tenantId = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    // Cache temizleme
    Task InvalidateAllAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
```

### 2. Entity-Specific Interface Örneği

```csharp
public interface IUserContractTypeCacheService : IEntityCacheService<UserContractTypeDto, Guid>
{
    // Entity'ye özel cache metotları
    Task<UserContractTypeDto?> GetByUserIdAsync(Guid userId, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task SetByUserIdAsync(Guid userId, UserContractTypeDto contractType, Guid? tenantId = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<List<UserContractTypeDto>?> GetByContractTypeIdAsync(Guid contractTypeId, Guid? tenantId = null, CancellationToken cancellationToken = default);

    // İlişkili cache temizleme metotları
    Task InvalidateUserCachesAsync(Guid userId, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task InvalidateContractTypeCachesAsync(Guid contractTypeId, Guid? tenantId = null, CancellationToken cancellationToken = default);
}
```

### 3. Cache Service Implementation

```csharp
public class UserContractTypeCacheService : BaseEntityCacheService<UserContractTypeDto, Guid>, IUserContractTypeCacheService, ITransientDependency
{
    public UserContractTypeCacheService(
        ICacheService<UserContractTypeDto> cacheService,
        ICacheService<List<UserContractTypeDto>> listCacheService,
        ICacheService<CacheValueWrapper<long>> countCacheService,
        IOptions<RedisCacheOptions> options,
        ILogger<UserContractTypeCacheService> logger)
        : base(cacheService, listCacheService, countCacheService, options, logger)
    {
    }

    public async Task<UserContractTypeDto?> GetByUserIdAsync(Guid userId, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        var key = CacheKeyManager.CreateCustomKey("UserContractType", "ByUserId", userId.ToString());
        return await CacheService.GetAsync(key, cancellationToken);
    }

    protected override async Task OnEntityChangedAsync(Guid id, UserContractTypeDto? entity, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        await base.OnEntityChangedAsync(id, entity, tenantId, cancellationToken);

        if (entity != null)
        {
            // İlişkili cache'leri temizle
            await InvalidateUserCachesAsync(entity.UserId, tenantId, cancellationToken);
            await InvalidateContractTypeCachesAsync(entity.ContractTypeId, tenantId, cancellationToken);
        }
    }
}
```

## AppService Cache Entegrasyonu

### Cache-First Pattern

```csharp
public override async Task<UserContractTypeDto> GetAsync(Guid id)
{
    // 1. Önce cache'den kontrol et
    var cachedEntity = await _cacheService.GetAsync(id, CurrentTenant.Id);
    if (cachedEntity != null)
    {
        return cachedEntity;
    }

    // 2. Cache'de yoksa veritabanından getir
    var entity = await Repository.GetAsync(id);
    var entityDto = ObjectMapper.Map<Entity, EntityDto>(entity);

    // 3. Cache'e kaydet
    await _cacheService.SetAsync(id, entityDto, CurrentTenant.Id);

    return entityDto;
}
```

### Liste Operasyonları (Optimize Edilmiş)

```csharp
public override async Task<PagedResultDto<EntityDto>> GetListAsync(GetEntityListDto input)
{
    // 1. Önce cache'den kontrol et (tek seferde hem liste hem count)
    var (cachedList, cachedCount) = await _cacheService.GetPagedListAsync(
        input.SkipCount,
        input.MaxResultCount,
        input.Sorting,
        input.Filter,
        CurrentTenant.Id);

    // 2. Her ikisi de cache'de varsa döndür (cache hit)
    if (cachedList != null && cachedCount.HasValue)
    {
        return new PagedResultDto<EntityDto>(cachedCount.Value, cachedList);
    }

    // 3. Cache'de yoksa veritabanından getir (cache miss)
    var result = await base.GetListAsync(input);

    // 4. Cache'e kaydet (sadece cache miss durumunda)
    await _cacheService.SetPagedListAsync(
        result.Items.ToList(),
        result.TotalCount,
        input.SkipCount,
        input.MaxResultCount,
        input.Sorting,
        input.Filter,
        CurrentTenant.Id);

    return result;
}
```

#### ⚡ Performans İyileştirmeleri:

1. **Tek Seferde Cache Kontrolü**: `GetPagedListAsync` hem liste hem count'u aynı anda kontrol eder
2. **Akıllı Cache Key**: Parametrelerin hash'i alınarak collision önlenir
3. **Conditional Set**: Sadece cache miss durumunda cache'e yazılır
4. **Atomic Operations**: Liste ve count cache'i birlikte güncellenir

### CRUD Operasyonlarında Cache Yönetimi

```csharp
public override async Task<EntityDto> CreateAsync(CreateEntityDto input)
{
    var result = await base.CreateAsync(input);

    // Cache'e kaydet ve liste cache'lerini temizle
    await _cacheService.SetAsync(result.Id, result, CurrentTenant.Id);
    await _cacheService.InvalidateAllAsync(CurrentTenant.Id);

    return result;
}

public override async Task<EntityDto> UpdateAsync(Guid id, UpdateEntityDto input)
{
    var result = await base.UpdateAsync(id, input);

    // Cache'i güncelle
    await _cacheService.SetAsync(id, result, CurrentTenant.Id);
    await _cacheService.InvalidateAllAsync(CurrentTenant.Id);

    return result;
}

public override async Task DeleteAsync(Guid id)
{
    await base.DeleteAsync(id);

    // Cache'den sil
    await _cacheService.RemoveAsync(id, CurrentTenant.Id);
    await _cacheService.InvalidateAllAsync(CurrentTenant.Id);
}
```

## Cache Key Yapısı

Cache key'ler şu formatta oluşturulur:

```
{EntityName}:{Operation}:{Parameter}:{TenantId}
```

Örnekler:
- `UserContractType:Entity:123e4567-e89b-12d3-a456-426614174000:tenant123`
- `UserContractType:ByUserId:user123:tenant123`
- `UserContractType:List_0_10_Name_:tenant123`

## Mevcut Cache Entegrasyonları

### ✅ Tam Entegre Entity'ler:
1. **Department** - Departman yönetimi
2. **SgkLogins** - SGK giriş bilgileri
3. **District** - İlçe yönetimi
4. **Province** - İl yönetimi
5. **Country** - Ülke yönetimi
6. **ContractType** - Sözleşme tipleri
7. **Book** - Kitap yönetimi
8. **SubTenant** - Alt kiracı yönetimi
9. **WorkDay** - Çalışma günleri
10. **UserFinanceInfo** - Kullanıcı finansal bilgileri
11. **UserAddress** - Kullanıcı adresleri
12. **UserContractType** - Kullanıcı sözleşme tipleri
13. **User** - Kullanıcı yönetimi (Cache service oluşturuldu)

## Cache Performans Metrikleri

- **Entity Cache TTL**: 1 saat
- **List Cache TTL**: 30 dakika
- **Count Cache TTL**: 15 dakika
- **Custom Cache TTL**: Entity'ye göre değişken

## Monitoring ve Debugging

Cache performansını izlemek için:

1. **Redis Monitoring**: Redis sunucusunda cache hit/miss oranları
2. **Application Logs**: Cache service'lerde loglama
3. **Performance Counters**: Cache operasyon süreleri

## Troubleshooting

### 🔧 Yaygın Sorunlar ve Çözümleri

#### 1. Cache Miss Sorunları

**Problem**: Entity güncellendiğinde cache temizlenmemiş
```csharp
// ❌ Yanlış - Cache temizlenmemiş
public async Task UpdateAsync(Guid id, UpdateEntityDto input)
{
    var result = await base.UpdateAsync(id, input);
    return result; // Cache güncellenmedi!
}
```

**Çözüm**: CRUD operasyonlarında cache yönetimi
```csharp
// ✅ Doğru - Cache güncellendi
public async Task UpdateAsync(Guid id, UpdateEntityDto input)
{
    var result = await base.UpdateAsync(id, input);

    await _cacheService.SetAsync(id, result, CurrentTenant.Id);
    await _cacheService.InvalidateAllAsync(CurrentTenant.Id);

    return result;
}
```

#### 1.1. Liste Cache Gereksiz Set Sorunu

**Problem**: Her liste çekişinde cache'e yazılıyor
```csharp
// ❌ Yanlış - Her seferinde cache'e yazıyor
public async Task<PagedResultDto<EntityDto>> GetListAsync(GetEntityListDto input)
{
    var cachedList = await _cacheService.GetListAsync(cacheKey, CurrentTenant.Id);

    // Cache hit olsa bile DB'ye gidiyor!
    var result = await base.GetListAsync(input);

    // Her seferinde cache'e yazıyor!
    await _cacheService.SetListAsync(result.Items, cacheKey, CurrentTenant.Id);

    return result;
}
```

**Çözüm**: Cache-first pattern ve conditional set
```csharp
// ✅ Doğru - Sadece cache miss durumunda DB'ye gidiyor
public async Task<PagedResultDto<EntityDto>> GetListAsync(GetEntityListDto input)
{
    var (cachedList, cachedCount) = await _cacheService.GetPagedListAsync(
        input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, CurrentTenant.Id);

    // Cache hit - DB'ye gitmeye gerek yok
    if (cachedList != null && cachedCount.HasValue)
    {
        return new PagedResultDto<EntityDto>(cachedCount.Value, cachedList);
    }

    // Cache miss - DB'den getir ve cache'e kaydet
    var result = await base.GetListAsync(input);
    await _cacheService.SetPagedListAsync(
        result.Items.ToList(), result.TotalCount,
        input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, CurrentTenant.Id);

    return result;
}
```

#### 2. Stale Data (Eski Veri) Sorunları

**Problem**: Cache TTL çok uzun ayarlanmış, güncel olmayan veriler dönüyor

**Çözüm**: Entity türüne göre TTL ayarlama
```csharp
// Sık değişen veriler için kısa TTL
await _cacheService.SetAsync(id, entity, CurrentTenant.Id, TimeSpan.FromMinutes(5));

// Az değişen veriler için uzun TTL
await _cacheService.SetAsync(id, entity, CurrentTenant.Id, TimeSpan.FromHours(2));
```

#### 3. Memory ve Performance Sorunları

**Problem**: Cache boyutu çok büyük, Redis memory dolmuş

**Çözüm**: Cache boyutu optimizasyonu
```csharp
// ❌ Yanlış - Gereksiz büyük DTO
public class UserDto
{
    public string LargeDescription { get; set; } // 10MB text
    public byte[] ProfileImage { get; set; } // 5MB image
}

// ✅ Doğru - Optimize edilmiş DTO
public class UserCacheDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // Büyük alanlar cache'e alınmaz
}
```

#### 4. Tenant Isolation Sorunları

**Problem**: Tenant'lar arası cache sızıntısı

**Çözüm**: Her cache operasyonunda tenant kontrolü
```csharp
// ❌ Yanlış - Tenant ID eksik
var cached = await _cacheService.GetAsync(id);

// ✅ Doğru - Tenant ID ile
var cached = await _cacheService.GetAsync(id, CurrentTenant.Id);
```

#### 5. Redis Connection Sorunları

**Problem**: Redis sunucusuna bağlanılamıyor

**Çözüm**: Connection string ve health check
```csharp
// appsettings.json
{
  "Redis": {
    "IsEnabled": true,
    "Configuration": "localhost:6379",
    "ConnectTimeout": 5000,
    "SyncTimeout": 5000
  }
}

// Health check implementasyonu
public async Task<bool> CheckRedisHealthAsync()
{
    try
    {
        await _cacheService.SetAsync("health-check", "ok", null, TimeSpan.FromSeconds(10));
        var result = await _cacheService.GetAsync("health-check", null);
        return result != null;
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Redis health check failed");
        return false;
    }
}
```

#### 6. Serialization Sorunları

**Problem**: Complex object'ler serialize edilemiyor

**Çözüm**: Cache-friendly DTO'lar kullanma
```csharp
// ❌ Yanlış - Complex navigation properties
public class EntityDto
{
    public virtual ICollection<RelatedEntity> RelatedEntities { get; set; }
    public virtual Parent Parent { get; set; }
}

// ✅ Doğru - Flat DTO structure
public class EntityCacheDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ParentId { get; set; }
    public string ParentName { get; set; } // Denormalized
}
```

#### 7. Cache Key Collision Sorunları

**Problem**: Farklı entity'ler aynı cache key'i kullanıyor

**Çözüm**: Unique cache key pattern
```csharp
// ❌ Yanlış - Collision riski
var key = $"Entity_{id}";

// ✅ Doğru - Unique pattern
var key = CacheKeyManager.CreateCustomKey("UserAddress", "Entity", id.ToString());
// Sonuç: "UserAddress:Entity:123:tenant456"
```

#### 8. Cache Invalidation Sorunları

**Problem**: İlişkili entity'ler güncellendiğinde cache temizlenmiyor

**Çözüm**: Cascade invalidation
```csharp
protected override async Task OnEntityChangedAsync(Guid id, UserAddressDto? entity, Guid? tenantId = null, CancellationToken cancellationToken = default)
{
    await base.OnEntityChangedAsync(id, entity, tenantId, cancellationToken);

    if (entity != null)
    {
        // İlişkili cache'leri temizle
        await InvalidateCountryCachesAsync(entity.CountryId, tenantId, cancellationToken);
        await InvalidateProvinceCachesAsync(entity.ProvinceId, tenantId, cancellationToken);
        await InvalidateDistrictCachesAsync(entity.DistrictId, tenantId, cancellationToken);
        await InvalidateUserCachesAsync(entity.UserId, tenantId, cancellationToken);
    }
}
```

### 🔍 Debug ve Monitoring

#### Cache Hit/Miss Monitoring
```csharp
public async Task<T> GetWithMonitoringAsync<T>(string key, Guid? tenantId = null)
{
    var stopwatch = Stopwatch.StartNew();
    var result = await _cacheService.GetAsync<T>(key, tenantId);
    stopwatch.Stop();

    if (result != null)
    {
        Logger.LogInformation("Cache HIT for key {Key} in {ElapsedMs}ms", key, stopwatch.ElapsedMilliseconds);
        // Metrics: Cache hit counter
    }
    else
    {
        Logger.LogWarning("Cache MISS for key {Key} in {ElapsedMs}ms", key, stopwatch.ElapsedMilliseconds);
        // Metrics: Cache miss counter
    }

    return result;
}
```

#### Redis Memory Monitoring
```bash
# Redis CLI komutları
redis-cli info memory
redis-cli memory usage [key]
redis-cli --bigkeys
```

#### Cache Performance Metrics
```csharp
public class CacheMetrics
{
    public long TotalRequests { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double HitRatio => TotalRequests > 0 ? (double)CacheHits / TotalRequests : 0;
    public TimeSpan AverageResponseTime { get; set; }
}
```

### 🚨 Emergency Procedures

#### Cache Tamamen Çöktüğünde
```csharp
public async Task<EntityDto> GetWithFallbackAsync(Guid id)
{
    try
    {
        // Önce cache'den dene
        var cached = await _cacheService.GetAsync(id, CurrentTenant.Id);
        if (cached != null) return cached;
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Cache failed, falling back to database for entity {Id}", id);
    }

    // Cache fail olursa DB'den getir
    var entity = await Repository.GetAsync(id);
    return ObjectMapper.Map<Entity, EntityDto>(entity);
}
```

#### Cache Temizleme (Emergency)
```csharp
// Tüm cache'i temizle (dikkatli kullanın!)
public async Task FlushAllCacheAsync()
{
    await _distributedCache.RemoveAsync("*"); // Pattern-based removal
    Logger.LogWarning("All cache flushed - Performance impact expected");
}

// Tenant-specific cache temizleme
public async Task FlushTenantCacheAsync(Guid tenantId)
{
    var pattern = $"*:{tenantId}";
    await _distributedCache.RemoveByPatternAsync(pattern);
    Logger.LogInformation("Cache flushed for tenant {TenantId}", tenantId);
}
```

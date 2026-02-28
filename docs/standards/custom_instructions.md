# Fitliyo Backend — Cursor/AI Çalışma Yönergesi (Çekirdek)

Bu doküman, Fitliyo backend repo’sunda Cursor/AI ile geliştirme yaparken uyulacak **çekirdek** kuralları tanımlar.

> UI ayrı repo’da **Next.js** ile geliştirilir. Bu repo’da UI/UX dokümantasyonu tutulmaz.

## 0) Dil

- Tüm açıklamalar ve dokümantasyon **Türkçe** olmalıdır.
- Kod isimleri (class/method/variable) İngilizce kalır.

## 1) API Sözleşmesi: “Birebir” Kuralı (UI için kritik)

- Endpoint/request/response/model **tahmin edilmez**.
- Tek doğruluk kaynağı: `docs/openapi/swagger.web.v1.full.json`

Zorunlu akış:

```bash
# Swagger snapshot üret
dotnet run --project src/Fitliyo.Web -- --generate-swagger docs/openapi/swagger.web.v1.full.json

# AppService dokümanlarını üret
python3 scripts/generate_appservice_docs.py
```

> [`docs/app-services/*.md`](../app-services/*.md) generator çıktısıdır; manuel edit gerekiyorsa önce generator güncellenir.

## 2) Mimari / Katman Prensipleri

- Katmanlar: `Domain` → `Application` → `HttpApi/Web` (ABP pattern)
- DTO’lar: `src/Fitliyo.Application.Contracts`
- İş kuralları mümkünse domain servislerde; AppService orchestration yapar.

Referans:
- [`docs/architecture/README.md`](../architecture/README.md)

## 3) Entity Kuralları (Pattern)

- **UserId varsa**: entity **`IHasUserId`** uygular.
- **Multi-tenant**: entity **`IMultiTenant`** uygular ve `TenantId` alanı bulunur.
- **SubTenant filtreleme**: ilgili entity **`IMultiSubTenant`** uygular ve `SubTenantId` alanı bulunur.
- **EF Core**: `protected` boş constructor bulunur.
- **Constructor**: gerekli alanlar constructor’da set edilir.
- **SubTenant setter**: `SetSubTenant(Guid? subTenantId)` metodu bulunur.
- **XML**: public entity ve kritik property’ler için Türkçe `<summary>` yazılır.

Referans:
- [`docs/architecture/MULTI_TENANCY.md`](../architecture/MULTI_TENANCY.md)
- [`docs/SubTenant_Filtreleme_Sistemi_Dokumani.md`](../SubTenant_Filtreleme_Sistemi_Dokumani.md)

## 4) DTO Kuralları

- DTO isimleri: `XxxDto`, input DTO: `CreateUpdateXxxDto`
- Input DTO’larda validation: `[Required]`, `[StringLength]` vb.
- Entity’de `UserId` varsa, DTO tarafında da `UserId` alanı bulunur.
- XML `<summary>` Türkçe yazılır.

## 5) AppService Kuralları (ABP)

- **Authorization**: `[Authorize(FitliyoPermissions.XXX)]`
- **Cache-first**: okuma operasyonlarında önce cache kontrol edilir.
- **Cache invalidation**: create/update/delete sonrası ilgili cache’ler güncellenir/temizlenir.
- **Logging**: structured logging (hassas veri loglanmaz).
- **Error handling**: `BusinessException` / `UserFriendlyException` standardı.

Referans:
- [`docs/standards/ERROR_HANDLING.md`](../standards/ERROR_HANDLING.md)
- [`docs/AUTHORIZATION-SYSTEM.md`](../AUTHORIZATION-SYSTEM.md)
- [`docs/infrastructure/REDIS_CACHE_IMPLEMENTATION.md`](../infrastructure/REDIS_CACHE_IMPLEMENTATION.md)

## 6) Dokümantasyon Workflow (ZORUNLU)

- Tek giriş: [`docs/README.md`](../README.md)
- Kod değişirse doküman değişir.
- AppService endpoint/request/response/model kısımları yalnızca swagger snapshot’tan gelir.

Referans:
- [`docs/standards/DOCUMENTATION_WORKFLOW.md`](../standards/DOCUMENTATION_WORKFLOW.md)

## 7) Operasyon / Observability (Referans)

- Job sistemi: [`docs/operations/README_JOB_SYSTEM.md`](../operations/README_JOB_SYSTEM.md)
- Logging (Elasticsearch): [`docs/observability/ELASTICSEARCH_GENERIC_LOGGING.md`](../observability/ELASTICSEARCH_GENERIC_LOGGING.md)
- Docker: [`docs/infrastructure/DOCKER.md`](../infrastructure/DOCKER.md)

## 8) Bu dokümanın kapsamı

Bu dosya **çekirdek yönerge** içindir.
Uzun template’ler/örnek implementasyonlar **dokümantasyon** altında tutulur ve buradan link verilir.

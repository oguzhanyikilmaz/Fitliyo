# Dokümantasyon Workflow ve Kurallar (Backend)

**Kapsam:** Her geliştirmede doküman güncelleme, ekip standardı, versiyonlama, gözden geçirme.
**Doküman Versiyonu:** v1.0
**Son Güncelleme:** 2026-01-03
**Sahip:** Backend Team
**Geçerlilik:** Fitliyo >= 2.0

## 1) Temel Kural: Kod Değişirse Doküman Değişir

Bir PR aşağıdakilerden herhangi birini içeriyorsa ilgili doküman(lar) **zorunlu** güncellenir:

- **Yeni endpoint** / endpoint değişikliği
- **DTO değişikliği** (field ekleme/silme/nullable değişimi/validation)
- **Permission değişikliği**
- **Cache davranışı** (TTL/invalidation/key pattern)
- **Multi-tenant / sub-tenant davranışı**
- **Error code / exception mapping**
- **Background job davranışı**
- **Deployment / operasyon** değişikliği

## 2) Doküman Seti (Minimum)

Her değişiklik için en az:

- İlgili teknik doküman (örn: auth, authorization, caching, logging, job)
- [`docs/CHANGELOG.md`](../CHANGELOG.md)
- İstemciyi etkiliyorsa [`docs/BACKEND_CHANGES_FOR_FRONTEND_TEAM.md`](../BACKEND_CHANGES_FOR_FRONTEND_TEAM.md)

## 3) AppService Dokümantasyonu

Her `*AppService` için [`docs/app-services/<AppServiceName>.md`](../app-services/<AppServiceName>.md) bulunur.

**Kural (ZORUNLU / Birebir Sözleşme):**
- AppService dokümanlarının **endpoint / parametre / request / response / model** kısımları **tahmin edilmez**.
- Tek doğruluk kaynağı: `docs/openapi/swagger.web.v1.full.json`
- Bu swagger snapshot, `Fitliyo.Web` host’un **kendi runtime konfigürasyonu** ile üretilir; UI repo/assets altındaki swagger çıktıları **kaynak değildir**.

### 3.1 Swagger Snapshot Üretimi (ZORUNLU)

```bash
# Repo root’tan çalıştırın
dotnet run --project src/Fitliyo.Web -- --generate-swagger docs/openapi/swagger.web.v1.full.json
```

> **Uyarı:** Bu komut `Fitliyo.Web` modüllerini initialize eder. Development ortamında hangi DB/servislere bağlandığınıza dikkat edin.

### 3.2 AppService Dokümanlarını Yeniden Üretme (ZORUNLU)

```bash
python3 scripts/generate_appservice_docs.py
```

### 3.3 Manuel Edit Kuralı

- [`docs/app-services/*.md`](../app-services/*.md) dosyaları **generator çıktısıdır**. Manuel edit yapılacaksa önce generator güncellenmeli, sonra tekrar üretilmelidir.

## 4) Doküman Şablonu ve Versiyonlama

- Versiyonlama standardı: [`docs/VERSIONING.md`](../VERSIONING.md)
- Her dokümanın üst başlığı zorunlu metadata içerir

## 5) Review (Doküman Kontrol Listesi)

PR review sırasında kontrol:

- Doküman başlığı metadata tamam mı?
- API contract (request/response) güncel mi?
- Permission/scope/role anlatımı güncel mi?
- Error senaryoları ve status kodlar güncel mi?
- Cache invalidation ve key pattern açıklanmış mı?

## 6) UI Repo Ayrımı (Next.js)

UI/UX, component, sayfa, tasarım dokümanları bu repoda tutulmaz.
Bu repoda sadece:

- API sözleşmesi
- Auth / authorization / scope kuralları
- Error formatı
- Operasyonel kurulum

yer alır.



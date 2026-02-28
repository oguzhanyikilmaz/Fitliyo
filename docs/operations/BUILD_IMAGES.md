# Build Container Images

**Workflow:** [`build-images.yml`](../../.github/workflows/build-images.yml)
**Tetikleme:** Manuel (`workflow_dispatch`)
**Son Güncelleme:** 2026-02-17

---

## Ne Yapar?

Kaynak koddan 4 Docker image build eder ve GitHub Container Registry'ye (GHCR) push eder. Tüm servisler **paralel** build alır. Build sonrası sunucuya son build numarası kaydedilir.

## Ne Zaman Çalıştırılır?

Kod değişikliği yapıldıktan sonra, deploy öncesinde. Kod değişmediği sürece tekrar çalıştırmaya gerek yok — Docker layer cache sayesinde image aynı kalır.

## Sonraki Adım

Build başarılı → [Deploy to Production](DEPLOY_PRODUCTION.md) workflow'unu çalıştır.

---

## Parametreler

| Parametre | Varsayılan | Açıklama |
|-----------|------------|----------|
| `build_dashboard` | `true` | Dashboard image'ını da build et (şu an kullanılmıyor, dashboard deploy sırasında build ediliyor) |

---

## Job Akışı

```
┌──────────────┐  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐
│  build-web   │  │ build-consumer │  │ build-migrator │  │ build-mobile-api │
│  ~1.5 dk     │  │   ~1.5 dk      │  │   ~30 sn       │  │    ~1.5 dk       │
└──────┬───────┘  └───────┬────────┘  └───────┬────────┘  └────────┬─────────┘
       │                  │                    │                    │
       └──────────────────┴────────────────────┴────────────────────┘
                                    │
                                    │ (hepsi başarılıysa)
                                    │
                          ┌─────────▼─────────┐
                          │  build-summary     │
                          │  Sonuçları logla + │
                          │  sunucuya kaydet   │
                          └────────────────────┘
```

**Toplam süre:** ~2-3 dakika (paralel, cache aktif)

---

## Build Edilen Image'lar

| Servis | Dockerfile | Image Tag | Açıklama |
|--------|-----------|-----------|----------|
| **Web** | `src/Fitliyo.Web/Dockerfile` | `Fitliyo-web:build-N` | Ana web uygulaması (ABP + Razor Pages) |
| **Consumer** | `src/Fitliyo.Consumer/Dockerfile` | `Fitliyo-consumer:build-N` | Background worker (RabbitMQ + Hangfire) |
| **Migrator** | `src/Fitliyo.DbMigrator/Dockerfile` | `Fitliyo-migrator:build-N` | EF Core DB migration (one-shot) |
| **Mobile API** | `src/Fitliyo.HttpApi.Host.Mobile/Dockerfile` | `Fitliyo-mobile-api:build-N` | Mobil uygulama API'si |

Her image iki tag ile push edilir:
- `build-{run_number}` — sabit, deploy'da kullanılır
- `{commit_sha}` — commit bazlı izlenebilirlik

---

## build-summary Job'u

4 build job başarılıysa çalışır:

1. Build sonuçlarını loglar
2. Sunucuya SSH ile bağlanıp `/root/data/latest-build.json` dosyasını yazar:

```json
{
  "build_tag": "build-155",
  "build_number": "155",
  "commit_sha": "abc1234...",
  "build_time": "2026-02-17T10:30:00Z",
  "image_prefix": "ghcr.io/emreakbs/iq-hr"
}
```

Bu dosya, [Deploy to Production](DEPLOY_PRODUCTION.md) workflow'unda `build_number` boş bırakıldığında son build'i otomatik bulmak için kullanılır.

---

## GHCR Image Yapısı

```
ghcr.io/emreakbs/iq-hr/
├── Fitliyo-web:build-N
├── Fitliyo-web:buildcache          # registry cache
├── Fitliyo-consumer:build-N
├── Fitliyo-consumer:buildcache
├── Fitliyo-migrator:build-N
├── Fitliyo-migrator:buildcache
├── Fitliyo-mobile-api:build-N
└── Fitliyo-mobile-api:buildcache
```

---

## İlgili Dokümanlar

- [Deployment Genel Bakış](DEPLOYMENT.md)
- [Deploy to Production](DEPLOY_PRODUCTION.md)

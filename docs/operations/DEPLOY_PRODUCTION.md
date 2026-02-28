# Deploy to Production (Blue-Green)

**Workflow:** [`deploy-production.yml`](../../.github/workflows/deploy-production.yml)
**Tetikleme:** Manuel (`workflow_dispatch`)
**Son Güncelleme:** 2026-02-17

---

## Ne Yapar?

Build edilmiş Docker image'ları **Blue-Green** stratejisi ile production sunucusuna deploy eder. Migration öncesi otomatik DB yedeği alır. Deploy sonrası build tag doğrulaması yapar; uyumsuzluk varsa otomatik rollback uygular.

## Ne Zaman Çalıştırılır?

[Build Images](BUILD_IMAGES.md) workflow'u başarıyla tamamlandıktan sonra.

## Önkoşul

GHCR'de build edilmiş image'lar mevcut olmalı. Yoksa `resolve-build` job'u hata verir ve "Önce Build Container Images workflow'unu çalıştırın" mesajı gösterir.

---

## Parametreler

| Parametre | Varsayılan | Açıklama |
|-----------|------------|----------|
| `build_number` | *(boş)* | Deploy edilecek build numarası. Boş bırakılırsa sunucudaki `/root/data/latest-build.json`'dan okunur. |
| `skip_migration` | `false` | `true` ise pre-deploy backup ve migration atlanır |
| `skip_dashboard` | `false` | `true` ise dashboard deploy'u atlanır |

---

## Job Akışı

```
┌──────────────────┐
│ 1. resolve-build  │  Build tag belirle (manuel veya latest-build.json)
│                    │  GHCR'de 4 image'ın varlığını doğrula
└────────┬──────────┘
         │
┌────────▼──────────────────┐
│ 2. prepare-infrastructure  │  Sunucuya bağlan
│                            │  Git pull (docker-compose güncellemeleri)
│                            │  Host veri dizinlerini oluştur
│                            │  Infrastructure servislerini başlat:
│                            │  sqlserver, redis, rabbitmq, elasticsearch,
│                            │  kibana, minio, Fitliyo-db-backup
│                            │  Health check bekle
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 3. pre-deploy-backup       │  Migration öncesi DB yedeği al
│    (skip_migration=true    │  → Fitliyo-db-backup container ile
│     ise ATLANIR)           │  → Fallback: doğrudan sqlcmd
│                            │  deploy-history.json güncelle
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 4. migrate-database        │  Fitliyo-migrator image'ını pull et
│    (skip_migration=true    │  docker run --rm ile çalıştır (one-shot)
│     ise ATLANIR)           │  EF Core migration uygula
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 5. deploy-services         │  Blue-Green deploy (3 servis sıralı):
│                            │
│                            │  Fitliyo-web:
│                            │    Blue başlat (geçici port 50000-50050)
│                            │    Health check (/health veya /)
│                            │    Başarılı → swap (port 43332)
│                            │    Başarısız → blue sil, eski devam
│                            │
│                            │  Fitliyo-consumer:
│                            │    Blue başlat (port yok, background worker)
│                            │    Health check (DLL kontrolü)
│                            │    Başarılı → swap
│                            │
│                            │  Fitliyo-mobile-api:
│                            │    Blue başlat (geçici port 50051-50100)
│                            │    Health check (/api/mobile/health)
│                            │    Başarılı → swap (port 43335)
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 6. deploy-dashboard        │  Dashboard repo git pull
│    (skip_dashboard=true    │  Docker build (Dockerfile.dev)
│     ise ATLANIR)           │  Blue-Green deploy (port 3000)
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 7. deploy-reverse-proxy    │  Nginx config test + reload
│                            │  Container yoksa başlat
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 8. verify-deployment       │  Tüm servislerin build tag'ini doğrula
│                            │  Health check (web, mobile-api, consumer)
│                            │  Tag uyumsuzluğu → OTOMATİK ROLLBACK
│                            │  deploy-history.json güncelle
│                            │  Temizlik (dangling image prune)
└────────────────────────────┘
```

---

## Blue-Green Deploy Mantığı

Her servis için aynı pattern uygulanır:

1. **Blue başlat:** Yeni image geçici bir port üzerinde `*-blue` adıyla başlatılır
2. **Health check:** 15-20 deneme × 5 saniye aralıkla kontrol edilir
3. **Başarılı →** Eski container durdurulur, blue container silinir, aynı image ana port ile yeniden başlatılır (`--restart unless-stopped`)
4. **Başarısız →** Blue container silinir, eski container çalışmaya devam eder, **deploy iptal edilir**

### Health Check Yöntemleri

| Servis | Yöntem | Endpoint / Kontrol |
|--------|--------|-------------------|
| Fitliyo-web | HTTP | `GET /health` → fallback `GET /` → fallback DLL kontrolü |
| Fitliyo-consumer | DLL | `docker exec test -f /app/Fitliyo.Consumer.dll` |
| Fitliyo-mobile-api | HTTP | `GET /api/mobile/health` → fallback DLL kontrolü |
| Fitliyo-dashboard | HTTP | `GET /` |

---

## Otomatik Rollback (verify-deployment)

`verify-deployment` job'u her servisin çalışan image tag'ini beklenen build tag ile karşılaştırır. Uyumsuzluk varsa:

1. `deploy-history.json`'dan önceki build'i okur
2. Önceki build image'larını pull eder
3. 3 servisi önceki build ile yeniden deploy eder
4. `deploy-history.json`'a rollback bilgisini yazar
5. Workflow'u **başarısız** olarak işaretler

---

## Sunucu Dosyaları

| Dosya | Ne Zaman Yazılır | İçerik |
|-------|-------------------|--------|
| `/root/data/latest-build.json` | Build sonrası | Son build tag, commit SHA |
| `/root/data/deploy-history.json` | Deploy/rollback sonrası | Mevcut build, önceki build, pre-deploy backup adı |
| `/root/data/sqlserver-backups/FitliyoDB_pre-deploy_*.bak` | Migration öncesi | Pre-deploy DB yedeği (7 gün tutulur) |

---

## Servis Portları

| Servis | Ana Port | Blue Port Aralığı |
|--------|----------|-------------------|
| Fitliyo-web | 43332 | 50000-50050 |
| Fitliyo-consumer | *(yok)* | *(yok)* |
| Fitliyo-mobile-api | 43335 | 50051-50100 |
| Fitliyo-dashboard | 3000 | 50101-50150 |

---

## Hata Durumunda

- **Health check başarısız** → Blue container silinir, eski container devam eder. Deploy iptal.
- **Build tag uyumsuzluğu** → verify-deployment otomatik rollback yapar.
- **Manuel müdahale gerekli** → [Deploy Rollback](DEPLOY_ROLLBACK.md) workflow'unu çalıştırın.

---

## İlgili Dokümanlar

- [Deployment Genel Bakış](DEPLOYMENT.md)
- [Build Images](BUILD_IMAGES.md) — önkoşul
- [Deploy Rollback](DEPLOY_ROLLBACK.md) — hata durumunda
- [DB Backup & Restore](DB_BACKUP_RESTORE.md)

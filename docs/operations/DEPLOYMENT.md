# Fitliyo Deployment Guide

**Doküman Versiyonu:** v4.0
**Son Güncelleme:** 2026-02-17
**Sahip:** DevOps

---

## Genel Bakış

Fitliyo'un canlıya alınması **iki ayrı workflow** ile yapılır: önce **Build**, sonra **Deploy**. Hatalı deploy durumunda **Rollback** workflow'u kullanılır. Veritabanı yedekleme/geri yükleme için ayrı workflow'lar mevcuttur.

Tüm workflow'lar `workflow_dispatch` ile **manuel** tetiklenir (backup hariç — cron + manuel).

```
┌─────────────────────────────────────────────────────────────────────┐
│                        NORMAL DEPLOY AKIŞI                         │
│                                                                     │
│   1. Build Images ──────► 2. Deploy to Production ──────► Canlı!   │
│                                                                     │
│   Kod değişti mi?          Build hazır mı?                          │
│   → Evet → Build al        → Evet → Deploy et                      │
│   → Hayır → Atla           → Hayır → Önce Build al                 │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                       HATA DURUMU                                   │
│                                                                     │
│   3. Deploy Rollback                                                │
│   → code-only / code-and-schema / full-restore                      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      VERİTABANI İŞLEMLERİ                           │
│                                                                     │
│   4. Database Backup (otomatik + manuel)                            │
│   5. Database Restore (manuel)                                      │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Workflow'lar

| # | Workflow | Dosya | Detay Doküman |
|---|----------|-------|---------------|
| 1 | **Build Images** | [`build-images.yml`](../../.github/workflows/build-images.yml) | [BUILD_IMAGES.md](BUILD_IMAGES.md) |
| 2 | **Deploy to Production** | [`deploy-production.yml`](../../.github/workflows/deploy-production.yml) | [DEPLOY_PRODUCTION.md](DEPLOY_PRODUCTION.md) |
| 3 | **Deploy Rollback** | [`deploy-rollback.yml`](../../.github/workflows/deploy-rollback.yml) | [DEPLOY_ROLLBACK.md](DEPLOY_ROLLBACK.md) |
| 4 | **Database Backup** | [`database-backup.yml`](../../.github/workflows/database-backup.yml) | [DATABASE_BACKUP.md](DATABASE_BACKUP.md) |
| 5 | **Database Restore** | [`database-restore.yml`](../../.github/workflows/database-restore.yml) | [DATABASE_RESTORE.md](DATABASE_RESTORE.md) |

---

## Tipik Senaryolar

### Normal deploy (kod değişikliği)

```
1. Kodu commit et ve push et
2. GitHub → Actions → "Build Container Images" → Run workflow
3. Build başarılı → Actions → "Deploy to Production" → Run workflow
```

Detay: [Build Images](BUILD_IMAGES.md) → [Deploy to Production](DEPLOY_PRODUCTION.md)

### Migration olmadan deploy

```
"Deploy to Production" → skip_migration: true
```

### Deploy hatalı, geri al

```
"Deploy Rollback" → rollback_strategy: code-only → confirm_rollback: EVET
```

Detay: [Deploy Rollback](DEPLOY_ROLLBACK.md)

### Veritabanı geri yükle

```
1. "Database Restore" → action: list          (yedekleri gör)
2. "Database Restore" → action: restore       (geri yükle)
```

Detay: [Database Restore](DATABASE_RESTORE.md)

---

## Servis Portları

| Servis | Port | Health Check |
|--------|------|-------------|
| Fitliyo-web | 43332 | `GET /health` |
| Fitliyo-mobile-api | 43335 | `GET /api/mobile/health` |
| Fitliyo-consumer | *(yok)* | DLL kontrolü |
| Fitliyo-dashboard | 3000 | `GET /` |

---

## GitHub Secrets

| Secret | Açıklama |
|--------|----------|
| `HOST` | Sunucu IP adresi |
| `USERNAME` | SSH kullanıcı adı |
| `SSH_PRIVATE_KEY` | SSH private key |
| `DASHBOARD_REPO_TOKEN` | Dashboard repo erişim token'ı |
| `VITE_GOOGLE_MAPS_API_KEY` | Google Maps API key |

**Repository ayarları:**
- Actions → General → Workflow permissions → "Read and write permissions" ✅
- Packages → Container registry access ✅

---

## İlgili Dokümanlar

- [DB Backup & Restore](DB_BACKUP_RESTORE.md)
- [Docker Kurulum Rehberi](../infrastructure/DOCKER.md)
- [CHANGELOG](../CHANGELOG.md)

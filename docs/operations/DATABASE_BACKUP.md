# Database Backup

**Workflow:** [`database-backup.yml`](../../.github/workflows/database-backup.yml)
**Tetikleme:** Cron (her gün UTC 03:00) + Manuel (`workflow_dispatch`)
**Son Güncelleme:** 2026-02-17

---

## Ne Yapar?

SQL Server veritabanının (`FitliyoDB`) sıkıştırılmış yedeğini alır. Eski yedekleri saklama süresine göre temizler. Yedek dosyasını doğrular.

## Yedekleme Mimarisi

Bu workflow **ikincil güvenlik katmanıdır**. Yedekleme iki katmanlı çalışır:

| Katman | Mekanizma | Zamanlama | Açıklama |
|--------|-----------|-----------|----------|
| **1 (Ana)** | `Fitliyo-db-backup` sidecar container | Her gün UTC 02:00 | `docker-compose.yml` içinde tanımlı, sunucu içinde çalışır |
| **2 (İkincil)** | Bu workflow | Her gün UTC 03:00 | GitHub Actions üzerinden, ana backup çalışmazsa devreye girer |

Ayrıca [Deploy to Production](DEPLOY_PRODUCTION.md) workflow'u migration öncesi otomatik `pre-deploy` yedeği alır.

---

## Parametreler

| Parametre | Varsayılan | Açıklama |
|-----------|------------|----------|
| `retention_days` | `3` | Günlük yedekler kaç gün tutulsun |

---

## Akış

```
┌────────────────────────┐
│ 1. SQL Server kontrol   │  Container çalışıyor mu?
│                         │  sqlcmd path bul
└────────┬───────────────┘
         │
┌────────▼───────────────┐
│ 2. Backup al            │  BACKUP DATABASE ... WITH COMPRESSION
│                         │  Dosya: FitliyoDB_YYYY-MM-DD.bak
└────────┬───────────────┘
         │
┌────────▼───────────────┐
│ 3. Eski yedekleri temizle│  Günlük: retention_days gün
│                         │  Pre-deploy/pre-restore: 7 gün
└────────┬───────────────┘
         │
┌────────▼───────────────┐
│ 4. Doğrula              │  RESTORE VERIFYONLY
└────────────────────────┘
```

---

## Saklama Süreleri

| Yedek Tipi | Saklama Süresi | Oluşturan |
|------------|---------------|-----------|
| Günlük (`FitliyoDB_YYYY-MM-DD.bak`) | 3 gün (ayarlanabilir) | Bu workflow + sidecar |
| Pre-deploy (`FitliyoDB_pre-deploy_*.bak`) | 7 gün | Deploy workflow |
| Pre-restore (`FitliyoDB_pre-restore_*.bak`) | 7 gün | Restore workflow |
| Manuel (`FitliyoDB_manual_*.bak`) | 3 gün | Manuel tetikleme |

Tüm yedekler: `/root/data/sqlserver-backups/`

---

## Manuel Tetikleme

```
GitHub → Actions → "Database Backup (Daily - Secondary)" → Run workflow
  retention_days: 3  (veya istenen gün sayısı)
```

---

## Container Üzerinden Manuel Yedek

Sidecar container çalışıyorsa doğrudan sunucuda:

```bash
# Manuel yedek al
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh manual

# Yedekleri listele
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh list

# Pre-deploy yedek
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh pre-deploy build-155
```

---

## İlgili Dokümanlar

- [Deployment Genel Bakış](DEPLOYMENT.md)
- [Database Restore](DATABASE_RESTORE.md)
- [DB Backup & Restore](DB_BACKUP_RESTORE.md)

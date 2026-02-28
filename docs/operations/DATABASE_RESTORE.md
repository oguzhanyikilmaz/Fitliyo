# Database Restore

**Workflow:** [`database-restore.yml`](../../.github/workflows/database-restore.yml)
**Tetikleme:** Manuel (`workflow_dispatch`)
**Son Güncelleme:** 2026-02-17

---

## Ne Yapar?

Mevcut yedeklerden SQL Server veritabanını (`FitliyoDB`) geri yükler. Geri yükleme öncesi otomatik güvenlik yedeği alır.

## Ne Zaman Çalıştırılır?

- Veritabanı bozulduğunda
- Belirli bir tarihe geri dönülmek istendiğinde
- Test amaçlı veri geri yüklemesi

> **Not:** Deploy kaynaklı sorunlar için önce [Deploy Rollback](DEPLOY_ROLLBACK.md) tercih edilmeli. Database Restore daha ağır bir operasyondur.

---

## Parametreler

| Parametre | Zorunlu | Açıklama |
|-----------|---------|----------|
| `action` | **Evet** | `list` (yedekleri listele) veya `restore` (geri yükle) |
| `backup_date` | restore için **Evet** | Geri yüklenecek yedek tarihi (`YYYY-MM-DD` formatında) |
| `confirm_restore` | restore için **Evet** | **EVET** yazılmalı |

---

## Kullanım: Adım Adım

### Adım 1 — Mevcut yedekleri listele

```
GitHub → Actions → "Database Restore" → Run workflow
  action: list
```

Çıktıda mevcut yedekler tarih, boyut ve dosya adıyla listelenir.

### Adım 2 — Geri yükle

```
GitHub → Actions → "Database Restore" → Run workflow
  action: restore
  backup_date: 2026-02-17
  confirm_restore: EVET
```

---

## Akış (restore modu)

```
┌─────────────────────────┐
│ 1. Validate Inputs       │  Tarih format kontrolü (YYYY-MM-DD)
│                          │  "EVET" onay kontrolü
└────────┬────────────────┘
         │
┌────────▼────────────────┐
│ 2. Pre-Restore Backup    │  Geri yükleme öncesi güvenlik yedeği al
│                          │  FitliyoDB_pre-restore_TIMESTAMP.bak
└────────┬────────────────┘
         │
┌────────▼────────────────┐
│ 3. Restore Database      │  Uygulama servislerini durdur
│                          │  (Fitliyo-web, consumer, mobile-api)
│                          │  Yedek dosyasını doğrula (VERIFYONLY)
│                          │  SET SINGLE_USER → RESTORE → SET MULTI_USER
│                          │  Uygulama servislerini yeniden başlat
│                          │  DB durumu kontrol et
└──────────────────────────┘
```

---

## Önemli Notlar

- Geri yükleme mevcut veritabanını **tamamen siler** ve yedeğiyle değiştirir
- Geri yükleme öncesi otomatik güvenlik yedeği alınır (`pre-restore_*.bak`) — geri dönülebilir
- İşlem sırasında uygulama servisleri durdurulur (~1-2 dk downtime)
- Yedek dosyası `RESTORE VERIFYONLY` ile doğrulanır; geçersizse işlem iptal olur ve servisler yeniden başlatılır

---

## Yedek Dosya Konumu ve Adlandırma

Tüm yedekler: `/root/data/sqlserver-backups/`

| Dosya Adı Formatı | Oluşturan |
|-------------------|-----------|
| `FitliyoDB_YYYY-MM-DD.bak` | Günlük backup (cron + manuel) |
| `FitliyoDB_pre-deploy_build-N_TIMESTAMP.bak` | Deploy workflow (migration öncesi) |
| `FitliyoDB_pre-restore_TIMESTAMP.bak` | Bu workflow (restore öncesi güvenlik) |
| `FitliyoDB_manual_TIMESTAMP.bak` | Manuel tetikleme / rollback öncesi |

---

## İlgili Dokümanlar

- [Deployment Genel Bakış](DEPLOYMENT.md)
- [Database Backup](DATABASE_BACKUP.md)
- [Deploy Rollback](DEPLOY_ROLLBACK.md)
- [DB Backup & Restore](DB_BACKUP_RESTORE.md)

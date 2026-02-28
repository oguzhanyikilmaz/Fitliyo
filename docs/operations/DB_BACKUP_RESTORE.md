# Veritabanı Yedekleme ve Geri Yükleme Dokümanı

**Doküman Versiyonu:** v2.0
**Son Güncelleme:** 2026-02-17
**Sahip:** DevOps

Bu doküman, veritabanı yedekleme (backup), geri yükleme (restore) ve deploy rollback işlemlerinin nasıl çalıştığını anlatır.

---

## Genel Bilgi

| Bilgi | Değer |
|-------|-------|
| Sunucu | 176.53.96.103 |
| Proje yolu | `/root/Fitliyo` |
| SQL Server container | `Fitliyo-sqlserver` |
| Backup container | `Fitliyo-db-backup` (sidecar) |
| Veritabanı adı | `FitliyoDB` |
| Yedekleme dizini (host) | `/root/data/sqlserver-backups` |
| Yedekleme dizini (container) | `/backups` |
| Backup script | `docker/backup/backup.sh` |

---

## 🆕 Yedekleme Mimarisi (v2.0)

Yedekleme sistemi **iki katmanlı** çalışır:

### Katman 1: Self-Contained Backup Container (ANA MEKANİZMA)

`docker-compose.yml` içindeki `Fitliyo-db-backup` sidecar container'ı hiçbir dış servise bağımlı olmadan, tamamen sunucu içinde çalışır.

- **Image**: `mcr.microsoft.com/mssql-tools:latest`
- **Zamanlama**: Her gün UTC 02:00 (Türkiye 05:00)
- **Saklama**: Günlük yedekler 3 gün, pre-deploy yedekleri 7 gün
- **Doğrulama**: Her yedek sonrası `RESTORE VERIFYONLY` ile doğrulama
- **Restart**: `unless-stopped` — sunucu yeniden başlatılsa bile otomatik çalışır

### Katman 2: GitHub Actions Workflow (İKİNCİL GÜVENLİK)

[`database-backup.yml`](../../.github/workflows/database-backup.yml) workflow'u ikincil güvenlik katmanı olarak çalışır.

- **Zamanlama**: Her gün UTC 03:00 (ana backup'tan 1 saat sonra)
- **Tetikleme**: Otomatik (cron) veya manuel (workflow_dispatch)
- Ana backup çalışmasa bile bu workflow günlük yedek alır

---

## Saklama Politikası (Retention)

| Yedek Tipi | Dosya Formatı | Saklama Süresi |
|------------|---------------|----------------|
| Günlük otomatik | `FitliyoDB_YYYY-MM-DD.bak` | **3 gün** |
| Manuel | `FitliyoDB_manual_YYYY-MM-DD_HHMMSS.bak` | **3 gün** |
| Pre-deploy | `FitliyoDB_pre-deploy_build-N_YYYY-MM-DD_HHMMSS.bak` | **7 gün** |
| Pre-restore (güvenlik) | `FitliyoDB_pre-restore_YYYY-MM-DD_HHMMSS.bak` | **7 gün** |

Eski yedekler her günlük backup sırasında otomatik temizlenir.

---

## Backup Script Kullanımı

Backup script (`docker/backup/backup.sh`) 4 modda çalışır:

### Daemon Modu (Varsayılan)

Container başlatıldığında otomatik çalışır. Manuel tetiklemeye gerek yoktur.

```bash
# Container otomatik daemon modunda çalışır
docker logs Fitliyo-db-backup --tail=50
```

### Manuel Yedekleme

```bash
# Tek seferlik yedek al
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh manual
```

### Pre-Deploy Yedekleme

Deploy workflow tarafından otomatik tetiklenir. Manuel çalıştırma:

```bash
# Deploy öncesi etiketli yedek al
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh pre-deploy build-152
```

### Yedekleri Listeleme

```bash
# Mevcut yedekleri listele
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh list
```

---

## Geri Yükleme (Restore)

### GitHub Actions ile Geri Yükleme

[`database-restore.yml`](../../.github/workflows/database-restore.yml) workflow'u ile:

1. Workflow'u **action: `list`** ile çalıştırarak mevcut yedekleri görün
2. Workflow'u **action: `restore`** ile çalıştırın:
   - `backup_date`: Geri yüklenecek yedek tarihi (YYYY-MM-DD)
   - `confirm_restore`: **EVET** yazın
3. Workflow otomatik olarak:
   - Geri yükleme öncesi güvenlik yedeği alır (`pre-restore`)
   - Uygulama servislerini durdurur
   - Yedek dosyasını doğrular
   - Veritabanını geri yükler
   - Uygulama servislerini yeniden başlatır

### Manuel Geri Yükleme (Sunucu Üzerinde)

```bash
# 1. Servisleri durdur
docker stop Fitliyo-web Fitliyo-consumer Fitliyo-mobile-api

# 2. sqlcmd ile restore
docker exec Fitliyo-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'FitliyoPassword123!' -C \
  -Q "
    ALTER DATABASE [FitliyoDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    RESTORE DATABASE [FitliyoDB]
    FROM DISK = '/backups/FitliyoDB_2026-02-17.bak'
    WITH REPLACE, STATS = 10;
    ALTER DATABASE [FitliyoDB] SET MULTI_USER;
  "

# 3. Servisleri yeniden başlat
docker start Fitliyo-web Fitliyo-consumer Fitliyo-mobile-api
```

---

## 🆕 Deploy Rollback

Hatalı deploy sonrası geri dönüş için [`deploy-rollback.yml`](../../.github/workflows/deploy-rollback.yml) workflow'u kullanılır.

### Rollback Stratejileri

| Strateji | Veri Kaybı | Kullanım Durumu |
|----------|-----------|-----------------|
| `code-only` | **SIFIR** | Migration sadece ADD COLUMN/TABLE yaptıysa (en yaygın) |
| `code-and-schema` | **SIFIR** (yeni kolon verisi hariç) | Additif migration + DB temizliği isteniyorsa |
| `full-restore` | **Olası** (data merge gerekebilir) | Yıkıcı migration (DROP/ALTER) yapıldıysa |

### code-only Stratejisi (TAVSIYE EDİLEN)

Eski build'i migration'sız deploy eder. Veritabanına dokunmaz.

**Neden güvenli**: SQL Server ve EF Core, mapping'de olmayan ekstra kolonları görmezden gelir. Eski kod ekstra kolonu bilmese bile sorunsuz çalışır.

```
Rollback Tetiklendi → Güvenlik Yedeği → Eski Build Pull → Blue-Green Deploy → Tamamlandı
```

### Rollback Workflow Kullanımı

1. GitHub Actions'ta `Deploy Rollback` workflow'unu tetikleyin
2. Parametreler:
   - `rollback_to_build`: Hedef build numarası (boş = otomatik önceki build)
   - `rollback_strategy`: `code-only` | `code-and-schema` | `full-restore`
   - `confirm_rollback`: **EVET**

### Deploy History

Her deploy sonrası sunucuda `/root/data/deploy-history.json` dosyası güncellenir:

```json
{
  "current_build": "build-152",
  "previous_build": "build-151",
  "deploy_time": "2026-02-17T10:30:00Z",
  "migration_ran": true,
  "pre_deploy_backup": "FitliyoDB_pre-deploy_build-152_2026-02-17_103000.bak"
}
```

Rollback workflow bu dosyadan önceki build'i ve pre-deploy backup'ı otomatik tespit eder.

---

## Doğrulama

### Yedek Sonrası Kontrol

```bash
# Container loglarını kontrol et
docker logs Fitliyo-db-backup --tail=30

# Yedek dosyalarını listele
ls -lhS /root/data/sqlserver-backups/FitliyoDB_*.bak

# Yedek dosyasını doğrula
docker exec Fitliyo-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'FitliyoPassword123!' -C \
  -Q "RESTORE VERIFYONLY FROM DISK = '/backups/FitliyoDB_2026-02-17.bak'"
```

### Restore Sonrası Kontrol

```bash
docker exec Fitliyo-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'FitliyoPassword123!' -C \
  -Q "SELECT name, state_desc FROM sys.databases WHERE name='FitliyoDB';"
```

Beklenen: `FitliyoDB` ve `ONLINE`

---

## Sorun Giderme

### Backup Container Çalışmıyor

```bash
# Container durumunu kontrol et
docker ps -a | grep Fitliyo-db-backup

# Container loglarını incele
docker logs Fitliyo-db-backup --tail=50

# Container'ı yeniden başlat
docker restart Fitliyo-db-backup

# Veya docker-compose ile
cd /root/Fitliyo && docker compose up -d Fitliyo-db-backup
```

### Günlük Yedek Alınmamış

```bash
# Son backup'ları kontrol et
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh list

# Manuel olarak hemen yedek al
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh manual
```

### Disk Alanı Yetersiz

```bash
# Disk durumunu kontrol et
df -h /root/data/sqlserver-backups

# Eski yedekleri manuel temizle
ls -lhS /root/data/sqlserver-backups/FitliyoDB_*.bak
# Gerekli olanları silme, eski tarihli olanları silin
```

### SQL Server Bağlantı Sorunu

```bash
# SQL Server container durumunu kontrol et
docker ps | grep Fitliyo-sqlserver
docker logs Fitliyo-sqlserver --tail=30
```

---

## Sık Kullanılan Yol ve Dosyalar

| Dosya/Yol | Açıklama |
|-----------|----------|
| `/root/data/sqlserver-backups/` | Yedek dosyaları (host) |
| `/backups/` | Yedek dosyaları (container içi, SQL Server + backup sidecar) |
| `docker/backup/backup.sh` | Backup script (repo içi) |
| `/root/data/deploy-history.json` | Deploy geçmişi (rollback için) |
| `.github/workflows/database-backup.yml` | GitHub Actions backup (ikincil) |
| `.github/workflows/database-restore.yml` | GitHub Actions restore |
| `.github/workflows/deploy-rollback.yml` | Rollback workflow |

---

## İlgili Dokümanlar

- [Deployment Guide](DEPLOYMENT.md)
- [Docker Kurulum Rehberi](../infrastructure/DOCKER.md)
- [CHANGELOG](../CHANGELOG.md)

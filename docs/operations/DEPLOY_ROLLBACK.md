# Deploy Rollback

**Workflow:** [`deploy-rollback.yml`](../../.github/workflows/deploy-rollback.yml)
**Tetikleme:** Manuel (`workflow_dispatch`)
**Son Güncelleme:** 2026-02-17

---

## Ne Yapar?

Hatalı deploy sonrası önceki build'e geri döner. 3 farklı strateji sunar: sadece kod geri alma, kod + şema temizliği veya tam DB restore.

## Ne Zaman Çalıştırılır?

[Deploy to Production](DEPLOY_PRODUCTION.md) sonrası uygulama hatalı çalışıyorsa veya beklenmeyen davranış varsa.

---

## Parametreler

| Parametre | Zorunlu | Varsayılan | Açıklama |
|-----------|---------|------------|----------|
| `rollback_to_build` | Hayır | *(boş)* | Hedef build numarası (örn: `150`). Boş bırakılırsa `deploy-history.json`'dan önceki build otomatik bulunur. |
| `rollback_strategy` | **Evet** | — | `code-only` / `code-and-schema` / `full-restore` |
| `confirm_rollback` | **Evet** | — | **EVET** yazılmalı, aksi halde workflow iptal olur |

---

## Job Akışı

```
┌──────────────────┐
│ 1. validate       │  "EVET" onay kontrolü
│    -rollback      │  Hedef build tespit (manuel veya deploy-history.json)
│                   │  Mevcut build = hedef build ise iptal
│                   │  full-restore ise pre-deploy backup varlık kontrolü
└────────┬─────────┘
         │
┌────────▼──────────────────┐
│ 2. pre-rollback-backup     │  Rollback öncesi güvenlik yedeği al
│                            │  (Fitliyo-db-backup veya fallback sqlcmd)
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 3. execute-rollback        │
│                            │  full-restore ise:
│                            │    Servisleri durdur
│                            │    Pre-deploy backup'tan DB restore
│                            │
│                            │  Tüm stratejilerde:
│                            │    Eski build image'larını pull et
│                            │    Fitliyo-web deploy (port 43332)
│                            │    Fitliyo-consumer deploy
│                            │    Fitliyo-mobile-api deploy (port 43335)
│                            │
│                            │  code-and-schema ise:
│                            │    Manuel şema temizliği talimatları
│                            │
│                            │  deploy-history.json güncelle
└────────┬──────────────────┘
         │
┌────────▼──────────────────┐
│ 4. verify-rollback         │  Container durumları + health check
│                            │  DB durumu kontrolü
│                            │  Nginx reload
│                            │  Temizlik
└────────────────────────────┘
```

---

## Strateji Karşılaştırması

### `code-only` (Tavsiye Edilen)

| | |
|---|---|
| **DB'ye dokunur mu?** | Hayır |
| **Veri kaybı** | Sıfır |
| **Downtime** | ~2-3 dakika |
| **Ne zaman?** | Migration sadece ADD COLUMN / ADD TABLE yaptıysa |

En yaygın ve en güvenli senaryo. Sadece uygulama kodunu eski build'e geri alır. Veritabanına dokunmaz.

**Neden güvenli:** SQL Server ve EF Core, mapping'de olmayan ekstra kolonları görmezden gelir. Migration ile eklenen kolon/tablo DB'de kalır ama eski kod sorunsuz çalışır.

### `code-and-schema`

| | |
|---|---|
| **DB'ye dokunur mu?** | Evet (manuel) |
| **Veri kaybı** | Sıfır (yeni kolon verileri hariç) |
| **Downtime** | ~5 dakika |
| **Ne zaman?** | Additif migration + şema temizliği gerekiyorsa |

Kod geri alınır, ardından workflow **manuel şema temizliği talimatlarını** gösterir:

1. Son migration'ın ne eklediğini inceleyin
2. Eklenen kolon/tabloları SQL ile DROP edin
3. `__EFMigrationsHistory` tablosundan ilgili kaydı silin

### `full-restore`

| | |
|---|---|
| **DB'ye dokunur mu?** | Evet (tam restore) |
| **Veri kaybı** | **Olası** — deploy sonrası girilen veriler kaybolur |
| **Downtime** | ~10-15 dakika |
| **Ne zaman?** | Yıkıcı migration (DROP COLUMN, ALTER TABLE, veri dönüşümü) |

Deploy öncesi alınan `pre-deploy backup`'tan tam veritabanı restore yapar. Ardından eski build ile deploy eder.

> **Dikkat:** Deploy ile rollback arasında girilen tüm veriler kaybolur.

---

## Örnek Kullanım

```
GitHub → Actions → "Deploy Rollback" → Run workflow

  rollback_to_build:  (boş bırak = otomatik önceki build)
  rollback_strategy:  code-only
  confirm_rollback:   EVET
```

Belirli bir build'e dönmek için:

```
  rollback_to_build:  150
  rollback_strategy:  code-only
  confirm_rollback:   EVET
```

---

## İlgili Dokümanlar

- [Deployment Genel Bakış](DEPLOYMENT.md)
- [Deploy to Production](DEPLOY_PRODUCTION.md)
- [DB Backup & Restore](DB_BACKUP_RESTORE.md)
- [Database Restore](DATABASE_RESTORE.md)

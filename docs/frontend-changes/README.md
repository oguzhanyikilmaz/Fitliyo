# Frontend Değişiklik Takibi

Backend'de yapılan ve frontend/UI ekibini etkileyen tüm değişiklikler bu klasörde takip edilir.

## Yapı

```
docs/frontend-changes/
├── README.md                          # Bu dosya (index + format standardı)
├── _archive-before-2026-02-18.md      # Eski format girişleri (tek dosya dönemi)
├── 2026-02-18-1.md                    # Her giriş ayrı dosya
├── 2026-02-18-2.md
├── 2026-02-19-1.md
└── ...
```

## Format Standardı

### Dosya Adı: `YYYY-MM-DD-N.md`

- `YYYY-MM-DD`: Değişiklik tarihi
- `N`: O gün içindeki sıra numarası (1'den başlar)

### Sıra Numarası Belirleme

Aynı günde yeni giriş eklerken bu klasördeki dosyaları kontrol et:
```
2026-02-18-1.md  → mevcut
2026-02-18-2.md  → mevcut
2026-02-18-3.md  → bu oluşturulacak (bir sonraki)
```

### Dosya İçerik Şablonu

```markdown
# [BADGE] Kısa Başlık

- **ID**: YYYY-MM-DD-N
- **Tarih**: YYYY-MM-DD
- **Etkilenen**: endpoint / DTO / permission listesi
- **Değişiklik Tipi**: Badge + açıklama
- **Açıklama**: Ne değişti (kısa ve net)
- **İstemcinin Yapması Gerekenler**: Aksiyon adımları veya "Yok — backend tarafında tamamlandı."
- **Detaylar**: İlgili doküman linki (varsa, gerçek dosya yolunu yaz)

---

(Gerekirse ek açıklamalar, tablo, örnek request/response buraya)
```

### Badge'ler

| Badge | Dosyada | Anlamı |
|-------|---------|--------|
| `⚠️ BREAKING` | Başlıkta | Frontend kod değişikliği ZORUNLU |
| `🟢 NON-BREAKING` | Başlıkta | Frontend'in bir şey yapmasına gerek yok |
| `🐛 BUG FIX` | Başlıkta | Mevcut sorun giderildi |
| `📄 DOKÜMAN` | Başlıkta | Yeni iş planı, API dokümanı |
| `🔧 ALTYAPI` | Başlıkta | Deployment, CI/CD, config |

---

## Değişiklik Listesi (Yeniden Eskiye)

> Yeni girişler bu listenin **en üstüne** eklenir.

- [2026-04-26-1 — Kullanıcı wellness (beslenme, antrenman, tercihler)](2026-04-26-1.md)


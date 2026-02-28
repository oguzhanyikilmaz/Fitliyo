# Dokümantasyon Versiyonlama Standardı (Backend)

**Amaç:** Dokümantasyonun herkes tarafından aynı şekilde üretilmesini, değişikliklerin izlenebilmesini ve “hangi kod sürümü için geçerli?” sorusunun net cevaplanmasını sağlamak.

## 1) Temel Kavramlar

- **Kod Versiyonu (Release)**: Uygulamanın sürümü (örn: `v2.4.1`).
- **Doküman Versiyonu (DocVersion)**: Dokümanın kendi sürümü (örn: `v1.2`).
- **Son Güncelleme (LastUpdated)**: Dokümanın en son güncellendiği tarih.
- **Sahip (Owner)**: Dokümanın doğruluğundan sorumlu ekip/kişi.

## 2) Zorunlu Başlık Şablonu (Her Dokümanda Olmalı)

Her `.md` dosyasının en üstünde aşağıdaki blok bulunmalıdır:

```text
# <Başlık>

**Kapsam:** <Kısa açıklama>
**Doküman Versiyonu:** vX.Y
**Son Güncelleme:** YYYY-MM-DD
**Sahip:** <Ekip/Role>
**Geçerlilik:** <Kod versiyonu / aralık> (örn: Fitliyo >= 2.0)
```

> **Not:** Doküman sürümü semver olmak zorunda değil; **vX.Y** yeterli. Büyük yapısal değişiklikte **X** artar.

## 3) Değişiklik Türleri ve Versiyon Artırma

- **Doküman düzeltme (typo / link / küçük açıklama)** → **Y artar** (v1.1 → v1.2)
- **Yeni bölüm ekleme / akış ekleme / yeni endpoint ekleme** → **Y artar**
- **Doküman yapısını değiştirme / eski bölümleri kaldırma / doküman kapsamını değiştirme** → **X artar** (v1.x → v2.0)

## 4) Değişiklik Kaydı (Changelog)

Her doküman güncellemesinde şu iki şey yapılır:

1. İlgili dokümanın başlığındaki **Son Güncelleme** + **Doküman Versiyonu** güncellenir
2. [`docs/CHANGELOG.md`](CHANGELOG.md) dosyasına bir kayıt eklenir

### 4.1) CHANGELOG Aylık Arşivleme

CHANGELOG dosyası büyümesini kontrol altında tutmak için aylık arşivleme yapılır:

- **Aktif ay:** `docs/CHANGELOG.md`'de tutulur
- **Eski aylar:** `docs/changelog/YYYY-MM.md` dosyalarına arşivlenir
- **Arşivleme periyodu:** Her ay sonunda önceki ay arşivlenir
- **Script:** `python3 scripts/archive_changelog.py` (varsayılan: önceki ay)
- **Belirli ay:** `python3 scripts/archive_changelog.py --month YYYY-MM`

## 5) “API Sözleşmesi Değişikliği” Kuralı

İstemciyi etkileyen her değişiklik (endpoint, DTO shape, permission, error code, header davranışı) için:

- [`docs/BACKEND_CHANGES_FOR_FRONTEND_TEAM.md`](BACKEND_CHANGES_FOR_FRONTEND_TEAM.md) güncellenir
- Değişiklik maddesi “Breaking / Non-breaking” olarak işaretlenir

## 6) Deprecation (Kullanımdan Kaldırma)

Bir doküman veya bölüm artık geçerli değilse silinmez; aşağıdaki şekilde işaretlenir:

```text
> **Deprecated:** YYYY-MM-DD itibariyle kullanılmıyor.
> **Yerine:** <Yeni doküman yolu>
```

---

**Son Güncelleme:** 2026-01-03
**Doküman Versiyonu:** v1.0
**Sahip:** Backend Team



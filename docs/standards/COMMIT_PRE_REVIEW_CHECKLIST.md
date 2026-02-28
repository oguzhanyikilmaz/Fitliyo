# Commit Öncesi Review Full-Check Checklist

Commit atmadan önce bu listeyi sırayla kontrol edin. Proje kurallarına göre zorunlu adımlar işaretlidir.

---

## 1. Kod ve Derleme

| # | Kontrol | Komut / Aksiyon |
|---|---------|------------------|
| 1.1 | **Solution derleniyor mu?** | `dotnet build` veya `dotnet build src/Fitliyo.Web/Fitliyo.Web.csproj` |
| 1.2 | **Linter hatası yok mu?** | IDE’de ilgili projelerde lint kontrolü; gerekirse `ReadLints` |
| 1.3 | **Testler geçiyor mu?** (opsiyonel ama önerilir) | `dotnet test` veya ilgili test projesi |

---

## 2. Dokümantasyon (ZORUNLU)

| # | Kontrol | Komut / Aksiyon |
|---|---------|------------------|
| 2.1 | **CHANGELOG güncel mi?** | `docs/CHANGELOG.md` — Her anlamlı kod değişikliğinde en üste yeni giriş (tarih + kısa başlık + özet). Sadece typo/yorum gibi önemsiz değişikliklerde gerekmez. |
| 2.2 | **Next.js frontend etkileniyor mu?** | API endpoint / DTO / Response / SignalR / permission değiştiyse → `docs/frontend-changes/YYYY-MM-DD-N.md` oluştur + `docs/frontend-changes/README.md` index’e ekle. MVC-only değişikliklerde gerekmez. |
| 2.3 | **AppService dokümanları güncel mi?** | AppService kodu değiştiyse: `python3 scripts/generate_appservice_docs.py` |
| 2.4 | **Swagger güncel mi?** | API sözleşmesi (endpoint/DTO/route) değiştiyse: `dotnet run --project src/Fitliyo.Web -- --generate-swagger docs/openapi/swagger.web.v1.full.json` |
| 2.5 | **Kırık link var mı?** | `python3 scripts/validate_docs_links.py` — Çıkış 0 olmalı. |

---

## 3. Otomatik Pre-Commit Script (Önerilen)

Tek komutla doküman kurallarını kontrol et:

```bash
bash scripts/pre-commit-docs-check.sh
```

- **Tam kontrol:** Tüm değişikliklere göre CHANGELOG / frontend-changes / link uyarıları.
- **Hızlı (sadece staged):** `bash scripts/pre-commit-docs-check.sh --quick`

Çıkış: `0` = başarılı, `1` = uyarı (commit engellemez), `2` = hata.

---

## 4. Proje Kuralları Özeti

- **Yetkilendirme:** `[Authorize(FitliyoPermissions....)]` **method-level**; class-level policy/permission yasak.
- **Self-data:** Kendi verisi için `[AllowSelfAccess("...", Module = SelfAccessModules....)]` kullan.
- **API sözleşmesi:** Tahmin yok; tek kaynak `docs/openapi/swagger.web.v1.full.json`.
- **Dinamik alanlar:** SetProperty, ExtraProperties, zenginleştirme → ilgili AppService dokümanında belirtilmeli.

---

## 5. Hızlı Komut Listesi (Kopyala-Yapıştır)

```bash
# Derleme
dotnet build

# Doküman link kontrolü
python3 scripts/validate_docs_links.py

# Pre-commit full-check (doküman kuralları)
bash scripts/pre-commit-docs-check.sh

# (API değiştiyse) Swagger snapshot
dotnet run --project src/Fitliyo.Web -- --generate-swagger docs/openapi/swagger.web.v1.full.json

# (AppService değiştiyse) AppService dokümanları
python3 scripts/generate_appservice_docs.py
```

---

**İlgili dokümanlar:**

- [Otomatik Doküman Güncelleme](../../.cursor/rules/auto-doc-update.mdc)
- [Dokümantasyon Kuralları](../../.cursor/rules/documentation-rules.mdc)
- [CHANGELOG](../CHANGELOG.md)

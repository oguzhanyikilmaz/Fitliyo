---
alwaysApply: true
---

# Fitliyo Backend — Proje Komutları

## Amaç

Cursor'ın backend proje komutlarını nasıl yorumlayıp çalıştıracağını tanımlar. Her komut şemaya uygun, dry-run destekli ve kural ihlallerinde durmalıdır.

## Komut Şeması (zorunlu)

Her komut şu yapıyı takip eder:
- id: benzersiz komut kimliği (kebab-case)
- trigger_phrases: doğal dil tetikleyicileri
- action: eşlenmiş komut adı
- params: [{ name, type, required, default }]
- dry_run_supported: boolean
- required_rules: bağımlı kural id'leri
- output_format: "file-tree" | "code-block" | "json-metadata"
- error_behavior: "stop-and-report" | "attempt-fallback"

## Temel Davranış (zorunlu)

- Kullanıcı mesajını **tek bir komut**a eşle; belirsizse 1 kısa soru sor.
- Komut çalıştırmadan önce ilgili `.cursor/rules/*.mdc` kurallarını oku.
- Swagger gereken komutlarda swagger dosyası yoksa → **stop-and-report**.
- Dosya oluşturan/değiştiren komutlar `dry_run` desteklemeli; önce dosya ağacı gösterilmeli.
- Yıkıcı işlemler (silme/rename) açık `confirm=true` olmadan yapılmaz.

---

## Standart Komutlar

### generate-swagger
- trigger_phrases: ["swagger üret", "swagger güncelle", "sync swagger", "api spec üret"]
- action: `generate-swagger`
- params:
  - output: string (default: "docs/openapi/swagger.web.v1.full.json")
- dry_run_supported: false
- required_rules: ["auto-generation"]
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- command:
  ```bash
  dotnet run --project src/Fitliyo.Web -- --generate-swagger docs/openapi/swagger.web.v1.full.json
  ```

### generate-appservice-docs
- trigger_phrases: ["doküman üret", "appservice dokümanları üret", "servis dokümanlarını güncelle"]
- action: `generate-appservice-docs`
- params:
  - dry_run: boolean (default: true)
- dry_run_supported: true
- required_rules: ["documentation-rules", "auto-generation"]
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- command:
  ```bash
  python3 scripts/generate_appservice_docs.py
  ```

### run-migrations
- trigger_phrases: ["migration ekle", "migration oluştur", "veritabanı güncelle", "db migrate"]
- action: `run-migrations`
- params:
  - name: string (required — migration adı)
  - action: "add" | "update" (default: "add")
- dry_run_supported: false
- required_rules: ["coding-standards"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- commands:
  ```bash
  # Migration ekleme
  dotnet ef migrations add {name} --project src/Fitliyo.EntityFrameworkCore --startup-project src/Fitliyo.Web

  # Veritabanı güncelleme
  dotnet ef database update --project src/Fitliyo.EntityFrameworkCore --startup-project src/Fitliyo.Web
  ```

### run-tests
- trigger_phrases: ["testleri çalıştır", "test koş", "unit test", "testler"]
- action: `run-tests`
- params:
  - project: string (optional — belirli test projesi)
  - filter: string (optional — test filtresi)
  - all: boolean (default: false — true ise tüm testler)
- dry_run_supported: true
- required_rules: []
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- commands:
  ```bash
  # Sadece değişen modüllerin testleri (varsayılan)
  bash scripts/run-affected-tests.sh

  # Tüm testler
  bash scripts/run-affected-tests.sh --all

  # Dry-run (hangi testlerin çalışacağını gör)
  bash scripts/run-affected-tests.sh --dry-run

  # Belirli sınıf/metot
  dotnet test test/Fitliyo.Application.Tests/Fitliyo.Application.Tests.csproj --filter "FullyQualifiedName~{filter}" --logger "console;verbosity=normal"
  ```

### audit-permissions
- trigger_phrases: ["yetki denetle", "permission kontrolü", "authorization audit"]
- action: `audit-permissions`
- params:
  - scope: string (default: "src/Fitliyo.Application")
- dry_run_supported: false
- required_rules: ["auto-generation"]
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- description: Tüm AppService'lerdeki `[Authorize]`, `[AllowSelfAccess]`, `[AllowAnonymous]` attribute'lerini tarayıp raporlar.

### scaffold-entity
- trigger_phrases: ["entity oluştur", "entity iskele", "yeni entity", "scaffold entity"]
- action: `scaffold-entity`
- params:
  - name: string (required — entity adı, PascalCase)
  - multiTenant: boolean (default: true)
  - hasUserId: boolean (default: false)
  - hasSubTenant: boolean (default: true)
  - include: string[] (options: entity, dto, appservice, efconfig) default ["entity", "dto", "appservice", "efconfig"]
  - dry_run: boolean (default: true)
- dry_run_supported: true
- required_rules: ["auto-generation", "entity-patterns", "coding-standards"]
- output_format: "file-tree"
- error_behavior: "stop-and-report"

### scaffold-appservice
- trigger_phrases: ["appservice oluştur", "servis iskele", "yeni servis"]
- action: `scaffold-appservice`
- params:
  - name: string (required — servis adı, PascalCase)
  - entity: string (required — bağlı entity)
  - include: string[] (options: crud, getlist, getbyid, create, update, delete) default ["crud"]
  - dry_run: boolean (default: true)
- dry_run_supported: true
- required_rules: ["auto-generation", "entity-patterns", "coding-standards"]
- output_format: "file-tree"
- error_behavior: "stop-and-report"

### build-project
- trigger_phrases: ["build", "projeyi derle", "compile"]
- action: `build-project`
- params:
  - configuration: "Debug" | "Release" (default: "Debug")
- dry_run_supported: false
- required_rules: []
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- command:
  ```bash
  dotnet build src/Fitliyo.Web/Fitliyo.Web.csproj --configuration {configuration}
  ```

### audit-naming
- trigger_phrases: ["isimlendirme denetle", "naming audit", "isim kontrolü"]
- action: `audit-naming`
- params:
  - scope: string (default: "src")
- dry_run_supported: false
- required_rules: ["coding-standards"]
- output_format: "json-metadata"
- error_behavior: "stop-and-report"

### code-review
- trigger_phrases: ["code review", "kod review", "review yap", "kodu incele", "review et", "PR review"]
- action: `code-review`
- params:
  - scope: string (default: "staged" — staged/file/directory/last-commit)
  - path: string (optional — belirli dosya veya klasör)
  - severity: "all" | "critical" | "high" (default: "all")
  - fix: boolean (default: false — true ise bulguları otomatik düzelt)
- dry_run_supported: true
- required_rules: ["code-review", "code-quality", "security-performance"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    Staged dosyaları, belirli bir dosyayı veya klasörü code review kurallarına göre tarar.
    8 kategori kontrol edilir: Güvenlik, Performans, ABP/Katman, Hata Yönetimi, Loglama, Kod Kalitesi, Multi-Tenancy, Dokümantasyon.
    Her bulgu seviye (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) ve kod referansı ile raporlanır.
    `fix=true` ile otomatik düzeltilebilir sorunlar (kullanılmayan import, naming vb.) otomatik düzeltilir.
    Sonuç standart review rapor formatında döner.

### code-review-file
- trigger_phrases: ["bu dosyayı review et", "dosya review", "bu dosyayı incele"]
- action: `code-review`
- params:
  - scope: "file"
  - path: string (required — dosya yolu, aktif dosya otomatik alınabilir)
  - severity: "all"
- dry_run_supported: false
- required_rules: ["code-review"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    Tek bir dosyayı kapsamlı code review yapar.
    Tüm 8 kategoriyi kontrol eder ve satır numarasıyla rapor üretir.

### code-review-commit
- trigger_phrases: ["commit review", "commit öncesi review", "pre-commit review"]
- action: `code-review`
- params:
  - scope: "staged"
  - severity: "all"
  - fix: false
- dry_run_supported: false
- required_rules: ["code-review", "auto-doc-update"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    Commit öncesi tüm staged dosyaları review eder.
    Kod review + doküman kontrol (CHANGELOG, frontend-changes, kırık link) birlikte çalışır.
    Kritik sorun varsa commit'i engelleme önerisi yapar.

### track-frontend-change
- trigger_phrases: ["frontend değişiklik ekle", "frontend bildirimi ekle", "ui değişiklik kaydet", "frontend change"]
- action: `track-frontend-change`
- params:
  - title: string (required — kısa başlık)
  - type: "breaking" | "non-breaking" | "bugfix" | "doc" | "infra" (required)
  - affected: string (required — etkilenen endpoint/DTO/permission)
  - description: string (required — ne değişti)
  - action_required: string (default: "Yok — backend tarafında tamamlandı.")
  - detail_link: string (optional — ilgili doküman linki)
- dry_run_supported: true
- required_rules: ["frontend-change-tracking"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    `docs/frontend-changes/YYYY-MM-DD-N.md` olarak ayrı dosya oluşturur.
    Tarih otomatik belirlenir (bugünün tarihi).
    Sıra numarası `docs/frontend-changes/` klasöründeki mevcut dosyalardan otomatik hesaplanır.
    Badge değişiklik tipine göre otomatik atanır.
    `docs/frontend-changes/README.md` index listesinin en üstüne yeni girişi ekler.

### check-frontend-impact
- trigger_phrases: ["frontend etkisi kontrol et", "bu commit frontend'i etkiliyor mu", "frontend impact"]
- action: `check-frontend-impact`
- params:
  - scope: string (default: "staged" — staged/unstaged/last-commit)
- dry_run_supported: false
- required_rules: ["frontend-change-tracking"]
- output_format: "json-metadata"
- error_behavior: "stop-and-report"
- description: |
    Staged veya son commit'teki değişiklikleri analiz eder.
    Endpoint, DTO, permission, response format değişikliği varsa frontend etkisini raporlar.
    Etki varsa `track-frontend-change` komutunu önerir.

### full-check
- trigger_phrases: ["full check", "tam kontrol", "her şeyi kontrol et", "toplu kontrol", "hepsini çalıştır", "release check", "tam review"]
- action: `full-check`
- params:
  - fix: boolean (default: false — true ise otomatik düzeltilebilir sorunları düzelt)
  - skip: string[] (optional — atlanacak adımlar, ör. ["swagger", "tests"])
- dry_run_supported: true
- required_rules: ["code-review", "auto-doc-update", "frontend-change-tracking", "coding-standards"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    Tüm kontrolleri sırayla çalıştırır. Her adım bağımsız; biri başarısız olsa da diğerleri çalışır.
    Sonunda toplu özet rapor üretir.

    **Çalıştırma sırası:**
    1. AppService dokümanlarını üret (`generate-appservice-docs`)
    2. Swagger snapshot üret (`generate-swagger`) — *kaynak kod değiştiyse*
    3. Code review (`code-review --scope=staged`)
    4. Commit öncesi doküman kontrolü (`pre-commit-docs-check`)
    5. Yetki denetimi (`audit-permissions`)
    6. İsimlendirme denetimi (`audit-naming`)
    7. Eksik test tespiti ve yazımı (`generate-missing-tests`)
    8. Kırık link kontrolü (`validate-docs-links`)
    9. Etkilenen modüllerin testleri (`bash scripts/run-affected-tests.sh`) — tüm testler için `--all`

    Her adım sonucunda ✅ / ⚠️ / ❌ badge ile sonuç gösterilir.
    Toplu rapor sonunda kaç adım başarılı, kaç uyarı, kaç hata olduğu özetlenir.

### generate-missing-tests
- trigger_phrases: ["eksik testleri yaz", "test üret", "missing tests", "testleri tamamla"]
- action: `generate-missing-tests`
- params:
  - scope: string (default: "staged" — staged/file/directory)
  - path: string (optional — belirli dosya veya klasör)
  - dry_run: boolean (default: true — önce neyin eksik olduğunu raporla)
- dry_run_supported: true
- required_rules: ["code-review", "coding-standards"]
- output_format: "file-tree"
- error_behavior: "stop-and-report"
- description: |
    Değişen veya belirtilen dosyalar için eksik testleri tespit eder ve yazar.

    **Kontrol sırası:**
    1. Değişen AppService/Domain servis dosyalarını tespit et
    2. `test/` altında karşılık gelen test dosyasını ara
    3. Test dosyası yoksa → tamamen yeni test dosyası oluştur
    4. Test dosyası varsa → yeni/değişen public metodlar için eksik test metodu ekle
    5. Her test metodu: arrange-act-assert pattern, anlamlı isim, Türkçe açıklama

    **Test isimlendirme:** `MethodAdi_Senaryo_BeklenenSonuc`
    Örnek: `CreateAsync_ValidInput_ReturnsDto`, `GetAsync_InvalidId_ThrowsEntityNotFound`

### explain-code
- trigger_phrases: ["açıkla", "explain", "ne yapıyor", "nasıl çalışıyor", "bu kodu açıkla", "akışı anlat", "ne işe yarıyor"]
- action: `explain-code`
- params:
  - path: string (optional — dosya yolu veya metot adı)
  - scope: string (optional — "file" / "method" / "flow", default: bağlama göre seç)
- dry_run_supported: false
- required_rules: ["project-structure", "entity-patterns"]
- output_format: "code-block"
- error_behavior: "stop-and-report"
- description: |
    Belirtilen kod, dosya veya iş akışını Türkçe olarak açıklar.

    **Açıklama formatı:**
    1. **Genel Amaç** — Bu kod ne yapar?
    2. **Bağımlılıklar** — Hangi servisleri/entity'leri kullanıyor?
    3. **Akış** — Adım adım neler oluyor?
    4. **Önemli Kararlar** — Neden böyle tasarlanmış?
    5. **Dikkat Edilmesi Gerekenler** — Sınırlamalar, edge case'ler, teknik borç

    Teknik terimler Türkçe açıklamayla verilir.
    Gereksiz detaydan kaçınılır; özlü ve anlaşılır olunur.
    Varsa ilgili dokümantasyona link verilir.

---

## Tam Kontrol (full-check) Çıktı Formatı

```markdown
## Tam Kontrol Raporu — YYYY-MM-DD HH:MM

| # | Adım | Durum | Detay |
|---|------|-------|-------|
| 1 | AppService dokümanları | ✅ | 3 dosya güncellendi |
| 2 | Swagger snapshot | ⏭️ | Atlandı (kaynak kod değişmedi) |
| 3 | Code review | ⚠️ | 2 uyarı, 0 kritik |
| 4 | Doküman kontrolü | ✅ | CHANGELOG güncel, linkler temiz |
| 5 | Yetki denetimi | ✅ | Tüm metodlarda [Authorize] mevcut |
| 6 | İsimlendirme denetimi | ✅ | Convention'a uygun |
| 7 | Eksik testler | ⚠️ | 2 metod için test eksik (oluşturuldu) |
| 8 | Kırık linkler | ✅ | 0 kırık link |

### Özet
- ✅ Başarılı: 6
- ⚠️ Uyarı: 2
- ❌ Hata: 0
- ⏭️ Atlandı: 1

### Detaylı Bulgular
(Her adımın bulguları aşağıda listelenir...)
```

---

## Yanıt Beklentileri

- **file-tree**: Tek bir fenced code block ile ağaç yapısı + yeni/değişen dosyalar listesi ve kısa JSON metadata (`{ files, actions }`).
- **json-metadata**: JSON code block: `{ status, details, warnings, errors }`.
- **code-block**: Sadece kod ve altında tek satır özet.

## Hata ve Durdurma Politikası

- Gerekli kural ihlal edilirse veya swagger eksikse → dur ve `error_behavior` payload'ı ile tam eksik öğeler ve önerilen düzeltmeyi döndür.
- Alan adları veya operationId'ler tahmin edilmez. Belirsizse → dur ve raporla.

## PR / Git Konvansiyonları

Dosya oluşturan her komut için `pr_suggestion` nesnesi döndür:
```json
{
  "branch": "feature/<kisa-aciklama>",
  "commit_message": "<tip>(<kapsam>): kısa özet",
  "pr_title": "Başlık",
  "pr_body": "Açıklama"
}
```

- Branch format: `feature/`, `bugfix/`, `hotfix/`, `refactor/`
- Commit format: `<tip>(<kapsam>): kısa Türkçe özet`

## Örnekler

- "Swagger üret" → `generate-swagger --output=docs/openapi/swagger.web.v1.full.json`
- "Entity oluştur UserCertificate" → `scaffold-entity --name=UserCertificate --multiTenant=true --dry_run=true`
- "Migration ekle AddUserCertificate" → `run-migrations --name=AddUserCertificate --action=add`
- "Testleri çalıştır" → `run-tests`
- "Code review" → `code-review --scope=staged --severity=all`
- "Bu dosyayı review et" → `code-review --scope=file --path=src/Fitliyo.Application/Users/UserAppService.cs`
- "Commit öncesi review" → `code-review --scope=staged` + doküman kontrol
- "Frontend değişiklik ekle" → `track-frontend-change --title="UserDto'ya middleName eklendi" --type=non-breaking --affected="GET /api/app/user" --description="Opsiyonel alan eklendi"`
- "Bu commit frontend'i etkiliyor mu" → `check-frontend-impact --scope=staged`
- "Full check" → `full-check` (8 adım sırayla çalışır)
- "Tam kontrol" → `full-check`
- "Her şeyi kontrol et" → `full-check`
- "Eksik testleri yaz" → `generate-missing-tests --scope=staged --dry_run=false`
- "Full check ama swagger atla" → `full-check --skip=["swagger"]`
- "Bu kodu açıkla" → `explain-code --scope=file --path=src/Fitliyo.Application/Users/UserAppService.cs`
- "CreateAsync nasıl çalışıyor" → `explain-code --scope=method --path=UserLeaveAppService.CreateAsync`
- "İzin akışını anlat" → `explain-code --scope=flow`

## Hızlı Kurallar

- Varsayılan olarak dry-run destekle.
- Belirsizlikte kullanıcı açıkça izin vermediği sürece dur.
- Çıktılar makine-dostu (JSON/kod) olsun; programatik uygulanabilsin.

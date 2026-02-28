# Fitliyo Backend Dokümantasyonu

**Kapsam:** Bu repo'nun **backend** (ABP.io / .NET 9) mimarisi, API sözleşmeleri, marketplace iş kuralları, ödeme akışı, cache, logging, background job ve operasyonel süreçleri.

**Platform:** Fitliyo — Personal trainer, diyetisyen, basketbol/futbol ve diğer spor koçlarını öğrencilerle buluşturan çok taraflı marketplace platformu.

> **Not:** Web frontend (Next.js 15) ve mobil (React Native + Expo) ayrı repo'larda geliştirilmektedir. Bu repoda UI/UX dokümantasyonu tutulmaz. Frontend ekibi için gereken bilgiler burada **API sözleşmesi** (endpoint'ler, izinler, error formatı, header'lar, cache davranışları) seviyesinde yer alır.

---

## 🏗️ Teknoloji Stack

| Bileşen | Teknoloji |
|---------|-----------|
| Backend Framework | .NET 9 + ABP Framework (Tiered Application) |
| Veritabanı | PostgreSQL 16 |
| Cache | Redis |
| Background Jobs | Hangfire |
| Dosya Depolama | AWS S3 / Azure Blob |
| E-posta | SendGrid |
| Push Notification | Firebase (FCM) |
| Ödeme | iyzico (TR) + Stripe (Uluslararası) |
| Real-time | SignalR |
| Logging | Serilog + Elasticsearch |
| Auth | OpenIddict (JWT Bearer) |

---

## 👥 Kullanıcı Rolleri

| Rol | Açıklama |
|-----|----------|
| **SuperAdmin** | Tam yetki, platform yönetimi |
| **Admin** | Kullanıcı/içerik yönetimi, raporlar, moderasyon |
| **Trainer** | Profil, paket, takvim, müşteri yönetimi, wallet |
| **Student** | Paket satın alma, seans takibi, mesajlaşma, yorum |
| **Guest** | Arama ve profil görüntüleme (anonim) |

**Detaylar:** [`AUTHORIZATION-SYSTEM.md`](AUTHORIZATION-SYSTEM.md)

---

## 🧩 Modül Yapısı

| # | Modül | Kapsam |
|---|-------|--------|
| 1 | TrainerModule | Profil, sertifika, galeri, müsaitlik |
| 2 | PackageModule | Paket tipleri, fiyatlandırma, takvim şablonu |
| 3 | OrderModule | Sipariş, seans kayıtları, iptal/iade |
| 4 | PaymentModule | Ödeme, escrow, wallet, para çekme |
| 5 | SubscriptionModule | Eğitmen abonelik planları, otomatik yenileme |
| 6 | MessagingModule | Conversation, Message, SignalR |
| 7 | ReviewModule | Değerlendirme, puan hesaplama |
| 8 | NotificationModule | In-app, e-posta, push |
| 9 | ContentModule | Blog, kategori, SEO |
| 10 | AdminModule | Raporlar, moderasyon, destek |

---

## 📚 Başlangıç

- **Dokümantasyon versiyonlama**: [`docs/VERSIONING.md`](VERSIONING.md)
- **Dokümantasyon workflow / katkı kuralı**: [`docs/standards/DOCUMENTATION_WORKFLOW.md`](standards/DOCUMENTATION_WORKFLOW.md)
- **Hata yönetimi standardı**: [`docs/standards/ERROR_HANDLING.md`](standards/ERROR_HANDLING.md)

## 🔐 Yetkilendirme

- **Yetkilendirme sistemi**: [`docs/AUTHORIZATION-SYSTEM.md`](AUTHORIZATION-SYSTEM.md)
- **Authentication akışı**: [`docs/security/AUTH_LOGIN_AND_CORS.md`](security/AUTH_LOGIN_AND_CORS.md)

## 💰 İş Kuralları

- **Veritabanı şeması**: [`docs/DATABASE-SCHEMA.md`](DATABASE-SCHEMA.md)
- **İş kuralları (ödeme, komisyon, iptal)**: [`docs/BUSINESS-RULES.md`](BUSINESS-RULES.md)

## ⚙️ Altyapı / Performans

- **Docker (kurulum & çalışma)**: [`docs/infrastructure/DOCKER.md`](infrastructure/DOCKER.md)
- **Redis cache (pattern, TTL, invalidation)**: [`docs/infrastructure/REDIS_CACHE_IMPLEMENTATION.md`](infrastructure/REDIS_CACHE_IMPLEMENTATION.md)
- **Elasticsearch / loglama standardı**: [`docs/observability/ELASTICSEARCH_GENERIC_LOGGING.md`](observability/ELASTICSEARCH_GENERIC_LOGGING.md)

## 🧰 Operasyonlar

- **Deployment**: [`docs/operations/DEPLOYMENT.md`](operations/DEPLOYMENT.md)
- **DB Backup / Restore**: [`docs/operations/DB_BACKUP_RESTORE.md`](operations/DB_BACKUP_RESTORE.md)
- **Background Job Sistemi**: [`docs/operations/README_JOB_SYSTEM.md`](operations/README_JOB_SYSTEM.md)

## 🔌 API Sözleşmesi ve İstemci Etkileri

- **Frontend değişiklik takibi**: [`docs/frontend-changes/README.md`](frontend-changes/README.md)

## 📄 Swagger / OpenAPI Snapshot

- **Web API sözleşmesi**: `docs/openapi/swagger.web.v1.full.json`
- Bu dosya AppService dokümanlarının endpoint/request/response/model bölümleri için **tek doğruluk kaynağıdır**.

## 🧩 Kural ve Standartlar

- **Cursor çalışma rehberi**: [`docs/standards/README-CURSOR.md`](standards/README-CURSOR.md)
- **Dokümantasyon workflow**: [`docs/standards/DOCUMENTATION_WORKFLOW.md`](standards/DOCUMENTATION_WORKFLOW.md)
- **Hata yönetimi standardı**: [`docs/standards/ERROR_HANDLING.md`](standards/ERROR_HANDLING.md)

---

## 📝 Changelog

- **Aktif ay:** [`docs/CHANGELOG.md`](CHANGELOG.md)
- **Arşiv:** [`docs/changelog/`](changelog/) — aylık arşivlenmiş girişler

---

## 🚀 Geliştirme Fazları

| Faz | Modüller | Durum |
|-----|----------|-------|
| Faz 1 | ABP iskelet → TrainerModule → PackageModule → Arama | 🔄 Devam ediyor |
| Faz 2 | OrderModule → PaymentModule → WalletModule → SubscriptionModule | ⏳ Planlandı |
| Faz 3 | MessagingModule → ReviewModule → NotificationModule → AdminModule | ⏳ Planlandı |
| Faz 4 | React Native mobil → Launch hazırlığı | ⏳ Planlandı |

---

**Dokümantasyon Durumu:** Marketplace mimarisine göre yeniden yapılandırıldı
**Son Güncelleme:** 2026-02-28
**Doküman Versiyonu:** v4.0

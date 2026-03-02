# Fitliyo Changelog

## 2026-03-02 - Marketplace test verileri (Data Seeder)

- **FitliyoMarketplaceDataSeedContributor:** Tüm marketplace entity'leri için anlamlı test verileri eklendi. DbMigrator çalıştığında (önce Identity seed: admin, egitmen, ogrenci), ardından eğitmen profili yoksa: kategoriler (Fitness, Spor Koçluğu, Beslenme, Pilates, Yoga), eğitmen profili (Ahmet Yılmaz, sertifikalar, galeri), abonelik planları (Ücretsiz, Basic, Pro), hizmet paketi, sipariş/seans/ödeme, değerlendirme, mesajlaşma, bildirimler, destek talebi, öne çıkanlar, blog yazıları, kullanıcı profilleri (sağlık), para çekme talebi, uyuşmazlık kaydı seed edilir. Tüm alanlar ilgili property'lere uygun anlamlı değerlerle doldurulur.
- **docs/SEED_KULLANICILAR.md:** Marketplace test verileri bölümü eklendi.

---

## 2026-03-02 - Öğrenci ve eğitmen için sağlık/profil sayfası (BMR, BMI, TDEE)

### Backend
- **UserProfile entity:** Kullanıcıya özel sağlık ve profil bilgisi (doğum tarihi, cinsiyet, boy, kilo, kan grubu, aktivite düzeyi, hedef, kronik hastalıklar, alerjiler, ilaçlar, sakatlıklar, acil iletişim, bel/kalça/boyun ölçüleri, uyku, sigara, alkol, dinlenim kalp atışı vb.). `AppUserProfiles` tablosu migration ile eklendi.
- **Enum’lar:** `Gender`, `ActivityLevel`, `FitnessGoal` (Domain.Shared).
- **UserProfileAppService:** `GetMyProfileAsync`, `UpdateMyProfileAsync`. Hesaplanan alanlar: yaş, BMI, BMR (Mifflin-St Jeor), TDEE (aktivite çarpanı), ideal kilo aralığı (BMI 18.5–24.9), vücut yağ % (US Navy formülü, bel/boyun/kalça ile).
- **API:** `GET /api/app/userProfile/getMyProfile`, `PUT /api/app/userProfile/updateMyProfile`. Sadece giriş yapan kullanıcı kendi profilini okur/günceller.

### Frontend
- **Profil sayfası:** Öğrenci (`/student/profile`) ve eğitmen (`/trainer/profile`) için ortak sağlık/profil formu. Kişisel bilgiler, ölçüler, sağlık bilgileri (kronik hastalık, alerji, ilaç, sakatlık, uyku, sigara, alkol, dinlenim nabız). Kaydetme sonrası hesaplanan metrikler (yaş, BMI, BMR, TDEE, ideal kilo aralığı, vücut yağ %) kartta gösteriliyor.
- **Öğrenci menü:** "Profilim" linki eklendi.
- **Tasarım:** Apple design system (card-apple, btn-apple-primary, text-apple-*).

### Dokümantasyon
- Migration: `20260302000000_AddUserProfiles.cs`. Snapshot’a `UserProfile` entity eklendi.

---

## 2026-02-28 - CORS eklendi (403 Forbidden /connect/token düzeltmesi)

- **FitliyoWebModule:** CORS yapılandırması eklendi. `ConfigureCors` ile `App:CorsOrigins` (varsayılan: `http://localhost:3000,http://localhost:5000`) kullanılıyor; `AllowAnyHeader`, `AllowAnyMethod`, `AllowCredentials`. Pipeline’da `app.UseCors()` eklendi (UseRouting’den önce).
- **appsettings.Development.json:** `App:CorsOrigins` eklendi: `http://localhost:3000,http://localhost:5000`.
- **docs/CALISTIRMA.md:** "Backend erişilebilir mi? Nasıl test edilir?" (curl, Swagger) ve "403 Forbidden — POST /connect/token" bölümleri eklendi.

---

## 2026-02-28 - Backend launch/tasks düzeltmesi (Swagger, port 5000)

- **launch.json:** Backend tek portta çalışacak şekilde güncellendi: `ASPNETCORE_URLS=http://localhost:5000`. HTTPS kaldırıldı (yerel geliştirmede sertifika sorunları önlenir). `serverReadyAction` ile backend hazır olunca **http://localhost:5000/swagger** otomatik açılır.
- **tasks.json:** Solution dosyası `Fitliyo.sln` → `Fitliyo.slnx` olarak güncellendi (Build Solution, Clean, Restore, Run Tests).
- **Web projesi:** `launchSettings.json` ve `appsettings.json` → `applicationUrl` / `SelfUrl` **http://localhost:5000** yapıldı. `appsettings.Development.json` eklendi: yerel PostgreSQL connection string (localhost:5432) ve `App.SelfUrl`.
- **Frontend:** Varsayılan API adresi `http://localhost:44331` → `http://localhost:5000` (lib/api.ts, giris/kayit sayfaları, .env.example, next.config.mjs, README).
- **docs/CALISTIRMA.md:** Port 5000 ve Swagger adresi güncellendi; "Backend ayağa kalkmıyorsa / Swagger açılmıyorsa" bölümü eklendi.

---

## 2026-03-01 - Kayıt / giriş tamamlama ve DataSeeder (Admin, Eğitmen, Öğrenci)

### Backend
- **OpenIddict Fitliyo_App:** Password + RefreshToken grant ile `Fitliyo_App` client’ı seed’e eklendi. DbMigrator `appsettings.json` içine `Fitliyo_App` (RootUrl: http://localhost:3000) tanımı eklendi.
- **FitliyoIdentityDataSeedContributor:** EntityFrameworkCore/Data altında eklendi. Roller: Admin, Trainer, Student. Kullanıcılar: admin@fitliyo.com (Admin), egitmen@fitliyo.com (Trainer), ogrenci@fitliyo.com (Student); şifre hepsi `Test123!`. DbMigrator her çalıştığında rol ve kullanıcılar yoksa oluşturulur.
- **FitliyoAccountAppService:** Kayıt sırasında `ExtraProperties["Role"]` (Trainer veya Student) okunup kullanıcıya atanıyor. `IAccountAppService` implementasyonu FitliyoAccountAppService ile değiştirildi.

### Frontend
- **Kayıt sayfası (/kayit):** Form alanları: kullanıcı adı, e-posta, hesap tipi (Öğrenci/Eğitmen), şifre, şifre tekrar. `POST /api/account/register` ile kayıt; başarıda `/giris?kayit=ok` yönlendirmesi.
- **Giriş sayfası (/giris):** `?kayit=ok` geldiğinde “Kayıt tamamlandı” mesajı gösteriliyor.

### Dokümantasyon
- **docs/CALISTIRMA.md:** Seed kullanıcı tablosu (admin, egitmen, ogrenci; şifre Test123!) eklendi.

---

## 2026-03-01 - Çalıştırma rehberi ve VS Code launch/tasks

- **Çalıştırma rehberi:** `docs/CALISTIRMA.md` eklendi. Veritabanı kurulumu, backend/frontend çalıştırma, VS Code launch ve task kullanımı adım adım anlatıldı.
- **launch.json:** Yinelenen içerik kaldırıldı. "Frontend (Next.js)" konfigürasyonu ve "Backend + Frontend (Web + Next.js)" compound eklendi. Backend portları 44331 (http) / 44332 (https) korundu.
- **tasks.json:** "Frontend: npm install" ve "Frontend: npm run dev" task’ları eklendi.
- **frontend:** Varsayılan API URL http://localhost:44331 olacak şekilde güncellendi (.env.example, lib/api.ts, giris sayfası). README ve docs/README’da CALISTIRMA.md linki eklendi.

---

## 2026-03-01 - Web Frontend Planı ve İskelet

- **Frontend planı:** `docs/FRONTEND_PLANI.md` eklendi. Rol bazlı (Öğrenci / Eğitmen / Admin) sayfa yapısı, API eşlemesi, uygulama fazları.
- **Next.js frontend** `frontend/` altında: Next.js 14, TypeScript, Tailwind; ortak layout (Header, footer); ana sayfa, giriş (token + role göre yönlendirme), kayıt placeholder; `/ogrenci`, `/egitmen`, `/admin` layout + menü + placeholder sayfalar; `lib/auth.ts`, `lib/api.ts`, `.env.example`. Detay: [FRONTEND_PLANI.md](FRONTEND_PLANI.md).

---

## 2026-03-01 - Eksiklik Analizi Uygulaması (Entity ve Alanlar)

### Session
- `Location` (string, max 256) ve `RescheduledFromSessionId` (Guid?) alanları eklendi.
- `OrderConsts.MaxLocationLength` eklendi.

### Order
- `PaymentId` (Guid?, opsiyonel) eklendi — ilişkili ödeme kaydı referansı.

### Payment modülü (yeni)
- **Payment** entity: OrderId, PaymentProvider (enum), ProviderPaymentId, Amount, Currency, Status (PaymentRecordStatus), PaymentMethod, CardLastFour, ReceiptUrl, RefundAmount, RefundedAt, ProviderResponse.
- **TrainerWallet** entity: TrainerProfileId, AvailableBalance, PendingBalance, TotalEarned, TotalWithdrawn, LastPayoutAt.
- **WalletTransaction** entity: TrainerWalletId, TransactionType (Credit/Debit/Refund/Payout), Amount, Description, ReferenceId, BalanceAfter.
- **WithdrawalRequest** entity: TrainerWalletId, Amount, Status, Iban, AccountHolderName, AdminNote, ProcessedAt.
- Enum’lar: PaymentProviderEnum, PaymentMethodEnum, PaymentRecordStatus, WalletTransactionType, WithdrawalRequestStatus.
- Consts: PaymentConsts, WalletConsts, WithdrawalConsts.
- Hata kodları: PaymentNotFound, WalletNotFound, InsufficientBalance, WithdrawalRequestNotFound.

### Review modülü (genişletme)
- **Review**: ServicePackageId, OverallRating (decimal), CommunicationRating, ExpertiseRating, ValueForMoneyRating, PunctualityRating (1–5), IsVerifiedPurchase, IsPublished, HelpfulCount.
- **ReviewHelpfulVote** entity: ReviewId, VoterUserId, IsHelpful.
- ReviewConsts.MaxAdminNoteLength eklendi.

### Support modülü (yeni)
- **SupportTicket** entity: Subject, Message, UserId, OrderId, Category, Status, Priority, AdminReply, AdminReplyDate, AssignedToUserId.
- Enum’lar: SupportTicketCategory, SupportTicketStatus, TicketPriority.
- SupportTicketConsts; hata kodu: SupportTicketNotFound.

### Admin modülü (yeni entity’ler)
- **FeaturedListing**: PageType (Homepage/Category/Search), TrainerProfileId, ServicePackageId, SortOrder, StartDate, EndDate, IsActive, AdminNote.
- **Dispute**: OrderId, OpenedByUserId, DisputeType, Description, Status, ResolutionNote, ResolvedByUserId, ResolvedAt.
- Enum’lar: FeaturedListingPageType, DisputeType, DisputeStatus.
- Consts: FeaturedListingConsts, DisputeConsts; hata kodları: DisputeNotFound, FeaturedListingNotFound.

### Content modülü (yeni)
- **BlogPost** entity: Title, Slug, Summary, Body, Status (Draft/Published/Archived), PublishedAt, AuthorName, AuthorUserId, FeaturedImageUrl.
- BlogPostConsts; BlogPostStatus enum; hata kodları: BlogPostNotFound, BlogPostSlugAlreadyExists.

### EF Core
- Tüm yeni entity’ler için DbSet ve Fluent API konfigürasyonu eklendi.
- Session için Location ve RescheduledFromSessionId; Review için yeni alanlar ve ServicePackageId FK; Order için PaymentId index.

### Migration
- `AddEksiklikEntityleri` migration’ı eklenmeli: `dotnet ef migrations add AddEksiklikEntityleri --project src/Fitliyo.EntityFrameworkCore --startup-project src/Fitliyo.Web --context FitliyoDbContext`

Detaylar: [`docs/EKSIKLIK_ANALIZI.md`](EKSIKLIK_ANALIZI.md)

---

## 2026-03-01 - Proje Rehberi (PROJE_REHBERI.md)

### Yeni doküman
- **docs/PROJE_REHBERI.md** eklendi: Projenin amacı, mevcut durum, tüm entity'lerin iş anlamları, iş kuralları özeti, projenin yapabildikleri ve ileriye dönük yapılabilecekler tek dokümanda anlatılıyor. `docs/README.md` içinde rehber linki eklendi.

---

## 2026-03-01 - Payment, Wallet, Support, Admin, Content AppService ve API

### Yeni AppService’ler ve API
- **PaymentAppService**: GetAsync, GetListAsync, GetByOrderIdAsync. Permission: Payments.View, Payments.Default.
- **WalletAppService**: GetMyWalletAsync (eğitmen cüzdanı, yoksa oluşturur), GetMyTransactionsAsync. Permission: Wallet.Default.
- **WithdrawalRequestAppService**: CreateAsync, GetAsync, GetMyRequestsAsync, GetListAsync (admin), ApproveAsync, RejectAsync, MarkProcessedAsync. Permission: Withdrawal.Default, Withdrawal.Manage.
- **SupportTicketAppService**: CreateAsync, GetAsync, GetMyTicketsAsync, GetListAsync (admin), ReplyAsync, UpdateStatusAsync. Permission: Support.Default, Support.Manage.
- **FeaturedListingAppService**: GetAsync, GetListAsync, CreateAsync, UpdateAsync, DeleteAsync. Permission: Admin.FeaturedListings.
- **DisputeAppService**: CreateAsync, GetAsync, GetMyDisputesAsync, GetListAsync (admin), ResolveAsync. Permission: Admin.Disputes.
- **BlogPostAppService**: GetAsync, GetBySlugAsync (AllowAnonymous), GetListAsync, GetPublishedListAsync (AllowAnonymous), CreateAsync, UpdateAsync, DeleteAsync, PublishAsync. Permission: Content.Default, Content.Create/Edit/Delete.

### Permissions
- FitliyoPermissions’a eklenenler: Payments (Default, View), Wallet (Default), Withdrawal (Default, Manage), Support (Default, Manage), Admin.FeaturedListings, Admin.Disputes, Content (Default, Create, Edit, Delete).
- FitliyoPermissionDefinitionProvider’da ilgili izinler tanımlandı.

### HttpApi
- Yeni controller’lar: PaymentController, WalletController, WithdrawalRequestController, SupportTicketController, FeaturedListingController, DisputeController, BlogPostController. Route prefix: `api/app/`.

---

## 2026-02-28 - Marketplace Mimarisi Yeniden Yapılandırma

### Platform dönüşümü: İK Yönetim Sistemi → Spor Koçluğu Marketplace
- Proje tanımı ve kimliği güncellendi (Spor & Sağlık Koçluğu Marketplace)
- Kullanıcı rolleri güncellendi: SuperAdmin, Admin, Trainer, Student, Guest
- Teknoloji stack güncellendi: PostgreSQL, AWS S3, SendGrid, FCM, iyzico + Stripe
- 10 modüllü marketplace mimarisi tanımlandı
- Veritabanı şeması oluşturuldu (25+ tablo)
- İş kuralları dokümante edildi (ödeme, escrow, komisyon, iptal/iade, abonelik)
- Yetkilendirme sistemi marketplace'e göre yeniden tasarlandı
- Cursor kuralları yeni mimariyle uyumlu güncellendi
- Detaylar: [`docs/README.md`](README.md), [`docs/DATABASE-SCHEMA.md`](DATABASE-SCHEMA.md), [`docs/BUSINESS-RULES.md`](BUSINESS-RULES.md), [`docs/AUTHORIZATION-SYSTEM.md`](AUTHORIZATION-SYSTEM.md)

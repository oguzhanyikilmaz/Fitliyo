# Fitliyo Proje Rehberi

**Amaç:** Projenin ne olduğu, hedefi, şu anki durumu, tüm entity’ler, iş kuralları ve ileriye dönük yapılabileceklerin tek dokümanda özetlenmesi.  
**Hedef kitle:** Geliştirici, ürün yöneticisi, yeni katılan ekip üyesi.  
**Son güncelleme:** 2026-03-01

---

## 1. Proje Nedir? Amacı Ne?

**Fitliyo**, **spor ve sağlık koçluğu marketplace**’idir. Yani:

- **Eğitmenler** (personal trainer, diyetisyen, basketbol/futbol koçu, yoga eğitmeni vb.) profillerini açar, hizmet **paketleri** tanımlar.
- **Öğrenciler** bu paketleri arar, satın alır, **seans** (ders) planlar ve eğitmenle çalışır.
- **Platform** aracı olur: listeleme, sipariş, ödeme, cüzdan, para çekme, değerlendirme, mesajlaşma, bildirim, destek ve içerik (blog) sunar.

**Tek cümle:** Eğitmenleri öğrencilerle buluşturan, sipariş–ödeme–cüzdan–yorum–destek akışına sahip çok taraflı bir pazar yeri.

---

## 2. Teknoloji ve Mimari (Kısa)

| Bileşen | Teknoloji |
|--------|-----------|
| Backend | .NET 9, ABP Framework (DDD, modüler) |
| Veritabanı | PostgreSQL 16 |
| Cache | Redis |
| Auth | OpenIddict (JWT Bearer) |
| Dosya | AWS S3 / Azure Blob (planlanan) |
| Ödeme | iyzico + Stripe (planlanan entegrasyon) |
| Real-time | SignalR (planlanan) |
| Frontend | Next.js 15 (ayrı repo) |
| Mobil | React Native + Expo (ayrı repo) |

Bu rehber **backend** (bu repo) odaklıdır; API ve iş mantığı burada tanımlanır.

---

## 3. Şu Ana Kadar Neler Yaptık? (Mevcut Durum)

### 3.1 Tamamlanan İşler (Özet)

1. **Mimari dönüşüm**  
   Proje, “İK yönetim sistemi” şablonundan **marketplace** mimarisine çekildi: roller (Trainer, Student, Admin), modüller ve veritabanı şeması buna göre güncellendi.

2. **Tüm temel entity’ler**  
   Aşağıda listelenen entity’ler domain’de tanımlı; çoğu için tablolar (migration) ve EF Core konfigürasyonu mevcut.

3. **Session ve Order genişletmeleri**  
   - Session: `Location`, `RescheduledFromSessionId`  
   - Order: `PaymentId` (opsiyonel, ödeme kaydına bağlantı)

4. **Payment modülü (entity + API)**  
   - **Payment**: Siparişe bağlı ödeme kaydı (sağlayıcı, tutar, durum, iade).  
   - **TrainerWallet**: Eğitmen bakiyesi (Available, Pending, TotalEarned, TotalWithdrawn).  
   - **WalletTransaction**: Bakiye hareketleri (Credit, Debit, Refund, Payout).  
   - **WithdrawalRequest**: Para çekme talebi (IBAN, durum, admin notu).  
   - **PaymentAppService**, **WalletAppService**, **WithdrawalRequestAppService** ve ilgili API’ler eklendi.

5. **Review genişletmesi**  
   - Çok boyutlu puan: `OverallRating`, `CommunicationRating`, `ExpertiseRating`, `ValueForMoneyRating`, `PunctualityRating`.  
   - `ServicePackageId`, `IsVerifiedPurchase`, `IsPublished`, `HelpfulCount`.  
   - **ReviewHelpfulVote** entity’si (kim, hangi yoruma “faydalı” dedi).

6. **Support modülü**  
   - **SupportTicket** entity (konu, mesaj, kategori, durum, öncelik, admin yanıtı).  
   - **SupportTicketAppService** ve API (talep açma, listeleme, yanıt, durum güncelleme).

7. **Admin modülü (yeni entity + API)**  
   - **FeaturedListing**: Öne çıkan listeleme (sayfa tipi, eğitmen/paket, sıra, tarih aralığı).  
   - **Dispute**: Uyuşmazlık (sipariş, açan kullanıcı, tip, açıklama, çözüm).  
   - **FeaturedListingAppService**, **DisputeAppService** ve API’ler.

8. **Content modülü**  
   - **BlogPost** entity (başlık, slug, özet, gövde, durum, yayın tarihi, yazar).  
   - **BlogPostAppService** ve API (CRUD, slug ile getir, yayınlanan liste, yayınlama).

9. **Yetkilendirme**  
   Tüm yeni modüller için permission’lar tanımlandı (Payments, Wallet, Withdrawal, Support, Admin.FeaturedListings, Admin.Disputes, Content).

10. **Docker / dağıtım**  
    Web, DbMigrator, PostgreSQL, Redis için Docker yapılandırması ve port (örn. 6000) ayarları yapıldı.

### 3.2 Henüz Yapılmayan / Eksik Kalanlar (Kısa)

- **Ödeme entegrasyonu:** iyzico ve Stripe için gerçek checkout / webhook implementasyonu yok (entity ve API iskeleti var).  
- **Escrow otomasyonu:** Sipariş ödeme → cüzdan Pending, seans tamamlanınca → Available’a geçiş için otomatik job/akış tam değil.  
- **Rezervasyon (booking) API’si:** “Slot seç → seans oluştur” için özel endpoint yok; Order + Session ile kısmen kapatılıyor.  
- **SignalR:** Mesajlaşma ve bildirim için hub’lar yok.  
- **Profil tamamlanma %:** Hesaplanan alan veya endpoint yok.  
- **Abonelik otomatik yenileme:** Recurring ödeme ve bildirim akışı yok.  
- **Gelişmiş arama:** Rating, yanıt oranı, şehir vb. ile sıralama/algoritma tam netleşmemiş.

Detaylı liste: [`docs/EKSIKLIK_ANALIZI.md`](EKSIKLIK_ANALIZI.md).

---

## 4. Tüm Entity’ler ve İş Anlamları

Aşağıdaki liste **Domain**’deki entity’leri, kısa iş anlamı ve birbirleriyle ilişkileriyle verir.

### 4.1 Eğitmen ve Kategori

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **TrainerProfile** | Eğitmenin platform profili (tek kullanıcı = tek profil). | UserId, Slug, Bio, TrainerType, City, AverageRating, IsVerified, SubscriptionTier. |
| **TrainerCertificate** | Eğitmenin sertifikaları. | TrainerProfileId, CertificateName, IssuingOrganization, IssueDate, DocumentUrl. |
| **TrainerGallery** | Eğitmene ait medya (foto/video). | TrainerProfileId, MediaType, MediaUrl, ThumbnailUrl, SortOrder. |
| **Category** | Hizmet kategorisi (örn. Personal Training, Beslenme). | Name, Slug, ParentId, IconUrl. |
| **TrainerCategoryMapping** | Eğitmen–kategori eşlemesi (bir eğitmen birden fazla kategori). | TrainerProfileId, CategoryId. |

**İş mantığı:** Eğitmen kayıt olur → profil oluşturur → sertifika ve galeri ekler → kategorilere bağlanır. Arama/listeleme kategori ve profile göre yapılır.

---

### 4.2 Paket (Hizmet Paketi)

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **ServicePackage** | Eğitmenin sattığı hizmet paketi (tek seans, 10 ders paketi vb.). | TrainerProfileId, PackageType, Title, Price, DiscountedPrice, SessionCount, MaxStudents, IsOnline, IsOnSite. |
| **PackageAvailabilitySchedule** | Paketin haftalık müsaitlik şablonu (hangi gün/saat). | ServicePackageId, DayOfWeek, StartTime, EndTime. |
| **PackageUnavailableDate** | Belirli tarihlerde müsait değil (tatil, özel gün). | ServicePackageId, UnavailableDate. |

**İş mantığı:** Eğitmen paket açar; fiyat, seans sayısı, müsaitlik ve müsait olmayan günler tanımlanır. Öğrenci paketi seçerek sipariş oluşturur.

---

### 4.3 Sipariş ve Seans

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **Order** | Öğrencinin bir paket satın alması (sipariş). | OrderNumber, StudentId, TrainerProfileId, ServicePackageId, Status, PaymentStatus, NetAmount, CommissionAmount, TrainerPayoutAmount, PaymentId, PaidAt, CompletedAt. |
| **Session** | Siparişe bağlı tek bir ders/seans. | OrderId, TrainerProfileId, StudentId, ScheduledStartTime, ScheduledEndTime, ActualStartTime, ActualEndTime, Status, MeetingUrl, Location, RescheduledFromSessionId, SequenceNumber. |

**İş mantığı:** Öğrenci paket seçer → Order oluşur (Pending). Ödeme sonrası Confirmed; seanslar Session olarak tutulur. Seans tamamlandıkça Order tamamlanabilir; komisyon ve eğitmen payı Order üzerinde hesaplanır.

---

### 4.4 Ödeme ve Cüzdan

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **Payment** | Siparişe bağlı ödeme kaydı (1 Order → 1 Payment). | OrderId, PaymentProvider, ProviderPaymentId, Amount, Currency, Status, RefundAmount, RefundedAt. |
| **TrainerWallet** | Eğitmen cüzdanı (bakiye özeti). | TrainerProfileId, AvailableBalance, PendingBalance, TotalEarned, TotalWithdrawn, LastPayoutAt. |
| **WalletTransaction** | Cüzdan hareketi (her bakiye değişimi). | TrainerWalletId, TransactionType (Credit/Debit/Refund/Payout), Amount, Description, ReferenceId, BalanceAfter. |
| **WithdrawalRequest** | Eğitmenin para çekme talebi. | TrainerWalletId, Amount, Status (Pending/Approved/Rejected/Processed), Iban, AccountHolderName, AdminNote, ProcessedAt. |

**İş mantığı:** Ödeme gelir → (ileride) PendingBalance artar; seans tamamlanınca AvailableBalance’a aktarılır. Eğitmen para çekme talebi açar → admin onaylar/reddeder → onaylananlar “işlendi” olunca Available’dan düşülür.

---

### 4.5 Değerlendirme

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **Review** | Öğrencinin sipariş/eğitmen için yorumu (sipariş başına 1). | OrderId, StudentId, TrainerProfileId, ServicePackageId, Rating, OverallRating, CommunicationRating, ExpertiseRating, ValueForMoneyRating, PunctualityRating, Comment, TrainerReply, IsVerifiedPurchase, IsPublished, HelpfulCount. |
| **ReviewHelpfulVote** | “Bu yorum faydalı” oyu. | ReviewId, VoterUserId, IsHelpful. |

**İş mantığı:** Sadece tamamlanmış sipariş sahibi yorum yapabilir; eğitmen yanıt verebilir. Çok boyutlu puanlar ve HelpfulCount ile sıralama/istatistik yapılabilir.

---

### 4.6 Mesajlaşma

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **Conversation** | İki kullanıcı arasındaki sohbet. | InitiatorId, ParticipantId (unique çift). |
| **Message** | Konuşmadaki tek mesaj. | ConversationId, SenderId, Content, AttachmentUrl, IsRead. |

**İş mantığı:** Eğitmen–öğrenci iletişimi; thread bazlı mesajlaşma. Real-time için SignalR planlanıyor.

---

### 4.7 Bildirim

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **Notification** | Kullanıcıya giden tek bildirim. | UserId, NotificationType, Channel, Title, Body, ActionUrl, IsRead, ReadAt. |

**İş mantığı:** Sipariş, mesaj, sistem duyuruları vb. için in-app (ve ileride e-posta/push) bildirim.

---

### 4.8 Abonelik (Eğitmen)

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **SubscriptionPlan** | Platformun sunduğu abonelik planı (Free, Basic, Pro). | Name, Tier, Price, CommissionRate, MaxPackagesPerMonth, FeaturesJson. |
| **TrainerSubscription** | Eğitmenin o plana aboneliği. | TrainerProfileId, SubscriptionPlanId, Status, StartDate, EndDate, PaidAmount. |

**İş mantığı:** Eğitmen plan seçer; paket limiti ve komisyon oranı plana göre uygulanır. Otomatik yenileme ileride eklenecek.

---

### 4.9 Destek ve Admin

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **SupportTicket** | Kullanıcı/eğitmen destek talebi. | Subject, Message, UserId, OrderId, Category, Status, Priority, AdminReply, AssignedToUserId. |
| **FeaturedListing** | Öne çıkarılmış listeleme (ana sayfa, kategori vb.). | PageType, TrainerProfileId, ServicePackageId, SortOrder, StartDate, EndDate, IsActive. |
| **Dispute** | Sipariş/ödeme uyuşmazlığı. | OrderId, OpenedByUserId, DisputeType, Description, Status, ResolutionNote, ResolvedByUserId. |

**İş mantığı:** Destek talebi açılır, admin yanıtlar/durum günceller. Öne çıkanlar admin tarafından atanır. Uyuşmazlık açılır, admin inceleyip çözüm notu ile kapatır.

---

### 4.10 İçerik

| Entity | Açıklama | Önemli alanlar / ilişkiler |
|--------|----------|----------------------------|
| **BlogPost** | Blog yazısı (SEO, duyuru, rehber). | Title, Slug, Summary, Body, Status, PublishedAt, AuthorName, AuthorUserId, FeaturedImageUrl. |

**İş mantığı:** Taslak → yayınla; slug ile public erişim; listeleme (yayınlananlar anonim açılabilir).

---

## 5. İş Kuralları (Özet)

- **Ödeme akışı:** Sipariş → checkout (ileride iyzico/Stripe) → Payment kaydı → Order Confirmed → (escrow) TrainerWallet Pending → seans tamamlanınca Available.  
- **Komisyon:** Varsayılan %15; abonelik planına göre (Free %15, Basic %12, Pro %10) override edilebilir.  
- **İptal/iade:** 48 saat öncesi %100, 24–48 saat %50, 24 saat içi iade yok (detay: [`BUSINESS-RULES.md`](BUSINESS-RULES.md)).  
- **Yorum:** Sadece tamamlanmış sipariş; sipariş başına 1 review; eğitmen yanıtı ve “faydalı” oyu var.  
- **Abonelik:** Free aylık 3 paket limiti; Basic/Pro sınırsız; otomatik yenileme sonra eklenecek.

Tam kurallar: [`docs/BUSINESS-RULES.md`](BUSINESS-RULES.md).

---

## 6. Proje Şu An Neler Yapabilir? (API / Özellikler)

Aşağıdakiler **mevcut API ve AppService’ler** ile yapılabilir (frontend bu endpoint’leri kullanarak uygulayabilir).

| Alan | Yapılabilecekler |
|------|-------------------|
| **Eğitmen** | Profil CRUD, sertifika/galeri ekleme, kategori eşleme, listeleme (filtreleme). |
| **Kategori** | Kategori CRUD, ağaç yapısı. |
| **Paket** | Paket CRUD, müsaitlik ve müsait olmayan günler (paket ile birlikte). |
| **Sipariş** | Sipariş oluşturma, “benim siparişlerim”, “eğitmen siparişleri”, iptal, tamamlama, siparişe ait seans listesi. |
| **Seans** | Session entity’de Location ve RescheduledFromSessionId ile takip (API tarafında seans CRUD/listing mevcut akışa bağlı). |
| **Ödeme** | Ödeme kaydı getir (id / orderId), listele (filtreli). Gerçek checkout/webhook henüz yok. |
| **Cüzdan** | Eğitmen “cüzdanım” (yoksa oluşturulur), “işlem geçmişim” (sayfalı). |
| **Para çekme** | Eğitmen talep açar; admin listeler, onaylar/reddeder, “işlendi” işaretler (bakiye düşümü yapılır). |
| **Yorum** | Review CRUD, eğitmen yanıtı, çok boyutlu puan ve HelpfulCount (ReviewHelpfulVote ile “faydalı” sayacı artırılabilir). |
| **Mesajlaşma** | Konuşma başlatma, mesaj gönderme, listeleme (real-time olmadan). |
| **Bildirim** | Kullanıcı bildirimleri listeleme, okunmamış sayı, okundu işaretle(me). |
| **Abonelik** | Plan listeleme, eğitmen abonelik (subscribe), plan CRUD (yetkili). |
| **Destek** | Talep açma, “benim taleplerim”, admin tüm talepleri listeleme, yanıt yazma, durum güncelleme. |
| **Öne çıkan** | Admin öne çıkan listeleme CRUD (sayfa tipi, eğitmen/paket, sıra, tarih). |
| **Uyuşmazlık** | Kullanıcı uyuşmazlık açma, “benim uyuşmazlıklarım”, admin listeleme ve çözüm notu ile kapatma. |
| **Blog** | Yazı CRUD, slug ile getir, yayınlanan liste (anonim), yayınlama. |
| **Admin** | Dashboard istatistikleri, kullanıcı yönetimi (izinlerle). |

Tüm endpoint’ler `api/app/...` altında; yetkilendirme permission’lara göre. Swagger/OpenAPI: `docs/openapi/swagger.web.v1.full.json` (üretilmiş snapshot).

---

## 7. Neler Yapılabilir? (İleriye Dönük Öneriler)

Öncelik sırasıyla kısa başlıklar:

1. **Ödeme entegrasyonu**  
   iyzico ve Stripe için checkout başlatma + webhook (Payment/Order güncelleme, WalletTransaction yazma).

2. **Escrow otomasyonu**  
   Sipariş ödemesi → PendingBalance; seans tamamlanınca (ve varsa 48 saat kuralı) → AvailableBalance’a aktaran job/domain servisi.

3. **Rezervasyon (booking) API’si**  
   Slot seçimi, seans oluşturma, iptal kuralları (48h/24h) ile tek akış.

4. **SignalR**  
   Mesajlaşma ve bildirim hub’ları; anlık mesaj ve “okundu”, anlık bildirim.

5. **Profil tamamlanma %**  
   TrainerProfile için hesaplanan alan veya endpoint (bio, sertifika, galeri, kategori vb. doluluk).

6. **Abonelik otomatik yenileme**  
   Dönem sonu ödeme, başarısız ödemede PastDue/Expired, e-posta/push bildirimi.

7. **Gelişmiş arama/sıralama**  
   Rating, yanıt oranı, şehir, kategori, profil tamamlanma ile filtre ve sıralama; isteğe bağlı Elasticsearch.

8. **2FA, FCM push, e-posta senaryoları**  
   Güvenlik ve bildirim deneyimini tamamlamak için.

Detaylı eksiklik listesi ve öncelik: [`docs/EKSIKLIK_ANALIZI.md`](EKSIKLIK_ANALIZI.md).

---

## 8. Kısa Özet Tablo

| Soru | Cevap |
|------|--------|
| **Proje ne?** | Spor/sağlık koçluğu marketplace’i (eğitmen–öğrenci buluşması, sipariş, ödeme, cüzdan, yorum, destek, blog). |
| **Şu anki durum?** | Entity’ler, temel API’ler ve yetkilendirme büyük ölçüde hazır; ödeme entegrasyonu, escrow otomasyonu, booking ve SignalR eksik. |
| **Ne yapabiliyor?** | Profil, paket, sipariş, seans, ödeme kaydı, cüzdan, para çekme, yorum, mesaj, bildirim, abonelik, destek, öne çıkan, uyuşmazlık, blog ve admin işlemleri API ile. |
| **Sırada ne var?** | iyzico/Stripe, escrow, booking endpoint’i, SignalR, profil %, abonelik yenileme, arama/sıralama. |

---

## 9. İlgili Dokümanlar

| Doküman | İçerik |
|---------|--------|
| [README.md](README.md) | Genel giriş, stack, modül listesi. |
| [docs/README.md](README.md) | Backend dokümantasyon girişi, linkler. |
| [docs/EKSIKLIK_ANALIZI.md](EKSIKLIK_ANALIZI.md) | Analiz dokümanına göre eksikler, öncelik. |
| [docs/BUSINESS-RULES.md](BUSINESS-RULES.md) | Ödeme, iptal, komisyon, abonelik, yorum kuralları. |
| [docs/DATABASE-SCHEMA.md](DATABASE-SCHEMA.md) | Tablo/alan şeması. |
| [docs/AUTHORIZATION-SYSTEM.md](AUTHORIZATION-SYSTEM.md) | Roller, permission’lar. |
| [docs/CHANGELOG.md](CHANGELOG.md) | Yapılan değişikliklerin kronolojisi. |

---

*Bu rehber, projenin “ne, nerede, ne durumda” sorularını tek yerden yanıtlamak için hazırlanmıştır. Güncellemeler CHANGELOG ve ilgili dokümanlarla uyumlu tutulmalıdır.*

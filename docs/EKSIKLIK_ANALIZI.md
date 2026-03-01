# Fitliyo Proje Eksiklik Analizi

**Kaynak:** `docs/FitMarketPro_Proje_Analiz.docx` (FitMarket Pro / Fitliyo kapsamlı proje analiz dokümanı)  
**Hedef:** Mevcut kod tabanı ile analiz dokümanındaki gereksinimlerin karşılaştırılması ve eksik kısımların listelenmesi.  
**Tarih:** 2026-02-28

---

## 1. Özet

| Kategori | Analiz Doc | Mevcut Durum | Eksik / Fark |
|----------|------------|--------------|---------------|
| **Modül sayısı** | 10 modül | 10 modül (yapı var) | Bazı modüller kısmi |
| **Entity / Tablo** | ~25+ tablo | ~18 tablo | Payment, Wallet, Blog, Support, FeaturedListing, Dispute vb. yok |
| **API / AppService** | Tüm endpoint grupları | CRUD ağırlıklı | Ödeme, wallet, arama, rezervasyon API’leri eksik |
| **Real-time** | SignalR (mesaj + bildirim) | Yok | SignalR entegrasyonu yok |
| **Ödeme entegrasyonu** | iyzico + Stripe | Yok | Sadece Order alanları var |

---

## 2. Modül Bazında Eksiklikler

### 2.1 TrainerModule ✅ (Büyük oranda tam)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| TrainerProfile | ✅ | ✅ | Entity + AppService |
| TrainerCertificate | ✅ | ✅ | |
| TrainerGallery | ✅ | ✅ | |
| Category + TrainerCategoryMapping | ✅ | ✅ | |
| Müsaitlik takvimi | Haftalık şablon + özel günler | PackageAvailabilitySchedule, PackageUnavailableDate var | API’de tek başına CRUD eksik; paket ile birlikte yönetiliyor olabilir |
| Profil tamamlanma % | İsteniyor | ❌ | Hesaplanan alan / endpoint yok |
| BankAccountInfo (şifreli) | İsteniyor | Doc’ta var, entity’de kontrol edilmeli | |

**Öneri:** Profil tamamlanma yüzdesi için hesaplanan alan veya endpoint eklenebilir.

---

### 2.2 PackageModule (ServicePackages) ✅ (Temel tam)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| ServicePackage CRUD | ✅ | ✅ | |
| PackageAvailabilitySchedule | ✅ | ✅ | Entity var; ayrı AppService/API isteğe bağlı |
| PackageUnavailableDate | ✅ | ✅ | |
| Paket tipleri, fiyat, iptal koşulları | ✅ | ✅ | |

**Eksik:** Takvim/rezervasyon tarafında “slot seçimi” ve “rezervasyon onayı” akışı analizde var, kodda net değil (Order + Session ile ilişkili olabilir).

---

### 2.3 OrderModule ✅ (Kısmi)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| Order CRUD | ✅ | ✅ | |
| OrderNumber, Status, PaymentStatus | ✅ | ✅ | |
| Session (seans kayıtları) | ✅ | ✅ | Session entity var |
| Session alanları | SessionNumber, ScheduledAt, ActualStartAt/EndAt, Location, MeetingLink, RescheduledFrom | SessionNumber→SequenceNumber, ScheduledStart/EndTime, MeetingUrl var | **Eksik:** Location, RescheduledFrom (ertelenen seans ref.) |
| İptal / iade akışı | İş kuralı dokümante | OrderAppService’te kısmen | Refund ve iade hesaplama tam değil |

**Eksik:**
- Session’da `Location`, `RescheduledFrom`.
- Rezervasyon (booking) endpoint’i: “slot seç → seans oluştur” akışı.
- İptal/iade oranı ve refund hesaplama (48h, 24h kuralı) net implemente edilmemiş olabilir.

---

### 2.4 PaymentModule ❌ (Büyük oranda eksik)

Analiz dokümanına göre olması gerekenler:

| Öğe | Açıklama | Mevcut |
|-----|----------|--------|
| **Payment** entity | Order’a bağlı ödeme kaydı (Provider, ProviderPaymentId, Amount, Status, RefundAmount vb.) | ❌ Yok. Sadece Order üzerinde PaymentProvider, PaymentTransactionId var. |
| **TrainerWallet** | AvailableBalance, PendingBalance, TotalEarned, TotalWithdrawn, LastPayoutAt | ❌ Yok |
| **WalletTransaction** | Credit, Debit, Refund, Payout hareketleri | ❌ Yok |
| **WithdrawalRequest** | Para çekme talebi, IBAN, Status (Pending, Approved, Rejected, Processed) | ❌ Yok |
| **iyzico entegrasyonu** | Checkout, webhook | ❌ Yok |
| **Stripe entegrasyonu** | Checkout, webhook | ❌ Yok |
| **Escrow mantığı** | Ödeme sonrası PendingBalance, seans tamamlanınca AvailableBalance | ❌ Yok |

**Yapılacaklar:**
1. Payment, TrainerWallet, WalletTransaction, WithdrawalRequest entity’leri ve migration.
2. PaymentAppService (veya Order içinde ödeme adımları): checkout başlat, webhook işle.
3. WalletAppService: bakiye sorgulama, işlem geçmişi.
4. WithdrawalRequestAppService: talep oluşturma, admin onay/red.
5. iyzico ve Stripe için servis katmanı ve webhook endpoint’leri.

---

### 2.5 SubscriptionModule ✅ (Temel var)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| SubscriptionPlan | ✅ | ✅ | |
| TrainerSubscription | ✅ | ✅ | UserSubscriptions doc’ta, projede TrainerSubscription |
| Plan limitleri (Free 3 paket/ay) | ✅ | SubscriptionConsts’ta sabitler var | İş kuralı Order/Package tarafında uygulanmalı |
| Otomatik yenileme | İsteniyor | ❌ | Background job + ödeme entegrasyonu gerekir |

**Eksik:** Otomatik yenileme (recurring payment) ve yenileme bildirimi (e-posta/push).

---

### 2.6 MessagingModule ✅ (Temel var, real-time yok)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| Conversation | ✅ | ✅ | |
| Message | ✅ | ✅ | |
| Mesaj thread, dosya/resim paylaşımı | İsteniyor | Message entity’de FileUrl vb. olabilir | Kontrol edilmeli |
| **SignalR Hub** (real-time) | /hubs/messaging | ❌ | Hub ve istemci tarafı yok |
| Okundu bilgisi, bildirim | İsteniyor | Kısmen Notification’a bağlanabilir | |

**Eksik:** SignalR ile anlık mesajlaşma ve “okundu” güncellemeleri.

---

### 2.7 ReviewModule ✅ (Kısmi, detay eksik)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| Review CRUD | ✅ | ✅ | |
| Sipariş başına 1 yorum | ✅ | ✅ | |
| Eğitmen yanıtı | ✅ | ✅ | TrainerReply, TrainerReplyDate |
| **Çok boyutlu puan** | OverallRating, Communication, Expertise, ValueForMoney, Punctuality | Sadece tek `Rating` (1–5) | ❌ 5 ayrı puan alanı yok |
| **ServicePackageId** | Hangi paket için yorum | ❌ | Review entity’de yok |
| **IsVerifiedPurchase, IsPublished, HelpfulCount** | İsteniyor | IsHidden var, diğerleri yok | ❌ |
| **ReviewHelpfulVotes** tablosu | “Faydalı” oylama | ❌ | Entity yok |

**Eksik:**
- Review’da: ServicePackageId, CommunicationRating, ExpertiseRating, ValueForMoneyRating, PunctualityRating, IsVerifiedPurchase, IsPublished, HelpfulCount.
- ReviewHelpfulVotes entity ve ilgili API.

---

### 2.8 NotificationModule ✅ (Temel var)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| Notification entity | ✅ | ✅ | |
| In-app, e-posta, push | İsteniyor | Channel enum ve yapı olabilir | Push/FCM ve e-posta gönderimi implementasyonu ayrı kontrol edilmeli |
| **SignalR Hub** (bildirim) | /hubs/notifications | ❌ | Yok |

**Eksik:** SignalR ile anlık bildirim, FCM/SendGrid ile gerçek gönderim (konfigüre edilmiş mi kontrol edilmeli).

---

### 2.9 ContentModule ❌ (Eksik)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| **BlogPost** | Yazar, başlık, slug, içerik, durum, view count, meta | ❌ | Entity yok |
| **BlogCategory** (içerik kategorisi) | Doc’ta ayrı olabilir | Categories eğitmen/kategori için; blog için ayrı tablo yok | ❌ |
| SEO (meta, slug) | İsteniyor | Blog olmadan anlamlı değil | Blog ile birlikte |

**Eksik:** BlogPost (ve isteğe bağlı BlogCategory) entity, migration, BlogPostAppService ve API.

---

### 2.10 AdminModule ✅ (Kısmi)

| Öğe | Analiz | Mevcut | Not |
|-----|--------|--------|-----|
| Dashboard / raporlar | İsteniyor | AdminAppService, GetDashboardAsync | ✅ Temel var |
| **SupportTickets** | Müşteri destek talepleri | ❌ | Entity ve AppService yok |
| **FeaturedListings** | Öne çıkarılmış profil/paket, süre, pozisyon | ❌ | Entity yok |
| **Disputes** | Anlaşmazlık (NoShow, QualityIssue, PaymentIssue), admin kararı | ❌ | Entity yok |
| Moderasyon (içerik/yorum) | İsteniyor | Review’da IsHidden var | Destek talebi ve dispute yönetimi yok |

**Eksik:**
- SupportTicket entity + SupportTicketAppService (oluşturma, listeleme, atama, çözüm).
- FeaturedListing entity + yönetim API’si.
- Dispute entity + DisputeAppService (açma, inceleme, karar, iade).

---

## 3. API ve Akış Eksiklikleri

| API / Akış | Analiz Doc | Mevcut | Not |
|------------|------------|--------|-----|
| `/api/bookings` — rezervasyon (slot seç, book, cancel) | İsteniyor | ❌ | Ayrı booking endpoint’i yok |
| `/api/payments/checkout` | İsteniyor | ❌ | Yok |
| `/api/payments/webhook` | iyzico/Stripe webhook | ❌ | Yok |
| Wallet bakiye ve işlem geçmişi | İsteniyor | ❌ | Wallet yok |
| Para çekme talebi (create, list, admin approve/reject) | İsteniyor | ❌ | Yok |
| **Gelişmiş arama** | `/api/search?q=&category=&city=&minRating=` | Kısmen (paket/eğitmen listesi) | Filtreler ve sıralama algoritması (rating, yanıt oranı, profil tamamlanma vb.) analizdeki gibi netleştirilmeli |
| **Eğitmen sıralama algoritması** | Rating×0.35 + ReviewCount×0.20 + ResponseRate×0.15 + … | ❌ | Sabit sıralama veya basit rating; formül uygulanmıyor |

---

## 4. Altyapı ve Entegrasyon Eksiklikleri

| Bileşen | Analiz | Mevcut | Not |
|---------|--------|--------|-----|
| **SignalR** | Mesajlaşma + bildirim hub’ları | ❌ | Projede MapHub / Hub sınıfı yok |
| **iyzico** | Ödeme (TR) | ❌ | Paket/entegrasyon yok |
| **Stripe** | Ödeme (uluslararası) | ❌ | Yok |
| **SendGrid / e-posta** | İşlem e-postaları | ABP e-posta modülü var | Konfigürasyon ve tetikleyen senaryolar netleştirilmeli |
| **Firebase (FCM)** | Push bildirim | ❌ | Mobil push için entegrasyon yok |
| **Elasticsearch** | Gelişmiş eğitmen arama (Faz 2+) | Doc’ta var, kodda kontrol edilmeli | Opsiyonel; basit SQL/EF arama ile başlanabilir |
| **2FA (TOTP)** | Güvenlik | ❌ | ABP’de 2FA modülü araştırılabilir |

---

## 5. Veritabanı Özeti (Doc vs Mevcut)

**Doc’ta olup projede entity’si olmayan tablolar:**

- Payments  
- TrainerWallet  
- WalletTransactions  
- WithdrawalRequests  
- BlogPosts (ve isteğe bağlı BlogCategories)  
- SupportTickets  
- FeaturedListings  
- Disputes  
- ReviewHelpfulVotes  

**Order:** Doc’ta `PaymentId` (FK → Payments) var; projede doğrudan Payment entity olmadığı için Order’da sadece PaymentProvider ve PaymentTransactionId tutuluyor. Ödeme modülü eklendiğinde PaymentId FK eklenebilir.

---

## 6. Öncelik Sıralaması (Öneri)

1. **Yüksek:** PaymentModule (Payment, TrainerWallet, WalletTransaction, WithdrawalRequest + iyzico/Stripe iskeleti, checkout/webhook).  
2. **Yüksek:** Rezervasyon/booking akışı (slot seçimi, seans oluşturma, iptal).  
3. **Orta:** Session alanları (Location, RescheduledFrom).  
4. **Orta:** Review genişletmesi (çok boyutlu puan, ServicePackageId, HelpfulVotes).  
5. **Orta:** SignalR (mesajlaşma + bildirim hub’ları).  
6. **Orta:** SupportTickets, Disputes, FeaturedListings.  
7. **Düşük:** ContentModule (BlogPost).  
8. **Düşük:** Arama ve sıralama algoritması, profil tamamlanma %, otomatik abonelik yenileme.

---

## 7. Referanslar

- **Proje analiz dokümanı:** `docs/FitMarketPro_Proje_Analiz.docx`  
- **Veritabanı şeması:** `docs/DATABASE-SCHEMA.md`  
- **İş kuralları:** `docs/BUSINESS-RULES.md`  
- **Genel dokümantasyon:** `docs/README.md`  

Bu doküman, analiz dokümanına göre “kendi kapsamında” geliştirme yaparken hangi kısımların eksik veya kısmi kaldığını tek bakışta görmek için kullanılabilir. İstersen bir sonraki adımda tek bir modül (ör. PaymentModule veya Booking) için detaylı implementasyon planı da çıkarılabilir.

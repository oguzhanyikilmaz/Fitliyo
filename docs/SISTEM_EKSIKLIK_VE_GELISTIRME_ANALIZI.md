# Fitliyo — Komple Sistem Eksiklik ve Geliştirme Analizi

**Tarih:** 2026-02-28  
**Amaç:** Backend, frontend, test ve operasyonel taraftaki eksiklikleri ve öncelikli geliştirme alanlarını tek dokümanda toplamak.

---

## 1. Özet Tablo

| Alan | Durum | Öncelik |
|------|--------|--------|
| **Backend API (CRUD)** | Büyük oranda tam (Trainer, Order, Payment, Wallet, Support, Blog, Admin, vb.) | — |
| **Frontend – Gerçek veri** | Kısmi: profil, sipariş, eğitmen listesi/detay, ana sayfa öne çıkanlar çalışıyor; birçok sayfa hâlâ placeholder | **Yüksek** |
| **Frontend – API path** | Bazı sayfalarda PascalCase/camelCase uyumsuzluğu (Swagger ile eşleştirilmeli) | Orta |
| **Test** | Sadece örnek (Sample) testler; marketplace AppService’ler için unit/integration test yok | **Yüksek** |
| **Ödeme (iyzico/Stripe)** | Entity ve temel akış var; gerçek checkout/webhook entegrasyonu yok | Yüksek |
| **SignalR** | Yok; mesajlaşma ve bildirim anlık değil | Orta |
| **Eksik sayfalar** | Öğrenci: mesajlar, paket detay/sipariş oluşturma; Eğitmen: profil (TrainerProfile), paket CRUD ekranları; Admin: gerçek liste/detay | **Yüksek** |

---

## 2. Backend — Mevcut ve Eksikler

### 2.1 Var Olan AppService’ler (API mevcut)

| Modül | AppService | Not |
|-------|------------|-----|
| Trainer | TrainerProfileAppService | GetBySlug, liste, CRUD |
| Kategori | CategoryAppService | Liste, filtre |
| Paket | ServicePackageAppService | CRUD |
| Sipariş | OrderAppService | GetMyOrders, GetTrainerOrders, GetSessions, GetAsync, Create, Cancel, Complete |
| Ödeme | PaymentAppService | Sipariş ödemesi kaydı |
| Cüzdan | WalletAppService | Bakiye, işlem geçmişi |
| Para çekme | WithdrawalRequestAppService | Talep oluşturma, listeleme |
| Destek | SupportTicketAppService | CRUD, my, list |
| Yorum | ReviewAppService | CRUD, eğitmen yanıtı |
| Mesajlaşma | MessagingAppService | Konuşma, mesaj |
| Bildirim | NotificationAppService | My, mark read |
| Blog | BlogPostAppService | CRUD, GetBySlug, Publish |
| Öne çıkan | FeaturedListingAppService | CRUD |
| Uyuşmazlık | DisputeAppService | List, resolve, my |
| Admin | AdminAppService | GetDashboardAsync |
| Abonelik | SubscriptionAppService | Plan yönetimi |
| Profil (sağlık) | UserProfileAppService | GetMyProfile, UpdateMyProfile (BMR, BMI, TDEE hesaplı) |

### 2.2 Backend’de Eksik veya Kısmi Olanlar

| Konu | Açıklama | Öneri |
|------|----------|--------|
| **Ödeme entegrasyonu** | iyzico/Stripe checkout ve webhook yok; sadece Order üzerinde alanlar ve Payment kaydı | Checkout başlat, webhook işle, Order’ı ödemeli duruma geçir |
| **Rezervasyon / slot** | “Slot seç → seans oluştur” için ayrı booking endpoint’i yok | Order/Session akışına slot seçimi ve onay adımı eklenebilir |
| **Session alanları** | Location, RescheduledFrom (ertelenen seans ref.) eksik olabilir | Entity ve API güncellenmeli |
| **İptal/iade kuralları** | 24h/48h kuralı ve refund hesaplama net dokümante/implemente değil | İş kuralı netleştirilip OrderAppService’e taşınmalı |
| **SignalR** | Mesajlaşma ve bildirim anlık değil | /hubs/messaging, /hubs/notifications hub’ları eklenmeli |
| **E-posta / Push** | SendGrid/FCM konfigürasyonu ve tetikleyen senaryolar (sipariş onayı, seans hatırlatma vb.) | ABP e-posta modülü ve event’lerle entegre edilmeli |
| **Profil tamamlanma %** | TrainerProfile’da hesaplanan alan veya endpoint yok | GetMe veya DTO’da ProfileCompletionPct dolduruluyorsa frontend’de kullanılabilir |
| **Gelişmiş arama** | Kategori, şehir, minRating, sıralama algoritması (rating×0.35 + …) | GetList filtreleri ve sıralama formülü eklenebilir |

---

## 3. Frontend — Sayfa Bazında Durum

### 3.1 Gerçek API ile Çalışan Sayfalar

| Route | Açıklama | Kullanılan API |
|-------|----------|----------------|
| `/login`, `/register` | Auth | Token, kayıt |
| `/` | Ana sayfa | Öne çıkan (featuredListing), eğitmen kartları (trainerProfile) |
| `/trainers`, `/trainers/[slug]`, `/trainers/id/[id]` | Eğitmen listesi ve detay | trainerProfile (liste, bySlug, by id), servicePackage (paket listesi) |
| `/student` | Öğrenci dashboard | order/getMyOrders, order/getSessions |
| `/student/orders`, `/student/orders/[id]` | Siparişlerim, sipariş detay | order/getMyOrders, order/[id], order/getSessions |
| `/student/sessions` | Seanslarım | order/getMyOrders + getSessions |
| `/student/profile`, `/trainer/profile` | Sağlık profili | UserProfile/GetMyProfile, UpdateMyProfile |

### 3.2 Placeholder (Sadece Metin / API Çağrısı Yok)

**2026-02-28 güncelleme:** Aşağıdaki satırlarda ✅ işaretli olanlar gerçek API ile dolduruldu.

| Route | Planlanan API | Öncelik | Durum |
|-------|----------------|--------|--------|
| `/student/notifications` | notifications/my, mark-read | Yüksek | ✅ Yapıldı |
| `/student/support` | support-tickets/my, POST | Yüksek | ✅ Yapıldı |
| `/trainer` | trainer-orders, wallet/my, notifications | Yüksek | ✅ Yapıldı |
| `/trainer/profile` | Şu an sadece UserHealthProfile; TrainerProfile (bio, sertifika, galeri) ayrı sayfa/sekme olabilir | Orta | Placeholder değil (profil var) |
| `/trainer/orders` | order/GetTrainerOrders | Yüksek | ✅ Yapıldı |
| `/trainer/sessions` | Seans listesi (trainer tarafı) | Yüksek | ✅ Yapıldı |
| `/trainer/wallet` | wallet/my, wallet/my/transactions | Yüksek | ✅ Yapıldı |
| `/trainer/withdrawals` | withdrawal-requests/my, POST | Yüksek | ✅ Yapıldı |
| `/trainer/notifications` | notifications/my | Orta | ✅ Yapıldı |
| `/trainer/support` | support-tickets/my, POST | Orta | ✅ Yapıldı |
| `/admin` | admin/dashboard (istatistikler) | Yüksek | ✅ Yapıldı |
| `/admin/support` | support-tickets/list, [id], reply | Yüksek | ✅ Yapıldı |
| `/admin/disputes` | disputes/list, [id], resolve | Yüksek | ✅ Yapıldı |
| `/admin/featured` | featured-listings CRUD | Orta | Placeholder |
| `/admin/blog` | blog-posts CRUD, publish | Orta | Placeholder |
| `/admin/withdrawals` | withdrawal-requests/list, approve/reject | Yüksek | ✅ Yapıldı |

### 3.3 Hiç Olmayan veya Eksik Sayfalar

**2026-02-28 güncelleme:** Aşağıdaki satırlarda ✅ işaretli olanlar eklendi.

| Route | Açıklama | Durum |
|-------|----------|--------|
| `/student/trainers/[slug]` veya ortak kullanım | Öğrenci menüsünden eğitmen detayı (şu an `/trainers/[slug]` var, menü linki kontrol edilmeli) | Mevcut (trainers sayfası) |
| `/student/packages/[id]` | Paket detayı + “Satın Al” → POST order | ✅ Yapıldı |
| `/student/messages` | Konuşma listesi + mesaj thread’i (MessagingAppService) | ✅ Yapıldı |
| `/trainer/packages`, `/trainer/packages/new`, `/trainer/packages/[id]` | Paket listesi, ekleme, düzenleme (ServicePackage CRUD) | ✅ Yapıldı |
| `/trainer/profile-edit` (TrainerProfile) | Bio, slug, şehir, linkler düzenleme (GetMyProfileAsync, UpdateAsync) | ✅ Yapıldı |
| `/admin/kullanicilar` | Kullanıcı listesi, rol atama (ABP Identity API) | Eksik |

### 3.4 Frontend Teknik Eksikler

- **API path:** Tüm çağrılar Swagger’daki path ile aynı olmalı (PascalCase: `/api/app/Order/GetMyOrdersAsync` vb.). ~~404 alan sayfalar için Swagger’dan path doğrulanmalı.~~ **✅ Yapıldı:** Order, TrainerProfile, FeaturedListing path’leri PascalCase’e çevrildi; GetAsync?id=, GetListAsync, GetMyOrdersAsync, GetSessionsAsync, GetBySlugAsync kullanılıyor.
- **lib/types.ts:** Backend DTO’larla tam uyum (yeni alanlar, nullable) kontrol edilmeli. Yorum güncellendi (PascalCase path referansı).
- **Hata ve boş durum:** 403/401 yönlendirmesi, boş liste mesajları, loading/error state’leri tutarlı olmalı. **✅ Yapıldı:** `apiFetch` içinde 401/403 → token temizlenip `/login?redirect=...` yönlendirmesi; `components/ui/ApiState.tsx` (LoadingState, ErrorState, EmptyState) eklendi.
- **Sayfalama:** Liste sayfalarında skipCount, maxResultCount, sorting parametreleri kullanılmalı (P12 kuralı). **✅ Yapıldı:** `lib/api.ts` içinde `DEFAULT_LIST_PARAMS` (skipCount: 0, maxResultCount: 50, sorting) tanımlandı; öğrenci/eğitmen sipariş, bildirim, destek, eğitmen listesi vb. sayfalarda kullanılıyor.

---

## 4. Test — Eksiklikler

| Konu | Durum | Öneri |
|------|--------|--------|
| **Unit test (Application)** | Sadece SampleAppServiceTests; Order, Trainer, Payment, Wallet, Support vb. için yok | Her kritik AppService için en az CRUD + özel metot testleri |
| **Integration test** | Örnek EfCore ve Web testleri var; marketplace senaryoları yok | Sipariş akışı, ödeme kaydı, cüzdan hareketi için integration test |
| **Test verisi** | DbMigrator ile seed var; test projelerinde TestDataSeeder / ortak fixture | Test base’de gerekli FK ve kullanıcı verisi hazır olmalı |
| **Frontend test** | E2E veya component test yok | Kritik akışlar (giriş, sipariş listesi) için Playwright/Cypress veya React Testing Library |

---

## 5. Operasyon ve Dokümantasyon

| Konu | Durum | Öneri |
|------|--------|--------|
| **CHANGELOG** | Güncel tutuluyor | Her anlamlı değişiklikte giriş eklenmeye devam |
| **AppService dokümanları** | generate_appservice_docs.py ile üretiliyor | AppService değişince script çalıştırılmalı |
| **Swagger snapshot** | docs/openapi/swagger.web.v1.full.json | API değişince yeniden üretilmeli |
| **Frontend değişiklik** | docs/frontend-changes/ | API/DTO değişince ilgili giriş eklenmeli |
| **EKSIKLIK_ANALIZI.md** | Eski (Payment, Wallet vb. artık var) | Bu doküman (SISTEM_EKSIKLIK_VE_GELISTIRME_ANALIZI.md) güncel özet; EKSIKLIK_ANALIZI modül detayı için referans alınabilir |

---

## 6. Öncelikli Yapılacaklar Listesi

### Yüksek öncelik (kısa vadede)

1. **Frontend – Placeholder sayfaları doldur**
   - Öğrenci: bildirimler, destek (liste + yeni talep).
   - Eğitmen: dashboard (trainer-orders, wallet özeti), siparişler, seanslar, cüzdan, para çekme, bildirimler, destek.
   - Admin: dashboard (GetDashboardAsync ile istatistikler), destek listesi/detay/yanıt, uyuşmazlık listesi/çözüm, para çekme talepleri listesi/onay.

2. **Frontend – Eksik sayfalar**
   - Öğrenci: paket detay + “Satın Al” (POST order), mesajlar (konuşma listesi + mesaj thread).
   - Eğitmen: paketlerim (liste + ekle/düzenle), TrainerProfile düzenleme (bio, sertifika, galeri).

3. **API path uyumu**
   - Tüm `apiFetch` çağrılarında Swagger’daki path kullanılmalı (PascalCase action/controller); 404 alan sayfalar düzeltilmeli.

4. **Test**
   - OrderAppService, TrainerProfileAppService, WalletAppService, SupportTicketAppService için en az temel CRUD ve özel metot testleri.

### Orta öncelik

5. **Ödeme**
   - iyzico (TR) checkout ve webhook iskeleti; ödeme tamamlanınca Order ve Payment güncelleme.

6. **SignalR**
   - Mesajlaşma ve bildirim hub’ları; frontend’de anlık güncelleme.

7. **Session**
   - Location, RescheduledFrom alanları ve iptal/erteleme kuralları.

8. **Admin**
   - Öne çıkanlar ve blog için tam CRUD ekranları; kullanıcı yönetimi (Identity API).

### Düşük öncelik

9. **Gelişmiş arama ve sıralama**
   - Eğitmen listesinde filtre (kategori, şehir, minRating) ve sıralama formülü.

10. **Profil tamamlanma %, otomatik abonelik yenileme, 2FA**
    - İş ve güvenlik iyileştirmeleri.

---

## 7. Referanslar

- [FRONTEND_PLANI.md](FRONTEND_PLANI.md) — Rol bazlı sayfa ve API planı  
- [FRONTEND_DURUM_ANALIZI.md](FRONTEND_DURUM_ANALIZI.md) — Mevcut sayfa durumu ve adım adım öneri  
- [EKSIKLIK_ANALIZI.md](EKSIKLIK_ANALIZI.md) — Modül bazında detay (bazı maddeler güncel değil; backend artık birçok entity’ye sahip)  
- [AUTHORIZATION-SYSTEM.md](AUTHORIZATION-SYSTEM.md) — Yetkilendirme  
- Backend Swagger: `http://localhost:5000/swagger` — Endpoint path’leri ve DTO’lar

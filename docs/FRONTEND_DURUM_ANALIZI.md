# Frontend Durum Analizi ve Devam Planı

**Tarih:** 2026-03-01  
**Referans:** [FRONTEND_PLANI.md](FRONTEND_PLANI.md)

---

## 1. Özet: Neredeyiz?

| Faz | İçerik | Durum | Not |
|-----|--------|--------|-----|
| **Faz 1** | Altyapı, Auth, rol yönlendirme, layout | ✅ **Tamamlandı** | Giriş/kayıt çalışıyor, token + rol → dashboard yönlendirmesi var |
| **Faz 2** | Misafir + Öğrenci akışı, gerçek API | 🟡 **İskelet var, API yok** | Sayfalar placeholder; veri çekilmiyor |
| **Faz 3** | Eğitmen akışı | 🟡 **İskelet var, API yok** | Aynı şekilde placeholder |
| **Faz 4** | Admin akışı | 🟡 **İskelet var, API yok** | Aynı şekilde placeholder |
| **Faz 5** | İyileştirmeler | ⬜ Yapılmadı | Mesajlaşma tam akış, arama, bildirim badge vb. |

---

## 2. Mevcut Sayfa ve Eksikler

### 2.1 Tamamlanan / Çalışan

- **`/giris`** – Giriş formu, `/connect/token`, token + rol kaydı, role göre yönlendirme ✅  
- **`/kayit`** – Kayıt formu (rol seçimi ile) ✅  
- **`/`** – Ana sayfa (hero + “Nasıl çalışır”) – **API yok**, öne çıkan eğitmen/kategori yok  
- **Header** – Logo, Eğitmenler linki, Giriş/Kayıt veya Panel/Çıkış ✅  
- **Layout’lar** – `ogrenci`, `egitmen`, `admin` için ayrı layout + menü var ✅  

### 2.2 Sayfa Var, Veri Yok (Placeholder)

| Route | Plan | Eksik |
|-------|------|--------|
| `/egitmenler` | Eğitmen listesi (misafir) | `GET /api/app/trainerProfile` (veya conventional path) çağrısı yok |
| `/ogrenci` | Öğrenci dashboard | my-orders, yaklaşan seanslar API yok |
| `/ogrenci/egitmenler` | Eğitmen keşfet | Aynı trainer list API yok |
| `/ogrenci/siparisler` | Siparişlerim | my-orders API yok |
| `/ogrenci/seanslar` | Seanslarım | Sipariş/seans API yok |
| `/ogrenci/bildirimler` | Bildirimler | notifications API yok |
| `/ogrenci/destek` | Destek talepleri | support-tickets API yok |
| `/egitmen` | Eğitmen dashboard | trainer-orders, wallet, notifications yok |
| `/egitmen/profil` | Profil düzenleme | trainer-profiles/me, update yok |
| `/egitmen/paketler` | Paketlerim | service-packages (trainer) yok |
| `/egitmen/siparisler`, `/egitmen/seanslar` | Siparişler / Seanslar | API yok |
| `/egitmen/cuzdan` | Cüzdan | wallet/my yok |
| `/egitmen/para-cekme` | Para çekme | withdrawal-requests yok |
| `/egitmen/bildirimler`, `/egitmen/destek` | Bildirim / Destek | API yok |
| `/admin` | Admin dashboard | admin/dashboard API yok |
| `/admin/destek`, `/admin/uyusmazliklar` | Destek / Uyuşmazlık | List/detail API yok |
| `/admin/one-cikanlar`, `/admin/blog`, `/admin/para-cekme` | Öne çıkan / Blog / Para çekme | CRUD API yok |

### 2.3 Hiç Olmayan Sayfalar

| Route | Açıklama |
|-------|----------|
| `/egitmenler/[slug]` veya `/egitmenler/[id]` | Eğitmen profili (public) – paketler, yorumlar, “Paket Seç” |
| `/paketler` | Public paket listesi (opsiyonel) |
| `/ogrenci/paketler/[id]` veya sipariş oluşturma | Paket detay + “Satın Al” → sipariş |
| `/ogrenci/siparisler/[id]` | Sipariş detay (seanslar, iptal) |
| `/ogrenci/mesajlar` | Mesajlaşma (Faz 2’de kısmen, Faz 5’te tam) |

---

## 3. Backend API Notu (Conventional Route)

HttpApi’deki explicit controller’lar kaldırıldığı için endpoint’ler **conventional** (Application assembly’den) üretiliyor. Path’ler genelde şu formatta:

- `GET /api/app/trainerProfile` (TrainerProfileAppService)
- `GET /api/app/order` (OrderAppService) – my-orders, trainer-orders için backend’de özel metot adları kullanılıyor olabilir
- `GET /api/app/admin` → `GET /api/app/admin/dashboard` (GetDashboardAsync)

Çalışan backend’de **Swagger** (`http://localhost:5000/swagger`) üzerinden tam path’leri kontrol etmek en doğrusu.

---

## 4. Önerilen Devam Sırası (Faz 2 Odaklı)

Aşağıdaki sırayla ilerlemek hem hızlı görünür sonuç verir hem de bir sonraki adımı besler.

### Adım 1: Ortak API + tipler (lib)

- [ ] `lib/api.ts` – Zaten var; gerekirse `apiFetch` ile sayfalama parametreleri (skipCount, maxResultCount) ekle.
- [ ] `lib/types.ts` (veya `types/api.ts`) – Backend DTO’lara uygun TypeScript tipleri: `TrainerProfileDto`, `PagedResultDto<T>`, `CategoryDto`, `OrderDto`, `DashboardDto` vb. (Swagger veya backend kontratlarından çıkarılabilir).

### Adım 2: Eğitmen listesi (misafir + öğrenci)

- [ ] **`/egitmenler`** ve **`/ogrenci/egitmenler`**: `GET /api/app/trainerProfile` (veya Swagger’daki list endpoint’i) ile liste çek; kart grid göster.
- [ ] İsteğe bağlı: Basit filtre (kategori) – `GET /api/app/category` ile kategorileri al, dropdown ile filtrele.

### Adım 3: Ana sayfa öne çıkanlar

- [ ] **`/`**: Öne çıkan eğitmenler için `GET /api/app/featuredListing` (veya featured-listings) veya doğrudan trainer listesinden “öne çıkan” alanı varsa filtrele; 4–6 kart göster.
- [ ] İsteğe bağlı: Kategoriler bölümü – `GET /api/app/category` ile kategorileri listele.

### Adım 4: Eğitmen profili (public)

- [ ] **`/egitmenler/[slug]/page.tsx`** (veya `[id]` – backend’de by-slug varsa slug kullan): Eğitmen detayı, paketleri (`servicePackage`), yorumlar (`review`) – “Paket Seç” butonu girişe veya paket detaya yönlendirir.

### Adım 5: Öğrenci dashboard + siparişler

- [ ] **`/ogrenci`**: `GET /api/app/order` (my-orders benzeri) ile son siparişler; “yaklaşan seanslar” için siparişlerden seans bilgisi türet (veya ayrı endpoint varsa kullan).
- [ ] **`/ogrenci/siparisler`**: Aynı my-orders listesi, tablo/kart görünümü.
- [ ] **`/ogrenci/siparisler/[id]`**: Sipariş detayı, seans listesi, iptal (backend’de varsa).

### Adım 6: Paket detay + sipariş oluşturma

- [ ] **`/ogrenci/paketler/[id]`** (veya `/ogrenci/siparis-olustur?packageId=...`): Paket detayı `GET /api/app/servicePackage/[id]`, “Satın Al” → `POST /api/app/order` (CreateAsync) ile sipariş oluştur.

### Adım 7: Öğrenci bildirim + destek

- [ ] **`/ogrenci/bildirimler`**: `GET /api/app/notification` (my) ile liste.
- [ ] **`/ogrenci/destek`**: `GET /api/app/supportTicket` (my) + yeni talep formu `POST /api/app/supportTicket`.

### Adım 8: Eğitmen ve Admin (Faz 3–4)

- Eğitmen: profil (me + update), paketler (CRUD), siparişler, cüzdan, para çekme, bildirim, destek.
- Admin: dashboard (GetDashboardAsync), destek listesi, uyuşmazlıklar, öne çıkanlar, blog, para çekme talepleri.

---

## 5. Hemen Başlanabilecek İlk İş (Öneri)

**Adım 1 + Adım 2:**  
Önce `lib/types.ts` içinde `PagedResultDto`, `TrainerProfileDto` (ve gerekirse `CategoryDto`) tiplerini tanımla; ardından **`/egitmenler`** sayfasında `apiFetch` ile eğitmen listesini çekip kart grid’de göster. Böylece:

- API base URL ve token kullanımı doğrulanır.
- Conventional route (trainerProfile) netleşir.
- Aynı liste bileşeni `/ogrenci/egitmenler` için de kullanılabilir.

Sonraki adım: Ana sayfaya öne çıkan eğitmenler (Adım 3) veya eğitmen profili sayfası (Adım 4).

---

## 6. Yapılan Geliştirmeler (2026-03-01)

| Adım | Açıklama | Dosyalar |
|------|----------|----------|
| 1 | Ana sayfa öne çıkan eğitmenler | `components/home/FeaturedTrainers.tsx`, `app/page.tsx` |
| 2 | Öğrenci dashboard (my-orders + yaklaşan seanslar) | `app/ogrenci/page.tsx` |
| 3 | Öğrenci siparişler listesi + detay | `app/ogrenci/siparisler/page.tsx`, `app/ogrenci/siparisler/[id]/page.tsx` |
| 4 | Paket listesi + paket detay + sipariş oluşturma (POST order) | `app/ogrenci/paketler/page.tsx`, `app/ogrenci/paketler/[id]/page.tsx` |
| 5 | Eğitmen profili sayfasında paket listesi API | `components/trainer/TrainerPackageList.tsx`, `app/egitmenler/[slug]/page.tsx`, `app/egitmenler/id/[id]/page.tsx` |
| - | Seanslarım sayfası | `app/ogrenci/seanslar/page.tsx` |
| - | API tipleri (Order, Session, FeaturedListing, ServicePackage vb.) | `lib/types.ts` |

**API path notu:** Conventional API'de action adları camelCase olabilir (`getMyOrders`, `getSessions`, `featuredListing`). 404 alırsan Swagger'dan tam path'i kontrol et.

---

## 7. Referanslar

- [FRONTEND_PLANI.md](FRONTEND_PLANI.md) – Fazlar ve sayfa listesi  
- [EKSIKLIK_ANALIZI.md](EKSIKLIK_ANALIZI.md) – Backend eksikleri  
- Backend Swagger: `http://localhost:5000/swagger` (çalışırken) – endpoint path’leri ve DTO’lar

# Fitliyo Web Frontend Planı

**Amaç:** Rol bazlı (Öğrenci / Eğitmen / Admin) farklı arayüzler; sıradan admin paneli değil, marketplace deneyimi.  
**Hedef:** Backend API’leri kullanan, giriş sonrası role göre farklı ekranlar sunan tek sayfa uygulaması.  
**Son güncelleme:** 2026-03-01

---

## 1. Genel İlkeler

- **Tek uygulama, üç deneyim:** Aynı frontend projesi; giriş yapan kullanıcının rolüne göre (Student / Trainer / Admin) farklı menü, sayfa seti ve layout.
- **Öğrenci:** Keşfet, ara, eğitmen/paket görüntüle, sipariş ver, seanslarım, mesajlar — marketplace tüketici deneyimi.
- **Eğitmen:** Profilim, paketlerim, siparişlerim, seanslarım, cüzdan, para çekme — işletme sahibi deneyimi.
- **Admin:** Dashboard, kullanıcılar, destek talepleri, uyuşmazlıklar, öne çıkanlar, blog — moderasyon ve yönetim paneli.
- **Misafir (Guest):** Giriş yapmadan ana sayfa, eğitmen listeleme, paket listeleme, eğitmen profili görüntüleme (kayıt/giriş CTA ile).

---

## 2. Teknoloji Önerisi

| Bileşen | Öneri | Not |
|--------|--------|-----|
| Framework | Next.js 15 (App Router) | Proje stack’inde tanımlı |
| Dil | TypeScript | API tipleri ve güvenli geliştirme |
| Stil | Tailwind CSS | Hızlı, tutarlı UI |
| State / Server | React Query (TanStack Query) + fetch/axios | API cache, loading, error |
| Auth | JWT (Backend OpenIddict) | Token’ı cookie veya localStorage; rol bilgisi token/me endpoint’inden |
| Form | React Hook Form + basit validasyon | Performans, az re-render |
| UI bileşenleri | shadcn/ui veya Headless UI | Erişilebilir, özelleştirilebilir |
| Konum | Bu repo içi `frontend/` | Tek repo ile backend + frontend birlikte çalışsın |

**Backend API base URL:** Geliştirmede `http://localhost:6000` (veya backend’in çalıştığı port); production’da ortam değişkeni.

---

## 3. Rol ve Route Yapısı

### 3.1 Route Özeti

| Rol | Prefix | Açıklama |
|-----|--------|----------|
| Misafir | `/` | Landing, arama, eğitmen/paket listesi, eğitmen profili (public) |
| Öğrenci | `/ogrenci` | Öğrenci dashboard ve tüm öğrenci sayfaları |
| Eğitmen | `/egitmen` | Eğitmen dashboard ve tüm eğitmen sayfaları |
| Admin | `/admin` | Admin dashboard ve tüm admin sayfaları |
| Ortak | `/giris`, `/kayit`, `/sifremi-unuttum` | Auth sayfaları |

### 3.2 Giriş Sonrası Yönlendirme

- **Student** → `/ogrenci` (veya `/ogrenci/dashboard`)
- **Trainer** → `/egitmen` (veya `/egrenci/dashboard`)
- **Admin** / **SuperAdmin** → `/admin` (veya `/admin/dashboard`)
- Rol bilgisi: Backend’den `/api/abp/application-configuration` veya özel bir “me” endpoint’i ile alınır; token içinde role claim de olabilir.

### 3.3 Layout Yapısı

- **Layout (root):** Ortak header (logo, giriş/kayıt veya kullanıcı menüsü), footer.
- **Layout `/ogrenci`:**
  - Sol veya üst menü: Dashboard, Eğitmenleri Keşfet, Paketler, Siparişlerim, Seanslarım, Mesajlar, Bildirimler, Destek, Çıkış.
- **Layout `/egitmen`:**
  - Menü: Dashboard, Profilim, Paketlerim, Siparişlerim, Seanslarım, Cüzdan, Para Çekme, Mesajlar, Bildirimler, Destek, Çıkış.
- **Layout `/admin`:**
  - Menü: Dashboard, Kullanıcılar, Destek Talepleri, Uyuşmazlıklar, Öne Çıkanlar, Blog, Bildirimler, Çıkış.

---

## 4. Sayfa ve Akış Detayı

### 4.1 Misafir (Guest) — Giriş yok

| Sayfa | Route | İçerik | Kullanılan API (örnek) |
|-------|--------|--------|-------------------------|
| Ana sayfa | `/` | Hero, öne çıkan eğitmenler, kategoriler, CTA “Eğitmen Ol” / “Giriş Yap” | `GET /api/app/featured-listings`, `GET /api/app/trainer-profiles` (liste), `GET /api/app/categories` |
| Eğitmen listesi | `/egitmenler` | Filtreler (kategori, şehir, puan), kart listesi, arama | `GET /api/app/trainer-profiles`, `GET /api/app/categories` |
| Paket listesi | `/paketler` | Paket kartları (eğitmen adı, fiyat, seans sayısı) | `GET /api/app/service-packages` (veya eğitmen bazlı) |
| Eğitmen profili (public) | `/egitmenler/[slug]` | Profil özeti, paketleri, yorumlar (okuma), “Paket Seç” butonu (girişe yönlendirir) | `GET /api/app/trainer-profiles/by-slug/[slug]` (veya id), `GET /api/app/service-packages`, `GET /api/app/reviews` |
| Giriş | `/giris` | E-posta/şifre, “Giriş Yap” | Backend auth (OpenIddict token endpoint) |
| Kayıt | `/kayit` | Form: e-posta, şifre, ad, rol (Öğrenci/Eğitmen) | Backend register endpoint |

---

### 4.2 Öğrenci (Student)

| Sayfa | Route | İçerik | Kullanılan API |
|-------|--------|--------|----------------|
| Dashboard | `/ogrenci` veya `/ogrenci/dashboard` | Özet: yaklaşan seanslar, son siparişler, kısa istatistik | `GET /api/app/orders/my-orders`, `GET /api/app/orders/[id]/sessions` (yaklaşanlar için) |
| Eğitmenleri keşfet | `/ogrenci/egitmenler` | Eğitmen arama/liste (kategori, şehir, puan), kart görünümü | `GET /api/app/trainer-profiles`, `GET /api/app/categories` |
| Eğitmen profili | `/ogrenci/egitmenler/[slug]` | Profil detay, paketler, yorumlar; “Paket Seç” → paket detay/sipariş | `GET /api/app/trainer-profiles/...`, `GET /api/app/service-packages`, `GET /api/app/reviews` |
| Paket detay / Satın al | `/ogrenci/paketler/[id]` veya `/ogrenci/siparis-olustur?packageId=...` | Paket bilgisi, fiyat, “Satın Al” → sipariş oluşturma (notlar vb.) | `GET /api/app/service-packages/[id]`, `POST /api/app/orders` (CreateAsync) |
| Siparişlerim | `/ogrenci/siparisler` | Liste (tarih, eğitmen, paket, durum), detay linki | `GET /api/app/orders/my-orders` |
| Sipariş detay | `/ogrenci/siparisler/[id]` | Sipariş bilgisi, seans listesi, iptal (kurallara uygun) | `GET /api/app/orders/[id]`, `GET /api/app/orders/[id]/sessions`, `POST /api/app/orders/[id]/cancel` |
| Seanslarım | `/ogrenci/seanslar` | Yaklaşan/geçmiş seanslar (tarih, eğitmen, MeetingUrl/Location) | Siparişlerden seanslar çekilir veya ayrı endpoint varsa kullanılır |
| Mesajlar | `/ogrenci/mesajlar` | Konuşma listesi, tıklanınca mesaj thread’i | `GET /api/app/messaging/conversations`, `GET /api/app/messaging/...` (mesaj listesi, gönder) |
| Bildirimler | `/ogrenci/bildirimler` | Liste, okundu işaretle | `GET /api/app/notifications/my`, `POST /api/app/notifications/mark-read` |
| Destek | `/ogrenci/destek` | Taleplerim listesi, yeni talep açma formu | `GET /api/app/support-tickets/my`, `POST /api/app/support-tickets` |
| Profil / Ayarlar | `/ogrenci/profil` | Ad, e-posta, şifre değiştir (backend’de account API varsa) | Account API |

---

### 4.3 Eğitmen (Trainer)

| Sayfa | Route | İçerik | Kullanılan API |
|-------|--------|--------|----------------|
| Dashboard | `/egitmen` veya `/egitmen/dashboard` | Özet: bekleyen siparişler, yaklaşan seanslar, cüzdan özeti (AvailableBalance), son işlemler | `GET /api/app/orders/trainer-orders`, `GET /api/app/wallet/my`, `GET /api/app/notifications/my` (sayı) |
| Profilim | `/egitmen/profil` | Profil düzenleme (bio, şehir, sertifikalar, galeri) | `GET /api/app/trainer-profiles/me`, `PUT /api/app/trainer-profiles/[id]`, sertifika/galeri endpoint’leri |
| Paketlerim | `/egitmen/paketler` | Paket listesi, ekle/düzenle/sil | `GET /api/app/service-packages` (trainer’a göre filtrelenmiş backend’de), `POST/PUT/DELETE` |
| Paket ekle/düzenle | `/egitmen/paketler/yeni`, `/egitmen/paketler/[id]` | Form: başlık, tip, fiyat, seans sayısı, müsaitlik (varsa) | `POST /api/app/service-packages`, `PUT /api/app/service-packages/[id]` |
| Siparişlerim | `/egitmen/siparisler` | Eğitmene gelen siparişler, durum, öğrenci bilgisi | `GET /api/app/orders/trainer-orders` |
| Seanslarım | `/egitmen/seanslar` | Yaklaşan/geçmiş seanslar, MeetingUrl/Location girişi, tamamla | Sipariş detayından seanslar; tamamlama için `POST /api/app/orders/[id]/complete` (varsa) |
| Cüzdan | `/egitmen/cüzdan` | AvailableBalance, PendingBalance, son işlemler listesi | `GET /api/app/wallet/my`, `GET /api/app/wallet/my/transactions` |
| Para çekme | `/egitmen/para-cekme` | Talep listesi, “Yeni talep” formu (tutar, IBAN, hesap adı) | `GET /api/app/withdrawal-requests/my`, `POST /api/app/withdrawal-requests` |
| Mesajlar | `/egitmen/mesajlar` | Konuşma listesi, mesajlaşma | `GET /api/app/messaging/...` |
| Bildirimler | `/egitmen/bildirimler` | Liste, okundu işaretle | `GET /api/app/notifications/my`, mark read |
| Destek | `/egitmen/destek` | Taleplerim, yeni talep | `GET /api/app/support-tickets/my`, `POST /api/app/support-tickets` |

---

### 4.4 Admin

| Sayfa | Route | İçerik | Kullanılan API |
|-------|--------|--------|----------------|
| Dashboard | `/admin` veya `/admin/dashboard` | İstatistikler (kullanıcı, sipariş, destek açık, uyuşmazlık açık) | `GET /api/app/admin/dashboard` (GetDashboardAsync) |
| Kullanıcı yönetimi | `/admin/kullanicilar` | Kullanıcı listesi, rol atama (backend Identity API) | ABP Identity API’leri |
| Destek talepleri | `/admin/destek` | Talep listesi, filtre (durum, kategori), detay, yanıt yazma | `GET /api/app/support-tickets/list`, `GET /api/app/support-tickets/[id]`, `POST /api/app/support-tickets/[id]/reply`, durum güncelleme |
| Uyuşmazlıklar | `/admin/uyusmazliklar` | Liste, detay, çözüm notu ile kapatma | `GET /api/app/disputes/list`, `GET /api/app/disputes/[id]`, `POST /api/app/disputes/[id]/resolve` |
| Öne çıkanlar | `/admin/one-cikanlar` | Liste, ekle/düzenle/sil (sayfa tipi, eğitmen/paket, sıra) | `GET /api/app/featured-listings`, `POST/PUT/DELETE` |
| Blog | `/admin/blog` | Yazı listesi, yeni/düzenle, yayınla | `GET /api/app/blog-posts`, `POST /api/app/blog-posts`, `PUT /api/app/blog-posts/[id]`, `POST /api/app/blog-posts/[id]/publish` |
| Para çekme talepleri | `/admin/para-cekme` | Talepler listesi, onay/red/işlendi | `GET /api/app/withdrawal-requests/list`, `POST approve/reject/mark-processed` |
| Bildirimler | `/admin/bildirimler` | (İsteğe bağlı) Sistem bildirimleri | `GET /api/app/notifications` (admin için liste varsa) |

---

## 5. API Endpoint Özeti (Frontend İçin)

Backend’de mevcut route’lar (base: `http://localhost:6000` veya env’den):

| Modül | Endpoint örneği | Kullanım |
|-------|------------------|----------|
| Auth | Backend OpenIddict (login, token) | Giriş, token alma |
| Eğitmen | `GET/POST/PUT /api/app/trainer-profiles` | Liste, oluştur, güncelle; slug ile get (varsa) |
| Kategori | `GET /api/app/categories` | Filtre listesi |
| Paket | `GET/POST/PUT/DELETE /api/app/service-packages` | Liste, CRUD |
| Sipariş | `GET /api/app/orders/my-orders`, `trainer-orders`, `POST .../cancel`, `.../complete`, `GET .../[id]/sessions` | Öğrenci/eğitmen siparişleri, seanslar |
| Ödeme | `GET /api/app/payments`, `by-order/[orderId]` | Sipariş ödeme bilgisi |
| Cüzdan | `GET /api/app/wallet/my`, `GET .../my/transactions` | Eğitmen cüzdan |
| Para çekme | `GET/POST /api/app/withdrawal-requests`, `.../my`, `.../list`, `.../[id]/approve` vb. | Eğitmen talep, admin onay |
| Yorum | `GET/POST /api/app/reviews`, reply | Liste, oluştur, eğitmen yanıtı |
| Mesajlaşma | `GET/POST /api/app/messaging/...` (conversations, messages) | Konuşma ve mesajlar |
| Bildirim | `GET /api/app/notifications/my`, mark-read | Liste, okundu |
| Destek | `GET/POST /api/app/support-tickets`, `.../my`, `.../list`, `.../[id]/reply` | Talep CRUD, admin yanıt |
| Öne çıkan | `GET/POST/PUT/DELETE /api/app/featured-listings` | Admin CRUD |
| Uyuşmazlık | `GET/POST /api/app/disputes`, `.../my`, `.../list`, `.../[id]/resolve` | Öğrenci/eğitmen açma, admin çözüm |
| Blog | `GET /api/app/blog-posts`, `published`, `by-slug/[slug]`, CRUD, publish | Public liste, admin CRUD |
| Admin | `GET /api/app/admin/dashboard` | İstatistikler |

---

## 6. Uygulama Fazları (Kodlama Sırası)

### Faz 1 — Altyapı ve Auth (Öncelik 1)
- Next.js 15 projesi oluştur (`frontend/`).
- Tailwind, TypeScript, ortam değişkeni (API URL).
- Auth: login sayfası, token alma, token’ı saklama (httpOnly cookie tercih edilir; yoksa localStorage).
- Rol çözümleme: token veya “me” endpoint’i ile rol bilgisi; giriş sonrası yönlendirme (Student → `/ogrenci`, Trainer → `/egitmen`, Admin → `/admin`).
- Ortak layout: header (logo, giriş/kayıt veya kullanıcı menüsü + rol bazlı “Dashboard” linki), footer.
- Route koruma: `/ogrenci/*`, `/egitmen/*`, `/admin/*` sadece ilgili rol için erişilebilir; değilse redirect.

### Faz 2 — Misafir ve Öğrenci Akışı (Öncelik 2)
- Ana sayfa `/`: hero, öne çıkan eğitmenler (API), kategoriler, CTA.
- `/egitmenler`: eğitmen listesi, basit filtre (kategori), kart görünümü.
- `/egitmenler/[slug]`: eğitmen profili (public), paketleri, yorumlar; “Paket Seç” / “Giriş Yap”.
- Giriş/kayıt sayfaları.
- Öğrenci layout + menü.
- `/ogrenci/dashboard`: özet (siparişler, yaklaşan seanslar).
- `/ogrenci/egitmenler`, `/ogrenci/egitmenler/[slug]`: aynı içerik, öğrenci menüsünden erişim.
- `/ogrenci/paketler`, paket detay, sipariş oluşturma (POST order).
- `/ogrenci/siparisler`, `/ogrenci/siparisler/[id]`: liste, detay, seanslar, iptal.
- `/ogrenci/seanslar`: seans listesi (siparişlerden türetilmiş).
- `/ogrenci/bildirimler`, `/ogrenci/destek` (talep listesi + yeni talep).

### Faz 3 — Eğitmen Akışı (Öncelik 3)
- Eğitmen layout + menü.
- `/egitmen/dashboard`: özet (siparişler, seanslar, cüzdan).
- `/egitmen/profil`: profil düzenleme, sertifika/galeri (backend endpoint’lerine göre).
- `/egitmen/paketler`: liste, ekle/düzenle/sil.
- `/egitmen/siparisler`, `/egitmen/seanslar`.
- `/egitmen/cüzdan`: bakiye, işlem geçmişi.
- `/egitmen/para-cekme`: talep listesi, yeni talep formu.
- `/egitmen/mesajlar`, `/egitmen/bildirimler`, `/egitmen/destek`.

### Faz 4 — Admin Akışı (Öncelik 4)
- Admin layout + menü.
- `/admin/dashboard`: istatistikler (Admin API).
- `/admin/destek`: talep listesi, detay, yanıt ve durum güncelleme.
- `/admin/uyusmazliklar`: liste, detay, çözüm notu ile kapatma.
- `/admin/one-cikanlar`: CRUD.
- `/admin/blog`: liste, CRUD, yayınlama.
- `/admin/para-cekme`: talepler, onay/red/işlendi.
- (İsteğe bağlı) `/admin/kullanicilar`: ABP Identity API ile kullanıcı listesi ve rol atama.

### Faz 5 — İyileştirmeler (Öncelik 5)
- Mesajlaşma sayfaları (öğrenci + eğitmen) tam akış.
- Arama/filtre geliştirmeleri (şehir, puan, kategori).
- Bildirim badge’i (header’da okunmamış sayı).
- Responsive ve erişilebilirlik kontrolleri.
- Hata sayfaları (404, 403) ve ortak hata mesajları.

---

## 7. Klasör Yapısı Önerisi (Next.js App Router)

```
frontend/
├── app/
│   ├── layout.tsx                 # Root layout (header, footer)
│   ├── page.tsx                   # Ana sayfa (/)
│   ├── giris/page.tsx
│   ├── kayit/page.tsx
│   ├── egitmenler/
│   │   ├── page.tsx               # Liste
│   │   └── [slug]/page.tsx        # Profil (public)
│   ├── paketler/page.tsx          # Public paket listesi (opsiyonel)
│   ├── ogrenci/
│   │   ├── layout.tsx             # Öğrenci menü
│   │   ├── page.tsx               # Dashboard
│   │   ├── egitmenler/...
│   │   ├── paketler/...
│   │   ├── siparisler/...
│   │   ├── seanslar/...
│   │   ├── mesajlar/...
│   │   ├── bildirimler/...
│   │   └── destek/...
│   ├── egitmen/
│   │   ├── layout.tsx             # Eğitmen menü
│   │   ├── page.tsx
│   │   ├── profil/...
│   │   ├── paketler/...
│   │   ├── siparisler/...
│   │   ├── seanslar/...
│   │   ├── cüzdan/...
│   │   ├── para-cekme/...
│   │   └── ...
│   └── admin/
│       ├── layout.tsx             # Admin menü
│       ├── page.tsx
│       ├── destek/...
│       ├── uyusmazliklar/...
│       ├── one-cikanlar/...
│       ├── blog/...
│       └── para-cekme/...
├── components/
│   ├── ui/                        # Button, Card, Input, Modal vb.
│   ├── layout/                    # Header, Footer, Sidebar
│   ├── ogrenci/                   # Öğrenciye özel bileşenler
│   ├── egitmen/                   # Eğitmene özel bileşenler
│   └── admin/                    # Admin’e özel bileşenler
├── lib/
│   ├── api.ts                     # API client (fetch + base URL, token)
│   ├── auth.ts                    # Token okuma, rol, logout
│   └── types.ts                  # API yanıt tipleri (DTO’lara uygun)
├── hooks/                         # useAuth, useMe, useOrders vb.
└── ...
```

---

## 8. Referanslar

- Backend API: `docs/openapi/swagger.web.v1.full.json` (üretilmiş Swagger).
- Yetkilendirme: `docs/AUTHORIZATION-SYSTEM.md`.
- Proje rehberi: `docs/PROJE_REHBERI.md`.
- Frontend değişiklik takibi: `docs/frontend-changes/README.md`.

Bu plan, frontend’in “ne yapacağını” ve “hangi sırayla” kodlanacağını tanımlar. Plana göre ilk adım: **Faz 1 (altyapı + auth + rol yönlendirme)** ile başlamak.

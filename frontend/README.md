# Fitliyo Web Frontend

Rol bazlı (Öğrenci / Eğitmen / Admin) Fitliyo marketplace arayüzü. Next.js 14, TypeScript, Tailwind CSS.

## Gereksinimler

- Node.js 20+
- Backend (Fitliyo.Web) çalışır durumda ve CORS’ta frontend origin’e izin veriyor olmalı

## Kurulum

```bash
cd frontend
cp .env.example .env.local
# .env.local içinde NEXT_PUBLIC_API_URL'yi backend adresine göre ayarla (örn. http://localhost:5000)
npm install
npm run dev
```

Tarayıcıda: [http://localhost:3000](http://localhost:3000)

## Rol ve Sayfalar

- **Misafir:** `/`, `/egitmenler`, `/giris`, `/kayit`
- **Öğrenci:** `/ogrenci` (dashboard), siparişler, seanslar, bildirimler, destek
- **Eğitmen:** `/egitmen` (dashboard), profil, paketler, siparişler, cüzdan, para çekme
- **Admin:** `/admin` (dashboard), destek, uyuşmazlıklar, öne çıkanlar, blog, para çekme

Giriş sonrası rol bilgisi token’dan alınır; Öğrenci → `/ogrenci`, Eğitmen → `/egitmen`, Admin/SuperAdmin → `/admin` yönlendirilir.

## Plan

Detaylı sayfa listesi ve API eşlemesi: [docs/FRONTEND_PLANI.md](../docs/FRONTEND_PLANI.md)

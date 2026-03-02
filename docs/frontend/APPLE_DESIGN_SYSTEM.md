# Fitliyo Frontend — Apple Tarzı Tasarım Sistemi

Bu doküman, Fitliyo web arayüzünün **Apple web sitesi** estetiğine göre nasıl tasarlandığını ve bundan sonraki tüm tasarımsal geliştirmelerde uyulacak kuralları tanımlar.

## 1. Genel İlkeler

- **Sade ve ferah**: Bol beyaz alan, gereksiz çerçeve ve gölgeden kaçınma.
- **Tipografi odaklı**: Başlıklar büyük ve net; metin hiyerarşisi belirgin.
- **Yumuşak etkileşim**: Hover’da hafif gölge veya opacity; sert geçişler yok.
- **Tutarlı dil**: Buton, link ve kartlar aynı dilde (pill/rounded, mavi vurgu).

## 2. Renk Paleti

| Token | Hex | Kullanım |
|-------|-----|----------|
| **primary** | `#0071e3` | Ana CTA, linkler, vurgular (Apple mavisi) |
| **primary-hover** | `#0077ed` | Buton/link hover |
| **apple-black** | `#1d1d1f` | Ana metin |
| **apple-gray** | `#6e6e73` | İkincil metin |
| **apple-grayLight** | `#86868b` | Placeholder, yardımcı metin |
| **apple-grayLighter** | `#d2d2d7` | İnce çizgiler, border |
| **apple-bg** | `#fbfbfd` | Sayfa arka planı (hafif gri-beyaz) |
| **apple-white** | `#ffffff` | Kartlar, header arka planı |

Tailwind’de: `text-apple-black`, `bg-primary`, `border-apple-grayLighter` vb.

## 3. Tipografi

- **Font ailesi**: Sistem fontu (SF Pro Display/Text macOS’ta, Segoe UI Windows’ta).  
  Tailwind: `font-sans`, başlıklar için `font-display`.
- **Boyutlar** (Tailwind extend):
  - `text-apple-hero`: Ana hero başlık (~3rem, sıkı satır).
  - `text-apple-title`: Bölüm başlıkları (~2.5rem).
  - `text-apple-subhead`: Alt başlık (~1.5rem).
  - `text-apple-body`: Gövde metni (~1.0625rem).
- **Ağırlık**: Başlıklar `font-semibold`, gövde `font-normal`; gereksiz bold kullanılmaz.

## 4. Bileşen Sınıfları (globals.css)

- **`.btn-apple-primary`**: Mavi dolgu, pill form (`rounded-full`), beyaz yazı. Ana aksiyonlar.
- **`.btn-apple-secondary`**: Şeffaf arka plan, ince border, mavi yazı. İkincil aksiyonlar.
- **`.link-apple`**: Mavi metin, hover’da underline.
- **`.card-apple`**: Beyaz arka plan, `rounded-apple-lg`, `shadow-apple`. Liste/kart blokları.

Yeni sayfa veya bileşen eklerken bu sınıfları kullan; yeni renk/buton stili eklemek yerine mevcut token’larla çalış.

## 5. Köşe ve Gölge

- **Kart / form**: `rounded-apple` (1.125rem) veya `rounded-apple-lg` (1.375rem).
- **Buton**: `rounded-full` (pill).
- **Gölge**: `shadow-apple`, `shadow-apple-md`, `shadow-apple-lg` (hafif, siyah ~4–8% opacity).

## 6. Header ve Navigasyon

- Sticky, yarı saydam arka plan + `backdrop-blur` (Apple nav hissi).
- Yükseklik ~48px (`h-12`), sade linkler; ana CTA “Kayıt Ol” için `btn-apple-primary`.

## 7. Formlar

- Input: `rounded-apple`, `border-apple-grayLighter`, focus’ta `ring-2 ring-primary/20`.
- Label: `text-apple-body font-medium text-apple-black`.
- Hata mesajı: `bg-red-50 text-red-600`, başarı: `bg-primary/10 text-primary`.

## 8. Referans Dosyalar

- **Tailwind tema**: `frontend/tailwind.config.ts`
- **Global stiller**: `frontend/app/globals.css`
- **Örnek sayfalar**: `frontend/app/page.tsx`, `frontend/app/login/page.tsx`, `frontend/components/layout/Header.tsx`

Bundan sonraki tüm tasarımsal geliştirmeler (yeni sayfalar, paneller, modaller, liste kartları) yukarıdaki renk, tipografi ve bileşen sınıflarına uyularak yapılmalıdır.

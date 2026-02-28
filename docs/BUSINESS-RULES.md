# Fitliyo İş Kuralları

**Son Güncelleme:** 2026-02-28
**Doküman Versiyonu:** v4.0

---

## 1. Ödeme Akışı

### 1.1 Sipariş → Ödeme → Escrow → Serbest Bırakma

```
1. Öğrenci paketi seçer → Order [Pending] oluşur
2. iyzico/Stripe checkout → ödeme sayfasına yönlendirilir
3. Webhook → Order [Confirmed], Payment [Completed]
4. Para escrow'a alınır → TrainerWallet.PendingBalance artar
5. İlk seans tamamlanması + 48 saat → TrainerWallet.AvailableBalance'a aktarılır
6. Eğitmen para çekme talebi → WithdrawalRequest [Pending]
7. Admin onayı → banka transferi → WithdrawalRequest [Processed]
```

### 1.2 Komisyon Hesaplama

```
PlatformCommissionAmount = TotalAmount × CommissionRate
TrainerEarningAmount = TotalAmount - PlatformCommissionAmount
```

**Varsayılan komisyon oranı:** %15

| Abonelik Planı | Komisyon Oranı |
|----------------|---------------|
| Free | %15 |
| Basic (299₺/ay) | %12 |
| Pro (699₺/ay) | %10 |

Komisyon oranı `SubscriptionPlan.CommissionRate` alanından override edilebilir.

### 1.3 Ödeme Sağlayıcıları

| Sağlayıcı | Pazar | Kullanım |
|-----------|-------|----------|
| iyzico | Türkiye | Birincil ödeme |
| Stripe | Uluslararası | İkincil ödeme |
| Manual | Admin | Manuel düzeltme |

---

## 2. İptal ve İade Politikası

### 2.1 Öğrenci İptali

| Süre | İade Oranı |
|------|-----------|
| 48+ saat öncesi | %100 iade |
| 24-48 saat arası | %50 iade |
| 24 saat içi | İade yok |

### 2.2 Eğitmen İptali

- Eğitmen iptal ederse → %100 iade (öğrenciye)
- Eğitmen no-show → %100 iade + ceza puanı (eğitmene)

### 2.3 İade Akışı

```
1. İptal talebi → süre kontrolü → iade oranı hesapla
2. Payment.RefundAmount güncelle
3. TrainerWallet.PendingBalance'dan düş
4. Ödeme sağlayıcısına refund isteği gönder
5. Order [Refunded] durumuna geçir
```

---

## 3. Abonelik Sistemi (Eğitmen)

### 3.1 Plan Detayları

| Plan | Aylık Fiyat | Yıllık Fiyat | Paket Limiti | Komisyon | Özellikler |
|------|------------|-------------|-------------|---------|------------|
| **Free** | 0₺ | 0₺ | 3 paket/ay | %15 | Temel profil |
| **Basic** | 299₺ | 2.990₺ | Sınırsız | %12 | Sınırsız paket |
| **Pro** | 699₺ | 6.990₺ | Sınırsız | %10 | Öncelikli listeleme + analitik |

### 3.2 Otomatik Yenileme

- `AutoRenew = true` → dönem sonu otomatik ödeme
- Ödeme başarısız → `PastDue` durumu → 3 gün süre → `Expired`
- İptal edilen abonelik dönem sonuna kadar aktif kalır

### 3.3 Free Plan Kısıtlamaları

- Aylık max 3 paket satışı
- Öncelikli listeleme yok
- Analitik rapor yok

---

## 4. Değerlendirme Kuralları

### 4.1 Yorum Yazma Koşulları

- **Sadece** tamamlanmış sipariş sahibi yorum yapabilir (`Order.Status == Completed`)
- Sipariş başına **1 yorum** (`OrderId` unique)
- Sipariş tamamlandıktan sonra **7 gün penceresi** (7 gün sonra yorum yazılamaz)
- `IsVerifiedPurchase = true` otomatik set edilir

### 4.2 Puan Hesaplama

```
OverallRating = (CommunicationRating + ExpertiseRating + ValueForMoneyRating + PunctualityRating) / 4
```

### 4.3 Eğitmen Ortalama Puan Güncelleme

Review oluşturulduğunda/güncellendiğinde `TrainerProfile.AverageRating` ve `TotalReviewCount` güncellenir:

```csharp
trainerProfile.AverageRating = reviews.Average(r => r.OverallRating);
trainerProfile.TotalReviewCount = reviews.Count;
```

### 4.4 Eğitmen Yanıtı

- Eğitmen kendi profiline gelen yorumlara yanıt verebilir
- `TrainerReply` + `TrainerRepliedAt` güncellenir
- Yanıt bir kere yazılır, düzenlenemez

---

## 5. Sıralama Algoritması

Eğitmen arama sonuçlarında kullanılan sıralama formülü:

```
Score = AverageRating × 0.35
      + log(TotalReviewCount + 1) × 0.20
      + ResponseRate × 0.15
      + ProfileCompletionPct × 0.10
      + RecentActivityScore × 0.10
      + SubscriptionTierScore × 0.10
```

| SubscriptionTier | Score |
|-----------------|-------|
| Free | 0.0 |
| Basic | 0.5 |
| Pro | 1.0 |

**RecentActivityScore:** Son 30 gün içinde aktif seans varsa 1.0, yoksa 0.0
**ResponseRate:** Mesajlara ortalama yanıt süresi bazlı (1 saat içi → 1.0, 24+ saat → 0.0)

---

## 6. Mesajlaşma Kuralları

### 6.1 Spam Koruması

- Satın alma yapılmadan ilk mesaj gönderilebilir
- **Saatte max 3 yeni konuşma** başlatılabilir (spam koruması)
- Eğitmen konuşmayı engelleyebilir (`IsBlockedByTrainer`)

### 6.2 Mesaj Tipleri

| Tip | Açıklama |
|-----|----------|
| Text | Metin mesajı |
| File | Dosya eki |
| Image | Resim |
| Video | Video |
| System | Sistem mesajı (otomatik — sipariş onayı, seans hatırlatma) |

---

## 7. Seans Hatırlatıcıları

| Zamanlama | Kanal | Mesaj |
|-----------|-------|-------|
| 24 saat önce | Push + In-app | "Yarın saat {saat}'de {eğitmen} ile seansınız var." |
| 1 saat önce | Push + In-app | "1 saat sonra {eğitmen} ile seansınız başlıyor." |

---

## 8. Seans Durumu Geçişleri

```
Scheduled → Completed (seans tamamlandı)
Scheduled → NoShow (katılım olmadı)
Scheduled → Cancelled (iptal edildi)
Scheduled → Rescheduled (yeniden planlandı → yeni Session oluşur)
```

### 8.1 No-Show Kuralları

- **Öğrenci no-show:** Seans Completed sayılır, iade yok
- **Eğitmen no-show:** %100 iade + ceza puanı

---

## 9. Wallet ve Para Çekme

### 9.1 Balance Türleri

| Tür | Açıklama |
|-----|----------|
| PendingBalance | Escrow'daki para (henüz serbest bırakılmamış) |
| AvailableBalance | Çekilebilir para |
| TotalEarned | Toplam kazanç (tarihsel) |
| TotalWithdrawn | Toplam çekilmiş (tarihsel) |

### 9.2 Para Çekme Akışı

```
1. Eğitmen → WithdrawalRequest [Pending] (minimum tutar: 100₺)
2. Admin inceleme → [Approved] veya [Rejected]
3. Approved → banka transferi → [Processed]
4. WalletTransaction [Payout] kaydı oluştur
5. AvailableBalance düşür, TotalWithdrawn artır
```

---

## 10. Featured Listings (Öne Çıkarma)

| PageType | Açıklama |
|----------|----------|
| Homepage | Ana sayfa öne çıkanlar |
| CategoryPage | Kategori sayfası öne çıkanlar |
| SearchResult | Arama sonuçlarında öne çıkanlar |

- Pro abonelik → otomatik öncelikli listeleme
- Ek ücretle featured listing satın alınabilir
- `StartDate` → `EndDate` aralığında aktif

---

**Doküman Versiyonu:** v4.0
**Son Güncelleme:** 2026-02-28

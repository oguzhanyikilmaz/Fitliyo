# Fitliyo Changelog

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

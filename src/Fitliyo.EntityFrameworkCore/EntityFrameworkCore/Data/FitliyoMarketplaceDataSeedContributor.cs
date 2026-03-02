using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Admin;
using Fitliyo.Categories;
using Fitliyo.Content;
using Fitliyo.Enums;
using Fitliyo.Messaging;
using Fitliyo.Notifications;
using Fitliyo.Orders;
using Fitliyo.Payments;
using Fitliyo.Profiles;
using Fitliyo.Reviews;
using Fitliyo.ServicePackages;
using Fitliyo.Subscriptions;
using Fitliyo.Support;
using Fitliyo.Trainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Fitliyo.EntityFrameworkCore.Data;

/// <summary>
/// Tüm marketplace entity'leri için anlamlı test verileri seed eder.
/// FitliyoIdentityDataSeedContributor (admin, egitmen, ogrenci) çalıştıktan sonra çalıştırılmalıdır.
/// </summary>
public class FitliyoMarketplaceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<TrainerCertificate, Guid> _trainerCertificateRepository;
    private readonly IRepository<TrainerGallery, Guid> _trainerGalleryRepository;
    private readonly FitliyoDbContext _dbContext;
    private readonly IRepository<SubscriptionPlan, Guid> _subscriptionPlanRepository;
    private readonly IRepository<TrainerWallet, Guid> _trainerWalletRepository;
    private readonly IRepository<ServicePackage, Guid> _servicePackageRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Session, Guid> _sessionRepository;
    private readonly IRepository<Payment, Guid> _paymentRepository;
    private readonly IRepository<Review, Guid> _reviewRepository;
    private readonly IRepository<Conversation, Guid> _conversationRepository;
    private readonly IRepository<Message, Guid> _messageRepository;
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<SupportTicket, Guid> _supportTicketRepository;
    private readonly IRepository<FeaturedListing, Guid> _featuredListingRepository;
    private readonly IRepository<BlogPost, Guid> _blogPostRepository;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IRepository<WithdrawalRequest, Guid> _withdrawalRequestRepository;
    private readonly IRepository<Dispute, Guid> _disputeRepository;
    private readonly ILogger<FitliyoMarketplaceDataSeedContributor> _logger;

    public FitliyoMarketplaceDataSeedContributor(
        IIdentityUserRepository userRepository,
        IGuidGenerator guidGenerator,
        IRepository<Category, Guid> categoryRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<TrainerCertificate, Guid> trainerCertificateRepository,
        IRepository<TrainerGallery, Guid> trainerGalleryRepository,
        FitliyoDbContext dbContext,
        IRepository<SubscriptionPlan, Guid> subscriptionPlanRepository,
        IRepository<TrainerWallet, Guid> trainerWalletRepository,
        IRepository<ServicePackage, Guid> servicePackageRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<Session, Guid> sessionRepository,
        IRepository<Payment, Guid> paymentRepository,
        IRepository<Review, Guid> reviewRepository,
        IRepository<Conversation, Guid> conversationRepository,
        IRepository<Message, Guid> messageRepository,
        IRepository<Notification, Guid> notificationRepository,
        IRepository<SupportTicket, Guid> supportTicketRepository,
        IRepository<FeaturedListing, Guid> featuredListingRepository,
        IRepository<BlogPost, Guid> blogPostRepository,
        IRepository<UserProfile, Guid> userProfileRepository,
        IRepository<WithdrawalRequest, Guid> withdrawalRequestRepository,
        IRepository<Dispute, Guid> disputeRepository,
        ILogger<FitliyoMarketplaceDataSeedContributor> logger)
    {
        _userRepository = userRepository;
        _guidGenerator = guidGenerator;
        _categoryRepository = categoryRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _trainerCertificateRepository = trainerCertificateRepository;
        _trainerGalleryRepository = trainerGalleryRepository;
        _dbContext = dbContext;
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _trainerWalletRepository = trainerWalletRepository;
        _servicePackageRepository = servicePackageRepository;
        _orderRepository = orderRepository;
        _sessionRepository = sessionRepository;
        _paymentRepository = paymentRepository;
        _reviewRepository = reviewRepository;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _notificationRepository = notificationRepository;
        _supportTicketRepository = supportTicketRepository;
        _featuredListingRepository = featuredListingRepository;
        _blogPostRepository = blogPostRepository;
        _userProfileRepository = userProfileRepository;
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _disputeRepository = disputeRepository;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var admin = await _userRepository.FindByNormalizedUserNameAsync("ADMINFITLIYO");
        var trainerUser = await _userRepository.FindByNormalizedUserNameAsync("EGITMEN");
        var studentUser = await _userRepository.FindByNormalizedUserNameAsync("OGRENCI");

        if (trainerUser == null || studentUser == null)
        {
            _logger.LogWarning("Marketplace seed atlandı: Önce DbMigrator ile admin/egitmen/ogrenci kullanıcılarını oluşturun.");
            return;
        }

        if (await _trainerProfileRepository.AnyAsync(x => x.UserId == trainerUser.Id))
        {
            _logger.LogInformation("Marketplace test verileri zaten mevcut, seed atlanıyor.");
            return;
        }

        _logger.LogInformation("Marketplace test verileri ekleniyor...");

        // Kategoriler (en az 5)
        var catFitness = await SeedCategoryAsync("Fitness & Kondisyon", "fitness-kondisyon", null, 1);
        var catSpor = await SeedCategoryAsync("Spor Koçluğu", "spor-koclugu", null, 2);
        var catBeslenme = await SeedCategoryAsync("Beslenme", "beslenme", null, 3);
        var catPilates = await SeedCategoryAsync("Pilates", "pilates", catFitness.Id, 10);
        var catYoga = await SeedCategoryAsync("Yoga", "yoga", catFitness.Id, 11);
        var categoryIds = new[] { catFitness.Id, catSpor.Id, catBeslenme.Id, catPilates.Id, catYoga.Id };

        var trainerProfile = await SeedTrainerProfileAsync(trainerUser.Id);
        await SeedTrainerCertificatesAsync(trainerProfile.Id, 5);
        await SeedTrainerGalleriesAsync(trainerProfile.Id, 5);
        await _dbContext.SaveChangesAsync();
        await SeedTrainerCategoryMappingsAsync(trainerProfile.Id, categoryIds);

        await SeedSubscriptionPlansAsync();
        var wallet = await SeedTrainerWalletAsync(trainerProfile.Id);
        var packages = await SeedServicePackagesAsync(trainerProfile.Id, 5);

        var orders = await SeedOrdersAsync(studentUser.Id, trainerProfile.Id, packages, 5);
        await SeedSessionsAsync(orders, trainerProfile.Id, studentUser.Id);
        var payments = await SeedPaymentsAsync(orders);
        for (var i = 0; i < orders.Count; i++)
        {
            orders[i].MarkAsPaid("Iyzico", $"TXN-SEED-{i + 1:D3}");
            orders[i].Complete();
            await _orderRepository.UpdateAsync(orders[i]);
        }
        await SeedReviewsAsync(orders, studentUser.Id, trainerProfile.Id, packages);

        await SeedConversationsAndMessagesAsync(studentUser.Id, trainerUser.Id, 5);
        await SeedNotificationsAsync(studentUser.Id, trainerUser.Id, 5);
        await SeedSupportTicketsAsync(studentUser.Id, 5);
        await SeedFeaturedListingsAsync(trainerProfile.Id, packages, 5);
        await SeedBlogPostsAsync(5);
        await SeedUserProfilesAsync(trainerUser.Id, studentUser.Id);
        await SeedWithdrawalRequestsAsync(wallet.Id, 5);
        if (admin != null)
            await SeedDisputesAsync(orders, studentUser.Id, admin.Id, 5);
    }

    private async Task<Category> SeedCategoryAsync(string name, string slug, Guid? parentId, int sortOrder)
    {
        var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
        if (existing != null) return existing;
        var id = _guidGenerator.Create();
        var cat = new Category(id, name, slug)
        {
            ParentId = parentId,
            SortOrder = sortOrder,
            Description = name + " alanında uzman eğitmenler ve programlar."
        };
        await _dbContext.Categories.AddAsync(cat);
        _logger.LogInformation("Kategori seed: {Name}", name);
        return cat;
    }

    private async Task<TrainerProfile> SeedTrainerProfileAsync(Guid userId)
    {
        var id = _guidGenerator.Create();
        var p = new TrainerProfile(id, userId, "ahmet-yilmaz-personal-trainer", TrainerType.PersonalTrainer)
        {
            Bio = "10 yılı aşkın deneyimle kişisel antrenör ve beslenme danışmanı. Spor bilimleri mezunu, sertifikalı CPT. Hedefinize ulaşmanız için kişiselleştirilmiş programlar sunuyorum.",
            ExperienceYears = 10,
            City = "İstanbul",
            District = "Kadıköy",
            IsOnlineAvailable = true,
            IsOnSiteAvailable = true,
            AverageRating = 4.8m,
            TotalReviewCount = 24,
            TotalStudentCount = 45,
            IsVerified = true,
            VerificationBadge = "Doğrulanmış Eğitmen",
            ProfileCompletionPct = 95,
            InstagramUrl = "https://instagram.com/ahmet_yilmaz_pt",
            WebsiteUrl = "https://ahmetyilmaz.fit"
        };
        await _dbContext.TrainerProfiles.AddAsync(p);
        _logger.LogInformation("Eğitmen profili seed: {Slug}", p.Slug);
        return p;
    }

    private async Task SeedTrainerCertificatesAsync(Guid trainerProfileId, int count = 5)
    {
        var certs = new (string name, string org, DateTime issue, DateTime? expiry)[]
        {
            ("NASM Certified Personal Trainer", "National Academy of Sports Medicine", new DateTime(2018, 6, 1), new DateTime(2026, 6, 1)),
            ("İlk Yardım Sertifikası", "Türk Kızılay", new DateTime(2023, 1, 15), null),
            ("ACE Health Coach", "American Council on Exercise", new DateTime(2020, 3, 1), new DateTime(2028, 3, 1)),
            ("Fitness Nutrition Specialist", "NASM", new DateTime(2021, 9, 1), new DateTime(2025, 9, 1)),
            ("TRX Suspension Training", "TRX Education", new DateTime(2022, 5, 10), new DateTime(2026, 5, 10))
        };
        for (var i = 0; i < Math.Min(count, certs.Length); i++)
        {
            var (name, org, issue, expiry) = certs[i];
            var c = new TrainerCertificate(_guidGenerator.Create(), trainerProfileId, name)
            {
                IssuingOrganization = org,
                IssueDate = issue,
                ExpiryDate = expiry,
                IsVerifiedByPlatform = true
            };
            await _dbContext.TrainerCertificates.AddAsync(c);
        }
        _logger.LogInformation("Eğitmen sertifikaları seed edildi: {Count} adet.", Math.Min(count, certs.Length));
    }

    private async Task SeedTrainerGalleriesAsync(Guid trainerProfileId, int count = 5)
    {
        var items = new[]
        {
            ("https://example.com/trainer-cover.jpg", "Antrenman stüdyosu", 0, true),
            ("https://example.com/trainer-2.jpg", "Bire bir seans", 1, false),
            ("https://example.com/trainer-3.jpg", "Grup dersi", 2, false),
            ("https://example.com/trainer-4.jpg", "Koçluk seansı", 3, false),
            ("https://example.com/trainer-5.jpg", "Sertifika töreni", 4, false)
        };
        for (var i = 0; i < Math.Min(count, items.Length); i++)
        {
            var (url, caption, order, isCover) = items[i];
            var g = new TrainerGallery(_guidGenerator.Create(), trainerProfileId, MediaType.Image, url)
            {
                Caption = caption,
                SortOrder = order,
                IsCoverImage = isCover
            };
            await _dbContext.TrainerGalleries.AddAsync(g);
        }
        _logger.LogInformation("Eğitmen galerisi seed edildi: {Count} adet.", Math.Min(count, items.Length));
    }

    private async Task SeedTrainerCategoryMappingsAsync(Guid trainerProfileId, Guid[] categoryIds)
    {
        // Raw SQL ile ekle: EF aynı context'te insert sırası nedeniyle CategoryId FK ihlali verebiliyor.
        // Kategoriler ve eğitmen zaten önceki SaveChangesAsync() ile DB'de.
        const string tableName = "\"AppTrainerCategoryMappings\"";
        foreach (var categoryId in categoryIds)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                $"INSERT INTO {tableName} (\"CategoryId\", \"TrainerProfileId\") VALUES ({{0}}, {{1}})",
                categoryId,
                trainerProfileId);
        }
        _logger.LogInformation("Eğitmen-kategori eşleşmeleri seed edildi.");
    }

    private async Task SeedSubscriptionPlansAsync()
    {
        try
        {
            if (await _subscriptionPlanRepository.AnyAsync()) return;

            var free = new SubscriptionPlan(_guidGenerator.Create(), "Ücretsiz", SubscriptionTier.Free, 0, 0.15m)
            {
                Description = "Temel liste, 3 paket limiti.",
                MaxPackageCount = 3,
                SortOrder = 0
            };
            var basic = new SubscriptionPlan(_guidGenerator.Create(), "Basic", SubscriptionTier.Basic, 99m, 0.10m)
            {
                Description = "10 paket, öne çıkan liste hakkı.",
                MaxPackageCount = 10,
                HasFeaturedListing = true,
                SortOrder = 1
            };
            var pro = new SubscriptionPlan(_guidGenerator.Create(), "Pro", SubscriptionTier.Pro, 249m, 0.08m)
            {
                Description = "Sınırsız paket, öncelikli destek, analitik.",
                MaxPackageCount = -1,
                HasFeaturedListing = true,
                HasPrioritySupport = true,
                HasAdvancedAnalytics = true,
                SortOrder = 2
            };
            var starter = new SubscriptionPlan(_guidGenerator.Create(), "Starter", SubscriptionTier.Free, 49m, 0.12m)
            {
                Description = "Başlangıç paketi, 5 paket limiti.",
                MaxPackageCount = 5,
                SortOrder = 3
            };
            var enterprise = new SubscriptionPlan(_guidGenerator.Create(), "Enterprise", SubscriptionTier.Pro, 499m, 0.05m)
            {
                Description = "Kurumsal eğitmenler için tam yetki.",
                MaxPackageCount = -1,
                HasFeaturedListing = true,
                HasPrioritySupport = true,
                HasAdvancedAnalytics = true,
                SortOrder = 4
            };
            await _subscriptionPlanRepository.InsertAsync(free);
            await _subscriptionPlanRepository.InsertAsync(basic);
            await _subscriptionPlanRepository.InsertAsync(pro);
            await _subscriptionPlanRepository.InsertAsync(starter);
            await _subscriptionPlanRepository.InsertAsync(enterprise);
            _logger.LogInformation("Abonelik planları seed edildi: 5 adet.");
        }
        catch (PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogWarning("AppSubscriptionPlans tablosu bulunamadı, abonelik planları seed atlanıyor. Eksik migration ekleyip DbMigrator tekrar çalıştırın.");
        }
    }

    private async Task<TrainerWallet> SeedTrainerWalletAsync(Guid trainerProfileId)
    {
        var w = new TrainerWallet(_guidGenerator.Create(), trainerProfileId);
        w.AddPending(750m);
        w.MovePendingToAvailable(750m);
        await _trainerWalletRepository.InsertAsync(w);
        _logger.LogInformation("Eğitmen cüzdanı seed edildi.");
        return w;
    }

    private async Task<List<ServicePackage>> SeedServicePackagesAsync(Guid trainerProfileId, int count = 5)
    {
        var packages = new[]
        {
            ("4 Seans Kişisel Antrenman Paketi", PackageType.Training, 1200m, 999m, 30, 4, 60, "fitness,personal-training,kondisyon"),
            ("8 Seans Yoğun Program", PackageType.Training, 2200m, 1899m, 60, 8, 60, "fitness,kondisyon,hedef"),
            ("Online Beslenme Danışmanlığı", PackageType.Nutrition, 800m, 699m, 30, 4, 45, "beslenme,diyet,online"),
            ("Pilates 10 Seans Paket", PackageType.Training, 1500m, 1299m, 45, 10, 55, "pilates,esneklik"),
            ("Kombine Antrenman + Beslenme", PackageType.Training, 2500m, 2199m, 60, 8, 60, "fitness,beslenme,kişisel")
        };
        var list = new List<ServicePackage>();
        for (var i = 0; i < Math.Min(count, packages.Length); i++)
        {
            var (title, type, price, discPrice, days, sessions, mins, tags) = packages[i];
            var id = _guidGenerator.Create();
            var p = new ServicePackage(id, trainerProfileId, title, type, price)
            {
                Description = title + " — hedeflerinize uygun program.",
                DiscountedPrice = discPrice,
                DurationDays = days,
                SessionCount = sessions,
                SessionDurationMinutes = mins,
                MaxStudents = 1,
                IsOnline = true,
                IsOnSite = true,
                CancellationHours = 24,
                CancellationPolicy = "Seans en az 24 saat önceden iptal edilmelidir.",
                WhatIsIncluded = "Kişisel program, takip, destek",
                Tags = tags,
                TotalSalesCount = 5 + i,
                AverageRating = 4.5m + (i * 0.1m),
                IsFeatured = i < 2
            };
            await _servicePackageRepository.InsertAsync(p);
            list.Add(p);
        }
        _logger.LogInformation("Hizmet paketleri seed edildi: {Count} adet.", list.Count);
        return list;
    }

    private async Task<List<Order>> SeedOrdersAsync(Guid studentId, Guid trainerProfileId, List<ServicePackage> packages, int count = 5)
    {
        var list = new List<Order>();
        for (var i = 0; i < count; i++)
        {
            var package = packages[i % packages.Count];
            var id = _guidGenerator.Create();
            var orderNum = "ORD-2026-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            var unitPrice = package.DiscountedPrice ?? package.Price;
            var order = new Order(id, orderNum, studentId, trainerProfileId, package.Id, unitPrice, 1);
            await _orderRepository.InsertAsync(order);
            list.Add(order);
        }
        _logger.LogInformation("Siparişler seed edildi: {Count} adet.", list.Count);
        return list;
    }

    private async Task SeedSessionsAsync(List<Order> orders, Guid trainerProfileId, Guid studentId)
    {
        for (var i = 0; i < orders.Count; i++)
        {
            var start = DateTime.Now.AddDays(-(orders.Count - i) * 7).Date.AddHours(10);
            var end = start.AddHours(1);
            var s = new Session(_guidGenerator.Create(), orders[i].Id, trainerProfileId, studentId, start, end, 1)
            {
                MeetingUrl = $"https://meet.example.com/seed-session-{i + 1}",
                TrainerNotes = "Isınma + antrenman + soğuma.",
                Status = SessionStatus.Completed,
                ActualStartTime = start,
                ActualEndTime = end
            };
            await _sessionRepository.InsertAsync(s);
        }
        _logger.LogInformation("Seanslar seed edildi: {Count} adet.", orders.Count);
    }

    private async Task<List<Payment>> SeedPaymentsAsync(List<Order> orders)
    {
        var list = new List<Payment>();
        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var amount = order.NetAmount;
            var p = new Payment(_guidGenerator.Create(), order.Id, PaymentProviderEnum.Iyzico, $"TXN-SEED-{i + 1:D3}", amount, "TRY");
            p.MarkCompleted();
            await _paymentRepository.InsertAsync(p);
            list.Add(p);
        }
        _logger.LogInformation("Ödemeler seed edildi: {Count} adet.", list.Count);
        return list;
    }

    private async Task SeedReviewsAsync(List<Order> orders, Guid studentId, Guid trainerProfileId, List<ServicePackage> packages)
    {
        var comments = new[]
        {
            "Çok verimli ve motive edici bir seans oldu. Kesinlikle tavsiye ederim.",
            "Profesyonel yaklaşım, hedeflerime ulaşmama yardımcı oldu.",
            "İletişim ve program takibi mükemmeldi.",
            "Fiyat/performans açısından çok memnunum.",
            "Her seans sonrası kendimi daha iyi hissediyorum."
        };
        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var r = new Review(_guidGenerator.Create(), order.Id, studentId, trainerProfileId, 5)
            {
                ServicePackageId = order.ServicePackageId,
                CommunicationRating = 5,
                ExpertiseRating = 5,
                ValueForMoneyRating = 4 + (i % 2),
                PunctualityRating = 5,
                Comment = comments[i % comments.Length],
                IsVerifiedPurchase = true,
                IsPublished = true
            };
            r.SetTrainerReply("Teşekkür ederim, birlikte çalışmaya devam edelim!");
            await _reviewRepository.InsertAsync(r);
        }
        _logger.LogInformation("Değerlendirmeler seed edildi: {Count} adet.", orders.Count);
    }

    private async Task SeedConversationsAndMessagesAsync(Guid studentId, Guid trainerId, int conversationCount = 5)
    {
        var subjects = new[] { "4 seanslık paket", "online seans", "fiyat bilgisi", "randevu", "beslenme danışmanlığı" };
        for (var c = 0; c < conversationCount; c++)
        {
            var conv = new Conversation(_guidGenerator.Create(), studentId, trainerId)
            {
                LastMessageAt = DateTime.Now.AddMinutes(-(conversationCount - c) * 10)
            };
            await _conversationRepository.InsertAsync(conv);
            await _messageRepository.InsertAsync(new Message(_guidGenerator.Create(), conv.Id, studentId, $"Merhaba, {subjects[c]} hakkında bilgi alabilir miyim?") { IsRead = true, ReadAt = DateTime.Now.AddMinutes(-(conversationCount - c) * 10) });
            await _messageRepository.InsertAsync(new Message(_guidGenerator.Create(), conv.Id, trainerId, "Merhaba! Tabii ki, size detayları yazayım."));
        }
        _logger.LogInformation("Konuşmalar ve mesajlar seed edildi: {Count} konuşma, en az 10 mesaj.", conversationCount);
    }

    private async Task SeedNotificationsAsync(Guid studentId, Guid trainerId, int count = 5)
    {
        var notifications = new[]
        {
            (studentId, NotificationType.OrderCompleted, "Siparişiniz tamamlandı", "Paket siparişiniz başarıyla tamamlandı.", "/student/orders", false),
            (trainerId, NotificationType.PaymentReceived, "Ödeme alındı", "Öğrenci ödemesi cüzdanınıza aktarıldı.", null, true),
            (studentId, NotificationType.SessionReminder, "Yarın seansınız var", "Yarın saat 10:00'da seansınız planlandı.", "/student/sessions", false),
            (trainerId, NotificationType.NewReview, "Yeni değerlendirme", "Öğrenciniz bir değerlendirme bıraktı.", "/trainer/reviews", false),
            (studentId, NotificationType.System, "Hoş geldiniz", "Fitliyo'ya hoş geldiniz. İyi antrenmanlar!", null, true)
        };
        for (var i = 0; i < Math.Min(count, notifications.Length); i++)
        {
            var (userId, type, title, body, url, isRead) = notifications[i];
            var n = new Notification(_guidGenerator.Create(), userId, type, NotificationChannel.InApp, title) { Body = body, ActionUrl = url, IsRead = isRead };
            if (isRead) n.ReadAt = DateTime.Now.AddHours(-i - 1);
            await _notificationRepository.InsertAsync(n);
        }
        _logger.LogInformation("Bildirimler seed edildi: {Count} adet.", Math.Min(count, notifications.Length));
    }

    private async Task SeedSupportTicketsAsync(Guid userId, int count = 5)
    {
        var tickets = new[]
        {
            ("Ödeme iade talebi", "Son siparişim iptal oldu, iade süreci hakkında bilgi almak istiyorum.", SupportTicketCategory.Payment),
            ("Hesap ayarları", "E-posta adresimi nasıl güncelleyebilirim?", SupportTicketCategory.Account),
            ("Teknik sorun", "Video görüşme bağlantısı açılmıyor.", SupportTicketCategory.Technical),
            ("Abonelik iptali", "Aylık aboneliğimi iptal etmek istiyorum.", SupportTicketCategory.Payment),
            ("Öneri", "Uygulamaya yeni özellik önerim var.", SupportTicketCategory.General)
        };
        for (var i = 0; i < Math.Min(count, tickets.Length); i++)
        {
            var (subject, body, category) = tickets[i];
            var t = new SupportTicket(_guidGenerator.Create(), subject, body, category, userId, null);
            await _supportTicketRepository.InsertAsync(t);
        }
        _logger.LogInformation("Destek talepleri seed edildi: {Count} adet.", Math.Min(count, tickets.Length));
    }

    private async Task SeedFeaturedListingsAsync(Guid trainerProfileId, List<ServicePackage> packages, int count = 5)
    {
        var pos = 1;
        var f1 = new FeaturedListing(_guidGenerator.Create(), FeaturedListingPageType.Homepage, pos++, trainerProfileId, null) { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddMonths(1).Date, AdminNote = "Ana sayfa eğitmen" };
        await _featuredListingRepository.InsertAsync(f1);
        for (var i = 0; i < Math.Min(count - 1, packages.Count); i++)
        {
            var f = new FeaturedListing(_guidGenerator.Create(), FeaturedListingPageType.Homepage, pos++, null, packages[i].Id) { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddMonths(1).Date, AdminNote = "Öne çıkan paket " + (i + 1) };
            await _featuredListingRepository.InsertAsync(f);
        }
        if (pos <= count)
        {
            var f2 = new FeaturedListing(_guidGenerator.Create(), FeaturedListingPageType.Category, pos, trainerProfileId, null) { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddMonths(1).Date, AdminNote = "Kategori öne çıkan" };
            await _featuredListingRepository.InsertAsync(f2);
        }
        _logger.LogInformation("Öne çıkan listeler seed edildi: {Count} adet.", count);
    }

    private async Task SeedBlogPostsAsync(int count = 5)
    {
        if (await _blogPostRepository.AnyAsync()) return;
        var posts = new[]
        {
            ("Yeni Başlayanlar İçin Fitness Rehberi", "yeni-baslayanlar-fitness-rehberi", "Fitness yolculuğuna adım atmak isteyenler için temel bilgiler.", "İlk kez spor salonuna gideceklere öneriler."),
            ("Evde Yapılabilecek 10 Egzersiz", "evde-10-egzersiz", "Salona gidemediğiniz günler için evde uygulayabileceğiniz egzersizler.", "Ekipman gerektirmeyen ev antrenmanları."),
            ("Doğru Beslenme ile Performans Artışı", "beslenme-performans", "Antrenman öncesi ve sonrası beslenme ipuçları.", "Sporcu beslenmesi temelleri."),
            ("Pilates ile Duruş Düzeltme", "pilates-durus", "Günlük hayatta sık görülen duruş bozuklukları ve pilates çözümleri.", "Pilates ve postür."),
            ("Motivasyonu Yüksek Tutmanın 5 Yolu", "motivasyon-5-yol", "Uzun vadede antrenman motivasyonunu korumak için öneriler.", "Psikolojik dayanıklılık.")
        };
        for (var i = 0; i < Math.Min(count, posts.Length); i++)
        {
            var (title, slug, content, summary) = posts[i];
            var b = new BlogPost(_guidGenerator.Create(), title, slug, content) { Summary = summary, Status = BlogPostStatus.Published, PublishedAt = DateTime.Now.AddDays(-(count - i)), AuthorName = "Fitliyo Editör" };
            await _blogPostRepository.InsertAsync(b);
        }
        _logger.LogInformation("Blog yazıları seed edildi: {Count} adet.", Math.Min(count, posts.Length));
    }

    private async Task SeedUserProfilesAsync(Guid trainerUserId, Guid studentUserId)
    {
        if (await _userProfileRepository.AnyAsync(x => x.UserId == trainerUserId)) return;
        var trainerProfile = new UserProfile(_guidGenerator.Create(), trainerUserId)
        {
            BirthDate = new DateTime(1988, 5, 15),
            Gender = Gender.Male,
            HeightCm = 178m,
            WeightKg = 82m,
            BloodType = "A+",
            ActivityLevel = ActivityLevel.VeryActive,
            FitnessGoal = FitnessGoal.Maintain,
            Phone = "+90 532 111 2233",
            EmergencyContact = "Ayşe Yılmaz - +90 533 444 5566",
            SleepHoursPerNight = 7,
            Smoking = false,
            RestingHeartRate = 58,
            Notes = "Eğitmen olarak kendi formumu korumak öncelikli."
        };
        var studentProfile = new UserProfile(_guidGenerator.Create(), studentUserId)
        {
            BirthDate = new DateTime(1995, 10, 8),
            Gender = Gender.Female,
            HeightCm = 165m,
            WeightKg = 68m,
            BloodType = "B+",
            ActivityLevel = ActivityLevel.Light,
            FitnessGoal = FitnessGoal.LoseWeight,
            TargetWeightKg = 62m,
            Phone = "+90 555 777 8899",
            EmergencyContact = "Mehmet Demir",
            SleepHoursPerNight = 6,
            Smoking = false,
            AlcoholConsumption = "Nadiren",
            WaistCm = 78m,
            HipCm = 98m,
            NeckCm = 32m,
            Notes = "Kilo vermek ve düzenli egzersiz alışkanlığı kazanmak istiyorum."
        };
        await _userProfileRepository.InsertAsync(trainerProfile);
        await _userProfileRepository.InsertAsync(studentProfile);
        _logger.LogInformation("Kullanıcı profilleri (sağlık) seed edildi.");
    }

    private async Task SeedWithdrawalRequestsAsync(Guid trainerWalletId, int count = 5)
    {
        var amounts = new[] { 500m, 300m, 750m, 400m, 600m };
        var ibans = new[] { "TR330006100519786457841326", "TR330006100519786457841327", "TR330006100519786457841328", "TR330006100519786457841329", "TR330006100519786457841330" };
        for (var i = 0; i < count; i++)
        {
            var wr = new WithdrawalRequest(_guidGenerator.Create(), trainerWalletId, amounts[i], ibans[i], "Ahmet Yılmaz");
            await _withdrawalRequestRepository.InsertAsync(wr);
        }
        _logger.LogInformation("Para çekme talepleri seed edildi: {Count} adet.", count);
    }

    private async Task SeedDisputesAsync(List<Order> orders, Guid openedByUserId, Guid resolvedByUserId, int count = 5)
    {
        var reasons = new[] { "Seans iptal edildi, iade talep ediyorum.", "Hizmet beklentiyi karşılamadı.", "Yanlış paket satıldı.", "İletişim sorunu yaşandı.", "Diğer neden." };
        for (var i = 0; i < Math.Min(count, orders.Count); i++)
        {
            var d = new Dispute(_guidGenerator.Create(), orders[i].Id, openedByUserId, DisputeType.Refund, reasons[i]);
            d.Resolve("İnceleme tamamlandı, iade yapıldı.", resolvedByUserId);
            await _disputeRepository.InsertAsync(d);
        }
        _logger.LogInformation("Uyuşmazlık kayıtları seed edildi: {Count} adet.", Math.Min(count, orders.Count));
    }
}

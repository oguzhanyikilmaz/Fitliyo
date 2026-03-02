using System;
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

        var catFitness = await SeedCategoryAsync("Fitness & Kondisyon", "fitness-kondisyon", null, 1);
        var catSpor = await SeedCategoryAsync("Spor Koçluğu", "spor-koclugu", null, 2);
        var catBeslenme = await SeedCategoryAsync("Beslenme", "beslenme", null, 3);
        var catPilates = await SeedCategoryAsync("Pilates", "pilates", catFitness.Id, 10);
        var catYoga = await SeedCategoryAsync("Yoga", "yoga", catFitness.Id, 11);

        var trainerProfile = await SeedTrainerProfileAsync(trainerUser.Id);
        await SeedTrainerCertificateAsync(trainerProfile.Id);
        await SeedTrainerGalleryAsync(trainerProfile.Id);
        // Kategoriler ve eğitmen DB'ye yazılsın; mapping FK'sı ihlal olmasın
        await _dbContext.SaveChangesAsync();
        // Mapping'leri raw SQL ile ekle (EF aynı UoW içinde insert sırası nedeniyle FK ihlali verebiliyor)
        await SeedTrainerCategoryMappingsAsync(trainerProfile.Id, new[] { catFitness.Id, catSpor.Id });

        await SeedSubscriptionPlansAsync();
        var wallet = await SeedTrainerWalletAsync(trainerProfile.Id);
        var package = await SeedServicePackageAsync(trainerProfile.Id);

        var order = await SeedOrderAsync(studentUser.Id, trainerProfile.Id, package.Id);
        await SeedSessionAsync(order.Id, trainerProfile.Id, studentUser.Id);
        var payment = await SeedPaymentAsync(order.Id);
        order.MarkAsPaid("Iyzico", "TXN-SEED-001");
        order.Complete();
        await _orderRepository.UpdateAsync(order);
        await SeedReviewAsync(order.Id, studentUser.Id, trainerProfile.Id, package.Id);

        await SeedConversationAndMessagesAsync(studentUser.Id, trainerUser.Id);
        await SeedNotificationsAsync(studentUser.Id, trainerUser.Id);
        await SeedSupportTicketAsync(studentUser.Id);
        await SeedFeaturedListingAsync(trainerProfile.Id, package.Id);
        await SeedBlogPostsAsync();
        await SeedUserProfilesAsync(trainerUser.Id, studentUser.Id);
        await SeedWithdrawalRequestAsync(wallet.Id);
        if (admin != null)
            await SeedDisputeAsync(order.Id, studentUser.Id, admin.Id);
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

    private async Task SeedTrainerCertificateAsync(Guid trainerProfileId)
    {
        var c1 = new TrainerCertificate(_guidGenerator.Create(), trainerProfileId, "NASM Certified Personal Trainer")
        {
            IssuingOrganization = "National Academy of Sports Medicine",
            IssueDate = new DateTime(2018, 6, 1),
            ExpiryDate = new DateTime(2026, 6, 1),
            IsVerifiedByPlatform = true
        };
        var c2 = new TrainerCertificate(_guidGenerator.Create(), trainerProfileId, "İlk Yardım Sertifikası")
        {
            IssuingOrganization = "Türk Kızılay",
            IssueDate = new DateTime(2023, 1, 15),
            IsVerifiedByPlatform = true
        };
        await _dbContext.TrainerCertificates.AddAsync(c1);
        await _dbContext.TrainerCertificates.AddAsync(c2);
        _logger.LogInformation("Eğitmen sertifikaları seed edildi.");
    }

    private async Task SeedTrainerGalleryAsync(Guid trainerProfileId)
    {
        var g1 = new TrainerGallery(_guidGenerator.Create(), trainerProfileId, MediaType.Image, "https://example.com/trainer-cover.jpg")
        {
            Caption = "Antrenman stüdyosu",
            SortOrder = 0,
            IsCoverImage = true
        };
        var g2 = new TrainerGallery(_guidGenerator.Create(), trainerProfileId, MediaType.Image, "https://example.com/trainer-2.jpg")
        {
            Caption = "Bire bir seans",
            SortOrder = 1
        };
        await _dbContext.TrainerGalleries.AddAsync(g1);
        await _dbContext.TrainerGalleries.AddAsync(g2);
        _logger.LogInformation("Eğitmen galerisi seed edildi.");
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
            await _subscriptionPlanRepository.InsertAsync(free);
            await _subscriptionPlanRepository.InsertAsync(basic);
            await _subscriptionPlanRepository.InsertAsync(pro);
            _logger.LogInformation("Abonelik planları seed edildi.");
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

    private async Task<ServicePackage> SeedServicePackageAsync(Guid trainerProfileId)
    {
        var id = _guidGenerator.Create();
        var p = new ServicePackage(id, trainerProfileId, "4 Seans Kişisel Antrenman Paketi", PackageType.Training, 1200m)
        {
            Description = "Ayda 4 seans, 60 dakikalık bire bir antrenman. Hedef belirleme ve ilerleme takibi dahil.",
            DiscountedPrice = 999m,
            DurationDays = 30,
            SessionCount = 4,
            SessionDurationMinutes = 60,
            MaxStudents = 1,
            IsOnline = true,
            IsOnSite = true,
            CancellationHours = 24,
            CancellationPolicy = "Seans en az 24 saat önceden iptal edilmelidir.",
            WhatIsIncluded = "Kişisel antrenman, beslenme önerileri, WhatsApp desteği",
            Tags = "fitness,personal-training,kondisyon",
            TotalSalesCount = 12,
            AverageRating = 4.8m,
            IsFeatured = true
        };
        await _servicePackageRepository.InsertAsync(p);
        _logger.LogInformation("Hizmet paketi seed: {Title}", p.Title);
        return p;
    }

    private async Task<Order> SeedOrderAsync(Guid studentId, Guid trainerProfileId, Guid servicePackageId)
    {
        var id = _guidGenerator.Create();
        var orderNum = "ORD-2026-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        var unitPrice = 999m;
        var order = new Order(id, orderNum, studentId, trainerProfileId, servicePackageId, unitPrice, 1);
        await _orderRepository.InsertAsync(order);
        _logger.LogInformation("Sipariş seed: {OrderNumber}", order.OrderNumber);
        return order;
    }

    private async Task SeedSessionAsync(Guid orderId, Guid trainerProfileId, Guid studentId)
    {
        var start = DateTime.Now.AddDays(2).Date.AddHours(10);
        var end = start.AddHours(1);
        var s = new Session(_guidGenerator.Create(), orderId, trainerProfileId, studentId, start, end, 1)
        {
            MeetingUrl = "https://meet.example.com/seed-session-1",
            TrainerNotes = "Isınma + kuvvet + soğuma planlandı.",
            Status = SessionStatus.Completed,
            ActualStartTime = start,
            ActualEndTime = end
        };
        await _sessionRepository.InsertAsync(s);
        _logger.LogInformation("Seans seed edildi.");
    }

    private async Task<Payment> SeedPaymentAsync(Guid orderId)
    {
        var p = new Payment(_guidGenerator.Create(), orderId, PaymentProviderEnum.Iyzico, "TXN-SEED-001", 798m, "TRY");
        p.MarkCompleted();
        await _paymentRepository.InsertAsync(p);
        _logger.LogInformation("Ödeme kaydı seed edildi.");
        return p;
    }

    private async Task SeedReviewAsync(Guid orderId, Guid studentId, Guid trainerProfileId, Guid? servicePackageId)
    {
        var r = new Review(_guidGenerator.Create(), orderId, studentId, trainerProfileId, 5)
        {
            ServicePackageId = servicePackageId,
            CommunicationRating = 5,
            ExpertiseRating = 5,
            ValueForMoneyRating = 5,
            PunctualityRating = 5,
            Comment = "Çok verimli ve motive edici bir seans oldu. Kesinlikle tavsiye ederim.",
            IsVerifiedPurchase = true,
            IsPublished = true
        };
        r.SetTrainerReply("Teşekkür ederim, birlikte çalışmaya devam edelim!");
        await _reviewRepository.InsertAsync(r);
        _logger.LogInformation("Değerlendirme seed edildi.");
    }

    private async Task SeedConversationAndMessagesAsync(Guid studentId, Guid trainerId)
    {
        var conv = new Conversation(_guidGenerator.Create(), studentId, trainerId)
        {
            LastMessageAt = DateTime.Now.AddMinutes(-5)
        };
        await _conversationRepository.InsertAsync(conv);
        var msg1 = new Message(_guidGenerator.Create(), conv.Id, studentId, "Merhaba, 4 seanslık paket hakkında bilgi alabilir miyim?")
        {
            IsRead = true,
            ReadAt = DateTime.Now.AddMinutes(-4)
        };
        var msg2 = new Message(_guidGenerator.Create(), conv.Id, trainerId, "Merhaba! Tabii, haftada bir seans şeklinde planlayabiliriz. Sorularınız varsa yazın.");
        await _messageRepository.InsertAsync(msg1);
        await _messageRepository.InsertAsync(msg2);
        _logger.LogInformation("Konuşma ve mesajlar seed edildi.");
    }

    private async Task SeedNotificationsAsync(Guid studentId, Guid trainerId)
    {
        var n1 = new Notification(_guidGenerator.Create(), studentId, NotificationType.OrderCompleted, NotificationChannel.InApp, "Siparişiniz tamamlandı")
        {
            Body = "4 Seans Kişisel Antrenman Paketi siparişiniz başarıyla tamamlandı.",
            ActionUrl = "/student/orders",
            IsRead = false
        };
        var n2 = new Notification(_guidGenerator.Create(), trainerId, NotificationType.PaymentReceived, NotificationChannel.InApp, "Ödeme alındı")
        {
            Body = "Öğrenci ödemesi cüzdanınıza aktarıldı.",
            IsRead = true,
            ReadAt = DateTime.Now.AddHours(-1)
        };
        await _notificationRepository.InsertAsync(n1);
        await _notificationRepository.InsertAsync(n2);
        _logger.LogInformation("Bildirimler seed edildi.");
    }

    private async Task SeedSupportTicketAsync(Guid userId)
    {
        var t = new SupportTicket(_guidGenerator.Create(), "Ödeme iade talebi", "Son siparişim iptal oldu, iade süreci hakkında bilgi almak istiyorum.", SupportTicketCategory.Payment, userId, null);
        await _supportTicketRepository.InsertAsync(t);
        _logger.LogInformation("Destek talebi seed edildi.");
    }

    private async Task SeedFeaturedListingAsync(Guid trainerProfileId, Guid servicePackageId)
    {
        var f1 = new FeaturedListing(_guidGenerator.Create(), FeaturedListingPageType.Homepage, 1, trainerProfileId, null)
        {
            StartDate = DateTime.Now.Date,
            EndDate = DateTime.Now.AddMonths(1).Date,
            AdminNote = "Ana sayfa öne çıkan eğitmen"
        };
        var f2 = new FeaturedListing(_guidGenerator.Create(), FeaturedListingPageType.Homepage, 2, null, servicePackageId)
        {
            StartDate = DateTime.Now.Date,
            EndDate = DateTime.Now.AddMonths(1).Date,
            AdminNote = "Öne çıkan paket"
        };
        await _featuredListingRepository.InsertAsync(f1);
        await _featuredListingRepository.InsertAsync(f2);
        _logger.LogInformation("Öne çıkan listeler seed edildi.");
    }

    private async Task SeedBlogPostsAsync()
    {
        if (await _blogPostRepository.AnyAsync()) return;
        var b1 = new BlogPost(_guidGenerator.Create(), "Yeni Başlayanlar İçin Fitness Rehberi", "yeni-baslayanlar-fitness-rehberi",
            "Fitness yolculuğuna adım atmak isteyenler için temel bilgiler: doğru antrenman sıklığı, beslenme ipuçları ve motivasyon.")
        {
            Summary = "İlk kez spor salonuna gideceklere öneriler.",
            Status = BlogPostStatus.Published,
            PublishedAt = DateTime.Now.AddDays(-5),
            AuthorName = "Fitliyo Editör"
        };
        var b2 = new BlogPost(_guidGenerator.Create(), "Evde Yapılabilecek 10 Egzersiz", "evde-10-egzersiz",
            "Salona gidemediğiniz günler için evde uygulayabileceğiniz etkili egzersizler ve set tekrar önerileri.")
        {
            Summary = "Ekipman gerektirmeyen ev antrenmanları.",
            Status = BlogPostStatus.Published,
            PublishedAt = DateTime.Now.AddDays(-2),
            AuthorName = "Fitliyo Editör"
        };
        await _blogPostRepository.InsertAsync(b1);
        await _blogPostRepository.InsertAsync(b2);
        _logger.LogInformation("Blog yazıları seed edildi.");
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

    private async Task SeedWithdrawalRequestAsync(Guid trainerWalletId)
    {
        var wr = new WithdrawalRequest(_guidGenerator.Create(), trainerWalletId, 500m, "TR330006100519786457841326", "Ahmet Yılmaz");
        await _withdrawalRequestRepository.InsertAsync(wr);
        _logger.LogInformation("Para çekme talebi seed edildi.");
    }

    private async Task SeedDisputeAsync(Guid orderId, Guid openedByUserId, Guid resolvedByUserId)
    {
        var d = new Dispute(_guidGenerator.Create(), orderId, openedByUserId, DisputeType.Refund, "Seans iptal edildi, iade talep ediyorum.");
        d.Resolve("Ödeme iade edildi.", resolvedByUserId);
        await _disputeRepository.InsertAsync(d);
        _logger.LogInformation("Uyuşmazlık kaydı seed edildi.");
    }
}

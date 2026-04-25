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
    private readonly IRepository<PackageAvailabilitySchedule, Guid> _packageAvailabilityScheduleRepository;
    private readonly IRepository<PackageUnavailableDate, Guid> _packageUnavailableDateRepository;
    private readonly IRepository<TrainerSubscription, Guid> _trainerSubscriptionRepository;
    private readonly IRepository<WalletTransaction, Guid> _walletTransactionRepository;
    private readonly IRepository<ReviewHelpfulVote, Guid> _reviewHelpfulVoteRepository;
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
        IRepository<PackageAvailabilitySchedule, Guid> packageAvailabilityScheduleRepository,
        IRepository<PackageUnavailableDate, Guid> packageUnavailableDateRepository,
        IRepository<TrainerSubscription, Guid> trainerSubscriptionRepository,
        IRepository<WalletTransaction, Guid> walletTransactionRepository,
        IRepository<ReviewHelpfulVote, Guid> reviewHelpfulVoteRepository,
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
        _packageAvailabilityScheduleRepository = packageAvailabilityScheduleRepository;
        _packageUnavailableDateRepository = packageUnavailableDateRepository;
        _trainerSubscriptionRepository = trainerSubscriptionRepository;
        _walletTransactionRepository = walletTransactionRepository;
        _reviewHelpfulVoteRepository = reviewHelpfulVoteRepository;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var admin = await _userRepository.FindByNormalizedUserNameAsync("ADMINFITLIYO");
        var trainerUsers = new List<IdentityUser>();
        var studentUsers = new List<IdentityUser>();
        foreach (var uname in new[] { "EGITMEN", "EGITMEN2", "EGITMEN3", "EGITMEN4", "EGITMEN5" })
        {
            var user = await _userRepository.FindByNormalizedUserNameAsync(uname);
            if (user != null) trainerUsers.Add(user);
        }
        foreach (var uname in new[] { "OGRENCI", "OGRENCI2", "OGRENCI3", "OGRENCI4", "OGRENCI5" })
        {
            var user = await _userRepository.FindByNormalizedUserNameAsync(uname);
            if (user != null) studentUsers.Add(user);
        }

        if (trainerUsers.Count == 0 || studentUsers.Count == 0)
        {
            _logger.LogWarning("Marketplace seed atlandı: Önce DbMigrator ile admin/egitmen/ogrenci kullanıcılarını oluşturun.");
            return;
        }

        if (await _trainerProfileRepository.AnyAsync(x => x.UserId == trainerUsers[0].Id))
        {
            _logger.LogInformation("Marketplace seed artımlı çalışacak: mevcut veriler korunup eksik domain kayıtları tamamlanacak.");
        }

        _logger.LogInformation("Marketplace test verileri ekleniyor...");

        // Kategoriler (en az 5)
        var catFitness = await SeedCategoryAsync("Fitness & Kondisyon", "fitness-kondisyon", null, 1);
        var catSpor = await SeedCategoryAsync("Spor Koçluğu", "spor-koclugu", null, 2);
        var catBeslenme = await SeedCategoryAsync("Beslenme", "beslenme", null, 3);
        var catPilates = await SeedCategoryAsync("Pilates", "pilates", catFitness.Id, 10);
        var catYoga = await SeedCategoryAsync("Yoga", "yoga", catFitness.Id, 11);
        var categoryIds = new[] { catFitness.Id, catSpor.Id, catBeslenme.Id, catPilates.Id, catYoga.Id };

        var trainerProfiles = new List<TrainerProfile>();
        var slugs = new[]
        {
            "ahmet-yilmaz-personal-trainer",
            "elif-kaya-diyetisyen",
            "mert-arslan-fitness-coach",
            "zeynep-aksoy-pilates-coach",
            "can-demir-yoga-instructor"
        };
        for (var i = 0; i < Math.Min(5, trainerUsers.Count); i++)
        {
            var existingProfile = await _trainerProfileRepository.FirstOrDefaultAsync(x => x.UserId == trainerUsers[i].Id);
            var profile = existingProfile ?? await SeedTrainerProfileAsync(trainerUsers[i].Id, slugs[i], i);
            trainerProfiles.Add(profile);
            if (!await _trainerCertificateRepository.AnyAsync(x => x.TrainerProfileId == profile.Id))
            {
                await SeedTrainerCertificatesAsync(profile.Id, 5);
            }
            if (!await _trainerGalleryRepository.AnyAsync(x => x.TrainerProfileId == profile.Id))
            {
                await SeedTrainerGalleriesAsync(profile.Id, 5);
            }
        }
        await _dbContext.SaveChangesAsync();
        foreach (var profile in trainerProfiles)
        {
            if (!await _dbContext.TrainerCategoryMappings.AnyAsync(x => x.TrainerProfileId == profile.Id))
            {
                await SeedTrainerCategoryMappingsAsync(profile.Id, categoryIds);
            }
        }

        var plans = await SeedSubscriptionPlansAsync();
        var wallets = new List<TrainerWallet>();
        var packages = new List<ServicePackage>();
        foreach (var profile in trainerProfiles)
        {
            wallets.Add(await SeedTrainerWalletAsync(profile.Id));
            packages.AddRange(await SeedServicePackagesAsync(profile.Id, 5));
        }

        var studentPool = studentUsers.Take(5).Select(x => x.Id).ToList();
        var orders = await SeedOrdersAsync(studentPool, trainerProfiles.Select(x => x.Id).ToList(), packages, 30);
        await SeedSessionsAsync(orders);
        var payments = await SeedPaymentsAsync(orders);
        for (var i = 0; i < orders.Count; i++)
        {
            orders[i].MarkAsPaid("Iyzico", $"TXN-SEED-{i + 1:D3}");
            orders[i].Complete();
            await _orderRepository.UpdateAsync(orders[i]);
        }
        await SeedReviewsAsync(orders, packages);
        await SeedReviewHelpfulVotesAsync(orders.Count);

        await SeedConversationsAndMessagesAsync(studentPool[0], trainerUsers[0].Id, 5);
        await SeedNotificationsAsync(studentPool[0], trainerUsers[0].Id, 5);
        await SeedSupportTicketsAsync(studentPool[0], 5);
        await SeedFeaturedListingsAsync(trainerProfiles[0].Id, packages, 6);
        await SeedBlogPostsAsync(6);
        await SeedUserProfilesAsync(trainerUsers.Select(x => x.Id).Take(5).ToList(), studentPool);
        foreach (var wallet in wallets)
        {
            await SeedWithdrawalRequestsAsync(wallet.Id, 6);
            await SeedWalletTransactionsAsync(wallet.Id, 6);
        }
        await SeedTrainerSubscriptionsAsync(trainerProfiles.Select(x => x.Id).ToList(), plans);
        await SeedPackageSchedulesAndUnavailabilitiesAsync(packages, 6);
        if (admin != null)
            await SeedDisputesAsync(orders, studentPool[0], admin.Id, 6);
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

    private async Task<TrainerProfile> SeedTrainerProfileAsync(Guid userId, string slug, int variant = 0)
    {
        var existing = await _dbContext.TrainerProfiles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserId == userId || x.Slug == slug);
        if (existing != null)
        {
            return existing;
        }

        var id = _guidGenerator.Create();
        var trainerTypes = new[]
        {
            TrainerType.PersonalTrainer,
            TrainerType.Dietitian,
            TrainerType.BasketballCoach,
            TrainerType.YogaInstructor,
            TrainerType.SwimmingCoach
        };
        var cities = new[] { "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya" };
        var p = new TrainerProfile(id, userId, slug, trainerTypes[variant % trainerTypes.Length])
        {
            Bio = "Spor bilimleri altyapısıyla bireysel hedeflere uygun programlar sunan profesyonel eğitmen.",
            ExperienceYears = 6 + variant,
            City = cities[variant % cities.Length],
            District = "Kadıköy",
            IsOnlineAvailable = true,
            IsOnSiteAvailable = true,
            AverageRating = 4.4m + (variant * 0.1m),
            TotalReviewCount = 10 + (variant * 3),
            TotalStudentCount = 20 + (variant * 5),
            IsVerified = true,
            VerificationBadge = "Doğrulanmış Eğitmen",
            ProfileCompletionPct = 85 + variant,
            InstagramUrl = $"https://instagram.com/{slug}",
            WebsiteUrl = $"https://{slug}.fit"
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

    private async Task<List<SubscriptionPlan>> SeedSubscriptionPlansAsync()
    {
        var existingPlans = await _subscriptionPlanRepository.GetListAsync();
        if (existingPlans.Count >= 5)
        {
            return existingPlans;
        }

        try
        {
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
            return new List<SubscriptionPlan> { free, basic, pro, starter, enterprise };
        }
        catch (PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogWarning("AppSubscriptionPlans tablosu bulunamadı, abonelik planları seed atlanıyor. Eksik migration ekleyip DbMigrator tekrar çalıştırın.");
            return new List<SubscriptionPlan>();
        }
    }

    private async Task<TrainerWallet> SeedTrainerWalletAsync(Guid trainerProfileId)
    {
        var existing = await _dbContext.TrainerWallets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TrainerProfileId == trainerProfileId);
        if (existing != null)
        {
            return existing;
        }

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

    private async Task<List<Order>> SeedOrdersAsync(List<Guid> studentIds, List<Guid> trainerProfileIds, List<ServicePackage> packages, int count = 30)
    {
        var list = new List<Order>();
        for (var i = 0; i < count; i++)
        {
            var package = packages[i % packages.Count];
            var studentId = studentIds[i % studentIds.Count];
            var trainerProfileId = trainerProfileIds[i % trainerProfileIds.Count];
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

    private async Task SeedSessionsAsync(List<Order> orders)
    {
        for (var i = 0; i < orders.Count; i++)
        {
            var start = DateTime.Now.AddDays(-(orders.Count - i) * 7).Date.AddHours(10);
            var end = start.AddHours(1);
            var s = new Session(_guidGenerator.Create(), orders[i].Id, orders[i].TrainerProfileId, orders[i].StudentId, start, end, 1)
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

    private async Task SeedReviewsAsync(List<Order> orders, List<ServicePackage> packages)
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
            var r = new Review(_guidGenerator.Create(), order.Id, order.StudentId, order.TrainerProfileId, 5)
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

    private async Task SeedUserProfilesAsync(List<Guid> trainerUserIds, List<Guid> studentUserIds)
    {
        foreach (var trainerUserId in trainerUserIds)
        {
            if (await _userProfileRepository.AnyAsync(x => x.UserId == trainerUserId)) continue;
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
                RestingHeartRate = 58
            };
            await _userProfileRepository.InsertAsync(trainerProfile);
        }
        foreach (var studentUserId in studentUserIds)
        {
            if (await _userProfileRepository.AnyAsync(x => x.UserId == studentUserId)) continue;
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
                NeckCm = 32m
            };
            await _userProfileRepository.InsertAsync(studentProfile);
        }
        _logger.LogInformation("Kullanıcı profilleri (sağlık) seed edildi: Eğitmen {TrainerCount}, Öğrenci {StudentCount}.", trainerUserIds.Count, studentUserIds.Count);
    }

    private async Task SeedWithdrawalRequestsAsync(Guid trainerWalletId, int count = 5)
    {
        if (await _withdrawalRequestRepository.AnyAsync(x => x.TrainerWalletId == trainerWalletId))
        {
            return;
        }

        var amounts = new[] { 500m, 300m, 750m, 400m, 600m, 450m, 520m, 680m };
        var generatedCount = Math.Min(count, amounts.Length);

        for (var i = 0; i < generatedCount; i++)
        {
            var iban = $"TR33000610051978645784{(1326 + i).ToString().PadLeft(4, '0')}";
            var wr = new WithdrawalRequest(_guidGenerator.Create(), trainerWalletId, amounts[i], iban, "Ahmet Yılmaz");
            await _withdrawalRequestRepository.InsertAsync(wr);
        }
        _logger.LogInformation("Para çekme talepleri seed edildi: {Count} adet.", generatedCount);
    }

    private async Task SeedDisputesAsync(List<Order> orders, Guid openedByUserId, Guid resolvedByUserId, int count = 5)
    {
        var reasons = new[] { "Seans iptal edildi, iade talep ediyorum.", "Hizmet beklentiyi karşılamadı.", "Yanlış paket satıldı.", "İletişim sorunu yaşandı.", "Diğer neden.", "Planlanan saat dışında hizmet verildi." };
        for (var i = 0; i < Math.Min(count, orders.Count); i++)
        {
            var d = new Dispute(_guidGenerator.Create(), orders[i].Id, openedByUserId, DisputeType.Refund, reasons[i]);
            d.Resolve("İnceleme tamamlandı, iade yapıldı.", resolvedByUserId);
            await _disputeRepository.InsertAsync(d);
        }
        _logger.LogInformation("Uyuşmazlık kayıtları seed edildi: {Count} adet.", Math.Min(count, orders.Count));
    }

    private async Task SeedTrainerSubscriptionsAsync(List<Guid> trainerProfileIds, List<SubscriptionPlan> plans)
    {
        if (plans.Count == 0) return;

        var statuses = new[]
        {
            SubscriptionStatus.Active,
            SubscriptionStatus.PastDue,
            SubscriptionStatus.Cancelled,
            SubscriptionStatus.Expired,
            SubscriptionStatus.Trial
        };

        for (var i = 0; i < Math.Min(5, trainerProfileIds.Count); i++)
        {
            var trainerProfileId = trainerProfileIds[i];
            if (await _trainerSubscriptionRepository.AnyAsync(x => x.TrainerProfileId == trainerProfileId))
            {
                continue;
            }

            var plan = plans[i % plans.Count];
            var startDate = DateTime.Now.AddMonths(-(i + 1));
            var endDate = DateTime.Now.AddMonths(1 + i);
            var sub = new TrainerSubscription(_guidGenerator.Create(), trainerProfileId, plan.Id, startDate, endDate, plan.Price)
            {
                Status = statuses[i % statuses.Length],
                PaymentReference = $"SUB-SEED-{i + 1:D3}",
                IsAutoRenew = i % 2 == 0
            };
            if (sub.Status == SubscriptionStatus.Cancelled)
            {
                sub.CancelledAt = DateTime.Now.AddDays(-5);
            }
            await _trainerSubscriptionRepository.InsertAsync(sub);
        }
        _logger.LogInformation("TrainerSubscription seed tamamlandı.");
    }

    private async Task SeedWalletTransactionsAsync(Guid trainerWalletId, int count = 6)
    {
        if (await _walletTransactionRepository.AnyAsync(x => x.TrainerWalletId == trainerWalletId))
        {
            return;
        }

        var template = new (WalletTransactionType Type, decimal Amount, string Description)[]
        {
            (WalletTransactionType.Credit, 750m, "Sipariş ödemesi"),
            (WalletTransactionType.Debit, 120m, "Komisyon kesintisi"),
            (WalletTransactionType.Credit, 450m, "Ek seans geliri"),
            (WalletTransactionType.Payout, 300m, "Banka hesabına aktarım"),
            (WalletTransactionType.Refund, 80m, "İade düzeltmesi"),
            (WalletTransactionType.Credit, 220m, "Yeni paket satışı")
        };

        decimal balance = 0;
        for (var i = 0; i < Math.Min(count, template.Length); i++)
        {
            var t = template[i];
            balance += t.Type == WalletTransactionType.Debit || t.Type == WalletTransactionType.Payout ? -t.Amount : t.Amount;
            var tx = new WalletTransaction(
                _guidGenerator.Create(),
                trainerWalletId,
                t.Type,
                t.Amount,
                t.Description,
                balance);
            await _walletTransactionRepository.InsertAsync(tx);
        }
        _logger.LogInformation("WalletTransaction seed edildi: {Count} adet.", Math.Min(count, template.Length));
    }

    private async Task SeedPackageSchedulesAndUnavailabilitiesAsync(List<ServicePackage> packages, int countPerDomain = 6)
    {
        var daySequence = new[]
        {
            DayOfWeekEnum.Monday,
            DayOfWeekEnum.Tuesday,
            DayOfWeekEnum.Wednesday,
            DayOfWeekEnum.Thursday,
            DayOfWeekEnum.Friday,
            DayOfWeekEnum.Saturday
        };

        var scheduleCreated = 0;
        var unavailableCreated = 0;

        for (var i = 0; i < Math.Min(countPerDomain, packages.Count); i++)
        {
            var package = packages[i];
            if (!await _packageAvailabilityScheduleRepository.AnyAsync(x => x.ServicePackageId == package.Id))
            {
                var s = new PackageAvailabilitySchedule(
                    _guidGenerator.Create(),
                    package.Id,
                    daySequence[i % daySequence.Length],
                    new TimeSpan(9 + (i % 3), 0, 0),
                    new TimeSpan(10 + (i % 3), 0, 0),
                    60);
                await _packageAvailabilityScheduleRepository.InsertAsync(s);
                scheduleCreated++;
            }

            if (!await _packageUnavailableDateRepository.AnyAsync(x => x.TrainerProfileId == package.TrainerProfileId && x.UnavailableDate.Date == DateTime.Now.Date.AddDays(i + 1)))
            {
                var u = new PackageUnavailableDate(_guidGenerator.Create(), package.TrainerProfileId, DateTime.Now.Date.AddDays(i + 1))
                {
                    Reason = i % 2 == 0 ? "Resmi tatil" : "Eğitmen izin günü"
                };
                await _packageUnavailableDateRepository.InsertAsync(u);
                unavailableCreated++;
            }
        }

        _logger.LogInformation("PackageAvailabilitySchedule seed: {ScheduleCount}, PackageUnavailableDate seed: {UnavailableCount}", scheduleCreated, unavailableCreated);
    }

    private async Task SeedReviewHelpfulVotesAsync(int targetCount = 6)
    {
        if (await _reviewHelpfulVoteRepository.AnyAsync())
        {
            return;
        }

        var reviews = await _reviewRepository.GetListAsync();
        var students = new List<IdentityUser>();
        foreach (var uname in new[] { "OGRENCI", "OGRENCI2", "OGRENCI3", "OGRENCI4", "OGRENCI5" })
        {
            var user = await _userRepository.FindByNormalizedUserNameAsync(uname);
            if (user != null) students.Add(user);
        }
        if (reviews.Count == 0 || students.Count == 0)
        {
            return;
        }

        var count = Math.Min(targetCount, reviews.Count);
        for (var i = 0; i < count; i++)
        {
            var review = reviews[i];
            var voter = students[i % students.Count];
            var vote = new ReviewHelpfulVote(_guidGenerator.Create(), review.Id, voter.Id, i % 2 == 0);
            await _reviewHelpfulVoteRepository.InsertAsync(vote);
        }
        _logger.LogInformation("ReviewHelpfulVote seed edildi: {Count} adet.", count);
    }
}

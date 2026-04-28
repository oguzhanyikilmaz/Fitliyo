using AutoMapper;
using Fitliyo.Admin;
using Fitliyo.Admin.Dtos;
using Fitliyo.Categories;
using Fitliyo.Categories.Dtos;
using Fitliyo.Content;
using Fitliyo.Content.Dtos;
using Fitliyo.Messaging;
using Fitliyo.Messaging.Dtos;
using Fitliyo.Notifications;
using Fitliyo.Notifications.Dtos;
using Fitliyo.Orders;
using Fitliyo.Orders.Dtos;
using Fitliyo.Payments;
using Fitliyo.Payments.Dtos;
using Fitliyo.ServicePackages;
using Fitliyo.ServicePackages.Dtos;
using Fitliyo.Reviews;
using Fitliyo.Reviews.Dtos;
using Fitliyo.Subscriptions;
using Fitliyo.Subscriptions.Dtos;
using Fitliyo.Support;
using Fitliyo.Support.Dtos;
using Fitliyo.Trainers;
using Fitliyo.Trainers.Dtos;
using Fitliyo.Profiles;
using Fitliyo.Profiles.Dtos;
using Fitliyo.Wellness;
using Fitliyo.Wellness.Dtos;

namespace Fitliyo;

public class FitliyoApplicationAutoMapperProfile : Profile
{
    public FitliyoApplicationAutoMapperProfile()
    {
        CreateMap<TrainerProfile, TrainerProfileDto>();
        CreateMap<TrainerCertificate, TrainerCertificateDto>();
        CreateMap<TrainerGallery, TrainerGalleryDto>();
        CreateMap<ServicePackage, ServicePackageDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<Session, SessionDto>();
        CreateMap<Review, ReviewDto>();
        CreateMap<Conversation, ConversationDto>();
        CreateMap<Message, MessageDto>();
        CreateMap<Notification, NotificationDto>();
        CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
        CreateMap<TrainerSubscription, TrainerSubscriptionDto>();
        CreateMap<Payment, PaymentDto>();
        CreateMap<TrainerWallet, TrainerWalletDto>();
        CreateMap<WalletTransaction, WalletTransactionDto>();
        CreateMap<WithdrawalRequest, WithdrawalRequestDto>();
        CreateMap<SupportTicket, SupportTicketDto>();
        CreateMap<FeaturedListing, FeaturedListingDto>();
        CreateMap<Dispute, DisputeDto>();
        CreateMap<BlogPost, BlogPostDto>();
        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<UserNotificationPreferences, UserNotificationPreferencesDto>();
        CreateMap<PersonalWorkoutProgram, PersonalWorkoutProgramDto>()
            .ForMember(d => d.Exercises, o => o.Ignore());
        CreateMap<PersonalWorkoutTemplateExercise, PersonalWorkoutTemplateExerciseDto>();
        CreateMap<UserWorkoutLog, UserWorkoutLogDto>().ForMember(d => d.Lines, o => o.Ignore());
        CreateMap<UserWorkoutLogLine, UserWorkoutLogLineDto>();
        CreateMap<PersonalNutritionPlan, PersonalNutritionPlanDto>();
        CreateMap<UserFoodItem, UserFoodItemDto>();
        CreateMap<UserDailyMealLog, UserDailyMealLogDto>()
            .ForMember(d => d.Entries, o => o.Ignore())
            .ForMember(d => d.TotalCalories, o => o.Ignore())
            .ForMember(d => d.TotalProteinG, o => o.Ignore())
            .ForMember(d => d.TotalCarbsG, o => o.Ignore())
            .ForMember(d => d.TotalFatG, o => o.Ignore());
        CreateMap<UserMealLogEntry, UserMealLogEntryDto>();
    }
}

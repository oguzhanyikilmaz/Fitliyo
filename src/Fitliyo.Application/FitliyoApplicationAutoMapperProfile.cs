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
    }
}

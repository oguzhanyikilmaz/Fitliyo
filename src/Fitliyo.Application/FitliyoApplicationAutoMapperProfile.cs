using AutoMapper;
using Fitliyo.Categories;
using Fitliyo.Categories.Dtos;
using Fitliyo.Messaging;
using Fitliyo.Messaging.Dtos;
using Fitliyo.Notifications;
using Fitliyo.Notifications.Dtos;
using Fitliyo.Orders;
using Fitliyo.Orders.Dtos;
using Fitliyo.Packages;
using Fitliyo.Packages.Dtos;
using Fitliyo.Reviews;
using Fitliyo.Reviews.Dtos;
using Fitliyo.Subscriptions;
using Fitliyo.Subscriptions.Dtos;
using Fitliyo.Trainers;
using Fitliyo.Trainers.Dtos;

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
    }
}

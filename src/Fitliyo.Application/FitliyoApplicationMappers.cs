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
using Riok.Mapperly.Abstractions;

namespace Fitliyo;

[Mapper]
public partial class FitliyoApplicationMappers
{
    public partial TrainerProfileDto TrainerProfileToDto(TrainerProfile source);
    public partial TrainerCertificateDto TrainerCertificateToDto(TrainerCertificate source);
    public partial TrainerGalleryDto TrainerGalleryToDto(TrainerGallery source);
    public partial ServicePackageDto ServicePackageToDto(ServicePackage source);
    public partial CategoryDto CategoryToDto(Category source);
    public partial OrderDto OrderToDto(Order source);
    public partial SessionDto SessionToDto(Session source);
    public partial ReviewDto ReviewToDto(Review source);
    public partial ConversationDto ConversationToDto(Conversation source);
    public partial MessageDto MessageToDto(Message source);
    public partial NotificationDto NotificationToDto(Notification source);
    public partial SubscriptionPlanDto SubscriptionPlanToDto(SubscriptionPlan source);
    public partial TrainerSubscriptionDto TrainerSubscriptionToDto(TrainerSubscription source);
}

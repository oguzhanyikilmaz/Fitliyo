using Fitliyo.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Fitliyo.Permissions;

public class FitliyoPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var fitliyoGroup = context.AddGroup(FitliyoPermissions.GroupName, L("Permission:Fitliyo"));

        var trainers = fitliyoGroup.AddPermission(FitliyoPermissions.Trainers.Default, L("Permission:Trainers"));
        trainers.AddChild(FitliyoPermissions.Trainers.Create, L("Permission:Trainers.Create"));
        trainers.AddChild(FitliyoPermissions.Trainers.Edit, L("Permission:Trainers.Edit"));
        trainers.AddChild(FitliyoPermissions.Trainers.Delete, L("Permission:Trainers.Delete"));
        trainers.AddChild(FitliyoPermissions.Trainers.Verify, L("Permission:Trainers.Verify"));

        var packages = fitliyoGroup.AddPermission(FitliyoPermissions.Packages.Default, L("Permission:Packages"));
        packages.AddChild(FitliyoPermissions.Packages.Create, L("Permission:Packages.Create"));
        packages.AddChild(FitliyoPermissions.Packages.Edit, L("Permission:Packages.Edit"));
        packages.AddChild(FitliyoPermissions.Packages.Delete, L("Permission:Packages.Delete"));

        var categories = fitliyoGroup.AddPermission(FitliyoPermissions.Categories.Default, L("Permission:Categories"));
        categories.AddChild(FitliyoPermissions.Categories.Create, L("Permission:Categories.Create"));
        categories.AddChild(FitliyoPermissions.Categories.Edit, L("Permission:Categories.Edit"));
        categories.AddChild(FitliyoPermissions.Categories.Delete, L("Permission:Categories.Delete"));

        var orders = fitliyoGroup.AddPermission(FitliyoPermissions.Orders.Default, L("Permission:Orders"));
        orders.AddChild(FitliyoPermissions.Orders.Cancel, L("Permission:Orders.Cancel"));

        var reviews = fitliyoGroup.AddPermission(FitliyoPermissions.Reviews.Default, L("Permission:Reviews"));
        reviews.AddChild(FitliyoPermissions.Reviews.Delete, L("Permission:Reviews.Delete"));

        fitliyoGroup.AddPermission(FitliyoPermissions.Messaging.Default, L("Permission:Messaging"));

        var subscriptions = fitliyoGroup.AddPermission(FitliyoPermissions.Subscriptions.Default, L("Permission:Subscriptions"));
        subscriptions.AddChild(FitliyoPermissions.Subscriptions.ManagePlans, L("Permission:Subscriptions.ManagePlans"));

        fitliyoGroup.AddPermission(FitliyoPermissions.Notifications.Default, L("Permission:Notifications"));

        var admin = fitliyoGroup.AddPermission(FitliyoPermissions.Admin.Dashboard, L("Permission:Admin.Dashboard"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Admin.UserManagement, L("Permission:Admin.UserManagement"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Admin.FeaturedListings, L("Permission:Admin.FeaturedListings"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Admin.Disputes, L("Permission:Admin.Disputes"));

        fitliyoGroup.AddPermission(FitliyoPermissions.Payments.Default, L("Permission:Payments"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Payments.View, L("Permission:Payments.View"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Wallet.Default, L("Permission:Wallet"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Withdrawal.Default, L("Permission:Withdrawal"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Withdrawal.Manage, L("Permission:Withdrawal.Manage"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Support.Default, L("Permission:Support"));
        fitliyoGroup.AddPermission(FitliyoPermissions.Support.Manage, L("Permission:Support.Manage"));
        var content = fitliyoGroup.AddPermission(FitliyoPermissions.Content.Default, L("Permission:Content"));
        content.AddChild(FitliyoPermissions.Content.Create, L("Permission:Content.Create"));
        content.AddChild(FitliyoPermissions.Content.Edit, L("Permission:Content.Edit"));
        content.AddChild(FitliyoPermissions.Content.Delete, L("Permission:Content.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FitliyoResource>(name);
    }
}

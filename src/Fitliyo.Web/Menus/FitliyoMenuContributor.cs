using System.Threading.Tasks;
using Fitliyo.Localization;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Fitliyo.Web.Menus;

public class FitliyoMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<FitliyoResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                FitliyoMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        context.Menu.Items.Insert(
            1,
            BuildMarketplaceMenu()
        );

        administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }

    private static ApplicationMenuItem BuildMarketplaceMenu()
    {
        var marketplace = new ApplicationMenuItem(
                FitliyoMenus.Marketplace,
                "Marketplace Yönetimi",
                icon: "fas fa-store-alt",
                order: 1
            );

        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceUsers, "Kullanıcılar", "~/Identity/Users", icon: "fas fa-users"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceOrders, "Siparişler", "~/Admin/Marketplace/orders", icon: "fas fa-shopping-cart"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceTrainers, "Eğitmen Profilleri", "~/Admin/Marketplace/trainers", icon: "fas fa-id-badge"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplacePackages, "Paketler", "~/Admin/Marketplace/packages", icon: "fas fa-box"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceCategories, "Kategoriler", "~/Admin/Marketplace/categories", icon: "fas fa-sitemap"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceSubscriptions, "Abonelik Planları", "~/Admin/Marketplace/subscriptions", icon: "fas fa-layer-group"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceReviews, "Yorumlar", "~/Admin/Marketplace/reviews", icon: "fas fa-star"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceNotifications, "Bildirimler", "~/Admin/Marketplace/notifications", icon: "fas fa-bell"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceSupport, "Destek Talepleri", "~/Admin/Marketplace/support", icon: "fas fa-headset"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceDisputes, "Uyuşmazlıklar", "~/Admin/Marketplace/disputes", icon: "fas fa-balance-scale"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceWithdrawals, "Para Çekme Talepleri", "~/Admin/Marketplace/withdrawals", icon: "fas fa-money-check-alt"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceFeatured, "Öne Çıkanlar", "~/Admin/Marketplace/featured", icon: "fas fa-fire"));
        marketplace.AddItem(new ApplicationMenuItem(FitliyoMenus.MarketplaceBlog, "Blog", "~/Admin/Marketplace/blog", icon: "fas fa-blog"));

        return marketplace;
    }
}

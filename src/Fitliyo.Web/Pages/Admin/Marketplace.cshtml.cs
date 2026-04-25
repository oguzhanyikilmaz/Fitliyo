namespace Fitliyo.Web.Pages.Admin;

public class MarketplaceModel : FitliyoPageModel
{
    public string CurrentModule { get; private set; } = string.Empty;
    public string PageTitle { get; private set; } = "Marketplace Yönetimi";

    public void OnGet(string? module = null)
    {
        CurrentModule = NormalizeModule(module);
        PageTitle = CurrentModule switch
        {
            "orders" => "Sipariş Yönetimi",
            "trainers" => "Eğitmen Profili Yönetimi",
            "packages" => "Paket Yönetimi",
            "categories" => "Kategori Yönetimi",
            "subscriptions" => "Abonelik Planı Yönetimi",
            "reviews" => "Yorum Yönetimi",
            "notifications" => "Bildirim Yönetimi",
            "support" => "Destek Talebi Yönetimi",
            "disputes" => "Uyuşmazlık Yönetimi",
            "withdrawals" => "Para Çekme Talebi Yönetimi",
            "featured" => "Öne Çıkan Yönetimi",
            "blog" => "Blog Yönetimi",
            _ => "Marketplace Yönetimi"
        };
    }

    private static string NormalizeModule(string? module)
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            return string.Empty;
        }

        var normalized = module.Trim().ToLowerInvariant();
        return normalized switch
        {
            "orders" or
            "trainers" or
            "packages" or
            "categories" or
            "subscriptions" or
            "reviews" or
            "notifications" or
            "support" or
            "disputes" or
            "withdrawals" or
            "featured" or
            "blog" => normalized,
            _ => string.Empty
        };
    }
}


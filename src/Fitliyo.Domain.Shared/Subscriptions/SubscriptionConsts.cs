namespace Fitliyo.Subscriptions;

public static class SubscriptionConsts
{
    public const int MaxPlanNameLength = 100;
    public const int MaxPlanDescriptionLength = 500;
    public const int MaxCurrencyLength = 3;
    public const int MaxPaymentReferenceLength = 128;
    public const int MaxFeaturesJsonLength = 2000;
    public const string DefaultCurrency = "TRY";

    public const int FreeMaxPackages = 3;
    public const int BasicMaxPackages = 10;
    public const int ProMaxPackages = -1;
    public const decimal BasicCommissionRate = 0.12m;
    public const decimal ProCommissionRate = 0.08m;
    public const decimal DefaultCommissionRate = 0.15m;
}

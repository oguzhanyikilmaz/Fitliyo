using Microsoft.Extensions.Localization;
using Fitliyo.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Fitliyo.Web;

[Dependency(ReplaceServices = true)]
public class FitliyoBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FitliyoResource> _localizer;

    public FitliyoBrandingProvider(IStringLocalizer<FitliyoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

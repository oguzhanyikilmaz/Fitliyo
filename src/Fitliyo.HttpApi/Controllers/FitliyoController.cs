using Fitliyo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Fitliyo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FitliyoController : AbpControllerBase
{
    protected FitliyoController()
    {
        LocalizationResource = typeof(FitliyoResource);
    }
}

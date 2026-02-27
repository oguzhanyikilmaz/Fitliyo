using Fitliyo.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Fitliyo.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class FitliyoPageModel : AbpPageModel
{
    protected FitliyoPageModel()
    {
        LocalizationResourceType = typeof(FitliyoResource);
    }
}

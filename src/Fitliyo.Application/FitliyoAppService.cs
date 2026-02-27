using System;
using System.Collections.Generic;
using System.Text;
using Fitliyo.Localization;
using Volo.Abp.Application.Services;

namespace Fitliyo;

/* Inherit your application services from this class.
 */
public abstract class FitliyoAppService : ApplicationService
{
    protected FitliyoAppService()
    {
        LocalizationResource = typeof(FitliyoResource);
    }
}

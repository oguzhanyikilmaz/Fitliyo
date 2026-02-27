using Volo.Abp.Settings;

namespace Fitliyo.Settings;

public class FitliyoSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FitliyoSettings.MySetting1));
    }
}

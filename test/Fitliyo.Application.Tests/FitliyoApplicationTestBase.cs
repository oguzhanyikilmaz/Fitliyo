using Volo.Abp.Modularity;

namespace Fitliyo;

public abstract class FitliyoApplicationTestBase<TStartupModule> : FitliyoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

using Volo.Abp.Modularity;

namespace Fitliyo;

/* Inherit from this class for your domain layer tests. */
public abstract class FitliyoDomainTestBase<TStartupModule> : FitliyoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

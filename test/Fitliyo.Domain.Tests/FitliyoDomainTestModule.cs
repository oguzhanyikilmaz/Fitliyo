using Volo.Abp.Modularity;

namespace Fitliyo;

[DependsOn(
    typeof(FitliyoDomainModule),
    typeof(FitliyoTestBaseModule)
)]
public class FitliyoDomainTestModule : AbpModule
{

}

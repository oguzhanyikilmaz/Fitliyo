using Volo.Abp.Modularity;

namespace Fitliyo;

[DependsOn(
    typeof(FitliyoApplicationModule),
    typeof(FitliyoDomainTestModule)
)]
public class FitliyoApplicationTestModule : AbpModule
{

}

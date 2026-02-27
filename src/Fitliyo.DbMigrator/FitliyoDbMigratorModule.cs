using Fitliyo.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Fitliyo.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FitliyoEntityFrameworkCoreModule),
    typeof(FitliyoApplicationContractsModule)
    )]
public class FitliyoDbMigratorModule : AbpModule
{
}

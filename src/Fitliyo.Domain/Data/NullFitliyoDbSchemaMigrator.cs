using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Fitliyo.Data;

/* This is used if database provider does't define
 * IFitliyoDbSchemaMigrator implementation.
 */
public class NullFitliyoDbSchemaMigrator : IFitliyoDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

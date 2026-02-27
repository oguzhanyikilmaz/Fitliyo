using System.Threading.Tasks;

namespace Fitliyo.Data;

public interface IFitliyoDbSchemaMigrator
{
    Task MigrateAsync();
}

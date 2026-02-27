using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fitliyo.Data;
using Volo.Abp.DependencyInjection;

namespace Fitliyo.EntityFrameworkCore;

public class EntityFrameworkCoreFitliyoDbSchemaMigrator
    : IFitliyoDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFitliyoDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the FitliyoDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FitliyoDbContext>()
            .Database
            .MigrateAsync();
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;

namespace Fitliyo.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // PostgreSQL timestamptz için: Npgsql 6+ yalnızca UTC kabul eder; ABP/Identity SecurityLog DateTime.Now (Local) kullandığı için legacy davranışı açıyoruz.
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<FitliyoWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();

            var genIndex = Array.FindIndex(
                args,
                a => string.Equals(a, "--generate-swagger", StringComparison.OrdinalIgnoreCase));
            if (genIndex >= 0)
            {
                if (genIndex + 1 >= args.Length)
                {
                    throw new InvalidOperationException(
                        "--generate-swagger sonrası çıktı dosya yolu gerekir (örn. docs/openapi/swagger.web.v1.full.json).");
                }

                // dotnet run CWD repo kökü veya src/Fitliyo.Web olabiliyor; docs/ yolunu depo köküne sabitle
                var env = app.Services.GetRequiredService<IWebHostEnvironment>();
                var repositoryRoot = Path.GetFullPath(
                    Path.Combine(env.ContentRootPath, "..", ".."));
                var rawOut = args[genIndex + 1];
                var outputPath = Path.IsPathRooted(rawOut)
                    ? rawOut
                    : Path.GetFullPath(rawOut, repositoryRoot);
                var outDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                var swagger = app.Services.GetRequiredService<ISwaggerProvider>();
                var document = swagger.GetSwagger("v1", null, null);
                using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                var jsonWriter = new OpenApiJsonWriter(stringWriter);
                document.SerializeAsV3(jsonWriter);
                await File.WriteAllTextAsync(outputPath, stringWriter.ToString());
                Log.Information("OpenAPI (swagger) dosyası yazıldı: {OutputPath}", outputPath);
                return 0;
            }

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

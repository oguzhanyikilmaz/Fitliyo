using Microsoft.AspNetCore.Builder;
using Fitliyo;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Fitliyo.Web.csproj");
await builder.RunAbpModuleAsync<FitliyoWebTestModule>(applicationName: "Fitliyo.Web" );

public partial class Program
{
}

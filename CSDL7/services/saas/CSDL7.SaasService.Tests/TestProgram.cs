using CSDL7.SaasService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.SaasService.csproj"); 
await builder.RunAbpModuleAsync<SaasServiceTestsModule>(applicationName: "CSDL7.SaasService");

public partial class TestProgram
{
}

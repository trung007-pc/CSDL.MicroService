using CSDL7.AdministrationService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.AdministrationService.csproj"); 
await builder.RunAbpModuleAsync<AdministrationServiceTestsModule>(applicationName: "CSDL7.AdministrationService");

public partial class TestProgram
{
}

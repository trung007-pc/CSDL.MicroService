using CSDL7.IdentityService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.IdentityService.csproj"); 
await builder.RunAbpModuleAsync<IdentityServiceTestsModule>(applicationName: "CSDL7.IdentityService");

public partial class TestProgram
{
}

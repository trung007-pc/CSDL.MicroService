using CSDL7.LanguageService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.LanguageService.csproj"); 
await builder.RunAbpModuleAsync<LanguageServiceTestsModule>(applicationName: "CSDL7.LanguageService");

public partial class TestProgram
{
}
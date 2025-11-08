using CSDL7.EmailService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.EmailService.csproj"); 
await builder.RunAbpModuleAsync<EmailServiceTestsModule>(applicationName: "CSDL7.EmailService");

public partial class TestProgram
{
}

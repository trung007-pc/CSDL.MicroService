using CSDL7.AuditLoggingService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.AuditLoggingService.csproj"); 
await builder.RunAbpModuleAsync<AuditLoggingServiceTestsModule>(applicationName: "CSDL7.AuditLoggingService");

public partial class TestProgram
{
}

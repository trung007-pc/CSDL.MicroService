using CSDL7.GdprService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.GdprService.csproj"); 
await builder.RunAbpModuleAsync<GdprServiceTestsModule>(applicationName: "CSDL7.GdprService");

public partial class TestProgram
{
}

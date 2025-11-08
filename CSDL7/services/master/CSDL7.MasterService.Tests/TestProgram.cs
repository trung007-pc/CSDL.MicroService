using CSDL7.MasterService.Tests;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CSDL7.MasterService.csproj"); 
await builder.RunAbpModuleAsync<MasterServiceTestsModule>(applicationName: "CSDL7.MasterService");

public partial class TestProgram
{
}

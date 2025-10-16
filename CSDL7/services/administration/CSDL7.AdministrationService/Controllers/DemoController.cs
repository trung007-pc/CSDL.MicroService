using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace CSDL7.AdministrationService.Controllers;

[Route("api/administration/demo")]
[Area("administration")]
[RemoteService(Name = "AdministrationService")]
public class DemoController : AbpController
{
    [HttpGet]
    [Route("hello")]
    public async Task<string> HelloWorld()
    {
        return await Task.FromResult("Hello World!");
    }
}
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace CSDL7.LanguageService.Controllers;

[Route("api/language-management/demo")]
[Area("language-management")]
[RemoteService(Name = "LanguageService")]
public class DemoController : AbpController
{
    [HttpGet]
    [Route("hello")]
    public async Task<string> HelloWorld()
    {
        return await Task.FromResult("Hello World!");
    }
}
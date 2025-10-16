using Volo.Abp.DependencyInjection;

namespace CSDL7.SaasService.Data;

public class SaasServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<SaasServiceDataSeeder> _logger;

    public SaasServiceDataSeeder(
        ILogger<SaasServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}
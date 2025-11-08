using Volo.Abp.DependencyInjection;

namespace CSDL7.MasterService.Data;

public class MasterServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<MasterServiceDataSeeder> _logger;

    public MasterServiceDataSeeder(
        ILogger<MasterServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}
using Volo.Abp.DependencyInjection;

namespace CSDL7.EmailService.Data;

public class EmailServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<EmailServiceDataSeeder> _logger;

    public EmailServiceDataSeeder(
        ILogger<EmailServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}
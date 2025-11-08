using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace CSDL7.EmailService.Data;

public class EmailServiceDatabaseMigrationEventHandler : EfCoreDatabaseMigrationEventHandlerBase<EmailServiceDbContext>
{
    private readonly EmailServiceDataSeeder _dataSeeder;

    public EmailServiceDatabaseMigrationEventHandler(
        ILoggerFactory loggerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantStore tenantStore,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        EmailServiceDataSeeder dataSeeder
    ) : base(
        EmailServiceDbContext.DatabaseName,
        currentTenant,
        unitOfWorkManager,
        tenantStore,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
        _dataSeeder = dataSeeder;
    }

    protected override async Task SeedAsync(Guid? tenantId)
    {
        await _dataSeeder.SeedAsync(tenantId);
    }
}
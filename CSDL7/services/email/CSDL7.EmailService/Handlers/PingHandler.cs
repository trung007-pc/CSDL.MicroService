using CSDL7.EmailService.Entities.Pings;
using CSDL7.Shared;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace CSDL7.EmailService;

public class PingHandler  : IDistributedEventHandler<PingCalledEto>, ITransientDependency
{
    private readonly PingManager _pingManager;
    public PingHandler(PingManager pingManager)
    {
        _pingManager = pingManager;
    }
    
    // QUAN TRỌNG: Thêm [UnitOfWork] để ABP quản lý DbContext lifecycle
    [UnitOfWork(isTransactional: true)]
    public virtual async Task HandleEventAsync(PingCalledEto eventData)
    {
        //InboxProcessor a = new InboxProcessor();
        await Task.Delay(2000);
        //throw new Exception();
        await _pingManager.UpdateAsync(eventData.Value);
        Console.WriteLine($"Updated ping: {eventData.Value}");
    }
}
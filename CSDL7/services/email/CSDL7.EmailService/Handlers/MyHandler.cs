using CSDL7.Shared;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace CSDL7.EmailService;

public class MyHandler  : IDistributedEventHandler<DepartmentCreatedEto>, ITransientDependency
{
    private readonly MasterService.Infrastructures.EmailService _emailService;
    public MyHandler(MasterService.Infrastructures.EmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task HandleEventAsync(DepartmentCreatedEto eventData)
    {
        await _emailService.SendEmailToOneAsync("trungn3loveyou@gmail.com", $"Hello Bạn đã tạo 1 phòng ban tên là {eventData.Name} vào lúc {eventData.CreatedAt}");
    }
}
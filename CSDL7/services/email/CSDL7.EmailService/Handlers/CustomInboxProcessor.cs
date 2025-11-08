
using Volo.Abp.EventBus.Distributed;

InboxProcessor

// using Microsoft.Extensions.Options;
// using Volo.Abp.DistributedLocking;
// using Volo.Abp.EventBus.Distributed;
// using Volo.Abp.Threading;
// using Volo.Abp.Timing;
// using Volo.Abp.Uow;
//
// namespace CSDL7.EmailService;
//
// public class CustomInboxProcessor : InboxProcessor
// {
//     public CustomInboxProcessor(IServiceProvider serviceProvider, AbpAsyncTimer timer,
//         IDistributedEventBus distributedEventBus, IAbpDistributedLock distributedLock,
//         IUnitOfWorkManager unitOfWorkManager, IClock clock,
//         IOptions<AbpEventBusBoxesOptions> eventBusBoxesOptions) : base(serviceProvider, timer, distributedEventBus,
//         distributedLock, unitOfWorkManager, clock, eventBusBoxesOptions)
//     {
//     }
//
//     protected override async Task RunAsync()
//     {
//         if (StoppingToken.IsCancellationRequested)
//         {
//             return;
//         }
//
//         await using (var handle = await DistributedLock.TryAcquireAsync(DistributedLockName, cancellationToken: StoppingToken))
//         {
//             if (handle != null)
//             {
//                 await DeleteOldEventsAsync();
//
//                 while (true)
//                 {
//                     var waitingEvents = await GetWaitingEventsAsync();
//                     if (waitingEvents.Count <= 0)
//                     {
//                         break;
//                     }
//
//                     Logger.LogInformation($"Found {waitingEvents.Count} events in the inbox.");
//
//                     foreach (var waitingEvent in waitingEvents)
//                     {
//                         using (var uow = UnitOfWorkManager.Begin(isTransactional: true, requiresNew: true))
//                         {
//                             await DistributedEventBus
//                                 .AsSupportsEventBoxes()
//                                 .ProcessFromInboxAsync(waitingEvent, InboxConfig);
//
//                             await Inbox.MarkAsProcessedAsync(waitingEvent.Id);
//
//                             await uow.CompleteAsync(StoppingToken);
//                         }
//
//                         Logger.LogInformation($"Processed the incoming event with id = {waitingEvent.Id:N}");
//                     }
//                 }
//             }
//             else
//             {
//                 Logger.LogDebug("Could not obtain the distributed lock: " + DistributedLockName);
//                 try
//                 {
//                     await Task.Delay(EventBusBoxesOptions.DistributedLockWaitDuration, StoppingToken);
//                 }
//                 catch (TaskCanceledException) { }
//             }
//         }
//     }
// }
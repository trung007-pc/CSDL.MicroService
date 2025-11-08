# RabbitMQ, Inbox Pattern và Scaling Strategy

## Tổng quan
Document này giải thích cơ chế hoạt động của RabbitMQ, vai trò của Prefetch Count, Distributed Locking, và phân tích khi nào nên/không nên dùng Inbox Pattern khi scale nhiều service instances.

---

## 1. RabbitMQ bắn message theo cơ chế gì?

### 1.1. Cơ chế phân phối message (Message Distribution)

#### **Round-Robin Distribution** (Default)
```
Producer → Exchange → Queue → Consumers (Round-Robin)

Message Flow:
┌─────────┐      ┌───────┐      ┌─────────┐
│Producer │─────→│ Queue │      │Consumer1│ ← msg 1, 5, 7, 9
└─────────┘      │       │─────→│Consumer2│ ← msg 2, 4, 6, 8, 10
                 │       │      │Consumer3│ ← msg 3, 13, 15...
                 └───────┘      └─────────┘
```

**Đặc điểm:**
- RabbitMQ phân phối message **lần lượt** cho từng consumer
- Consumer nào **ACK xong** thì nhận message tiếp theo
- **KHÔNG đảm bảo** consumer 1 xử lý xong message 1 trước khi consumer 2 nhận message 2

#### **Ví dụ thực tế:**
```
Timeline:
t0: Consumer1 nhận msg 1 (xử lý 5 giây)
t1: Consumer2 nhận msg 2 (xử lý 1 giây)
t2: Consumer2 ACK msg 2 → nhận msg 3
t3: Consumer3 nhận msg 4
t5: Consumer1 ACK msg 1 → nhận msg 5

Kết quả: msg 2, 3, 4 có thể hoàn thành TRƯỚC msg 1!
```

### 1.2. Thứ tự xử lý message (Message Ordering)

#### **Trường hợp 1: Single Consumer**
```
Producer → Queue → Single Consumer

✅ ĐẢM BẢO thứ tự: 1 → 2 → 3 → 4 → 5
```
- Consumer xử lý **tuần tự** từng message
- Phải ACK message hiện tại mới nhận message tiếp theo
- **Prefetch Count = 1** đảm bảo strict ordering

#### **Trường hợp 2: Multiple Consumers**
```
Producer → Queue → Multiple Consumers

❌ KHÔNG ĐẢM BẢO thứ tự toàn cục
✅ ĐẢM BẢO: Mỗi message được xử lý đúng 1 lần (với ACK)
```

**Ví dụ:**
```csharp
// 3 consumers cùng lắng nghe 1 queue
Consumer1: Nhận msg 1, 4, 7 (xử lý chậm)
Consumer2: Nhận msg 2, 5, 8 (xử lý nhanh)
Consumer3: Nhận msg 3, 6, 9 (xử lý trung bình)

Kết quả hoàn thành: 2 → 3 → 5 → 6 → 8 → 1 → 4 → 7 → 9
                     ❌ Không theo thứ tự!
```

### 1.3. Khi nào RabbitMQ requeue message?

```
Message Flow với Error Handling:

Consumer nhận message
   ↓
Xử lý thành công? ─YES→ ACK → Message bị xóa khỏi Queue
   ↓ NO
NACK/Reject với requeue=true
   ↓
Message quay lại Queue (đầu hoặc cuối tùy config)
   ↓
RabbitMQ phân phối lại cho consumer khác (hoặc cùng consumer)
```

**Lưu ý:** Requeue làm phá vỡ thứ tự message!

---

## 2. Prefetch Count là gì?

### 2.1. Định nghĩa

```csharp
Configure<AbpRabbitMqEventBusOptions>(options =>
{
    options.PrefetchCount = 5;  // Consumer buffer size
});
```

**Prefetch Count = Số lượng message tối đa RabbitMQ gửi cho consumer TRƯỚC KHI consumer ACK**

### 2.2. Cơ chế hoạt động

#### **Prefetch Count = 1** (Default - Strict)
```
RabbitMQ                           Consumer
Queue: [1][2][3][4][5]            Buffer: []

Step 1: RabbitMQ gửi msg 1        Buffer: [1]
        Queue: [2][3][4][5]
        
Step 2: Consumer xử lý msg 1      Buffer: [1] (processing)
        (phải đợi ACK)
        
Step 3: Consumer ACK msg 1        Buffer: []
        RabbitMQ gửi msg 2        Buffer: [2]
```

**Đặc điểm:**
- ✅ **Fair distribution**: Consumer nhanh xử lý nhiều hơn
- ✅ **Ordering**: Đảm bảo thứ tự với single consumer
- ❌ **Latency**: Network round-trip giữa mỗi message

#### **Prefetch Count = 5** (Optimized)
```
┌─────────────────────────────────────────────────────────────┐
│ RabbitMQ Server                                             │
│                                                             │
│  Queue: [M1][M2][M3][M4][M5][M6][M7][M8][M9][M10]          │
│                                                             │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           │ Network
                           │
                           ↓
┌─────────────────────────────────────────────────────────────┐
│ Consumer Application (EmailService Instance)                │
│                                                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Consumer Buffer (Memory - Prefetch Count = 5)      │    │
│  │                                                     │    │
│  │  [M1][M2][M3][M4][M5]  ← 5 messages trong RAM      │    │
│  │   ↓                                                 │    │
│  │  Unacked messages (chưa xử lý xong)                │    │
│  └────────────────────────────────────────────────────┘    │
│                           ↓                                 │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Event Handler (Processing)                         │    │
│  │                                                     │    │
│  │  Processing M1 → (2s) → ACK                        │    │
│  │                                                     │    │
│  │  ⚠️ Xử lý TUẦN TỰ từng message                     │    │
│  └────────────────────────────────────────────────────┘    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Đặc điểm:**
- ✅ **Performance**: Giảm network latency
- ✅ **Throughput**: Consumer buffer luôn đầy
- ⚠️ **Ordering**: Không đảm bảo thứ tự nếu xử lý parallel
- ⚠️ **Uneven load**: Consumer chậm vẫn giữ 5 messages trong buffer

### 2.3. Prefetch Count ≠ Parallelism

**Sai lầm phổ biến:**
```csharp
// ❌ HIỂU SAI: Prefetch = 5 nghĩa là xử lý 5 messages song song
options.PrefetchCount = 5;  

// ✅ ĐÚNG: Prefetch = 5 nghĩa là consumer buffer giữ tối đa 5 messages
// Consumer vẫn xử lý tuần tự (trừ khi dùng async/await hoặc multi-thread)
```

**Ví dụ minh họa:**
```csharp
public class Handler : IDistributedEventHandler<PingCalledEto>
{
    // Case 1: Synchronous - Xử lý tuần tự dù Prefetch = 5
    public async Task HandleEventAsync(PingCalledEto eventData)
    {
        await Task.Delay(2000);  // Mỗi message mất 2 giây
        // Message 1 → 2 → 3 → 4 → 5 (tuần tự)
    }
}
```

### 2.4. Chọn Prefetch Count phù hợp

| Prefetch Count | Use Case | Pros | Cons |
|---|---|---|---|
| **1** | Strict ordering, fair distribution | Công bằng, đảm bảo thứ tự | Latency cao |
| **10-50** | High throughput, stateless processing | Performance tốt | Unfair distribution |
| **100+** | Batch processing, bulk operations | Throughput cực cao | Memory usage cao |

**Khuyến nghị:**
```csharp
// Standard microservices
options.PrefetchCount = 10;

// Critical ordering (payment, inventory)
options.PrefetchCount = 1;

// Batch processing (email, notification)
options.PrefetchCount = 100;
```

---

## 3. Distributed Locking with Redis

### 3.1. Distributed Lock là gì?

**Vấn đề:**
```
Scenario: 3 Instances EmailService, Inbox enabled

┌─────────────────────────────────────────────────────────┐
│ AbpEventInbox Table                                     │
│                                                         │
│ [Record #1: Processed=false]                           │
│ [Record #2: Processed=false]                           │
│ [Record #3: Processed=false]                           │
└─────────────────────────────────────────────────────────┘
           ↑           ↑           ↑
           │           │           │
    ┌──────┴──┐  ┌─────┴────┐  ┌──┴──────┐
    │Instance1│  │Instance2 │  │Instance3│
    │Worker   │  │Worker    │  │Worker   │
    └─────────┘  └──────────┘  └─────────┘

Timeline:
T0: All 3 workers query: SELECT * FROM AbpEventInbox WHERE Processed=false
    → Tất cả đều thấy Record #1!

T1: Instance1 execute handler for Record #1
    Instance2 execute handler for Record #1  ← ❌ TRÙNG!
    Instance3 execute handler for Record #1  ← ❌ TRÙNG!

❌ KẾT QUẢ: Message bị xử lý 3 LẦN!
```

**Giải pháp: Distributed Lock**
```
Service 1: Acquire lock "message:123" → ✅ Success → Xử lý
Service 2: Acquire lock "message:123" → ❌ Failed → Skip (hoặc đợi)
Service 3: Acquire lock "message:123" → ❌ Failed → Skip (hoặc đợi)

Service 1: Release lock "message:123" → Service 2/3 có thể acquire
```

### 3.2. Cơ chế hoạt động với Redis

![a](distributelocking.jpg)


### 3.3. ABP Framework Implementation

```csharp
public class PingHandler : IDistributedEventHandler<PingCalledEto>
{
    private readonly IAbpDistributedLock _distributedLock;
    
    public async Task HandleEventAsync(PingCalledEto eventData)
    {
        // Tạo lock key dựa trên event data
        var lockKey = $"ping:process:{eventData.Value}";
        
        // Acquire lock với timeout 10 giây
        await using (var handle = await _distributedLock.TryAcquireAsync(
            lockKey, 
            TimeSpan.FromSeconds(10)))
        {
            if (handle == null)
            {
                // Lock failed → Message đang được xử lý bởi instance khác
                _logger.LogWarning($"Cannot acquire lock for {lockKey}");
                return;  // Skip hoặc retry later
            }
            
            // Lock success → Xử lý message
            await _pingManager.UpdateAsync(eventData.Value);
            
            // Lock tự động release khi dispose
        }
    }
}
```

### 3.4. Lock Expiration & Renewal

```
Timeline:

t0: Service 1 acquire lock (expire = 30s)
    Redis: SET lock:msg:123 "service-1" EX 30

t10: Service 1 đang xử lý (chưa xong)
     Redis: TTL lock:msg:123 = 20s

t25: Service 1 vẫn xử lý (sắp hết lock)
     Redis: TTL lock:msg:123 = 5s
     
     → Service 1 RENEW lock: EXPIRE lock:msg:123 30
     Redis: TTL lock:msg:123 = 30s (reset)

t35: Service 1 xử lý xong → Release lock
     Redis: DEL lock:msg:123
```

**ABP tự động renew lock nếu:**
- Lock holder còn alive
- Processing time > 80% lock duration

### 3.5. Distributed Lock Patterns

#### **Pattern 1: Pessimistic Locking** (ABP Default)
```csharp
// Acquire lock TRƯỚC khi xử lý
await using (var handle = await _distributedLock.TryAcquireAsync(key))
{
    if (handle == null) return;  // Skip if cannot lock
    await ProcessAsync();
}
```
- ✅ Tránh duplicate processing
- ❌ Throughput thấp (chỉ 1 instance xử lý tại 1 thời điểm)

#### **Pattern 2: Optimistic Locking**
```csharp
// Xử lý trước, check conflict SAU
await ProcessAsync();

var success = await _repository.UpdateWithVersionCheckAsync(entity);
if (!success)
{
    // Conflict → Rollback hoặc retry
    await RollbackAsync();
}
```
- ✅ Throughput cao
- ❌ Có thể duplicate processing → phải rollback

---

## 4. Inbox Pattern với Scaling: Có còn hợp lý?
❌ NHƯỢC ĐIỂM:

1. Distributed Lock → CHỈ 1 INSTANCE xử lý tại 1 thời điểm
   
   Performance:
   - Không dùng Inbox: 3 instances = 3× throughput
   - Dùng Inbox: 3 instances = 1× throughput (vì lock)
   
   → KHÔNG SCALE ĐƯỢC!

2. Single Point of Bottleneck
   
   Timeline:
   T0: Instance1 acquire lock → Xử lý 20 records (40 giây)
   T40: Instance1 release lock
   T40: Instance2 acquire lock → Xử lý 20 records (40 giây)
   T80: Instance2 release lock
   ...
   
   → Giống như CHỈ CÓ 1 INSTANCE!

3. Lock Contention
   
   3 instances cùng TryAcquire:
   - Instance1: SUCCESS → Làm việc
   - Instance2: FAIL → Chờ (IDLE)
   - Instance3: FAIL → Chờ (IDLE)
   
   → 2/3 instances LÃNG PHÍ!
🎯 KẾT LUẬN: KHI NÀO HỢP LÝ?
1. Message BUSINESS CRITICAL
   - Payment processing
   - Order creation
   - Financial transactions
   - < 100 messages/second
   - Mỗi message quan trọng
   → Cần đảm bảo: At-least-once, No duplicate
   → Performance KHÔNG quan trọng bằng correctness

❌ KHÔNG NÊN DÙNG INBOX khi:
1. HIGH THROUGHPUT
   - 1000 messages/second
   - Cần scale horizontal
   
   → Inbox + Lock = Bottleneck
   
💡 KHUYẾN NGHỊ CHO EMAILSERVICE
Hybrid Approach (Best of Both Worlds):
```
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Inboxes.Configure(config =>
    {
        config.UseDbContext<EmailServiceDbContext>();
        
        // ✅ CHỈ dùng Inbox cho events QUAN TRỌNG
        config.EventSelector = eventType =>
        {
            // Business critical events
            if (eventType == typeof(OrderCreatedEto)) return true;
            if (eventType == typeof(PaymentProcessedEto)) return true;
            if (eventType == typeof(SendEmailEto)) return true;
            
            // Simple events → Direct processing
            if (eventType == typeof(PingCalledEto)) return false;
            if (eventType == typeof(CacheInvalidatedEto)) return false;
            
            return false;  // Default: No Inbox
        };
    });
});

Configure<AbpRabbitMqEventBusOptions>(options =>
{
    options.PrefetchCount = 5;  // Direct events scale tốt
});
```
### 4.1. Inbox Pattern là gì?

```
┌─────────────────────────────────────────┐
│         RabbitMQ Consumer               │
│                                         │
│  1. Nhận message từ RabbitMQ            │
│  2. Lưu vào IncomingEventInfo table     │
│  3. ACK RabbitMQ ngay lập tức           │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│    Background Worker (Inbox Processor)  │
│                                         │
│  1. Query IncomingEventInfo table       │
│  2. Acquire Distributed Lock per record │
│  3. Execute Handler                     │
│  4. Mark as Processed                   │
└─────────────────────────────────────────┘
```

**Mục đích:**
- ✅ **At-least-once delivery**: Message không bị mất dù service crash
- ✅ **Transactional**: Inbox insert + Business logic trong 1 transaction
- ✅ **Retry mechanism**: Auto retry failed messages

### 4.2. Scaling với Inbox Pattern

#### **Kịch bản: 3 Service Instances**

```
Instance 1          Instance 2          Instance 3
    ↓                   ↓                   ↓
RabbitMQ Queue (Round-Robin Distribution)
    ↓                   ↓                   ↓
Inbox Table (Shared Database)
┌─────────────────────────────────────────────┐
│ ID │ EventName    │ Data      │ Processed  │
├────┼──────────────┼───────────┼────────────┤
│ 1  │ PingCalled   │ value=70  │ false      │ ← Instance 1 acquire lock
│ 2  │ PingCalled   │ value=71  │ false      │ ← Instance 2 acquire lock
│ 3  │ PingCalled   │ value=72  │ false      │ ← Instance 3 acquire lock
│ 4  │ PingCalled   │ value=73  │ false      │ ← Waiting...
└─────────────────────────────────────────────┘

Background Worker:
- Mỗi instance chạy worker query Inbox table
- Acquire Distributed Lock per IncomingEventInfo.Id
- Xử lý parallel (nhiều instance cùng lúc)
```

#### **Vấn đề phát sinh:**

**1. Out-of-Order Processing**
```
Timeline:
t0: Instance 1 nhận msg 70 → Insert Inbox (ID=1)
t1: Instance 2 nhận msg 71 → Insert Inbox (ID=2)
t2: Instance 3 nhận msg 72 → Insert Inbox (ID=3)

t3: Worker 1 query Inbox → Acquire lock ID=1 (msg 70) → Xử lý 5 giây
t4: Worker 2 query Inbox → Acquire lock ID=2 (msg 71) → Xử lý 1 giây
t5: Worker 3 query Inbox → Acquire lock ID=3 (msg 72) → Xử lý 2 giây

t5: Worker 2 finish msg 71 ✅
t7: Worker 3 finish msg 72 ✅
t8: Worker 1 finish msg 70 ✅

Kết quả: 71 → 72 → 70 (❌ Không theo thứ tự!)
```

**2. Lock Contention**
```sql
-- ABP Inbox Worker query (mỗi 1 giây)
SELECT * FROM AbpIncomingEventInfo
WHERE Processed = false
ORDER BY CreationTime
LIMIT 20;  -- Prefetch 20 records

-- 3 instances cùng query → Database load x3
-- Distributed Lock overhead (Redis call per record)
```

**3. Database Bottleneck**
```
RabbitMQ: 10,000 msg/s → Inbox Table: 10,000 INSERT/s
                      → Inbox Worker: 10,000 SELECT/UPDATE/s
                      
Database: ❌ Cannot handle! (PostgreSQL ~5,000 TPS limit)
```

### 4.3. Khi nào NÊN dùng Inbox Pattern?

#### ✅ **Use Case 1: Critical Transactional Events**

```csharp
// Ví dụ: Payment Completed Event
public class PaymentCompletedHandler
{
    [UnitOfWork]
    public async Task HandleEventAsync(PaymentCompletedEto eventData)
    {
        // Inbox Pattern đảm bảo:
        // 1. Message được lưu DB (không mất)
        // 2. Handler + Business logic trong cùng transaction
        
        using var uow = _unitOfWorkManager.Begin();
        
        // Insert Inbox record
        await _inboxManager.EnqueueAsync(eventData);
        
        // Business logic
        var order = await _orderRepository.GetAsync(eventData.OrderId);
        order.MarkAsPaid();
        await _orderRepository.UpdateAsync(order);
        
        await uow.CompleteAsync();  // Commit cả 2
    }
}
```

**Tại sao cần Inbox?**
- Payment event KHÔNG được mất (critical)
- Phải đảm bảo order.Status = Paid được lưu DB
- Nếu service crash giữa chừng → Inbox retry

#### ✅ **Use Case 2: Idempotent Operations**

```csharp
// Ví dụ: Send Email Event
public class SendEmailHandler
{
    public async Task HandleEventAsync(SendEmailEto eventData)
    {
        // Inbox Pattern giúp:
        // - Tránh send duplicate email (check Inbox.Processed)
        // - Auto retry nếu SMTP server down
        
        var alreadyProcessed = await _inboxRepository
            .AnyAsync(x => x.MessageId == eventData.MessageId 
                        && x.Processed);
                        
        if (alreadyProcessed) return;  // Skip duplicate
        
        await _emailSender.SendAsync(eventData.To, eventData.Subject);
    }
}
```

#### ✅ **Use Case 3: Distributed Saga/Orchestration**

```csharp
// Ví dụ: Order Saga
public class OrderSagaHandler
{
    // Step 1: Reserve Inventory
    // Step 2: Process Payment
    // Step 3: Create Shipment
    
    // Inbox đảm bảo các step được execute đúng thứ tự
    // Retry nếu bất kỳ step nào fail
}
```

### 4.4. Khi nào KHÔNG NÊN dùng Inbox Pattern?

#### ❌ **Use Case 1: High-Throughput Stateless Events**

```csharp
// Ví dụ: Logging Event (10,000 msg/s)
public class UserActivityLogHandler
{
    public async Task HandleEventAsync(UserActivityEto eventData)
    {
        // ❌ Inbox overhead:
        // - 10,000 INSERT Inbox/s
        // - 10,000 UPDATE Processed=true/s
        // - Distributed Lock overhead
        // - Database bottleneck
        
        // ✅ Direct processing:
        await _logger.LogAsync(eventData);
        // Nếu fail → OK, log không critical
    }
}
```

**Lý do:**
- Throughput > 1,000 msg/s → Database không chịu nổi
- Event không critical (mất vài log không sao)
- Idempotent by nature (có thể retry mà không duplicate)

#### ❌ **Use Case 2: Ordering-Critical Events**

```csharp
// Ví dụ: Stock Price Update (phải theo thứ tự)
public class StockPriceHandler
{
    public async Task HandleEventAsync(StockPriceEto eventData)
    {
        // ❌ Inbox Pattern:
        // - Multiple workers xử lý parallel
        // - Message 2 có thể finish trước Message 1
        // - Distributed Lock không đảm bảo thứ tự global
        
        // ✅ Direct RabbitMQ + Single Consumer:
        // - Prefetch Count = 1
        // - 1 consumer per stock symbol
        // - Đảm bảo thứ tự strict
    }
}
```

**Giải pháp thay thế:**
```csharp
Configure<AbpRabbitMqEventBusOptions>(options =>
{
    options.PrefetchCount = 1;  // Strict ordering
    
    // Disable Inbox for StockPriceEto
    options.EventSelector = eventType =>
    {
        if (eventType == typeof(StockPriceEto)) return false;
        return true;  // Enable cho các event khác
    };
});
```

#### ❌ **Use Case 3: Real-time Processing**

```csharp
// Ví dụ: Chat Message
public class ChatMessageHandler
{
    public async Task HandleEventAsync(ChatMessageEto eventData)
    {
        // ❌ Inbox Pattern:
        // - Inbox Worker query mỗi 1 giây (delay!)
        // - Database overhead
        
        // ✅ Direct RabbitMQ:
        // - Latency < 100ms
        // - Push notification ngay lập tức
    }
}
```

### 4.5. Hybrid Approach (Khuyến nghị)

```csharp
Configure<AbpDistributedEventBusOptions>(config =>
{
    // Selective Inbox: Chỉ enable cho critical events
    config.Inboxes.Configure(options =>
    {
        options.EventSelector = eventType =>
        {
            // ✅ Enable Inbox
            if (eventType == typeof(PaymentCompletedEto)) return true;
            if (eventType == typeof(OrderCreatedEto)) return true;
            if (eventType == typeof(InventoryReservedEto)) return true;
            
            // ❌ Disable Inbox (Direct RabbitMQ)
            if (eventType == typeof(UserActivityLogEto)) return false;
            if (eventType == typeof(StockPriceEto)) return false;
            if (eventType == typeof(ChatMessageEto)) return false;
            
            return false;  // Default: Disable
        };
    });
});
```

### Chiến lược KHÔNG dùng Inbox Pattern

```

Dựa trên phân tích từ document, đây là chiến lược chi tiết khi KHÔNG dùng Inbox Pattern:

┌─────────────────────────────────────────────────────────────┐
│                    MasterService (Publisher)                 │
│                                                              │
│  await _eventBus.PublishAsync(new PingCalledEto { ... })   │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ↓
┌─────────────────────────────────────────────────────────────┐
│                    RabbitMQ (Message Queue)                  │
│                                                              │
│  Exchange: CSDL7                                            │
│  Queue: PingCalled (Durable, No TTL)                       │
│  Routing: Round-Robin Distribution                          │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ↓
┌─────────────────────────────────────────────────────────────┐
│              EmailService (3 Instances - No Inbox)           │
│                                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│  │ Instance 1 │  │ Instance 2 │  │ Instance 3 │           │
│  │ Prefetch=5 │  │ Prefetch=5 │  │ Prefetch=5 │           │
│  └────────────┘  └────────────┘  └────────────┘           │
│         ↓               ↓               ↓                   │
│  PingHandler → Direct Processing (No DB, No Lock)          │
└─────────────────────────────────────────────────────────────┘
```
```csharp
public class PingHandler : 
    IDistributedEventHandler<PingCalledEto>,
    ITransientDependency
{
    private readonly PingManager _pingManager;
    private readonly ILogger<PingHandler> _logger;
    
    public PingHandler(
        PingManager pingManager,
        ILogger<PingHandler> logger)
    {
        _pingManager = pingManager;
        _logger = logger;
    }
    
    // ⭐ KEY: UnitOfWork để manage DbContext lifetime
    [UnitOfWork(isTransactional: true)]
    public virtual async Task HandleEventAsync(PingCalledEto eventData)
    {
        try
        {
            _logger.LogInformation($"🔵 Processing ping: {eventData.Value}");
            
            // Simulate processing time
            await Task.Delay(2000);
            
            // Business logic
            await _pingManager.UpdateAsync(eventData.Value);
            
            _logger.LogInformation($"✅ Updated ping: {eventData.Value}");
            
            // ✅ SUCCESS → ABP auto ACK RabbitMQ
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Failed to process ping: {eventData.Value}");
                        
            // Option 1: THROW → RabbitMQ NACK → Requeue → Retry
            // T0: Handler throw exception
            // → RabbitMQ NACK (requeue=true)
            //  → Message quay lại queue
    
            // T1: Consumer nhận lại message (Attempt #2)
            // → Retry...
    
            // T2: Nếu vẫn fail → Attempt #3
            // → Nếu vẫn fail → Message bị DROP (hoặc vào Dead Letter Queue)
            throw;
        }
    }
}
```
```
Kiến trúc tổng quan

┌──────────────────────────────────────────────────────────┐
│ Main Queue: CSDL7.PingCalled                             │
└────────────┬─────────────────────────────────────────────┘
             │
             ↓ Retry 3 lần
             │ (Polly or RabbitMQ native)
             │
    ┌────────┴────────┐
    │                 │
    ↓ Transient Error ↓ Permanent Error
    │                 │
┌───┴────────────┐ ┌──┴─────────────────────────────┐
│ Dead Letter    │ │ Database Table:                │
│ Queue (DLQ)    │ │ AbpFailedEvents                │
│                │ │                                 │
│ TTL: 1 hour    │ │ - MessageId                    │
│ Max Retry: 5   │ │ - EventName                    │
│                │ │ - EventData (JSON)             │
│ Auto Requeue   │ │ - Exception                    │
│ to Main Queue  │ │ - FailedAt, RetryCount         │
└────────────────┘ │ - Status (Pending/Fixed)       │
                   └────────────────────────────────┘
                                │
                                ↓
                   ┌────────────────────────────────┐
                   │ Admin Dashboard                │
                   │ - View failed events           │
                   │ - Manual retry                 │
                   │ - Fix data & reprocess         │
                   └────────────────────────────────┘
```
## Tài liệu tham khảo

1. [RabbitMQ Consumer Prefetch](https://www.rabbitmq.com/docs/consumer-prefetch)
2. [ABP Distributed Event Bus](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed)
3. [Redis Distributed Lock](https://redis.io/docs/latest/develop/use/patterns/distributed-locks/)
4. [ABP Inbox/Outbox Pattern](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/inbox-outbox)

---

**Created:** 2025-11-07  
**Author:** GitHub Copilot  
**Version:** 1.0

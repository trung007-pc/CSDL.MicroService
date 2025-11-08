## ✅ Lợi Ích Của Kiến Trúc N-Layer

### 1. **Separation of Concerns**
- Mỗi layer có trách nhiệm riêng biệt
- Dễ maintain và debug
- Code tổ chức rõ ràng

### 2. **Testability**
- Mock được dependencies thông qua interfaces
- Unit test từng layer độc lập
- Integration test cho toàn bộ flow

### 3. **Scalability**
- Có thể thay đổi implementation mà không ảnh hưởng layer khác
- Dễ dàng thêm features mới
- Horizontal scaling cho từng layer

### 4. **Maintainability**
- Business logic tập trung trong Domain layer
- Infrastructure changes không ảnh hưởng business logic
- Dễ dàng refactor và optimize

### 5. **Reusability**
- Domain logic có thể sử dụng trong nhiều context khác nhau
- Application services có thể được expose qua nhiều channels (Web API, gRPC, etc.)

## 🎯 Best Practices Được Áp Dụng

### 1. **Dependency Inversion Principle**
```csharp
// Application layer phụ thuộc vào abstraction, không phụ thuộc vào concrete implementation
public DepartmentsAppService(IDepartmentRepository departmentRepository)
```

### 2. **Repository Pattern**
```csharp
// Abstraction cho data access
public interface IDepartmentRepository : IRepository<Department, Guid>
```

### 3. **Domain-Driven Design**
```csharp
// Aggregate Root với business rules
public class Department : FullAuditedAggregateRoot<Guid>
```

### 4. **Command Query Responsibility Segregation (CQRS)**
- Tách biệt operations cho read và write
- Optimized queries trong repository

### 5. **Unit of Work Pattern**
- ABP Framework tự động handle transaction boundaries
- Consistent data state across operations


## 🤔 **TẠI SAO CÓ CẢ APPLICATION SERVICE VÀ DOMAIN SERVICE?**

### ❓ **Câu Hỏi Thường Gặp**

> "Tại sao không viết logic vào Application Service cho đơn giản mà lại phải tạo Domain Service rồi Application Service gọi lại? Phức tạp vậy làm gì?"

Đây là câu hỏi rất phổ biến! Hãy để tôi giải thích chi tiết:

### 🎯 **1. TRÁCH NHIỆM KHÁC NHAU**

#### **🏛️ Domain Service - "Core Business Logic"**

Domain Service chứa **pure business logic**, không quan tâm đến:
- Ai gọi nó (Web API, gRPC, Console App)
- Data format (DTO, JSON, XML)
- Technical concerns (HTTP status codes, caching, logging)

```csharp
// ✅ Domain Service - Pure business logic
public class DepartmentManager : DomainService
{
    protected IDepartmentRepository _departmentRepository;

    public async Task<Department> CreateAsync(string name)
    {
        // ✅ Business Rule 1: Name không được rỗng
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        // ✅ Business Rule 2: Name không được trùng
        var existingDept = await _departmentRepository.FindByNameAsync(name);
        if (existingDept != null)
            throw new DomainException("Department with this name already exists");
        
        // ✅ Business Rule 3: Format name
        name = name.Trim().ToUpper();
        
        // ✅ Business Rule 4: Create with proper ID
        var department = new Department(GuidGenerator.Create(), name);
        
        // ✅ Business Rule 5: Set default values
        department.Status = DepartmentStatus.Active;
        department.CreatedDate = Clock.Now;
        
        return await _departmentRepository.InsertAsync(department);
    }
}
```

#### **🔧 Application Service - "Use Case Orchestration"**

Application Service **orchestrate** (điều phối) business workflow và xử lý technical concerns:

```csharp
// ✅ Application Service - Orchestration & Technical concerns
[Authorize(MasterServicePermissions.Departments.Create)]  // ← Authorization
public class DepartmentsAppService : ApplicationService
{
    protected IDepartmentRepository _departmentRepository;
    protected DepartmentManager _departmentManager;  // ← Sử dụng Domain Service
    protected IEmailService _emailService;
    protected ILogger<DepartmentsAppService> _logger;

    public virtual async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // 🔧 Technical: Logging
        _logger.LogInformation("Creating department with name: {Name}", input.Name);
        
        try
        {
            // 🏛️ Business Logic: Gọi Domain Service
            var department = await _departmentManager.CreateAsync(input.Name);
            
            // 🔧 Technical: Send notification
            await _emailService.SendAsync(
                input.ManagerEmail,
                "Department Created",
                $"New department {department.Name} has been created");
            
            // 🔧 Technical: Update cache
            await _cache.RemoveAsync("department-list");
            
            // 🔧 Technical: Publish event
            await _eventBus.PublishAsync(new DepartmentCreatedEvent(department.Id));
            
            // 🔧 Technical: Mapping to DTO
            var result = ObjectMapper.Map<Department, DepartmentDto>(department);
            
            _logger.LogInformation("Department created successfully: {Id}", department.Id);
            
            return result;
        }
        catch (DomainException ex)
        {
            // 🔧 Technical: Exception handling
            _logger.LogWarning("Failed to create department: {Message}", ex.Message);
            throw new BusinessException(ex.Message);
        }
    }
}
```

### 🎪 **2. TÁI SỬ DỤNG BUSINESS LOGIC**

#### **❌ KHÔNG TỐT: Logic trong Application Service**

```csharp
// ❌ Logic trực tiếp trong Application Service
public class DepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // Business logic bị nhúng trong Application Service
        Check.NotNullOrWhiteSpace(input.Name, nameof(input.Name));
        
        var existingDept = await _repository.FindByNameAsync(input.Name);
        if (existingDept != null)
            throw new BusinessException("Department exists");
            
        var department = new Department(GuidGenerator.Create(), input.Name);
        await _repository.InsertAsync(department);
        
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}

// 😱 VẤN ĐỀ: Nếu có thêm kênh giao tiếp khác (gRPC, Console, Background Job)
// phải DUPLICATE business logic!

public class DepartmentGrpcService : GrpcService
{
    public async Task<DepartmentResponse> CreateDepartment(CreateDepartmentRequest request)
    {
        // 😱 Phải copy-paste lại business logic
        Check.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        
        var existingDept = await _repository.FindByNameAsync(request.Name);
        if (existingDept != null)
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Department exists"));
            
        var department = new Department(GuidGenerator.Create(), request.Name);
        await _repository.InsertAsync(department);
        
        return new DepartmentResponse { Id = department.Id.ToString() };
    }
}
```

#### **✅ TỐT: Logic trong Domain Service**

```csharp
// ✅ Business logic tập trung trong Domain Service
public class DepartmentManager : DomainService
{
    public async Task<Department> CreateAsync(string name)
    {
        // Business logic CHỈ VIẾT MỘT LẦN
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        var existingDept = await _repository.FindByNameAsync(name);
        if (existingDept != null)
            throw new DomainException("Department exists");
            
        var department = new Department(GuidGenerator.Create(), name);
        return await _repository.InsertAsync(department);
    }
}

// ✅ Web API Application Service - Gọi Domain Service
public class DepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        var department = await _departmentManager.CreateAsync(input.Name);  // ← Reuse
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}

// ✅ gRPC Service - Gọi CÙNG Domain Service
public class DepartmentGrpcService : GrpcService
{
    public async Task<DepartmentResponse> CreateDepartment(CreateDepartmentRequest request)
    {
        var department = await _departmentManager.CreateAsync(request.Name);  // ← Reuse
        return new DepartmentResponse { Id = department.Id.ToString() };
    }
}

// ✅ Console Command - Gọi CÙNG Domain Service
public class ImportDepartmentsCommand
{
    public async Task ExecuteAsync(List<string> departmentNames)
    {
        foreach (var name in departmentNames)
        {
            await _departmentManager.CreateAsync(name);  // ← Reuse
        }
    }
}

// ✅ Background Job - Gọi CÙNG Domain Service
public class SyncDepartmentsJob : IBackgroundJob
{
    public async Task ExecuteAsync()
    {
        var externalDepts = await _externalApi.GetDepartmentsAsync();
        foreach (var dept in externalDepts)
        {
            await _departmentManager.CreateAsync(dept.Name);  // ← Reuse
        }
    }
}
```

### 🧪 **3. DỄ DÀNG KIỂM THỬ**

#### **❌ KHÔNG TỐT: Test phải setup Application Service**

```csharp
// ❌ Test Application Service - Phải setup nhiều thứ
[Test]
public async Task CreateDepartment_ShouldThrowException_WhenNameExists()
{
    // 😱 Phải setup tất cả dependencies của Application Service
    var mockRepo = new Mock<IDepartmentRepository>();
    var mockEmailService = new Mock<IEmailService>();
    var mockCache = new Mock<IDistributedCache>();
    var mockEventBus = new Mock<IEventBus>();
    var mockLogger = new Mock<ILogger<DepartmentsAppService>>();
    var mockMapper = new Mock<IObjectMapper>();
    
    mockRepo.Setup(x => x.FindByNameAsync("IT")).ReturnsAsync(new Department(...));
    
    var service = new DepartmentsAppService(
        mockRepo.Object,
        mockEmailService.Object,
        mockCache.Object,
        mockEventBus.Object,
        mockLogger.Object,
        mockMapper.Object);  // 😱 Quá nhiều dependencies!
    
    // Act & Assert
    await Assert.ThrowsAsync<BusinessException>(
        () => service.CreateAsync(new CreateDepartmentDto { Name = "IT" }));
}
```

#### **✅ TỐT: Test Domain Service - Đơn giản**

```csharp
// ✅ Test Domain Service - CHỈ CẦN repository
[Test]
public async Task CreateDepartment_ShouldThrowException_WhenNameExists()
{
    // ✅ Chỉ cần mock repository
    var mockRepo = new Mock<IDepartmentRepository>();
    mockRepo.Setup(x => x.FindByNameAsync("IT"))
           .ReturnsAsync(new Department(Guid.NewGuid(), "IT"));
    
    var manager = new DepartmentManager(mockRepo.Object);
    
    // Act & Assert - Test thuần business logic
    await Assert.ThrowsAsync<DomainException>(
        () => manager.CreateAsync("IT"));
}

[Test]
public async Task CreateDepartment_ShouldFormatName()
{
    // ✅ Test business rule: format name
    var mockRepo = new Mock<IDepartmentRepository>();
    mockRepo.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((Department)null);
    mockRepo.Setup(x => x.InsertAsync(It.IsAny<Department>())).ReturnsAsync((Department d) => d);
    
    var manager = new DepartmentManager(mockRepo.Object);
    
    var result = await manager.CreateAsync("  it department  ");
    
    Assert.Equal("IT DEPARTMENT", result.Name);  // ✅ Verify business rule
}
```

### 🔄 **4. BUSINESS LOGIC THAY ĐỔI THƯỜNG XUYÊN**

```csharp
// 🏛️ Domain Service - Business logic có thể thay đổi
public class DepartmentManager : DomainService
{
    public async Task<Department> CreateAsync(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        // ✅ Business Rule mới: Check name length
        if (name.Length > 100)
            throw new DomainException("Department name too long");
        
        // ✅ Business Rule mới: Check special characters
        if (Regex.IsMatch(name, @"[^a-zA-Z0-9\s]"))
            throw new DomainException("Department name contains invalid characters");
        
        // ✅ Business Rule mới: Check reserved names
        var reservedNames = new[] { "ADMIN", "SYSTEM", "ROOT" };
        if (reservedNames.Contains(name.ToUpper()))
            throw new DomainException("This name is reserved");
        
        var existingDept = await _repository.FindByNameAsync(name);
        if (existingDept != null)
            throw new DomainException("Department exists");
            
        var department = new Department(GuidGenerator.Create(), name);
        return await _repository.InsertAsync(department);
    }
}

// 🔧 Application Service - KHÔNG CẦN THAY ĐỔI!
public class DepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // ✅ Code này không thay đổi khi business rules thay đổi
        var department = await _departmentManager.CreateAsync(input.Name);
        await _emailService.SendAsync(...);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}
```

### 📦 **5. MICROSERVICES VÀ BOUNDED CONTEXTS**

```csharp
// 🏛️ Domain Service - Có thể chia sẻ giữa microservices
// Project: CSDL7.Domain (Shared)
public class DepartmentManager : DomainService
{
    public async Task<Department> CreateAsync(string name)
    {
        // Core business logic
    }
}

// ✅ Microservice 1: Master Service
public class MasterDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        var dept = await _departmentManager.CreateAsync(input.Name);  // ← Shared logic
        // Master-specific orchestration
        return MapToDto(dept);
    }
}

// ✅ Microservice 2: HR Service  
public class HRDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentResponse> CreateDepartment(CreateDepartmentCommand cmd)
    {
        var dept = await _departmentManager.CreateAsync(cmd.Name);  // ← Same logic
        // HR-specific orchestration
        await _employeeService.AssignManagerAsync(dept.Id, cmd.ManagerId);
        return new DepartmentResponse(dept);
    }
}
```

### 🎯 **6. SINGLE RESPONSIBILITY PRINCIPLE**

```csharp
// ✅ Mỗi class có MỘT trách nhiệm duy nhất

// 🏛️ Domain Service: Business Logic
public class DepartmentManager
{
    // Trách nhiệm: Đảm bảo business rules được tuân thủ
    public async Task<Department> CreateAsync(string name) { }
}

// 🔧 Application Service: Orchestration
public class DepartmentsAppService
{
    // Trách nhiệm: Điều phối workflow, technical concerns
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input) { }
}

// 🗄️ Repository: Data Access
public class EfCoreDepartmentRepository
{
    // Trách nhiệm: Persistence
    public async Task<Department> InsertAsync(Department department) { }
}

// 📧 Email Service: External Communication
public class EmailService
{
    // Trách nhiệm: Gửi email
    public async Task SendAsync(string to, string subject, string body) { }
}
```

### 🏆 **KẾT LUẬN: KHI NÀO DÙNG CÁI GÌ?**

#### **🏛️ Viết trong DOMAIN SERVICE khi:**

✅ Logic là **business rule** (không thay đổi theo kênh giao tiếp)
✅ Logic cần **tái sử dụng** ở nhiều nơi
✅ Logic cần **test độc lập** với technical concerns
✅ Logic liên quan đến **nhiều entities** hoặc **complex validation**

```csharp
// ✅ Domain Service
- Validate business rules
- Check unique constraints
- Calculate business values
- Enforce domain invariants
- Complex entity creation
```

#### **🔧 Viết trong APPLICATION SERVICE khi:**

✅ Logic là **technical concern** (authorization, logging, caching)
✅ Logic là **orchestration** (gọi nhiều domain services)
✅ Logic là **data transformation** (DTO ↔ Entity mapping)
✅ Logic là **external integration** (email, events, APIs)

```csharp
// ✅ Application Service
- Authorization checks
- Logging & monitoring
- Caching
- Email notifications
- Event publishing
- DTO mapping
- Transaction coordination
```

### 💡 **TRẢ LỜI NGẮN GỌN CHO CÂU HỎI**

> **Q: Tại sao không viết logic vào Application Service cho đơn giản?**

**A:** Vì:
1. **Tái sử dụng**: Domain Service có thể gọi từ Web API, gRPC, Console, Background Jobs
2. **Dễ test**: Test business logic không cần setup HTTP, cache, email, etc.
3. **Tách biệt concerns**: Business logic không bị lẫn với technical concerns
4. **Maintainability**: Business rules thay đổi chỉ sửa một chỗ
5. **Scalability**: Domain logic có thể share giữa microservices

**Đơn giản hơn trong ngắn hạn, nhưng phức tạp hơn trong dài hạn!** 🚀

---

## 📧 **EMAIL VÀ CACHING NÊN ĐẶT Ở ĐÂU?**

### 🎯 **Nguyên Tắc Vàng: Phân Biệt Business Logic vs Technical Concern**

#### **📋 Quy Tắc Đơn Giản:**

```
🏛️ DOMAIN SERVICE = Business Logic (WHAT & WHY)
    ↓
    "Department phải có tên duy nhất" - Business Rule
    "Giá sản phẩm phải > 0" - Business Rule
    "Order chỉ cancel được khi status = Pending" - Business Rule

🔧 APPLICATION SERVICE = Technical Concern (HOW)
    ↓
    "Gửi email thông báo" - Technical Implementation
    "Cache kết quả" - Performance Optimization
    "Log activity" - Monitoring
```

### 📧 **1. EMAIL - NÊN ĐẶT Ở APPLICATION SERVICE**

#### **✅ TẠI SAO EMAIL Ở APPLICATION SERVICE?**

Email là **side effect** (tác dụng phụ), **KHÔNG PHẢI** business rule:

```csharp
// 🏛️ DOMAIN SERVICE - Pure business logic
public class DepartmentManager : DomainService
{
    public async Task<Department> CreateAsync(string name)
    {
        // ✅ Business Rule: Validate name
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        // ✅ Business Rule: Check uniqueness
        if (await _repository.ExistsByNameAsync(name))
            throw new DomainException("Department name already exists");
        
        // ✅ Business Rule: Create entity
        var department = new Department(GuidGenerator.Create(), name);
        
        // ✅ Business Rule: Set default status
        department.Status = DepartmentStatus.Active;
        
        // ❌ KHÔNG NÊN: Gửi email ở đây
        // await _emailService.SendAsync(...);  // ← BAD!
        
        return await _repository.InsertAsync(department);
    }
}

// 🔧 APPLICATION SERVICE - Technical concerns
public class DepartmentsAppService : ApplicationService
{
    protected DepartmentManager _departmentManager;
    protected IEmailService _emailService;  // ← Email service
    
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // 🏛️ Execute business logic
        var department = await _departmentManager.CreateAsync(input.Name);
        
        // ✅ ĐÚNG: Gửi email ở đây - Technical side effect
        await _emailService.SendAsync(
            input.ManagerEmail,
            "New Department Created",
            $"Department {department.Name} has been created successfully.");
        
        // ✅ Additional notifications
        await _emailService.SendAsync(
            "admin@company.com",
            "Department Alert",
            $"New department {department.Name} was created by {CurrentUser.UserName}");
        
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}
```

#### **🎯 LÝ DO CHI TIẾT:**

**1. Email KHÔNG PHẢI business rule:**
```csharp
// ❓ Câu hỏi: "Department có được tạo nếu email gửi thất bại?"
// ✅ CÓ! → Email là side effect, không phải business requirement

// 🏛️ Business Rule (bắt buộc):
if (name.Length > 100)
    throw new DomainException("Name too long");  // ← Phải pass mới tạo được

// 🔧 Side Effect (không bắt buộc):
try {
    await _emailService.SendAsync(...);  // ← Fail cũng OK, vẫn tạo Department
} catch {
    _logger.LogWarning("Failed to send email");  // ← Log warning, không throw
}
```

**2. Email phụ thuộc vào context (kênh giao tiếp):**
```csharp
// ✅ Web API - Gửi email cho user
public class WebDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        var dept = await _domainManager.CreateAsync(input.Name);
        await _emailService.SendAsync(input.UserEmail, ...);  // ← Có email input
        return MapToDto(dept);
    }
}

// ✅ Background Job - Không cần email
public class SyncDepartmentsJob : IBackgroundJob
{
    public async Task ExecuteAsync()
    {
        var externalDepts = await _externalApi.GetDepartmentsAsync();
        foreach (var dept in externalDepts)
        {
            await _domainManager.CreateAsync(dept.Name);  // ← Không gửi email
        }
    }
}

// ✅ Console Import - Có thể log thay vì email
public class ImportDepartmentsCommand
{
    public async Task ExecuteAsync(List<string> names)
    {
        foreach (var name in names)
        {
            var dept = await _domainManager.CreateAsync(name);
            Console.WriteLine($"Created: {dept.Name}");  // ← Console log, không email
        }
    }
}
```

**3. Email service là infrastructure concern:**
```csharp
// ❌ Nếu Domain Service phụ thuộc Email
public class DepartmentManager : DomainService
{
    private IEmailService _emailService;  // ← BAD! Domain phụ thuộc infrastructure
    
    public async Task<Department> CreateAsync(string name)
    {
        var dept = new Department(GuidGenerator.Create(), name);
        await _repository.InsertAsync(dept);
        await _emailService.SendAsync(...);  // ← Violation of clean architecture!
        return dept;
    }
}

// ✅ Domain Service không biết về Email
public class DepartmentManager : DomainService
{
    // Chỉ có Repository interface (từ domain)
    private IDepartmentRepository _repository;
    
    public async Task<Department> CreateAsync(string name)
    {
        // Pure business logic
        var dept = new Department(GuidGenerator.Create(), name);
        return await _repository.InsertAsync(dept);
    }
}
```

### 🗄️ **2. CACHING - NÊN ĐẶT Ở APPLICATION SERVICE**

#### **✅ TẠI SAO CACHING Ở APPLICATION SERVICE?**

Caching là **performance optimization**, **KHÔNG PHẢI** business logic:

```csharp
// 🏛️ DOMAIN SERVICE - No caching
public class DepartmentManager : DomainService
{
    public async Task<Department> GetByIdAsync(Guid id)
    {
        // ✅ Business logic: Get department
        var department = await _repository.GetAsync(id);
        
        // ✅ Business rule: Check if active
        if (!department.IsActive)
            throw new DomainException("Department is not active");
        
        // ❌ KHÔNG NÊN: Caching ở đây
        // var cached = await _cache.GetAsync(...);  // ← BAD!
        
        return department;
    }
}

// 🔧 APPLICATION SERVICE - With caching
public class DepartmentsAppService : ApplicationService
{
    protected DepartmentManager _departmentManager;
    protected IDistributedCache _cache;
    
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var cacheKey = $"department:{id}";
        
        // ✅ ĐÚNG: Check cache first - Performance optimization
        var cachedDto = await _cache.GetAsync<DepartmentDto>(cacheKey);
        if (cachedDto != null)
        {
            _logger.LogInformation("Cache hit for department {Id}", id);
            return cachedDto;
        }
        
        // 🏛️ Execute business logic nếu cache miss
        var department = await _departmentManager.GetByIdAsync(id);
        
        // ✅ ĐÚNG: Cache the result
        var dto = ObjectMapper.Map<Department, DepartmentDto>(department);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
        
        return dto;
    }
    
    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        // 🏛️ Execute business logic
        var department = await _departmentManager.UpdateAsync(id, input.Name);
        
        // ✅ ĐÚNG: Invalidate cache - Side effect
        var cacheKey = $"department:{id}";
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveAsync("department-list");  // ← Invalidate list cache too
        
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}
```

#### **🎯 LÝ DO CHI TIẾT:**

**1. Caching KHÔNG ẢNH HƯỞNG business behavior:**
```csharp
// ❓ Câu hỏi: "Department có khác nhau nếu có/không cache?"
// ❌ KHÔNG! → Caching chỉ là performance optimization

// 🏛️ Business Logic (luôn đúng):
var dept1 = await _domainManager.GetByIdAsync(id);  // ← Always returns correct data

// 🔧 With Caching (performance tốt hơn, logic giống nhau):
var dept2 = await _appService.GetAsync(id);  // ← Same result, but faster
```

**2. Cache strategy phụ thuộc vào use case:**
```csharp
// ✅ Web API - Cache 10 minutes
public class WebDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var cached = await _cache.GetAsync<DepartmentDto>($"dept:{id}");
        if (cached != null) return cached;
        
        var dept = await _domainManager.GetByIdAsync(id);
        await _cache.SetAsync($"dept:{id}", dto, TimeSpan.FromMinutes(10));
        return dto;
    }
}

// ✅ Admin API - No cache (cần data real-time)
public class AdminDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var dept = await _domainManager.GetByIdAsync(id);  // ← Always fresh data
        return ObjectMapper.Map<Department, DepartmentDto>(dept);
    }
}

// ✅ Report Service - Cache 1 hour (reports ít thay đổi)
public class ReportDepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var cached = await _cache.GetAsync<DepartmentDto>($"dept:{id}");
        if (cached != null) return cached;
        
        var dept = await _domainManager.GetByIdAsync(id);
        await _cache.SetAsync($"dept:{id}", dto, TimeSpan.FromHours(1));  // ← Longer cache
        return dto;
    }
}
```

**3. Caching là infrastructure detail:**
```csharp
// ❌ Nếu Domain Service phụ thuộc Cache
public class DepartmentManager : DomainService
{
    private IDistributedCache _cache;  // ← BAD! Domain phụ thuộc infrastructure
    
    public async Task<Department> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync(...);  // ← Violation!
        if (cached != null) return cached;
        
        return await _repository.GetAsync(id);
    }
}

// ✅ Domain Service thuần túy
public class DepartmentManager : DomainService
{
    public async Task<Department> GetByIdAsync(Guid id)
    {
        // Pure business logic - không biết về cache
        var department = await _repository.GetAsync(id);
        
        if (!department.IsActive)
            throw new DomainException("Department is inactive");
            
        return department;
    }
}
```

### 📊 **3. BẢNG TỔNG HỢP: ĐẶT Ở ĐÂU?**

| Concern | Layer | Lý Do |
|---------|-------|-------|
| **📧 Email** | Application | Side effect, phụ thuộc context, infrastructure |
| **🗄️ Caching** | Application | Performance optimization, không ảnh hưởng logic |
| **📝 Logging** | Application | Monitoring, không phải business requirement |
| **🔐 Authorization** | Application | Technical security, phụ thuộc context |
| **🔄 Event Publishing** | Application | Integration concern, side effect |
| **📊 Metrics/Monitoring** | Application | Observability, technical concern |
| **✅ Business Validation** | Domain | Core business rule |
| **🔒 Unique Constraints** | Domain | Business invariant |
| **📐 Calculations** | Domain | Business logic |
| **🎯 Entity Creation** | Domain | Business rules enforcement |

### 🎪 **4. VÍ DỤ THỰC TẾ: TẠO DEPARTMENT**

```csharp
// 🏛️ DOMAIN SERVICE - Pure business logic
public class DepartmentManager : DomainService
{
    protected IDepartmentRepository _repository;
    
    public async Task<Department> CreateAsync(string name)
    {
        // ✅ Business validation
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        if (name.Length > 100)
            throw new DomainException("Department name is too long");
        
        // ✅ Business rule: unique name
        if (await _repository.ExistsByNameAsync(name))
            throw new DomainException("Department name already exists");
        
        // ✅ Business logic: create entity
        var department = new Department(GuidGenerator.Create(), name.Trim().ToUpper());
        department.Status = DepartmentStatus.Active;
        department.CreatedDate = Clock.Now;
        
        // ✅ Persist
        return await _repository.InsertAsync(department);
    }
}

// 🔧 APPLICATION SERVICE - Orchestration & technical concerns
[Authorize(MasterServicePermissions.Departments.Create)]  // ← Authorization
public class DepartmentsAppService : ApplicationService
{
    protected DepartmentManager _departmentManager;
    protected IEmailService _emailService;
    protected IDistributedCache _cache;
    protected IEventBus _eventBus;
    protected ILogger<DepartmentsAppService> _logger;
    
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // 📝 Logging - Technical
        _logger.LogInformation(
            "User {UserId} is creating department: {Name}", 
            CurrentUser.Id, 
            input.Name);
        
        try
        {
            // 🏛️ EXECUTE BUSINESS LOGIC
            var department = await _departmentManager.CreateAsync(input.Name);
            
            // 📧 Send email notification - Technical side effect
            try
            {
                await _emailService.SendAsync(
                    input.ManagerEmail,
                    "Department Created",
                    $"New department '{department.Name}' has been created.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email for department {Id}", department.Id);
                // Don't fail the operation if email fails
            }
            
            // 🗄️ Invalidate cache - Performance concern
            await _cache.RemoveAsync("department-list");
            await _cache.RemoveAsync("active-departments");
            
            // 🔄 Publish domain event - Integration concern
            await _eventBus.PublishAsync(new DepartmentCreatedEvent
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                CreatedBy = CurrentUser.UserName,
                CreatedAt = Clock.Now
            });
            
            // 📊 Update metrics - Monitoring
            Metrics.Counter("departments_created_total").Inc();
            
            // 🔧 Map to DTO - Data transformation
            var result = ObjectMapper.Map<Department, DepartmentDto>(department);
            
            _logger.LogInformation(
                "Department {Id} created successfully by user {UserId}", 
                department.Id, 
                CurrentUser.Id);
            
            return result;
        }
        catch (DomainException ex)
        {
            // 📝 Log business rule violation
            _logger.LogWarning(
                "Failed to create department: {Message}. User: {UserId}", 
                ex.Message, 
                CurrentUser.Id);
            throw new BusinessException(ex.Message);
        }
        catch (Exception ex)
        {
            // 📝 Log unexpected errors
            _logger.LogError(ex, "Unexpected error creating department");
            throw;
        }
    }
}
```

### 🏆 **KẾT LUẬN**

**📧 EMAIL:**
- ✅ Đặt ở **Application Service**
- Lý do: Side effect, phụ thuộc context, infrastructure concern

**🗄️ CACHING:**
- ✅ Đặt ở **Application Service**
- Lý do: Performance optimization, không ảnh hưởng business logic

**🎯 NGUYÊN TẮC:**
```
If (concern là BUSINESS RULE)
    → Domain Service
Else if (concern là TECHNICAL/INFRASTRUCTURE)
    → Application Service
```

**💡 LỜI KHUYÊN:**
> "Hỏi bản thân: Nếu tôi thay đổi từ REST API sang gRPC, concern này có thay đổi không?"
> - Nếu CÓ → Application Service
> - Nếu KHÔNG → Domain Service

## 📊 Kết Luận

Master Service đã implement **kiến trúc n-layer một cách xuất sắc** với:

✅ **Structured Layers**: 4 layer rõ ràng với trách nhiệm riêng biệt
✅ **Proper Dependencies**: Dependency flow đúng hướng
✅ **Clean Code**: Code dễ đọc, maintain và extend
✅ **Best Practices**: Áp dụng các pattern và principles hiện đại
✅ **Enterprise Ready**: Sẵn sàng cho production với monitoring, caching, security

Đây là một example tuyệt vời cho việc implement microservice với n-layer architecture trong .NET ecosystem.
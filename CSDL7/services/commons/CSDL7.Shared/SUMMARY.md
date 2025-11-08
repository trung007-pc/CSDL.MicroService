```bash
# CSDL7.Shared - Thư viện Chung

## ✅ Đã tạo thành công!

Thư viện Shared đã được tạo tại: `d:\CSDL.QG\CSDL7\services\commons\CSDL7.Shared`

## 📁 Cấu trúc Thư viện

```
CSDL7.Shared/
├── Constants/
│   └── SharedConstants.cs          # Hằng số: MaxLength, ValidationPatterns, DefaultValues
├── Dtos/
│   ├── BaseEntityDtos.cs           # Base DTOs: AuditedEntityDto, FullAuditedEntityDto
│   └── PagedRequestDto.cs          # DTOs phân trang: PagedAndSortedRequestDto, PagedResultDto
├── Enums/
│   └── CommonEnums.cs              # Enums: ActiveStatus, Gender
├── Exceptions/
│   └── BusinessException.cs        # Exception nghiệp vụ
├── Extensions/
│   ├── CollectionExtensions.cs     # Extensions cho Collection
│   ├── DateTimeExtensions.cs       # Extensions cho DateTime
│   ├── QueryableExtensions.cs      # Extensions cho IQueryable
│   └── StringExtensions.cs         # Extensions cho String
├── Helpers/
│   └── CommonHelper.cs             # Helpers: GenerateRandomCode, GenerateSlug, FormatPhoneNumber
├── CSDL7SharedModule.cs            # ABP Module
├── CSDL7.Shared.csproj             # Project file
└── README.md                       # Tài liệu
```

## 🚀 Cách sử dụng

### 1. Thêm Reference vào Service của bạn

Mở file `.csproj` của service (ví dụ: `CSDL7.MasterService.csproj`):

```xml
<ItemGroup>
  <ProjectReference Include="..\..\commons\CSDL7.Shared\CSDL7.Shared.csproj" />
</ItemGroup>
```

### 2. Thêm DependsOn vào Module

Trong file Module của service (ví dụ: `CSDL7MasterServiceModule.cs`):

```csharp
using CSDL7.Shared;

[DependsOn(
    typeof(CSDL7SharedModule),  // ← Thêm dòng này
    typeof(AbpAutofacModule),
    // ... các module khác
)]
public class CSDL7MasterServiceModule : AbpModule
{
    // ...
}
```

### 3. Build lại Service

```powershell
cd d:\CSDL.QG\CSDL7\services\master\CSDL7.MasterService
dotnet build
```

## 💡 Ví dụ Sử dụng

### Constants

```csharp
using CSDL7.Shared.Constants;

// Sử dụng hằng số độ dài
if (input.Name.Length > SharedConstants.MaxLength.Name)
{
    throw new Exception("Tên quá dài");
}

// Pattern validation
var emailPattern = SharedConstants.ValidationPatterns.Email;
```

### String Extensions

```csharp
using CSDL7.Shared.Extensions;

var text = "  Tiếng Việt  ";

// Check empty
if (!text.IsNullOrWhiteSpace())
{
    // Remove tones
    var normalized = text.RemoveVietnameseTones(); // "Tieng Viet"
    
    // Truncate
    var short = text.Truncate(10);
}
```

### DateTime Extensions

```csharp
using CSDL7.Shared.Extensions;

var today = DateTime.Now;

// Ngày đầu/cuối tháng
var firstDay = today.GetFirstDayOfMonth();
var lastDay = today.GetLastDayOfMonth();

// Ngày đầu/cuối tuần
var monday = today.GetStartOfWeek();
var sunday = today.GetEndOfWeek();

// Check weekend
var isWeekend = today.IsWeekend();

// Start/End of day
var startOfDay = today.StartOfDay();    // 00:00:00
var endOfDay = today.EndOfDay();        // 23:59:59.999
```

### Collection Extensions

```csharp
using CSDL7.Shared.Extensions;

var items = new List<string>();

// Check empty
if (items.IsNullOrEmpty())
{
    // Add multiple items
    items.AddRange(new[] { "A", "B", "C" });
}

// Remove items by condition
items.RemoveWhere(x => x.StartsWith("A"));
```

### Queryable Extensions

```csharp
using CSDL7.Shared.Extensions;

var query = _repository.AsQueryable();

// Conditional where
query = query.WhereIf(!keyword.IsNullOrEmpty(), x => x.Name.Contains(keyword));

// Paging
query = query.PageBy(skipCount, maxResultCount);
```

### Common Helpers

```csharp
using CSDL7.Shared.Helpers;

// Generate random code
var code = CommonHelper.GenerateRandomCode(8); // "AB12XY89"

// Generate slug
var slug = CommonHelper.GenerateSlug("Tiêu đề bài viết"); 
// Result: "tieu-de-bai-viet"

// Format phone
var phone = CommonHelper.FormatPhoneNumber("0123456789");
// Result: "0123 456 789"
```

### Base DTOs

```csharp
using CSDL7.Shared.Dtos;

// DTO kế thừa sẵn thông tin audit
public class DepartmentDto : AuditedEntityDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    // Id, CreationTime, CreatorId, ... đã có sẵn từ base
}

// Request phân trang
public class GetDepartmentsInput : PagedAndSortedRequestDto
{
    public string? Status { get; set; }
    // Keyword, SkipCount, MaxResultCount, Sorting đã có sẵn
}

// Response phân trang
public async Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
{
    var query = _repository.AsQueryable()
        .WhereIf(!input.Keyword.IsNullOrEmpty(), x => x.Name.Contains(input.Keyword));
    
    var totalCount = await query.CountAsync();
    var items = await query.PageBy(input.SkipCount, input.MaxResultCount).ToListAsync();
    var dtos = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(items);
    
    return new PagedResultDto<DepartmentDto>(totalCount, dtos);
}
```

### Enums

```csharp
using CSDL7.Shared.Enums;

public class Department
{
    public string Name { get; set; }
    public ActiveStatus Status { get; set; } = ActiveStatus.Active;
}

public class Person
{
    public string Name { get; set; }
    public Gender Gender { get; set; }
}
```

### Exceptions

```csharp
using CSDL7.Shared.Exceptions;

public async Task DeleteAsync(Guid id)
{
    var department = await _repository.FindAsync(id);
    if (department == null)
    {
        throw new BusinessException(
            message: "Không tìm thấy phòng ban",
            code: "DEPARTMENT_NOT_FOUND"
        );
    }
    
    // Check business rule
    if (department.HasEmployees)
    {
        throw new BusinessException(
            message: "Không thể xóa phòng ban đã có nhân viên",
            code: "DEPARTMENT_HAS_EMPLOYEES"
        );
    }
    
    await _repository.DeleteAsync(department);
}
```

## 📦 Services có thể sử dụng

Tất cả các services trong hệ thống đều có thể import và sử dụng thư viện này:

- ✅ MasterService
- ✅ IdentityService
- ✅ SaasService
- ✅ AdministrationService
- ✅ AuditLoggingService
- ✅ EmailService
- ✅ GdprService
- ✅ LanguageService

## 🎯 Lợi ích

1. **Tái sử dụng code**: Không cần viết lại các hàm chung
2. **Nhất quán**: Tất cả services dùng cùng logic, cùng chuẩn
3. **Dễ bảo trì**: Sửa 1 chỗ, tất cả services đều được cập nhật
4. **Tăng năng suất**: Tập trung vào business logic thay vì utility code

## 📝 Lưu ý

- Chỉ thêm code thực sự được dùng bởi **nhiều services**
- Giữ code **đơn giản** và **không phụ thuộc** vào business logic cụ thể
- Test kỹ trước khi thêm vào thư viện
- Document rõ ràng cho mỗi method/class

## 🔄 Mở rộng

Khi cần thêm utilities mới:

1. Tạo file trong thư mục tương ứng
2. Build thư viện: `dotnet build`
3. Các services reference sẽ tự động có thể sử dụng

## ✅ Build Status

```
✓ CSDL7.Shared.csproj - Build thành công
✓ Tất cả files đã được tạo
✓ Sẵn sàng để các services import
```

---

📚 Xem thêm tài liệu chi tiết tại: `services/commons/CSDL7.Shared/README.md`
📖 Hướng dẫn tích hợp: `services/commons/INTEGRATION_GUIDE.md`
```

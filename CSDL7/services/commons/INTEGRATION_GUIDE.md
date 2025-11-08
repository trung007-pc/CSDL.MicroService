# Hướng dẫn sử dụng CSDL7.Shared trong MasterService

## Bước 1: Thêm Project Reference

Mở file `CSDL7.MasterService.csproj` và thêm reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\commons\CSDL7.Shared\CSDL7.Shared.csproj" />
</ItemGroup>
```

## Bước 2: Thêm DependsOn vào Module

Mở file `CSDL7MasterServiceModule.cs` và thêm:

```csharp
using CSDL7.Shared;

[DependsOn(
    typeof(CSDL7SharedModule),  // Thêm dòng này
    typeof(AbpAutofacModule),
    // ... các module khác
)]
public class CSDL7MasterServiceModule : AbpModule
{
    // ...
}
```

## Bước 3: Sử dụng trong Code

### Ví dụ 1: Sử dụng trong DepartmentsAppService

```csharp
using CSDL7.Shared.Extensions;
using CSDL7.Shared.Helpers;
using CSDL7.Shared.Constants;

public class DepartmentsAppService : ApplicationService
{
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        // Validate độ dài
        if (input.Name.Length > SharedConstants.MaxLength.Name)
        {
            throw new BusinessException("Tên quá dài");
        }

        // Tạo code tự động nếu không có
        var code = input.Code.IsNullOrWhiteSpace() 
            ? CommonHelper.GenerateRandomCode(8)
            : input.Code;

        // Tạo slug từ tên
        var slug = CommonHelper.GenerateSlug(input.Name);

        var department = new Department
        {
            Name = input.Name,
            Code = code,
            Slug = slug,
            // ...
        };

        await _repository.InsertAsync(department);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }
}
```

### Ví dụ 2: Sử dụng Extensions

```csharp
using CSDL7.Shared.Extensions;

public class SomeService
{
    public void ProcessData()
    {
        var text = "  Công ty ABC  ";
        
        // Trim và check empty
        if (!text.IsNullOrWhiteSpace())
        {
            var cleaned = text.Trim();
            var noTones = cleaned.RemoveVietnameseTones();
            Console.WriteLine(noTones); // "Cong ty ABC"
        }

        // DateTime extensions
        var today = DateTime.Now;
        var startOfWeek = today.GetStartOfWeek();
        var endOfWeek = today.GetEndOfWeek();
        var isWeekend = today.IsWeekend();
    }
}
```

### Ví dụ 3: Sử dụng Enums

```csharp
using CSDL7.Shared.Enums;

public class Department
{
    public string Name { get; set; }
    public ActiveStatus Status { get; set; } = ActiveStatus.Active;
}
```

## Build Project

```powershell
cd d:\CSDL.QG\CSDL7\services\commons\CSDL7.Shared
dotnet build
```

## Test trong MasterService

```powershell
cd d:\CSDL.QG\CSDL7\services\master\CSDL7.MasterService
dotnet build
```

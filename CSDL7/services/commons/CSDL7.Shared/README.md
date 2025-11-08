# CSDL7.Shared

Thư viện chung chứa các utilities, helpers, extensions, constants được sử dụng xuyên suốt các services trong hệ thống CSDL7.

## Cấu trúc

```
CSDL7.Shared/
├── Constants/          # Các hằng số chung
├── Enums/             # Các enum chung
├── Exceptions/        # Các exception tùy chỉnh
├── Extensions/        # Extension methods
├── Helpers/           # Helper classes
└── CSDL7SharedModule.cs
```

## Cài đặt

Thêm reference vào project của bạn:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\commons\CSDL7.Shared\CSDL7.Shared.csproj" />
</ItemGroup>
```

Thêm DependsOn vào Module của service:

```csharp
[DependsOn(typeof(CSDL7SharedModule))]
public class YourServiceModule : AbpModule
{
    // ...
}
```

## Sử dụng

### Constants

```csharp
using CSDL7.Shared.Constants;

// Sử dụng hằng số
var maxLength = SharedConstants.MaxLength.Name;
var emailPattern = SharedConstants.ValidationPatterns.Email;
```

### Extensions

```csharp
using CSDL7.Shared.Extensions;

// String extensions
var text = "  Hello World  ";
var isEmpty = text.IsNullOrEmpty();
var truncated = text.Truncate(10);
var noTones = "Tiếng Việt".RemoveVietnameseTones(); // "Tieng Viet"

// DateTime extensions
var today = DateTime.Now;
var firstDay = today.GetFirstDayOfMonth();
var isWeekend = today.IsWeekend();
```

### Helpers

```csharp
using CSDL7.Shared.Helpers;

// Tạo mã code ngẫu nhiên
var code = CommonHelper.GenerateRandomCode(8);

// Tạo slug
var slug = CommonHelper.GenerateSlug("Tiêu đề bài viết"); // "tieu-de-bai-viet"

// Format số điện thoại
var phone = CommonHelper.FormatPhoneNumber("0123456789"); // "0123 456 789"
```

### Enums

```csharp
using CSDL7.Shared.Enums;

var status = ActiveStatus.Active;
var gender = Gender.Male;
```

### Exceptions

```csharp
using CSDL7.Shared.Exceptions;

throw new BusinessException("Lỗi nghiệp vụ", "ERROR_CODE");
```

## Phát triển

Khi cần thêm utilities/helpers chung cho nhiều services:

1. Thêm code vào thư mục tương ứng trong CSDL7.Shared
2. Build lại project
3. Các services khác sẽ tự động có thể sử dụng

## Lưu ý

- Chỉ thêm code thực sự được sử dụng bởi nhiều services
- Giữ code đơn giản và không phụ thuộc vào business logic cụ thể
- Đảm bảo test kỹ trước khi thêm vào thư viện này

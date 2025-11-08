namespace CSDL7.Shared.Enums;

/// <summary>
/// Trạng thái hoạt động
/// </summary>
public enum ActiveStatus
{
    /// <summary>
    /// Không hoạt động
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Đang hoạt động
    /// </summary>
    Active = 1,

    /// <summary>
    /// Tạm khóa
    /// </summary>
    Locked = 2,

    /// <summary>
    /// Đã xóa
    /// </summary>
    Deleted = 3
}

/// <summary>
/// Loại giới tính
/// </summary>
public enum Gender
{
    /// <summary>
    /// Không xác định
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Nam
    /// </summary>
    Male = 1,

    /// <summary>
    /// Nữ
    /// </summary>
    Female = 2
}

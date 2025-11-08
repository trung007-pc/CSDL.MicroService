namespace CSDL7.Shared;

public class DepartmentCreatedEto : IOutBoxPattern
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
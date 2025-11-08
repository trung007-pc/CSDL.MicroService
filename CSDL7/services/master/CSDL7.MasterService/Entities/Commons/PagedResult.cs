namespace CSDL7.MasterService.Entities.Commons;

public class PagedData<T>
{
    public long TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}
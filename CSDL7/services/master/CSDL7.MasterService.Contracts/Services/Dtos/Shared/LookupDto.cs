namespace CSDL7.MasterService.Services.Dtos.Shared
{
    public class LookupDto<TKey>
    {
        public TKey Id { get; set; }

        public string DisplayName { get; set; } = null!;
    }
}
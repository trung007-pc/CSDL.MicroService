namespace CSDL7.MasterService.Entities.Wards
{
    public static class WardConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Ward." : string.Empty);
        }

    }
}
namespace CSDL7.MasterService.Entities.Districts
{
    public static class DistrictConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "District." : string.Empty);
        }

    }
}
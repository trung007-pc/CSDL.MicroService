namespace CSDL7.MasterService.Entities.Departments
{
    public static class DepartmentConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Department." : string.Empty);
        }

    }
}
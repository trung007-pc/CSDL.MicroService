namespace CSDL7.EmailService.Entities.Pings
{
    public static class PingConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Ping." : string.Empty);
        }

    }
}
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSDL7.MasterService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class MasterServiceDbContextFactory : IDesignTimeDbContextFactory<MasterServiceDbContext>
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    public MasterServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MasterServiceDbContext>()
            .UseNpgsql(GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__MasterService_Migrations");
            });

        return new MasterServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(MasterServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{MasterServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

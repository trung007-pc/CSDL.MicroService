using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSDL7.EmailService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class EmailServiceDbContextFactory : IDesignTimeDbContextFactory<EmailServiceDbContext>
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    public EmailServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EmailServiceDbContext>()
            .UseNpgsql(GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__EmailService_Migrations");
            });

        return new EmailServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(EmailServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{EmailServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

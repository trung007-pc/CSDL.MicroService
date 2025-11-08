using CSDL7.EmailService.Entities.Pings;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;

namespace CSDL7.EmailService.Data;

[ConnectionStringName(DatabaseName)]
public class EmailServiceDbContext :
    AbpDbContext<EmailServiceDbContext>,
    IHasEventInbox,
    IHasEventOutbox
{
    public DbSet<Ping> Pings { get; set; } = null!;
    public const string DbTablePrefix = "";
    public const string DbSchema = null;

    public const string DatabaseName = "EmailService";

    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public EmailServiceDbContext(DbContextOptions<EmailServiceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        if (builder.IsHostDatabase())
        {
            builder.Entity<Ping>(b =>
            {
                b.ToTable(DbTablePrefix + "Pings", DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Ping.Name)).IsRequired();
                b.Property(x => x.Value).HasColumnName(nameof(Ping.Value));
            });

        }
    }
}
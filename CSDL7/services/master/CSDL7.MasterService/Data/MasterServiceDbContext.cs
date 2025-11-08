using CSDL7.MasterService.Entities.Wards;
using CSDL7.MasterService.Entities.Districts;
using CSDL7.MasterService.Entities.Provinces;
using CSDL7.MasterService.Entities.Departments;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;

namespace CSDL7.MasterService.Data;

[ConnectionStringName(DatabaseName)]
public class MasterServiceDbContext :
    AbpDbContext<MasterServiceDbContext>,
    IHasEventInbox,
    IHasEventOutbox
{
    public DbSet<Ward> Wards { get; set; } = null!;
    public DbSet<District> Districts { get; set; } = null!;
    public DbSet<Province> Provinces { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public const string DbTablePrefix = "";
    public const string DbSchema = null;

    public const string DatabaseName = "MasterService";

    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public MasterServiceDbContext(DbContextOptions<MasterServiceDbContext> options)
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
            builder.Entity<Department>(b =>
            {
                b.ToTable(DbTablePrefix + "Departments", DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Department.Name)).IsRequired();
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Province>(b =>
            {
                b.ToTable(DbTablePrefix + "Provinces", DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Province.Name)).IsRequired();
                b.Property(x => x.ProvinceCode).HasColumnName(nameof(Province.ProvinceCode));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<District>(b =>
            {
                b.ToTable(DbTablePrefix + "Districts", DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(District.Name)).IsRequired();
                b.Property(x => x.Code).HasColumnName(nameof(District.Code));
                b.HasOne<Province>().WithMany().IsRequired().HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Ward>(b =>
            {
                b.ToTable(DbTablePrefix + "Wards", DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Ward.Name)).IsRequired();
                b.Property(x => x.Code).HasColumnName(nameof(Ward.Code));
                b.HasOne<District>().WithMany().IsRequired().HasForeignKey(x => x.DistrictId).OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Operator)
                .WithMany()
                .HasForeignKey(o => o.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Engineer)
                .WithMany()
                .HasForeignKey(o => o.EngineerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(o => o.StatusId);
            entity.HasIndex(o => o.CreationDate);

            entity.Property(o => o.Model).HasMaxLength(100);
            entity.Property(o => o.Details).HasMaxLength(1000);
            entity.Property(o => o.ClientEmail).HasMaxLength(100);


            entity.HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId);

            entity.HasOne(o => o.DeviceType)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DeviceTypeId);

            entity.HasOne(o => o.ServiceType)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ServiceTypeId);
        });

        builder.Entity<Photo>(entity =>
        {
            entity.HasIndex(p => p.OrderId);

            entity.Property(p => p.Link)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(p => p.Order)
                .WithMany(o => o.Photos)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Status>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
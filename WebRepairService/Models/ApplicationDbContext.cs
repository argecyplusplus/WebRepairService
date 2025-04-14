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

        // Order relationships
        builder.Entity<Order>(entity =>
        {
            entity.HasIndex(o => o.StatusId);
            entity.HasIndex(o => o.CreationDate);
            entity.HasIndex(o => o.UserId);

            entity.Property(o => o.Model).HasMaxLength(100);
            entity.Property(o => o.Details).HasMaxLength(1000);
            entity.Property(o => o.ClientEmail).HasMaxLength(100);

            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

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

        // Photo configuration
        builder.Entity<Photo>(entity =>
        {
            entity.HasIndex(p => p.OrderId);

            entity.Property(p => p.Link)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(p => p.Order)
                .WithMany(o => o.Photos)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Или Restrict в зависимости от требований
        });

        // Дополнительные настройки для других сущностей
        builder.Entity<Status>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
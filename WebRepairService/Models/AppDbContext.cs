using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

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

        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

        builder.Entity<Order>()
            .HasOne(o => o.Status)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.StatusId);

        builder.Entity<Order>()
            .HasOne(o => o.DeviceType)
            .WithMany(d => d.Orders)
            .HasForeignKey(o => o.DeviceTypeId);

        builder.Entity<Order>()
            .HasOne(o => o.ServiceType)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ServiceTypeId);

        builder.Entity<Photo>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Photos)
            .HasForeignKey(p => p.OrderId);
    }
}

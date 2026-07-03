using EcoMeal.EcoMealAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Infrastructure;

public class EcoMealDBContext : DbContext
{
    public EcoMealDBContext(DbContextOptions<EcoMealDBContext> options) 
        :base(options)
    {}

    public DbSet<User> User { get; set;}
    public DbSet<BusinessType> BusinessType { get; set;}
    public DbSet<PackageType> PackageType { get; set;}
    public DbSet<Business> Businesses { get; set;}
    public DbSet<Package> Package { get; set;}
    public DbSet<Order> Orders { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Business configuration
        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Business -> BusinessType (Many-to-One)
            entity.HasOne(b => b.BusinessType)
                .WithMany(bt => bt.Businesses)
                .HasForeignKey(b => b.BusinessTypeId);
            
            // Business -> Packages (One-to-Many)
            entity.HasMany(b => b.Packages)
                .WithOne(p => p.Business)
                .HasForeignKey(p => p.BusinessId);
        });

        // Package configuration
        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Package -> PackageType (Many-to-One)
            entity.HasOne(p => p.PackageType)
                .WithMany(pt => pt.Packages)
                .HasForeignKey(p => p.PackageTypeId);
            
            // Package -> Orders (One-to-Many)
            entity.HasMany(p => p.Orders)
                .WithOne(o => o.Package)
                .HasForeignKey(o => o.PackageId);
            
            // Price precision
            entity.Property(p => p.Price)
                .HasPrecision(5, 2);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // User -> Orders (One-to-Many)
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
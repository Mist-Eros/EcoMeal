using EcoMeal.EcoMealAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Infrastructure;

public class EcoMealDBContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public EcoMealDBContext(DbContextOptions<EcoMealDBContext> options) 
        :base(options)
    {}
    public DbSet<BusinessType> BusinessType { get; set;}
    public DbSet<PackageType> PackageType { get; set;}
    public DbSet<Business> Businesses { get; set;}
    public DbSet<Package> Package { get; set;}
    public DbSet<Order> Orders { get; set;}
    public DbSet<Rating> Ratings { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(b => b.BusinessType)
                .WithMany(bt => bt.Businesses)
                .HasForeignKey(b => b.BusinessTypeId);
            
            entity.HasMany(b => b.Packages)
                .WithOne(p => p.Business)
                .HasForeignKey(p => p.BusinessId);
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(p => p.PackageType)
                .WithMany(pt => pt.Packages)
                .HasForeignKey(p => p.PackageTypeId);
            
            entity.HasMany(p => p.Orders)
                .WithOne(o => o.Package)
                .HasForeignKey(o => o.PackageId);
            
            entity.Property(p => p.Price)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            entity.HasOne(r => r.Business)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BusinessId);
        });
    }
}
using EcoMeal.EcoMealAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Infrastructure;

public class EcoMealDBContext : DbContext
{
    public EcoMealDBContext(DbContextOptions<EcoMealDBContext> options) 
        :base(options)
    {}

    public DbSet<User> User { get; set;}
}
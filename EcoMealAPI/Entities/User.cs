namespace EcoMeal.EcoMealAPI.Entities;

public class User
{
    public int Id { get; set;}
    public required string Name { get; set;}
    public required string Email { get; set;}
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
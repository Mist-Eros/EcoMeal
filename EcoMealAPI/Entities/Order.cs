using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class Order
{
    public int Id { get; set;}
    
    [ForeignKey(nameof(User))]
    public int UserId { get; set;}
    
    [ForeignKey(nameof(Package))]
    public int PackageId { get; set;}
    
    public short Status { get; set;}
    public DateTime Date { get; set;}
    
    public User User { get; set; }
    public Package Package { get; set; }
}
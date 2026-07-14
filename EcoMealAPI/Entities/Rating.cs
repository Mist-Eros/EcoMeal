using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class Rating
{
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [ForeignKey(nameof(Business))]
    public int BusinessId { get; set; }

    public int Stars { get; set; }

    public User User { get; set; }
    public Business Business { get; set; }
}

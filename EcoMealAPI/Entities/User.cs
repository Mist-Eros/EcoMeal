namespace EcoMeal.EcoMealAPI.Entities;

public class User
{
    public int Id { get; set;}
    public required string Name { get; set;}
    // Can also have it as
  /*public sttring Name { get; set;} = "";
    
  */
    public required string Contact { get; set;}
}
namespace EcoMeal.EcoMealAPI.Models;

public class OrderGetDTO
{
    public int Id { get; set;}
    public required string PackageName { get; set;}
    public required int Status { get; set;}
    public decimal Price { get; set;}
    public int BusinessId { get; set;}
    public required string BusinessName { get; set;}
    public DateTime Date { get; set;}
    public string? UserName { get; set;}
    public string? UserContact { get; set;}
    public string? UserEmail { get; set;}
}
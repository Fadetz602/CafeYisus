using CafeYisus.Models;

namespace CafeYisus.DTOs
{
    public class DrinkDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
       public string? CategoryName {  get; set; }
    }
}

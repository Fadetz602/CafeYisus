namespace CafeYisus.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public ICollection<Drink> Drinks { get; set; } = new List<Drink>();
    }
}

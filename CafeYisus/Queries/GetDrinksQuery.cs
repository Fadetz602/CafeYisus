namespace CafeYisus.Queries
{
    public class GetDrinksQuery : ObjectQuery
    {

        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }

    }
}

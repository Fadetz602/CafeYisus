namespace CafeYisus.Queries
{
    public class GetCategoriesQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? Name { get; set; }

    }
}

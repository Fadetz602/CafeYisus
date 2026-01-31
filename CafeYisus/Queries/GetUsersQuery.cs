namespace CafeYisus.Queries
{
    public class GetUsersQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? OrderBy { get; set; } = "id";
    }
}

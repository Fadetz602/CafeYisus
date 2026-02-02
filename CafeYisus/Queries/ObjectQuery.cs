namespace CafeYisus.Queries
{
    public class ObjectQuery
    {
        private int page;
        public int Page
        {
            get { return page; }
            set
            {
                if (page <= 0) page = 1;
            }
        }
        private int pageSize;
        public int PageSize
        {
            get { return pageSize; }
            set { if (pageSize <= 0) pageSize = 10; }
        }
        public string? OrderBy { get; set; } = "id";
    }
}

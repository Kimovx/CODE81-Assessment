namespace CODE81_Assessment.Application.Common
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}

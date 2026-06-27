using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookByStatusDto
    {
        public BookStatus Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

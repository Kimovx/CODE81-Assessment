namespace CODE81_Assessment.Application.DTOs.UserLogs
{
    public class UserLogDto
    {
        public int Id { get; set; }

        public DateTimeOffset LogTime { get; set; }

        public bool IsSuccess { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? Action { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; } = null!;
    }
}

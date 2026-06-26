namespace CODE81_Assessment.Application.DTOs.Auth
{
    public class AuthRequestDto
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}

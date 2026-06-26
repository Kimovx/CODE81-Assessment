namespace CODE81_Assessment.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public required string AccessToken { get; set; }

        public string? RefreshToken { get; set; } // For Internal Using 
    }
}

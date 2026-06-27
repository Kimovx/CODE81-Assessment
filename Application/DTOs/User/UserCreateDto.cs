using System.ComponentModel.DataAnnotations;

namespace CODE81_Assessment.Application.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        public required string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public required string Password { get; set; } = null!;

        public required int RoleId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CODE81_Assessment.Application.DTOs.User
{
    public class UserUpdateDto
    {
        public string UserName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public int? RoleId { get; set; }
    }
}

using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public Role Role { get; set; }
    }
}

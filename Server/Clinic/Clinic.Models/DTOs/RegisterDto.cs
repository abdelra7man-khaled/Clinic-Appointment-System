using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public Role Role { get; set; }
    }
}

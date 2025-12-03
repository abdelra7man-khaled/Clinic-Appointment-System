using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username;
        [EmailAddress]
        public string Email;
        [Required]
        public string Password;
        Role Role;
    }
}

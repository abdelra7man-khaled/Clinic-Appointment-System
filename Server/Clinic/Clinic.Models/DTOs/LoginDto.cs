using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Username;
        [Required]
        public string Password;
    }
}

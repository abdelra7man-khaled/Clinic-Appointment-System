using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }

        public AuthResponseDto(string token, string username, Role role)
        {
            Token = token;
            Username = username;
            Role = role;
        }
    }
}

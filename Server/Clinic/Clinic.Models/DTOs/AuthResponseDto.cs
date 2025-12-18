using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; }
        public string Username { get; }
        public Role Role { get; }

        public AuthResponseDto(string Token, string Username, Role Role)
        {
            this.Token = Token;
            this.Username = Username;
            this.Role = Role;
        }
    }
}

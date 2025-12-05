using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class AuthResponseDto
    {
        public string Token;
        public string Username;
        public Role Role;

        public AuthResponseDto(string Token, string Username, Role Role)
        {
            this.Token = Token;
            this.Username = Username;
            this.Role = Role;
        }
    }
}

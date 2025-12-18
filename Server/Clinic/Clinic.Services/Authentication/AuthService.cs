using Clinic.DataAccess.Repository.IRepository;
using Clinic.Services.Exceptions;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clinic.Services.Authentication
{
    public class AuthService(IUnitOfWork _unitOfWork, IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _jwt = configuration.GetSection("JwtSettings");

        private string Hash(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
        private string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwt["SecretKey"]!);
            var claims = new List<Claim> {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_jwt["TokenExpiryInMinutes"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwt["Issuer"],
                Audience = _jwt["Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var exists = _unitOfWork.Users.Query().Any(u => u.Username == registerDto.Username ||
                                                        u.Email == registerDto.Email);
            if (exists)
                throw new ServiceException("User already exists");

            var user = new ApplicationUser
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = Hash(registerDto.Password),
                Role = registerDto.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();


            int[] RandomBalances = { 5000, 3700, 1500, 900, 250 };
            if (user.Role == Role.Patient)
            {
                await _unitOfWork.Patients.AddAsync(new Patient
                {
                    UserId = user.Id,
                    FullName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber!,
                    Balance = RandomBalances[Random.Shared.Next(RandomBalances.Length)]
                });
            }
            if (user.Role == Role.Doctor)
            {
                await _unitOfWork.Doctors.AddAsync(new Doctor
                {
                    UserId = user.Id,
                    FullName = registerDto.FullName,
                    Biography = registerDto.Biography!
                });
            }
            await _unitOfWork.SaveChangesAsync();


            var token = GenerateToken(user);
            return new AuthResponseDto(token, user.Username, user.Role);
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var hash = Hash(dto.Password);
            var user = _unitOfWork.Users.Query().FirstOrDefault(u => (u.Username == dto.Username || u.Email == dto.Username) &&
                                                                u.PasswordHash == hash);

            if (user is null)
                throw new ServiceException("Invalid credentials");

            var token = GenerateToken(user);
            return new AuthResponseDto(token, user.Username, user.Role);
        }

    }
}

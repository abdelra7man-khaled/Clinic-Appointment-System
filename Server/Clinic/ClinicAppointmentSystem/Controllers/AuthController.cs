using Clinic.Models.DTOs;
using Clinic.Services.Authentication;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _auth) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            Logger.Instance.LogInfo("/Auth/register - User Registeration");

            try
            {
                var response = await _auth.RegisterAsync(dto);
                Logger.Instance.LogSuccess($"/Auth/register - New User [username={response.Username}] Registeration Successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("/Auth/register -  User Registeration Failed");

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            Logger.Instance.LogInfo("/Auth/login - User Login");
            try
            {
                var response = await _auth.LoginAsync(dto);
                Logger.Instance.LogSuccess($"/Auth/login - New User [username={response.Username}]SignIn Successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("/Auth/login -  User SignIn Failed");

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}

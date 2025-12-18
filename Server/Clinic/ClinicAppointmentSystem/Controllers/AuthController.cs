using Clinic.Models.DTOs;
using Clinic.Services.Authentication;
using Clinic.Services.Logging;
using Clinic.Services.Exceptions;
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
            catch (ServiceException ex)
            {
                Logger.Instance.LogWarning($"/Auth/register - Business Error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError($"/Auth/register - User Registeration Failed: {ex}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
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
            catch (ServiceException ex)
            {
                Logger.Instance.LogWarning($"/Auth/login - Business Error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError($"/Auth/login - User SignIn Failed: {ex}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}

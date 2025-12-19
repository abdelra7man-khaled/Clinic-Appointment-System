using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecialtyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] SpecialtyDto specialtyDto)
        {
            Logger.Instance.LogInfo("/api/Specialty - Creating new specialty");

            if (string.IsNullOrEmpty(specialtyDto.Name))
            {
                return BadRequest("Specialty name is required");
            }

            // Check if exists
            var exists = _unitOfWork.Specialties.Query().Any(s => s.Name.ToLower() == specialtyDto.Name.ToLower());
            if (exists)
            {
                return BadRequest("Specialty already exists");
            }

            var specialty = new Specialty { Name = specialtyDto.Name };
            await _unitOfWork.Specialties.AddAsync(specialty);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/api/Specialty - Successfully created specialty: {specialty.Name}");
            return Ok(specialty);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Allow public or authorized access mainly for dropdowns
            var specialties = _unitOfWork.Specialties.Query().ToList();
            return Ok(specialties);
        }
    }
}

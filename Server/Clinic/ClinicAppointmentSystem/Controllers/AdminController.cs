using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IUnitOfWork _unitOfWork) : ControllerBase
    {

        [HttpGet("doctors")]
        public IActionResult GetAllDoctors()
        {
            Logger.Instance.LogInfo("/admin/doctors/ - Fetching all doctors with specialties");

            var doctors = _unitOfWork.Doctors.Query()
                        .Include(d => d.User)
                        .Include(d => d.DoctorSpecialties)
                        .ThenInclude(ds => ds.Specialty)
                        .ToList()
                        .Select(d => new
                        {
                            d.Id,
                            d.FullName,
                            d.Biography,
                            Email = d.User.Email,
                            Specialties = d.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
                        });
            Logger.Instance.LogSuccess("/admin/doctors/ - Successfully fetched all doctors with specialties");

            return Ok(doctors);
        }

        [HttpPost("add/doctor")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDto doctorDto)
        {
            Logger.Instance.LogInfo("/admin/add/doctor/ - Add new doctor");

            var user = new ApplicationUser
            {
                Username = doctorDto.Username,
                Email = doctorDto.Email,
                Role = Role.Doctor,
                PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes("Default@123"))
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var newDoctor = new Doctor
            {
                UserId = user.Id,
                FullName = doctorDto.FullName,
                Biography = doctorDto.Biography
            };

            await _unitOfWork.Doctors.AddAsync(newDoctor);
            await _unitOfWork.SaveChangesAsync();

            if (doctorDto.SpecialtyIds != null && doctorDto.SpecialtyIds.Any())
            {
                foreach (var s in doctorDto.SpecialtyIds)
                {
                    await _unitOfWork.DoctorSpecialties.AddAsync(new DoctorSpecialty
                    {
                        DoctorId = newDoctor.Id,
                        SpecialtyId = s
                    });
                }
                await _unitOfWork.SaveChangesAsync();
            }

            Logger.Instance.LogSuccess("/admin/add/doctor/ - Doctor added successfully");

            return Ok(new
            {
                Message = "Doctor added successfully",
                Doctor = newDoctor
            });
        }

        [HttpGet("specialties")]
        public IActionResult GetAllSpecialties()
        {
            Logger.Instance.LogInfo("/admin/specialties - Fetch all specialties");

            var specialties = _unitOfWork.Specialties.Query()
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .ToList();

            Logger.Instance.LogSuccess($"/admin/specialties - Returned {specialties.Count} specialties");

            return Ok(specialties);
        }

        [HttpGet("specialties/{id}")]
        public IActionResult GetSpecialtyById(int id)
        {
            Logger.Instance.LogInfo($"/admin/specialties/{id} - Fetch specialty");

            var specialties = _unitOfWork.Specialties.Query()
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .ToList();

            Logger.Instance.LogSuccess($"/admin/specialties/{id} - Returned Required specialty successfully");

            return Ok(specialties);
        }

        [HttpPost("specialty/add")]
        public async Task<IActionResult> AddSpecialty([FromBody] Specialty newSpecialty)
        {
            Logger.Instance.LogInfo("/admin/add/specialty/ - Add new specialty");

            if (string.IsNullOrWhiteSpace(newSpecialty.Name))
            {
                Logger.Instance.LogError("/admin/add/specialty/ - Specialty name is required");
                return BadRequest();
            }

            await _unitOfWork.Specialties.AddAsync(newSpecialty);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess("/admin/add/specialty/ - Specialty added successfully");

            return Ok(newSpecialty);
        }

        [HttpDelete("/delete/doctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            Logger.Instance.LogInfo($"admin/delete/doctor/{id} - Delete Doctor");

            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null)
            {
                Logger.Instance.LogError($"admin/delete/doctor/{id} - Doctor not found");
                return NotFound();
            }


            var appointments = _unitOfWork.Appointments.Query()
                .Where(a => a.DoctorId == doctor.Id)
                .ToList();
            _unitOfWork.Appointments.RemoveRange(appointments);

            var specialties = _unitOfWork.DoctorSpecialties.Query()
                .Where(ds => ds.DoctorId == doctor.Id)
                .ToList();
            _unitOfWork.DoctorSpecialties.RemoveRange(specialties);


            _unitOfWork.Doctors.Remove(doctor);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"admin/delete/doctor/{id} - Doctor deleted successfully");

            return Ok(new { Message = "Doctor deleted successfully" });
        }
    }
}

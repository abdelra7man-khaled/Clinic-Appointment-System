using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpPost("add/doctors")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDto doctorDto)
        {

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

            return Ok(new
            {
                Message = "Doctor added successfully",
                Doctor = newDoctor
            });
        }



        [HttpGet("doctors")]
        public IActionResult GetAllDoctors()
        {
            return Ok(_unitOfWork.Doctors.Query().Include(d => d.User).ToList());
        }

        [HttpPost("add/specialty")]
        public async Task<IActionResult> AddSpecialty([FromBody] Specialty newSpecialty)
        {
            if (string.IsNullOrWhiteSpace(newSpecialty.Name))
                return BadRequest("Specialty name is required");

            await _unitOfWork.Specialties.AddAsync(newSpecialty);
            await _unitOfWork.SaveChangesAsync();

            return Ok(newSpecialty);
        }

        [HttpGet("doctors/{id}")]
        public IActionResult GetDoctor(int id)
        {
            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.User)
                                    .Include(d => d.DoctorSpecialties)
                                    .ThenInclude(ds => ds.Specialty)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
                return NotFound("Doctor not found");

            return Ok(new
            {
                doctor.Id,
                doctor.FullName,
                doctor.Biography,
                Specialties = doctor.DoctorSpecialties
                                    .Select(s => s.Specialty.Name)
                                    .ToList()
            });
        }


        [HttpPut("doctors/{id}/update/specialties")]
        public async Task<IActionResult> UpdateDoctorSpecialties(int id, [FromBody] UpdateDoctorSpecialtiesDto newSpecialtiesDto)
        {
            var doctor = _unitOfWork.Doctors.Query()
                .Include(d => d.DoctorSpecialties)
                .FirstOrDefault(d => d.Id == id);

            if (doctor == null)
                return NotFound("Doctor not found");

            var oldSpecialties = doctor.DoctorSpecialties.ToList();
            foreach (var s in oldSpecialties)
                _unitOfWork.DoctorSpecialties.Remove(s);

            foreach (var specialtyId in newSpecialtiesDto.SpecialtyIds)
            {
                await _unitOfWork.DoctorSpecialties.AddAsync(new DoctorSpecialty
                {
                    DoctorId = doctor.Id,
                    SpecialtyId = specialtyId
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return Ok("Doctor specialties updated successfully");
        }


    }
}

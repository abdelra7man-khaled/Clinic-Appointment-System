using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Admin")]
    public class DoctorController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            Logger.Instance.LogInfo("doctor/all - Fetching all doctors with specialties");

            var doctors = _unitOfWork.Doctors.Query()
                                .Include(d => d.User)
                                .ToList()
                                .Select(d => new
                                {
                                    d.Id,
                                    d.FullName,
                                    Specialties = d.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
                                });

            Logger.Instance.LogSuccess("doctor/all - Successfully fetched all doctors with specialties");
            return Ok(doctors);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Logger.Instance.LogInfo($"doctor/{id} - Fetching doctor details");

            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.User)
                                    .Include(d => d.DoctorSpecialties)
                                    .ThenInclude(ds => ds.Specialty)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id} - Doctor not found");
                return NotFound();
            }

            Logger.Instance.LogSuccess($"doctor/{id} - Successfully fetched doctor details");

            return Ok(new
            {
                doctor.Id,
                doctor.FullName,
                doctor.Biography,
                Specialties = doctor.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
            });

        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            Logger.Instance.LogInfo($"doctor/{id}/update - Updating doctor information");

            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id}/update - Doctor not found");
                return NotFound();
            }

            doctor.FullName = updateDoctorDto.FullName ?? doctor.FullName;
            doctor.Biography = updateDoctorDto.Biography ?? doctor.Biography;

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"doctor/{id}/update - Successfully updated doctor information");

            return Ok(doctor);
        }

        [HttpPut("{id}/specialties/update")]
        public async Task<IActionResult> UpdateSpecialties(int id, [FromBody] UpdateDoctorSpecialtiesDto updateDoctorSpecialtiesDto)
        {
            Logger.Instance.LogInfo($"doctor/{id}/specialties/update - Updating doctor specialties");

            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.DoctorSpecialties)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id}/specialties/update - Doctor not found");
                return NotFound();
            }

            var oldSpecialties = doctor.DoctorSpecialties.ToList();

            _unitOfWork.DoctorSpecialties.RemoveRange(oldSpecialties);

            foreach (var specialtyId in updateDoctorSpecialtiesDto.SpecialtyIds)
            {
                await _unitOfWork.DoctorSpecialties.AddAsync(new DoctorSpecialty
                {
                    DoctorId = doctor.Id,
                    SpecialtyId = specialtyId
                });
            }

            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"doctor/{id}/specialties/update - Successfully updated doctor specialties");

            return Ok("Specialties updated successfully");
        }

    }
}

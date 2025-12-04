using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var doctors = _unitOfWork.Doctors.Query()
                                .Include(d => d.User)
                                .ToList()
                                .Select(d => new
                                {
                                    d.Id,
                                    d.FullName,
                                    Specialties = d.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
                                });

            return Ok(doctors);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
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
                Specialties = doctor.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
            });
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null)
                return NotFound("Doctor not found");

            doctor.FullName = updateDoctorDto.FullName ?? doctor.FullName;
            doctor.Biography = updateDoctorDto.Biography ?? doctor.Biography;

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            return Ok(doctor);
        }

        [HttpPut("{id}/specialties/update")]
        public async Task<IActionResult> UpdateSpecialties(int id, [FromBody] UpdateDoctorSpecialtiesDto updateDoctorSpecialtiesDto)
        {
            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.DoctorSpecialties)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
                return NotFound("Doctor not found");

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
            return Ok("Specialties updated successfully");
        }

    }
}

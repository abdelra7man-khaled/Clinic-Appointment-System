using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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

            var specialty = _unitOfWork.Specialties.Query()
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .FirstOrDefault();

            if (specialty == null) return NotFound();

            Logger.Instance.LogSuccess($"/admin/specialties/{id} - Returned Required specialty successfully");

            return Ok(specialty);
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

        [HttpDelete("/api/delete/doctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            Logger.Instance.LogInfo($"admin/delete/doctor/{id} - Delete Doctor");

            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null)
            {
                Logger.Instance.LogError($"admin/delete/doctor/{id} - Doctor not found");
                return NotFound();
            }

            // 1. Delete Appointments & Associated Payments
            var appointments = _unitOfWork.Appointments.Query()
                .Where(a => a.DoctorId == doctor.Id)
                .ToList();

            if (appointments.Any())
            {
                var appointmentIds = appointments.Select(a => a.Id).ToList();
                var payments = _unitOfWork.Payments.Query()
                    .Where(p => appointmentIds.Contains(p.AppointmentId))
                    .ToList();
                
                if (payments.Any())
                {
                    _unitOfWork.Payments.RemoveRange(payments);
                }

                _unitOfWork.Appointments.RemoveRange(appointments);
            }

            // 2. Delete Specialties
            var specialties = _unitOfWork.DoctorSpecialties.Query()
                .Where(ds => ds.DoctorId == doctor.Id)
                .ToList();
            if (specialties.Any())
            {
                _unitOfWork.DoctorSpecialties.RemoveRange(specialties);
            }

            // 3. Delete Reviews
            var reviews = _unitOfWork.Reviews.Query()
                .Where(r => r.DoctorId == doctor.Id)
                .ToList();
            if (reviews.Any())
            {
                _unitOfWork.Reviews.RemoveRange(reviews);
            }

            // 4. Delete Schedules
            var schedules = _unitOfWork.DoctorSchedules.Query()
                .Where(s => s.DoctorId == doctor.Id)
                .ToList();
            if (schedules.Any())
            {
                _unitOfWork.DoctorSchedules.RemoveRange(schedules);
            }

            // 5. Delete PatientFavorites
            var favorites = _unitOfWork.PatientFavorites.Query()
                .Where(f => f.DoctorId == doctor.Id)
                .ToList();
            if (favorites.Any())
            {
                _unitOfWork.PatientFavorites.RemoveRange(favorites);
            }

            _unitOfWork.Doctors.Remove(doctor);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"admin/delete/doctor/{id} - Doctor deleted successfully");

            return Ok(new { Message = "Doctor deleted successfully" });
        }
        [HttpGet("dashboard/stats")]
        public IActionResult GetDashboardStats()
        {
            Logger.Instance.LogInfo("/api/Admin/dashboard/stats - Fetching dashboard stats");

            var totalPatients = _unitOfWork.Patients.Query().Count();
            var totalDoctors = _unitOfWork.Doctors.Query().Count();
            var totalAppointments = _unitOfWork.Appointments.Query().Count();
            var totalRevenue = _unitOfWork.Payments.Query()
                .Where(p => p.IsConfirmed)
                .Sum(p => p.Amount);

            var stats = new
            {
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalAppointments = totalAppointments,
                TotalRevenue = totalRevenue
            };

            Logger.Instance.LogSuccess("/api/Admin/dashboard/stats - Successfully fetched dashboard stats");

            return Ok(stats);
        }

        [HttpGet("patients")]
        public IActionResult GetAllPatients()
        {
            Logger.Instance.LogInfo("/api/Admin/patients - Fetching all patients");

            var patients = _unitOfWork.Patients.Query()
                .Include(p => p.User)
                .Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.Gender,
                    p.DateOfBirth,
                    p.PhoneNumber,
                    p.Address,
                    p.BloodType,
                    p.Height,
                    p.Weight,
                    p.Allergies,
                    Email = p.User.Email,
                    Username = p.User.Username
                })
                .ToList();

            Logger.Instance.LogSuccess($"/api/Admin/patients - Successfully fetched {patients.Count} patients");

            return Ok(patients);
        }

        [HttpGet("appointments")]
        public IActionResult GetAllAppointments()
        {
            Logger.Instance.LogInfo("/api/Admin/appointments - Fetching all appointments");

            var appointments = _unitOfWork.Appointments.Query()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Select(a => new
                {
                    a.Id,
                    PatientName = a.Patient.FullName,
                    DoctorName = a.Doctor.FullName,
                    a.StartTime,
                    a.EndTime,
                    Status = a.Status.ToString(),
                    AppointmentType = a.AppointmentType.ToString(),
                    a.Fee,
                    a.Notes,
                    a.IsPaid,
                    a.PaymentTransactionId
                })
                .ToList();

            Logger.Instance.LogSuccess($"/api/Admin/appointments - Successfully fetched {appointments.Count} appointments");

            return Ok(appointments);
        }
    }
}

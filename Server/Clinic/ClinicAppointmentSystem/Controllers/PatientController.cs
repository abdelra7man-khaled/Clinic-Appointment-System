using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Models;
using Clinic.Services.Appointments;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult GetProfile()
        {
            Logger.Instance.LogInfo("/patient/me - Fetching patient profile");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query()
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userId);

            if (patient == null) return NotFound("Patient profile not found");

            var dto = new PatientDto
            {
                Id = patient.Id,
                Username = User.FindFirst(ClaimTypes.Name)!.Value,
                Email = patient.User.Email, 
                FullName = patient.FullName,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                Age = patient.DateOfBirth.HasValue ? (int)((DateTime.Now - patient.DateOfBirth.Value).TotalDays / 365.25) : null,
                BloodType = patient.BloodType,
                Height = patient.Height,
                Weight = patient.Weight,
                Address = patient.Address,
                Allergies = patient.Allergies,
                PhoneNumber = patient.PhoneNumber,
                Balance = patient.Balance,
                IsProfileComplete = !string.IsNullOrEmpty(patient.Gender) && patient.DateOfBirth.HasValue && !string.IsNullOrEmpty(patient.PhoneNumber)
            };

            Logger.Instance.LogSuccess("/patient/me - Successfully fetched patient profile");
            return Ok(dto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientDto dto)
        {
            Logger.Instance.LogInfo("/patient/profile - Updating patient profile");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);

            if (patient == null) return NotFound("Patient profile not found");

            patient.FirstName = dto.FirstName ?? patient.FirstName;
            patient.LastName = dto.LastName ?? patient.LastName;
            patient.Gender = dto.Gender ?? patient.Gender;
            patient.DateOfBirth = dto.DateOfBirth ?? patient.DateOfBirth;
            patient.BloodType = dto.BloodType ?? patient.BloodType;
            patient.Height = dto.Height ?? patient.Height;
            patient.Weight = dto.Weight ?? patient.Weight;
            patient.Address = dto.Address ?? patient.Address;
            patient.Allergies = dto.Allergies ?? patient.Allergies;
            patient.PhoneNumber = dto.PhoneNumber ?? patient.PhoneNumber;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess("/patient/profile - Successfully updated patient profile");
            return Ok("Profile updated successfully");
        }

        [HttpPost("doctors/favorite/{doctorId}")]
        public async Task<IActionResult> ToggleFavorite(int doctorId)
        {
             Logger.Instance.LogInfo($"/patient/doctors/favorite/{doctorId} - Toggling favorite");
             var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
             var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
             if (patient == null) return NotFound("Patient not found");

             var existing = _unitOfWork.PatientFavorites.Query()
                 .FirstOrDefault(pf => pf.PatientId == patient.Id && pf.DoctorId == doctorId);

             if (existing != null)
             {
                 _unitOfWork.PatientFavorites.Remove(existing);
                 await _unitOfWork.SaveChangesAsync();
                 return Ok("Doctor removed from favorites");
             }
             else
             {
                 await _unitOfWork.PatientFavorites.AddAsync(new PatientFavorite { PatientId = patient.Id, DoctorId = doctorId });
                 await _unitOfWork.SaveChangesAsync();
                 return Ok("Doctor added to favorites");
             }
        }

        [HttpGet("doctors/favorites")]
        public IActionResult GetFavorites()
        {
            Logger.Instance.LogInfo("/patient/doctors/favorites - Fetching favorite doctors");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient == null) return NotFound("Patient not found");

            var favorites = _unitOfWork.PatientFavorites.Query()
                .Where(pf => pf.PatientId == patient.Id)
                .Include(pf => pf.Doctor)
                .Select(pf => new 
                { 
                    pf.Doctor.Id, 
                    pf.Doctor.FullName, 
                    pf.Doctor.PhotoUrl, 
                    pf.Doctor.AverageRating 
                })
                .ToList();

            return Ok(favorites);
        }

        [HttpGet("appointments/upcoming")]
        public IActionResult GetUpcomingAppointments()
        {
            Logger.Instance.LogInfo("/patient/appointments/upcoming - Fetching upcoming appointments");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient == null) return NotFound("Patient not found");

            var now = DateTime.UtcNow;
            var upcomingWindow = now.AddDays(3);

            var appointments = _unitOfWork.Appointments.Query()
                .Where(a => a.PatientId == patient.Id && a.StartTime >= now && a.StartTime <= upcomingWindow)
                .Include(a => a.Doctor)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    DoctorName = a.Doctor.FullName,
                    DoctorPhoto = a.Doctor.PhotoUrl,
                    a.StartTime,
                    a.EndTime,
                    a.Status
                })
                .ToList();

            return Ok(appointments);
        }
        [HttpPost("appointments/book")]
        public async Task<IActionResult> Book([FromBody] AppointmentCreateDto appointmentRequest)
        {
            Logger.Instance.LogInfo($"/patient/appointments/book - Patient Book Appointment");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient == null)
            {
                Logger.Instance.LogError("/patient/appointments/book - patient not exist");
                return BadRequest("Patient profile not found. Please complete your profile first.");
            }


            // check if exist booking in this time slot
            var overlappingAppointment = _unitOfWork.Appointments.Query()
                .FirstOrDefault(a => a.DoctorId == appointmentRequest.DoctorId && a.Status != AppointmentStatus.Cancelled
                    && a.StartTime < appointmentRequest.EndTime && appointmentRequest.StartTime < a.EndTime);

            if (overlappingAppointment != null)
            {
                Logger.Instance.LogWarning($"/patient/appointments/book - Time slot overlap with Appt {overlappingAppointment.Id}");
                return BadRequest($"Time slot not available. Overlaps with existing appointment: {overlappingAppointment.StartTime:HH:mm} - {overlappingAppointment.EndTime:HH:mm}");
            }


            var newAppointment = AppointmentFactory.CreateAppointment(
                appointmentRequest.AppointmentType,
                patient.Id,
                appointmentRequest.DoctorId,
                appointmentRequest.StartTime,
                appointmentRequest.EndTime,
                appointmentRequest.Fee,
                appointmentRequest.Notes
            );

            await _unitOfWork.Appointments.AddAsync(newAppointment);
            await _unitOfWork.SaveChangesAsync();

            // Update Doctor Total Patients Appointed
            var doctor = await _unitOfWork.Doctors.GetAsync(appointmentRequest.DoctorId);
             if(doctor != null) {
                doctor.TotalPatients++;
                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
            }

            Logger.Instance.LogSuccess($"/patient/appointments/book - Patient {patient.Id} Book Appointment {newAppointment.Id}");
            return Ok(new { Message = "Appointment booked successfully", AppointmentId = newAppointment.Id, Appointment = newAppointment });
        }

        [HttpGet("appointments")]
        public IActionResult MyAppointments()
        {
            Logger.Instance.LogInfo($"/patient/appointments - Fetching Patient Appointments");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query()
                                    .Include(p => p.Appointments)
                                    .ThenInclude(a => a.Doctor)
                                    .FirstOrDefault(p => p.UserId == userId);

            if (patient is null)
            {
                Logger.Instance.LogError("/patient/appointments - patient not exist");
                return NotFound();
            }


            Logger.Instance.LogSuccess($"/patient/appointments - Patient {patient.Id} Fetch His Appointments Successfully");

            return Ok(patient.Appointments
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorPhoto = a.Doctor.PhotoUrl, 
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Fee,
                    a.Notes
                }));
        }

        [HttpPost("appointments/{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            Logger.Instance.LogInfo($"/patient/appointments/{id}/cancel - Patient Cancel Request Appointment {id}");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient is null)
            {
                Logger.Instance.LogError("/patient/appointments/{id}/cancel - patient not exist");
                return BadRequest();
            }

            var appointment = _unitOfWork.Appointments.Query()
                .FirstOrDefault(a => a.Id == id && a.PatientId == patient.Id);

            if (appointment is null)
            {
                Logger.Instance.LogError("/patient/appointments/{id}/cancel - appointment not exist");
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/patient/appointments/{id}/cancel - Patient {patient.Id} Cancel Appointment {appointment.Id}");

            return Ok(new { message = "Appointment cancelled successfully", appointment });
        }
        [HttpPost("doctors/rate")]
        public async Task<IActionResult> RateDoctor([FromBody] ReviewDto reviewDto)
        {
            Logger.Instance.LogInfo("/patient/doctors/rate - Submitting review");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            
            if (patient == null) return BadRequest("Patient profile not found");

            var review = new Review
            {
                PatientId = patient.Id,
                DoctorId = reviewDto.DoctorId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Update Doctor Average Rating
            var doctor = await _unitOfWork.Doctors.GetAsync(reviewDto.DoctorId);
            if(doctor != null) {
                var ratings = _unitOfWork.Reviews.Query().Where(r => r.DoctorId == doctor.Id).Select(r => r.Rating).ToList();
                if(ratings.Any()) {
                    doctor.AverageRating = (decimal)ratings.Average();
                    _unitOfWork.Doctors.Update(doctor);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            Logger.Instance.LogSuccess("/patient/doctors/rate - Review submitted successfully");
            return Ok("Review submitted successfully");
        }

        [HttpGet("doctors/top")]
        public IActionResult GetTopRatedDoctors()
        {
            Logger.Instance.LogInfo("/patient/doctors/top - Fetching top rated doctors");
            
            var topDoctors = _unitOfWork.Doctors.Query()
                .Select(d => new
                {
                    d.Id,
                    d.FullName,
                    d.Biography,
                    AverageRating = _unitOfWork.Reviews.Query().Where(r => r.DoctorId == d.Id).Any() 
                                    ? _unitOfWork.Reviews.Query().Where(r => r.DoctorId == d.Id).Average(r => r.Rating) 
                                    : 0,
                    ReviewCount = _unitOfWork.Reviews.Query().Count(r => r.DoctorId == d.Id)
                })
                .OrderByDescending(d => d.AverageRating)
                .Take(5)
                .ToList();

            Logger.Instance.LogSuccess("/patient/doctors/top - Successfully fetched top rated doctors");
            return Ok(topDoctors);
        }
    }
}

using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Appointments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().Include(p => p.User).FirstOrDefault(p => p.UserId == userId);
            if (patient == null)
                return NotFound();

            return Ok(new
            {
                patient.Id,
                patient.FullName,
                patient.PhoneNumber,
                patient.Balance,
                Email = patient.User.Email
            });
        }


        [HttpPost("appointments/book")]
        public async Task<IActionResult> Book([FromBody] AppointmentCreateDto appointmentRequest)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient == null)
                return BadRequest("patient not exist");


            // check if exist booking in this time slot
            var isOverlap = _unitOfWork.Appointments.Query()
                .Any(a => a.DoctorId == appointmentRequest.DoctorId && a.Status != AppointmentStatus.Cancelled
                    && a.StartTime < appointmentRequest.EndTime && appointmentRequest.StartTime < a.EndTime);

            if (isOverlap)
                return BadRequest("Time slot not available");


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
            return Ok(newAppointment);
        }

        [HttpGet("appointments")]
        public IActionResult MyAppointments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var patient = _unitOfWork.Patients.Query()
                                    .Include(p => p.Appointments)
                                    .ThenInclude(a => a.Doctor)
                                    .FirstOrDefault(p => p.UserId == userId);

            if (patient is null)
                return NotFound("patient not found");

            return Ok(patient.Appointments
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    DoctorName = a.Doctor.FullName,
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.UserId == userId);
            if (patient is null)
                return BadRequest("Patient not found");

            var appointment = _unitOfWork.Appointments.Query()
                .FirstOrDefault(a => a.Id == id && a.PatientId == patient.Id);

            if (appointment is null)
                return NotFound("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "Appointment cancelled successfully", appointment });
        }
    }
}

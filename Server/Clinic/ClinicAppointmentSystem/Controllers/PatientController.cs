using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
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
                patient.DateOfBirth,
                Email = patient.User.Email
            });
        }


        [HttpPost("appointments")]
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


            var newAppointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = appointmentRequest.DoctorId,
                StartTime = appointmentRequest.StartTime,
                EndTime = appointmentRequest.EndTime,
                Status = AppointmentStatus.Pending,
                Fee = appointmentRequest.Fee,
                Notes = appointmentRequest.Notes
            };

            await _unitOfWork.Appointments.AddAsync(newAppointment);
            await _unitOfWork.SaveChangesAsync();
            return Ok(newAppointment);
        }
    }
}

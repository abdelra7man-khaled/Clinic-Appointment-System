using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("doctor/{doctorId}/schedule")]
        public IActionResult DoctorSchedule(int doctorId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var query = _unitOfWork.Appointments.Query()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId);

            if (dateFrom.HasValue)
                query = query.Where(a => a.StartTime >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(a => a.EndTime <= dateTo.Value);

            return Ok(query.OrderBy(a => a.StartTime).ToList());
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found");

            appointment.Status = AppointmentStatus.Confirmed;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found");

            var payment = _unitOfWork.Payments.Query()
                .FirstOrDefault(p => p.AppointmentId == appointment.Id);
            if (payment != null)
                _unitOfWork.Payments.Remove(payment);

            _unitOfWork.Appointments.Remove(appointment);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "Appointment deleted successfully" });
        }


    }
}

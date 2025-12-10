using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Admin")]
    public class AppointmentController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("doctor/{doctorId}/schedule")]
        public IActionResult DoctorSchedule(int doctorId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            Logger.Instance.LogInfo($"/appointment/doctor/{doctorId}/schedule - Show Doctor Appointment Schedule");

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
            Logger.Instance.LogInfo($"/appointment/{id}/confirm -  Doctor Confirm Appointment");

            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment is null)
            {
                Logger.Instance.LogError("/appointment/{id}/confirm - Appointment not found");
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Confirmed;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/appointment/{id}/confirm- Appointment {appointment.Id} Confirmed Successfully");

            return Ok(appointment);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            Logger.Instance.LogInfo($"/appointment/{id}/cancel -  Doctor Cancel Appointment");

            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment is null)
            {
                Logger.Instance.LogError($"/appointment/{id}/cancel - Appointment not found");
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/appointment/{id}/cancel- Appointment {appointment.Id} Canceled Successfully");

            return Ok(appointment);
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            Logger.Instance.LogInfo($"/appointment/{id}/delete -  Doctor Delete Appointment");

            var appointment = await _unitOfWork.Appointments.GetAsync(id);
            if (appointment is null)
            {
                Logger.Instance.LogError("/appointment/{id}/delete - Appointment not found");
                return NotFound();
            }

            var payment = _unitOfWork.Payments.Query()
                                     .FirstOrDefault(p => p.AppointmentId == appointment.Id);
            if (payment is not null)
                _unitOfWork.Payments.Remove(payment);

            _unitOfWork.Appointments.Remove(appointment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/appointment/{id}/delete- Appointment {appointment.Id} Deleted Successfully");

            return Ok(new { message = "Appointment deleted successfully" });
        }


    }
}

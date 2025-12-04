using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PaymentDto paymentDto)
        {
            var appointment = await _unitOfWork.Appointments.GetAsync(paymentDto.AppointmentId);
            if (appointment is null)
                return NotFound("Appointment not found");

            var payment = new Payment
            {
                AppointmentId = paymentDto.AppointmentId,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                PaidAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return Ok(payment);
        }
    }
}

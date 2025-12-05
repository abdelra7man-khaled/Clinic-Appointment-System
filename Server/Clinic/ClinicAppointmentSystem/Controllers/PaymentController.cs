using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Payments;
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
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.Id == paymentDto.PatientId);

            if (patient is null)
            {
                return NotFound("Patient not found");
            }

            var appointment = await _unitOfWork.Appointments.GetAsync(paymentDto.AppointmentId);
            if (appointment is null)
            {
                return NotFound("Appointment not found");
            }

            var context = paymentDto.PaymentMethod switch
            {
                PaymentMethod.Cash => new PaymentContext(new CashPaymentStrategy()),
                PaymentMethod.CreditCard => new PaymentContext(new CreditCardPaymentStrategy()),
                _ => new PaymentContext(new CashPaymentStrategy())
            };

            var paid = context.Pay(paymentDto.Amount,
                patient,
                paymentDto.PaymentMethod == PaymentMethod.CreditCard ? paymentDto.CardDetails : null);

            if (!paid)
            {
                return BadRequest("Payment failed. Check balance or credit card details.");
            }

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

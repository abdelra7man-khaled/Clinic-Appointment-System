using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Clinic.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient,Doctor,Admin")]
    public class PaymentController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PaymentDto paymentDto)
        {
            Logger.Instance.LogInfo("/payments/pay/ - Making Payment Process");
            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.Id == paymentDto.PatientId);

            if (patient is null)
            {
                Logger.Instance.LogError("/payments/pay/ - Patient not found");
                return NotFound();
            }

            var appointment = await _unitOfWork.Appointments.GetAsync(paymentDto.AppointmentId);
            if (appointment is null)
            {
                Logger.Instance.LogError("/payments/pay/ - Appointment not found");
                return NotFound();
            }

            var context = paymentDto.PaymentMethod switch
            {
                PaymentMethod.Cash => new PaymentContext(new CashPaymentStrategy()),
                PaymentMethod.CreditCard => new PaymentContext(new CreditCardPaymentStrategyProxy()),
                _ => new PaymentContext(new CashPaymentStrategy())
            };

            var paid = context.Pay(paymentDto.Amount,
                patient,
                paymentDto.PaymentMethod == PaymentMethod.CreditCard ? paymentDto.CardDetails : null!);

            if (!paid)
            {
                Logger.Instance.LogError("/payments/pay/ - Payment failed. Check balance or credit card details.");
                return BadRequest();
            }

            var payment = new Payment
            {
                AppointmentId = paymentDto.AppointmentId,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                PaidAt = DateTime.UtcNow
            };

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/payments/pay/ - Payment Procces Made By {patient.Id} with Amount {payment.Amount}");
            return Ok(payment);
        }

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Patient,Admin")]
        public IActionResult PaymentHistory(int patientId)
        {
            Logger.Instance.LogInfo($"/payments/patient/{patientId} - Show Payment History Of Patient {patientId}");

            var payments = _unitOfWork.Payments.Query()
                                    .Where(p => p.Appointment.PatientId == patientId)
                                    .Include(p => p.Appointment)
                                    .ThenInclude(a => a.Doctor)
                                    .ToList();


            return Ok(payments.Select(p => new
            {
                p.Id,
                p.Amount,
                p.PaymentMethod,
                p.PaidAt,
                AppointmentDoctor = p.Appointment.Doctor.FullName,
                AppointmentStart = p.Appointment.StartTime
            }));
        }

        [HttpGet("history")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            Logger.Instance.LogInfo("/payments/history - Show all Payments History");

            var payments = _unitOfWork.Payments.Query()
                .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
                .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
                .ToList();

            var totalRevenue = payments.Sum(p => p.Amount);

            return Ok(new
            {
                TotalPayments = payments.Count,
                TotalRevenue = totalRevenue,
                Payments = payments.Select(p => new
                {
                    p.Id,
                    p.Amount,
                    p.PaymentMethod,
                    p.PaidAt,
                    Patient = p.Appointment.Patient.FullName,
                    Doctor = p.Appointment.Doctor.FullName
                })
            });
        }

    }


}

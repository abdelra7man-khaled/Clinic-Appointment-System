using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Clinic.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            // If PatientId is missing and user is a Patient, infer from token
            if (paymentDto.PatientId == 0 && User.IsInRole("Patient"))
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    var p = _unitOfWork.Patients.Query().FirstOrDefault(x => x.UserId == userId);
                    if (p != null) paymentDto.PatientId = p.Id;
                }
            }

            var patient = _unitOfWork.Patients.Query().FirstOrDefault(p => p.Id == paymentDto.PatientId);

            if (patient is null)
            {
                Logger.Instance.LogError($"/payments/pay/ - Patient not found (ID: {paymentDto.PatientId})");
                return NotFound("Patient not found. Please provide valid PatientId or log in as Patient.");
            }

            var appointment = await _unitOfWork.Appointments.GetAsync(paymentDto.AppointmentId);
            if (appointment is null)
            {
                Logger.Instance.LogError("/payments/pay/ - Appointment not found");
                return NotFound();
            }

            // In a real scenario, we might verify credit card validty here without charging yet,
            // or put a hold. For this flow, we just record the attempt.
            
            var payment = new Payment
            {
                AppointmentId = paymentDto.AppointmentId,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                PaidAt = DateTime.UtcNow,
                IsConfirmed = false // Pending Admin Confirmation
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            
            // Link Payment to Appointment immediately so we can track it
            appointment.PaymentTransactionId = payment.Id.ToString();
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/payments/pay/ - Payment Request Created By {patient.Id} with Amount {payment.Amount}. Waiting for Admin Confirmation.");
            return Ok(new { Message = "Payment recorded. Waiting for Admin confirmation.", PaymentId = payment.Id });
        }

        [HttpPost("confirm/{paymentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            Logger.Instance.LogInfo($"/payments/confirm/{paymentId} - Admin Confirming Payment");

            var payment = await _unitOfWork.Payments.GetAsync(paymentId);
            if (payment == null) return NotFound("Payment not found");

            if (payment.IsConfirmed) return BadRequest("Payment already confirmed");

            var appointment = _unitOfWork.Appointments.Query()
                                                    .Include(a => a.Doctor)
                                                    .Include(a => a.Patient)
                                                    .FirstOrDefault(a => a.Id == payment.AppointmentId);

            if (appointment == null) return BadRequest("Associated appointment not found");

            // 1. Deduct from Patient Balance
            if (appointment.Patient.Balance < payment.Amount)
            {
                return BadRequest("Insufficient patient balance");
            }
            appointment.Patient.Balance -= payment.Amount;

            // 2. Add to Doctor Balance
            appointment.Doctor.Balance += payment.Amount;

            // 3. Mark Payment Confirmed
            payment.IsConfirmed = true;

            // 4. Mark Appointment Paid
            appointment.IsPaid = true;

            _unitOfWork.Patients.Update(appointment.Patient);
            _unitOfWork.Doctors.Update(appointment.Doctor);
            _unitOfWork.Payments.Update(payment);
            _unitOfWork.Appointments.Update(appointment);

            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"/payments/confirm/{paymentId} - Payment Confirmed. Transferred {payment.Amount} from {appointment.Patient.FullName} to Dr. {appointment.Doctor.FullName}");
            return Ok("Payment confirmed and balances updated");
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

        [HttpGet("admin/payments/dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            Logger.Instance.LogInfo("/payments/admin/payments/dashboard - Show all Payments History");

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
                    p.IsConfirmed,
                    Patient = p.Appointment.Patient.FullName,
                    Doctor = p.Appointment.Doctor.FullName
                })
            });
        }

    }


}

using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class PaymentDto
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentDetails? CardDetails { get; set; }
    }
}

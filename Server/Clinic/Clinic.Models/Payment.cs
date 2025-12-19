using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public bool IsConfirmed { get; set; }
    }
}

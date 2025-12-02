using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal Fee { get; set; }
        public string Notes { get; set; }
    }
}

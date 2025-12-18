using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class AppointmentCreateDto
    {
        public int DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public decimal Fee { get; set; }
        public string Notes { get; set; }
    }
}
